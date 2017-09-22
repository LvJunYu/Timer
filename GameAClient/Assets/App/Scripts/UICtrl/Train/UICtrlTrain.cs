﻿using System;
using SoyEngine;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 阶层养成
    /// </summary>
    [UIResAutoSetup(EResScenary.UIHome)]
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

        private int[] _gradeMaxLv =
        {
            3, 6, 10, 15
        };

        private int[] _indexOrder =
        {
            0, 1, 3, 4, 2
        };

        private const string _lineName = "img_train_net_line";
        private const string _pointName = "img_train_net_point";
        private const int _maxPropertyCount = 5;
        private UserTrainProperty _userTrainProperty;
        private UMCtrlTrainPropertyItem[] _propertyItems;
        private UMCtrlTrainPropertyInfo[] _propertyInfos;
        private TrainProperty[] _trainProperties;
        private bool _isTraining;

        private TrainProperty _curTrainingProperty;
        private float _checkTime;
        private Vector4[] _map = new Vector4[_maxPropertyCount];
        private Vector3[] _mapScreenPos = new Vector3[_maxPropertyCount];
        private Camera _uiCamera;
        private List<Transform> _lineCach;
        private List<Transform> _pointCach;
        private Sprite _lineSprite;
        private Sprite _pointSprite;
        private Dictionary<int, List<Vector3>> _propertyPosDic;
        private int _curMaxLv;
        private int _curMinLv;
        private int _count;
        private float _angel;

        private void CreateUmItems()
        {
            _angel = 90 - (180 - 360 / 5) / (float) 2;
            _userTrainProperty = LocalUser.Instance.UserTrainProperty;
            _trainProperties = _userTrainProperty.TrainProperties;
            //创建属性UMItem和UMInfo
            _propertyItems = new UMCtrlTrainPropertyItem[_maxPropertyCount];
            _propertyInfos = new UMCtrlTrainPropertyInfo[_maxPropertyCount];
            for (int i = 0; i < _trainProperties.Length; i++)
            {
                _propertyItems[i] = new UMCtrlTrainPropertyItem(_trainProperties[i]);
                _propertyItems[i].Init(_cachedView.PropertyListRTF, ResScenary);
                _propertyInfos[i] = new UMCtrlTrainPropertyInfo(_trainProperties[i]);
                _propertyInfos[i].Init(_cachedView.InfoListRTF, ResScenary);
                Sprite sprite = JoyResManager.Instance.GetSprite(_spriteNames[i]);
                _propertyItems[i].InitView(sprite, _propertyNames[i]);
                _propertyInfos[i].InitView(sprite);
            }
        }

        private void RefreshView()
        {
            if (!_isOpen) return;
            _userTrainProperty = LocalUser.Instance.UserTrainProperty;
            _trainProperties = _userTrainProperty.TrainProperties;
            //判断是否在训练
            _isTraining = false;
            _curTrainingProperty = null;
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
                if (null == _curTrainingProperty)
                {
                    LogHelper.Error("_isTraining==ture, but _curTrainingProperty==null");
                    return;
                }
                _cachedView.TrainingTxt.text =
                    string.Format("{0}中", _propertyNames[(int) (_curTrainingProperty.Property) - 1]);
                _cachedView.ValueDescTxt.text = string.Format("{0}→{1}", _curTrainingProperty.ValueDesc,
                    _curTrainingProperty.NextValueDesc);
            }
            _cachedView.IsTraining.SetActive(_isTraining);
            _cachedView.PropertyListRTF.gameObject.SetActive(!_isTraining);
            //刷新拥有的培养点
            int grade = _userTrainProperty.Grade;
            _cachedView.OwnedTrainPointTxt.text = grade.ToString();
            //刷新阶层Image
            for (int i = 0; i < _cachedView.GradeImgs.Length; i++)
            {
                _cachedView.GradeImgs[i].SetActive(grade == i + 1);
            }
            //刷新Map
