﻿using SoyEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlPersonalInformation : UICtrlResManagedBase<UIViewPersonalInformation>
    {
        public UserInfoDetail UserInfoDetail;
        public bool IsMyself;
        private EMenu _curMenu = EMenu.None;
        private UPCtrlPersonalInfoBase _curMenuCtrl;
        private UPCtrlPersonalInfoBase[] _menuCtrlArray;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtn);
            _cachedView.ChatBtn.onClick.AddListener(OnChatBtn);
            _cachedView.BlockBtn.onClick.AddListener(OnBlockBtn);
            _cachedView.RemoveBlockBtn.onClick.AddListener(OnRemoveBlockBtn);

            _menuCtrlArray = new UPCtrlPersonalInfoBase[(int) EMenu.Max];
            var upCtrlPersonalInfoBasicInfo = new UPCtrlPersonalInfoBasicInfo();
            upCtrlPersonalInfoBasicInfo.SetResScenary(ResScenary);
            upCtrlPersonalInfoBasicInfo.SetMenu(EMenu.BasicInfo);
            upCtrlPersonalInfoBasicInfo.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.BasicInfo] = upCtrlPersonalInfoBasicInfo;
            
            var upCtrlPersonalInfoAchieve = new UPCtrlPersonalInfoAchieve();
            upCtrlPersonalInfoAchieve.SetResScenary(ResScenary);
            upCtrlPersonalInfoAchieve.SetMenu(EMenu.AchivementInfo);
            upCtrlPersonalInfoAchieve.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.AchivementInfo] = upCtrlPersonalInfoAchieve;
            
            var upCtrlPersonalInfoDetailPublish = new UPCtrlPersonalInfoDetailPublish();
            upCtrlPersonalInfoDetailPublish.SetResScenary(ResScenary);
            upCtrlPersonalInfoDetailPublish.SetMenu(EMenu.Publish);
            upCtrlPersonalInfoDetailPublish.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Publish] = upCtrlPersonalInfoDetailPublish;

            var upCtrlPersonalInfoDetailCollect = new UPCtrlPersonalInfoDetailCollect();
            upCtrlPersonalInfoDetailCollect.SetResScenary(ResScenary);
            upCtrlPersonalInfoDetailCollect.SetMenu(EMenu.Collects);
            upCtrlPersonalInfoDetailCollect.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Collects] = upCtrlPersonalInfoDetailCollect;

            var upCtrlPersonalInforRecords = new UPCtrlPersonalInfoRecords();
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

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<UserInfoDetail>(EMessengerType.OnRelationShipChanged, OnRelationShipChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            UserInfoDetail = parameter as UserInfoDetail;
            if (null == UserInfoDetail)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlPersonalInformation>();
            }
            IsMyself = UserInfoDetail == LocalUser.Instance.User;
            _cachedView.BtnsObj.SetActiveEx(!IsMyself);
            _cachedView.AvatarRawImage.texture = SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().AvatarRenderTexture;
            RefreshBtns();
            _cachedView.TabGroup.SelectIndex((int) EMenu.BasicInfo, true);
        }

        protected override void OnClose()
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            UserInfoDetail = null;
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

        private void RefreshBtns()
        {
            if (IsMyself) return;
            bool block = UserInfoDetail.UserInfoSimple.RelationWithMe.BlockedByMe;
            bool follow = UserInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe;
            _cachedView.FollowBtn.SetActiveEx(!block && !follow);
            _cachedView.FollowDisableObj.SetActiveEx(!block && follow);
            _cachedView.ChatBtn.SetActiveEx(!block);
            _cachedView.BlockDisableObj.SetActiveEx(block);
            _cachedView.RemoveBlockBtn.SetActiveEx(block);
            _cachedView.BlockBtn.SetActiveEx(!block);
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPersonalInformation>();
        }

        private void OnFollowBtn()
        {
            LocalUser.Instance.RelationUserList.RequestFollowUser(UserInfoDetail);
        }

        private void OnBlockBtn()
        {
            LocalUser.Instance.RelationUserList.RequestBlockUser(UserInfoDetail);
        }

        private void OnRemoveBlockBtn()
        {
            LocalUser.Instance.RelationUserList.RequestRemoveBlockUser(UserInfoDetail);
        }

        private void OnChatBtn()
        {
        }

        private void OnRelationShipChanged(UserInfoDetail userInfoDetail)
        {
            if (userInfoDetail == UserInfoDetail)
            {
                RefreshBtns();
            }
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