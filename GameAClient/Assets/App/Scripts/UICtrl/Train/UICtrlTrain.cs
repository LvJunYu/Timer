using System;
using SoyEngine;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine.Proto;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameA
{
    /// <summary>
    /// 阶层养成
    /// </summary>
    [UIAutoSetup]
    public class UICtrlTrain : UICtrlAnimationBase<UIViewTrain>
    {
        private string[] _spriteNames =
        {
            "icon_train_heart",
            "icon_train_run",
            "icon_train_jump",
            "icon_train_shield",
            "icon_train_magnet"
        };

        private string[] _propertyNames =
        {
            "冥想训练",
            "长跑训练",
            "跳远训练",
            "拳击训练",
            "摩擦训练"
        };

        private const int _maxPropertyCount = 5;
        private int[] _gradeMaxLv;
        private UMCtrlTrainPropertyItem[] _propertyItems;
        private UMCtrlTrainPropertyInfo[] _propertyInfos;
        private TrainProperty[] _trainProperties;
        private int _curGrade;
        private int _curTrainPoint;
        private bool _isTraining;
        private TrainProperty _curTrainingProperty;
        private int _curRemainTime;
        private float _checkTime;

        private void CreateUMItems()
        {
            List<TrainProperty> _userTrainProperty = LocalUser.Instance.UserTrainProperty.ItemDataList;
            _curGrade = LocalUser.Instance.UserTrainProperty.Grade;
            _curTrainPoint = LocalUser.Instance.UserTrainProperty.TrainPoint;
            //临时数据
            _curGrade = 1;
            _curTrainPoint = 250;
            //创建属性UMItem和UMInfo
            _isTraining = false;
            _propertyItems = new UMCtrlTrainPropertyItem[_maxPropertyCount];
            _propertyInfos = new UMCtrlTrainPropertyInfo[_maxPropertyCount];
            _trainProperties = new TrainProperty[_maxPropertyCount];
            for (int i = 0; i < _maxPropertyCount; i++)
            {
                //初始化属性数据
//                int level = _userTrainProperty[i].Level;
                //临时数据
                int level = Random.Range(1, 4);

                _trainProperties[i] = new TrainProperty(i + 1, level, _curGrade);
                _propertyItems[i] = new UMCtrlTrainPropertyItem(_trainProperties[i]);
                _propertyItems[i].Init(_cachedView.PropertyListRTF);
                _propertyInfos[i] = new UMCtrlTrainPropertyInfo(_trainProperties[i]);
                _propertyInfos[i].Init(_cachedView.InfoListRTF);
                Sprite sprite = ResourcesManager.Instance.GetSprite(_spriteNames[i]);
                _propertyItems[i].InitView(sprite, _propertyNames[i]);
                _propertyInfos[i].InitView(sprite);
            }
        }

        private void RefreshView()
        {
            if (!_isOpen) return;
            _isTraining = false;
            _curTrainingProperty = null;
            //刷新属性
            for (int i = 0; i < _trainProperties.Length; i++)
            {
                if (_trainProperties[i].IsTraining)
                {
                    _isTraining = true;
                    _curTrainingProperty = _trainProperties[i];
                }
                _propertyItems[i].Refresh();
                _propertyInfos[i].Refresh();
            }
            if (_isTraining)
            {
                _cachedView.TrainingTxt.text =
                    string.Format("{0}中", _propertyNames[(int) (_curTrainingProperty.Property) - 1]);
                _cachedView.ValueDescTxt.text = string.Format("{0}→{1}", _curTrainingProperty.ValueDesc,
                    _curTrainingProperty.NextValueDesc);
            }
            _cachedView.IsTraining.SetActive(_isTraining);
            _cachedView.PropertyListRTF.gameObject.SetActive(!_isTraining);
            //刷新拥有的培养点
            _cachedView.OwnedTrainPointTxt.text = _curTrainPoint.ToString();
            //刷新阶层Image
            for (int i = 0; i < _cachedView.GradeImgs.Length; i++)
            {
                _cachedView.GradeImgs[i].SetActive(_curGrade == i + 1);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!_isTraining) return;
            if (Time.time > _checkTime)
            {
                _curRemainTime = _curTrainingProperty.RemainTrainingTime;
                //若超过1小时，每分钟刷新
                if (_curRemainTime > 3600)
                {
                    int checkInterval = _curRemainTime % 60;
                    if (0 == checkInterval)
                        checkInterval = 60;
                    _checkTime = Time.time + checkInterval;
                }
                //否则每秒刷新
                else
                {
                    float checkInterval = Mathf.Ceil(Time.time) - Time.time;
                    if (0 == checkInterval)
                        checkInterval = 1;
                    _checkTime = (Time.time + checkInterval);
                    Debug.Log("Time.time = " + Time.time);
                    Debug.Log("checkInterval = " + checkInterval);
                }
                _cachedView.RemainTimeTxt.text = _curTrainingProperty.RemainTrainingTimeDesc;
                _cachedView.FinishCostTxt.text = _curTrainingProperty.FinishCost.ToString();
                _cachedView.TrainingSlider.value =
                    _curTrainingProperty.RemainTrainingTime / (float) _curTrainingProperty.Time;
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.FinishImmediatelyBtn.onClick.AddListener(OnFinishImmediatelyBtn);
//            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
//            LocalUser.Instance.UserTrainProperty.Request(LocalUser.Instance.UserGuid,
//                () =>
//                {
//                    CreateUMItems();
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                },
//                code =>
//                {
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    LogHelper.Error("Network error when get UserTrainProperty, {0}", code);
//                });
            CreateUMItems();
        }

        private void OnFinishImmediatelyBtn()
        {
            SocialGUIManager.ShowPopupDialog(
                string.Format("是否使用{0}钻石立即完成训练。", _curTrainingProperty.FinishCost),
                null,
                new KeyValuePair<string, Action>("确定", () => { RequestFinishUpgradeTrainProperty(); }),
                new KeyValuePair<string, Action>("取消", () => { }));
        }

        private void RequestFinishUpgradeTrainProperty()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "完成训练");
            RemoteCommands.CompleteUpgradeTrainProperty(_curTrainingProperty.Property, _curTrainingProperty.Level + 1,
                _curTrainingProperty.RemainTrainingTimeMill, res =>
                {
                    if (res.ResultCode == (int) ECompleteUpgradeTrainPropertyCode.CUTPC_Success)
                    {
                        FinishUpgradeTrainProperty();
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    }
                    else
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        LogHelper.Debug("立即完成训练失败");
                    }
                }, code =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    //测试，服务器完成后删除
                    LogHelper.Debug("服务器请求失败，客服端进行测试");
                    FinishUpgradeTrainProperty();
                });
        }

        private void FinishUpgradeTrainProperty()
        {
            _curTrainingProperty.FinishUpgrade();
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlTrain>();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnCharacterUpgradeProperty, OnCharacterUpgradeProperty);
            RegisterEvent(EMessengerType.OnCharacterUpgradeGrade, OnCharacterUpgradeGrade);
        }

        private void OnCharacterUpgradeProperty()
        {
            RefreshView();
        }

        private void OnCharacterUpgradeGrade()
        {
            RefreshView();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshView();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpUI;
        }
    }
}