
namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlMail : UICtrlAnimationBase<UIViewMail>
    {
        private EMenu _curMenu = EMenu.None;
        private UPCtrlMailBase _curMenuCtrl;
        private UPCtrlMailBase[] _menuCtrlArray;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _menuCtrlArray = new UPCtrlMailBase[(int) EMenu.Max];

            var upCtrlMailFriend = new UPCtrlMailFriend();
            upCtrlMailFriend.SetResScenary(ResScenary);
            upCtrlMailFriend.SetMenu(EMenu.FriendMail);
            upCtrlMailFriend.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.FriendMail] = upCtrlMailFriend;
            
            var upCtrlMailSystem = new UPCtrlMailSystem();
            upCtrlMailSystem.SetResScenary(ResScenary);
            upCtrlMailSystem.SetMenu(EMenu.SystemMail);
            upCtrlMailSystem.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.SystemMail] = upCtrlMailSystem;
            
            var upCtrlMailInfoCenter = new UPCtrlMailInfoCenter();
            upCtrlMailInfoCenter.SetResScenary(ResScenary);
            upCtrlMailInfoCenter.SetMenu(EMenu.InfoCenter);
            upCtrlMailInfoCenter.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.InfoCenter] = upCtrlMailInfoCenter;

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

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (_curMenu == EMenu.None)
            {
                _cachedView.TabGroup.SelectIndex((int) EMenu.FriendMail, true);
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
        
        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlMail>();
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

        public enum EMenu
        {
            None = -1,
            FriendMail,
            SystemMail,
            InfoCenter,
            Max
        }
    }
}
