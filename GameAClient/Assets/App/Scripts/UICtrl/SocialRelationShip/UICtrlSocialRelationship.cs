using SoyEngine;

namespace GameA
{
    public static class RelationCommonString
    {
        public static string FollowedStr = "已关注";
        public static string FollowStr = "关注";
        public static string BlockedStr = "已屏蔽";
        public static string BlockStr = "屏蔽";
        public static string FriendStr = "相互关注";
    }

    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlSocialRelationship : UICtrlAnimationBase<UIViewSocialRelationship>
    {
        private EMenu _curMenu = EMenu.None;
        private UPCtrlBase<UICtrlSocialRelationship, UIViewSocialRelationship> _curMenuCtrl;
        private UPCtrlBase<UICtrlSocialRelationship, UIViewSocialRelationship>[] _menuCtrlArray;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnReturnBtnClick);
            _menuCtrlArray = new UPCtrlBase<UICtrlSocialRelationship, UIViewSocialRelationship>[(int) EMenu.Max];
            var upCtrlRelationshipFollow = new UPCtrlRelationshipFollow();
            upCtrlRelationshipFollow.SetResScenary(ResScenary);
            upCtrlRelationshipFollow.SetMenu(EMenu.Follow);
            upCtrlRelationshipFollow.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Follow] = upCtrlRelationshipFollow;

            var upCtrlRelationshipFans = new UPCtrlRelationshipFans();
            upCtrlRelationshipFans.SetResScenary(ResScenary);
            upCtrlRelationshipFans.SetMenu(EMenu.Fans);
            upCtrlRelationshipFans.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Fans] = upCtrlRelationshipFans;

            var upCtrlRelationshipFriends = new UPCtrlRelationshipFriends();
            upCtrlRelationshipFriends.SetResScenary(ResScenary);
            upCtrlRelationshipFriends.SetMenu(EMenu.Friends);
            upCtrlRelationshipFriends.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Friends] = upCtrlRelationshipFriends;

            var upCtrlRelationshipAddNew = new UPCtrlRelationshipAddNew();
            upCtrlRelationshipAddNew.SetResScenary(ResScenary);
            upCtrlRelationshipAddNew.SetMenu(EMenu.AddNew);
            upCtrlRelationshipAddNew.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.AddNew] = upCtrlRelationshipAddNew;

            var upCtrlRelationshipBlock = new UPCtrlRelationshipBlock();
            upCtrlRelationshipBlock.SetResScenary(ResScenary);
            upCtrlRelationshipBlock.SetMenu(EMenu.Block);
            upCtrlRelationshipBlock.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Block] = upCtrlRelationshipBlock;

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
            _groupId = (int) EUIGroupType.MainPopUpUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<UserInfoDetail>(EMessengerType.OnRelationShipChanged, OnRelationShipChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (_curMenu == EMenu.None)
            {
                _cachedView.TabGroup.SelectIndex((int) EMenu.Follow, true);
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

        private void OnReturnBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlSocialRelationship>();
        }

        private void OnRelationShipChanged(UserInfoDetail arg1)
        {
            if (_curMenuCtrl != null)
            {
                ((IOnChangeHandler<UserInfoDetail>) _curMenuCtrl).OnChangeHandler(arg1);
            }
        }

        public enum EMenu
        {
            None = -1,
            Follow,
            Fans,
            Friends,
            AddNew,
            Block,
            Max
        }
    }
}