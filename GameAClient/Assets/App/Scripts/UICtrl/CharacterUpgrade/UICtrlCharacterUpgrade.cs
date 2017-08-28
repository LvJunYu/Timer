using SoyEngine;
using System.Collections.Generic;
using UnityEngine;

namespace GameA
{
    /// <summary>
    /// 阶层养成
    /// </summary>
    [UIAutoSetup]
    public class UICtrlCharacterUpgrade : UICtrlAnimationBase<UIViewCharacterUpgrade>
    {
        private const int _maxPropertyCount = 5;
        private int[] _gradeMaxLv;
        private UMCtrlCharacterUpgradeItem[] _propertyItems;
        private TrainProperty[] _trainProperties;
        private int _curGrade;

        private void CreateUMItems()
        {
            //临时数据
            _curGrade = 0;
            //创建属性UMItem
            _propertyItems = new UMCtrlCharacterUpgradeItem[_maxPropertyCount];
            _trainProperties = new TrainProperty[_maxPropertyCount];
            for (int i = 0; i < _maxPropertyCount; i++)
            {
                //初始化属性数据，todo获取服务器数据
                _trainProperties[i] = new TrainProperty(i + 1, 1, _curGrade);
                _propertyItems[i] = new UMCtrlCharacterUpgradeItem(_trainProperties[i]);
                _propertyItems[i].Init(_cachedView.PropertyListRTF);
                _propertyItems[i].InitView();
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
            CreateUMItems();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshView();
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlCharacterUpgrade>();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpUI;
        }
    }
}