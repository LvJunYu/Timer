﻿using SoyEngine;

namespace GameA
{
    public static class RelationCommonString
    {
        public const string FollowedStr = "已关注";
        public const string FollowStr = "关 注";
        public const string BlockedStr = "已屏蔽";
        public const string BlockStr = "屏蔽";
        public const string FriendStr = "相互关注";
    }

    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlSocialRelationship : UICtrlAnimationBase<UIViewSocialRelationship>, ICheckOverlay
    {
        private EMenu _curMenu = EMenu.None;
        private UPCtrlRelationshipBase _curMenuCtrl;
        private UPCtrlRelationshipBase[] _menuCtrlArray;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnReturnBtnClick);
            _menuCtrlArray = new UPCtrlRelationshipBase[(int) EMenu.Max];
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
            BadWordManger.Instance.InputFeidAddListen(_cachedView.SeachInputField);
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

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (null != parameter)
            {
                _curMenu = (EMenu) parameter;
            }
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
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                _menuCtrlArray[i].HasInited = false;
            }
            _cachedView.SeachInputField.text = string.Empty;
            base.OnClose();
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