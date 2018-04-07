using System.Collections.Generic;
using SoyEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlStoryMode : UICtrlAnimationBase<UIViewStoryMode>
    {
        private List<Project> _dataList;
        private bool _pushGoldEnergyStyle;

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>()
                    .PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamond);
                _pushGoldEnergyStyle = true;
            }
        }

        protected override void OnClose()
        {
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }

            base.OnClose();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ExitBtn.onClick.AddListener(OnReturnBtnClick);
            RequestData();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        private void OnReturnBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlStoryMode>();
        }


        private void OnNextBtn()
        {
        }

        private void OnPrevBtn()
        {
        }

        public void RequestData(bool append = false)
        {
            AppData.Instance.OfficialProjectList.RequestRpg(RefreshView,
                () => { SocialGUIManager.ShowPopupDialog("获取关卡失败!"); });
        }

        private void RefreshView()
        {
            _dataList = AppData.Instance.OfficialProjectList.RpgProjectList;
            for (int i = 0; i < _dataList.Count; i++)
            {
                UMCtrlProjectStory um = new UMCtrlProjectStory();
                um.Init(_cachedView.ContentRect, EResScenary.UIHome);
                um.Set(_dataList[i]);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}