//            RefreshMap();
        }

        private void RefreshMap()
        {
            RefreshPointAndLine();
            for (int i = 0; i < _maxPropertyCount; i++)
            {
                if (null == _uiCamera)
                    _uiCamera = SocialGUIManager.Instance.UIRoot.Canvas.worldCamera;
                //计算属性位置点
                Vector3 targetPos;
                if (_trainProperties[i].Level == _curMaxLv)
                    targetPos = _cachedView.MapOutPoints[i].position;
                else if (_trainProperties[i].Level == _curMinLv)
                    targetPos = _cachedView.MapInPoints[i].position;
                else
                    targetPos = _propertyPosDic[i][_trainProperties[i].Level - _curMinLv - 1];
                _mapScreenPos[i] = _uiCamera.WorldToScreenPoint(targetPos);
            }
            _map[0] = new Vector4(_mapScreenPos[0].x, Screen.height - _mapScreenPos[0].y, _mapScreenPos[0].z, 0);
            _map[1] = new Vector4(_mapScreenPos[1].x, Screen.height - _mapScreenPos[1].y, _mapScreenPos[1].z, 0);
            _map[2] = new Vector4(_mapScreenPos[3].x, Screen.height - _mapScreenPos[3].y, _mapScreenPos[3].z, 0);
            _map[3] = new Vector4(_mapScreenPos[4].x, Screen.height - _mapScreenPos[4].y, _mapScreenPos[4].z, 0);
            _map[4] = new Vector4(_mapScreenPos[2].x, Screen.height - _mapScreenPos[2].y, _mapScreenPos[2].z, 0);
            _cachedView.MapMaterial.SetVectorArray("Value", _map); //传递顶点屏幕位置信息给shader 
            _cachedView.MapMaterial.SetInt("PointNum", 5); //传递顶点数量给shader 
            //检查能否升阶
            _cachedView.UpgradeGradeBtn.gameObject.SetActive(CheckUpgradeGrade());
        }

        private void RefreshPointAndLine()
        {
            ClearPointAndLine();
            //清除属性位置字典
            if (null == _propertyPosDic)
            {
                _propertyPosDic = new Dictionary<int, List<Vector3>>(_maxPropertyCount);
                _propertyPosDic.Add(0, new List<Vector3>(5));
                _propertyPosDic.Add(1, new List<Vector3>(5));
                _propertyPosDic.Add(2, new List<Vector3>(5));
                _propertyPosDic.Add(3, new List<Vector3>(5));
                _propertyPosDic.Add(4, new List<Vector3>(5));
            }
            for (int i = 0; i < _maxPropertyCount; i++)
            {
                _propertyPosDic[i].Clear();
            }
            //若已打最大阶层，不产生点和线
            int grade = _userTrainProperty.Grade;
            if (grade > _gradeMaxLv.Length)
            {
                _curMaxLv = _gradeMaxLv[_gradeMaxLv.Length - 1];
                _curMinLv = _gradeMaxLv[_gradeMaxLv.Length - 1];
                _count = 0;
                return;
            }
            _curMaxLv = _gradeMaxLv[grade - 1];
            _curMinLv = grade == 1 ? 1 : _gradeMaxLv[grade - 2];
            _count = _curMaxLv - _curMinLv - 1;
            //设置等级分割点
            for (int i = 0; i < _count; i++)
            {
                for (int j = 0; j < _maxPropertyCount; j++)
                {
                    Transform point = GetPoint();
                    Vector3 delta = _cachedView.MapOutPoints[j].position - _cachedView.MapInPoints[j].position;
                    point.position = _cachedView.MapInPoints[j].position +
                                     delta * (i + 1) / (_count + 1);
                    _propertyPosDic[j].Add(point.position);
                }
            }
            //设置分割线
            for (int i = 0; i < _count; i++)
            {
                for (int j = 0; j < _maxPropertyCount; j++)
                {
                    int curIndex = _indexOrder[j];
                    int nextIndex = _indexOrder[(j + 1) % _maxPropertyCount];
                    Vector3 center = 1 / (float) 2 * (_propertyPosDic[curIndex][i] + _propertyPosDic[nextIndex][i]);
                    float distance = Vector3.Distance(_propertyPosDic[curIndex][i], _propertyPosDic[nextIndex][i]);
                    float angel = 360 / (float) _maxPropertyCount * j + _angel;
                    Transform line = GetLine();
                    line.position = center;
                    line.eulerAngles = new Vector3(0, 0, angel);
                    line.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, distance);
                }
            }
        }

        private void OnUpgradeGradeBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在升阶");
            RemoteCommands.UpgradeTrainGrade(_userTrainProperty.Grade + 1, res =>
            {
                if (res.ResultCode == (int) EUpgradeTrainGradeCode.UTPG_Success)
                {
                    UpgradeGrade();
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                }
                else
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Debug("训练升阶失败");
                }
            }, code =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                //测试，服务器完成后删除
                LogHelper.Debug("服务器请求失败，客服端进行测试");
                UpgradeGrade();
            });
        }

        private void OnFinishImmediatelyBtn()
        {
            SocialGUIManager.ShowPopupDialog(
                string.Format("是否使用{0}钻石立即完成训练。", _curTrainingProperty.FinishCostDiamond),
                null,
                new KeyValuePair<string, Action>("确定", () =>
                {
                    if (_isTraining)
                        RequestFinishUpgradeTrainProperty();
                }),
                new KeyValuePair<string, Action>("取消", () => { }));
        }

        private void RequestFinishUpgradeTrainProperty()
        {
            if (!GameATools.CheckDiamond(_curTrainingProperty.FinishCostDiamond)) return;
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
            if (!_isTraining) return;
            if (!GameATools.CheckDiamond(_curTrainingProperty.FinishCostDiamond)) return;
            if (GameATools.LocalUseDiamond(_curTrainingProperty.FinishCostDiamond))
            {
                _curTrainingProperty.FinishUpgrade();
                _curTrainingProperty = null;
                _isTraining = false;
                _checkTime = 0;
                SocialGUIManager.ShowPopupDialog("恭喜您完成训练！",
                    null,
                    new KeyValuePair<string, Action>("确定", () => { }));
            }
        }

        private void UpgradeGrade()
        {
            if (!CheckUpgradeGrade()) return;
            _userTrainProperty.UpgradeGrade();
            //弹窗
            SocialGUIManager.ShowPopupDialog("恭喜您完成升阶！",
                null,
                new KeyValuePair<string, Action>("确定", () => { }));
        }

        private bool CheckUpgradeGrade()
        {
            if (_userTrainProperty.Grade > _gradeMaxLv.Length)
                return false;
            for (int i = 0; i < _trainProperties.Length; i++)
            {
                if (_trainProperties[i].Level < _curMaxLv)
                    return false;
            }
            return true;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlTrain>();
        }

        private void OnCharacterUpgradeProperty()
        {
            RefreshMap();
            RefreshView();
        }

        private void OnCharacterUpgradeGrade()
        {
            RefreshMap();
            RefreshView();
        }

        private Transform GetPoint()
        {
            if (null == _pointCach)
                _pointCach = new List<Transform>(20);
            Transform tf = _pointCach.Find(p => p.gameObject.activeSelf == false);
            if (tf != null)
            {
                tf.gameObject.SetActive(true);
                return tf;
            }
            Image img = new GameObject("Point").AddComponent<Image>();
            if (null == _pointSprite)
                _pointSprite = JoyResManager.Instance.GetSprite(_pointName);
            img.sprite = _pointSprite;
            tf = img.GetComponent<Transform>();
            tf.SetParent(_cachedView.MapImg);
            img.SetNativeSize();
            _pointCach.Add(tf);
            return tf;
        }

        private Transform GetLine()
        {
            if (null == _lineCach)
                _lineCach = new List<Transform>(20);
            Transform tf = _lineCach.Find(p => p.gameObject.activeSelf == false);
            if (tf != null)
            {
                tf.gameObject.SetActive(true);
                return tf;
            }
            Image img = new GameObject("Line").AddComponent<Image>();
            if (null == _lineSprite)
                _lineSprite = JoyResManager.Instance.GetSprite(_lineName);
            img.sprite = _lineSprite;
            tf = img.GetComponent<Transform>();
            tf.SetParent(_cachedView.MapImg);
            img.SetNativeSize();
            _lineCach.Add(tf);
            return tf;
        }

        private void ClearPointAndLine()
        {
            if (null == _lineCach)
                _lineCach = new List<Transform>(20);
            if (null == _pointCach)
                _pointCach = new List<Transform>(20);
            for (int i = 0; i < _lineCach.Count; i++)
                _lineCach[i].gameObject.SetActive(false);
            for (int i = 0; i < _pointCach.Count; i++)
                _pointCach[i].gameObject.SetActive(false);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!_isOpen) return;
            if (!_isTraining) return;
            if (Time.time > _checkTime)
            {
                int curRemainTime = _curTrainingProperty.RemainTrainingTime;
                if (curRemainTime <= 0)
                {
                    FinishUpgradeTrainProperty();
                    return;
                }
                //若超过1小时，每分钟刷新
                if (curRemainTime > 3600)
                {
                    int checkInterval = curRemainTime % 60;
                    if (0 == checkInterval)
                        checkInterval = 60;
                    _checkTime = Time.time + checkInterval;
                }
                //否则每秒刷新
                else
                {
                    float checkInterval = Mathf.Ceil(Time.time) - Time.time;
                    if (checkInterval < 0.01f)
                        checkInterval = 1;
                    _checkTime = (Time.time + checkInterval);
                }
                _cachedView.RemainTimeTxt.text = _curTrainingProperty.RemainTrainingTimeDesc;
                _cachedView.FinishCostTxt.text = _curTrainingProperty.FinishCostDiamond.ToString();
                _cachedView.TrainingSlider.value =
                    _curTrainingProperty.RemainTrainingTime / (float) _curTrainingProperty.Time;
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.UpgradeGradeBtn.onClick.AddListener(OnUpgradeGradeBtn);
            _cachedView.FinishImmediatelyBtn.onClick.AddListener(OnFinishImmediatelyBtn);
            CreateUmItems();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshView();
//            LocalUser.Instance.Achievement.AddAchievementCount(1, 1);
        }

        protected override void OnOpenAnimationUpdate()
        {
            base.OnOpenAnimationUpdate();
            RefreshMap();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }
        
        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnUpgradeTrainProperty, OnCharacterUpgradeProperty);
            RegisterEvent(EMessengerType.OnUpgradeTrainGrade, OnCharacterUpgradeGrade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpUI;
        }
    }
}