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
    public class UICtrlCharacterUpgrade : UICtrlAnimationBase<UIViewCharacterUpgrade>
    {
        private string[] _spriteNames =
        {
            "icon_train_heart",
            "icon_train_run",
            "icon_train_jump",
            "icon_train_nutrition",
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
        private UMCtrlCharacterUpgradeItem[] _propertyItems;
        private UMCtrlCharacterUpgradeInfo[] _propertyInfos;
        private TrainProperty[] _trainProperties;
        private int _curGrade;

        private void CreateUMItems()
        {
            //临时数据
            _curGrade = 1;
            //创建属性UMItem
            _propertyItems = new UMCtrlCharacterUpgradeItem[_maxPropertyCount];
            _propertyInfos = new UMCtrlCharacterUpgradeInfo[_maxPropertyCount];
            _trainProperties = new TrainProperty[_maxPropertyCount];
            for (int i = 0; i < _maxPropertyCount; i++)
            {
                //初始化属性数据，todo获取服务器数据
                _trainProperties[i] = new TrainProperty(i + 1, 1, _curGrade);
                _propertyItems[i] = new UMCtrlCharacterUpgradeItem(_trainProperties[i]);
                _propertyItems[i].Init(_cachedView.PropertyListRTF);
                _propertyInfos[i] = new UMCtrlCharacterUpgradeInfo(_trainProperties[i]);
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
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            CreateUMItems();
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlCharacterUpgrade>();
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