using SoyEngine;
using System.Collections.Generic;
using NewResourceSolution;
using UnityEngine;

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

        private void CreateUMItems()
        {
            List<TrainProperty> _userTrainProperty = LocalUser.Instance.UserTrainProperty.ItemDataList;
            _curGrade = LocalUser.Instance.UserTrainProperty.Grade;
            //临时数据
            _curGrade = 1;
            //创建属性UMItem
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
            for (int i = 0; i < _trainProperties.Length; i++)
            {
                _propertyItems[i].Refresh();
                _propertyInfos[i].Refresh();
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
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