using SoyEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlPersonalInformation : UICtrlAnimationBase<UIViewPersonalInformation>
    {
        private long _userId;
        private EMenu _curMenu = EMenu.None;
        private UPCtrlPersonalInforBase _curMenuCtrl;
        private UPCtrlPersonalInforBase[] _menuCtrlArray;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _menuCtrlArray = new UPCtrlPersonalInforBase[(int) EMenu.Max];
            var upCtrlPersonalInforRecords = new UPCtrlPersonalInforRecords();
            upCtrlPersonalInforRecords.SetResScenary(ResScenary);
            upCtrlPersonalInforRecords.SetMenu(EMenu.Records);
            upCtrlPersonalInforRecords.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Records] = upCtrlPersonalInforRecords;

            for (int i = 0; i < _cachedView.MenuButtonAry.Length; i++)
            {
                var index = i;
                _cachedView.TabGroup.AddButton(_cachedView.MenuButtonAry[i], _cachedView.MenuSelectedButtonAry[i],
                    b => ClickMenu(index, b));
                if (i < _menuCtrlArray.Length && null != _menuCtrlArray[i])
                {
                    _menuCtrlArray[i].Close();
                }
            }
        }

        protected override void OnDestroy()
        {
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                if (_menuCtrlArray[i] != null)
                {
                    _menuCtrlArray[i].OnDestroy();
                }
            }
            _curMenuCtrl = null;
            base.OnDestroy();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI;
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (_curMenu == EMenu.None)
            {
                _cachedView.TabGroup.SelectIndex((int) EMenu.BasicInfo, true);
            }
            else
            {
                _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
            }
        }

        protected override void OnClose()
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            base.OnClose();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        private void ChangeMenu(EMenu menu)
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            _curMenu = menu;
            var inx = (int) _curMenu;
            if (inx < _menuCtrlArray.Length)
            {
                _curMenuCtrl = _menuCtrlArray[inx];
            }
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Open();
            }
        }

        private void ClickMenu(int selectInx, bool open)
        {
            if (open)
            {
                ChangeMenu((EMenu) selectInx);
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPersonalInformation>();
        }

        public enum EMenu
        {
            None = -1,
            BasicInfo,
            AchivementInfo,
            Publish,
            Collects,
            Records,
            Max
        }
    }
}
