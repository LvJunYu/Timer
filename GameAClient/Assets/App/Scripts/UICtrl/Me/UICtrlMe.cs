/********************************************************************
** Filename : UICtrlSoy
** Author : Dong
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSoy
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlMe : UISocialContentCtrlBase<UIViewMe>, IUIWithTitle, IUIWithTaskBar, IUIWithRightCustomButton
    {
        #region 常量与字段
        private const long UserInfoRequestInterval = 1 * GameTimer.Hour2Ticks;
        private bool _isRequest = false;
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.LoginBtn.onClick.AddListener(OnLoginBtnClick);
            _cachedView.RegisterBtn.onClick.AddListener(OnRegisterBtnClick);
            _cachedView.EditUserInfoBtn.onClick.AddListener(OnEditUserInfoClick);
            _cachedView.UserInfoBtn.onClick.AddListener(OnUserInfoClick);
            _cachedView.FollowListBtn.onClick.AddListener(OnFollowListBtnClick);
            _cachedView.FollowerListBtn.onClick.AddListener(OnFollowerListBtnClick);

            _cachedView.MyPublishedProjectBtn.Button.onClick.AddListener(OnMyPublishedProjectBtnClick);
            _cachedView.MyRecordBtn.Button.onClick.AddListener(OnMyRecordBtnClick);
            _cachedView.MyFavoriteProjectBtn.Button.onClick.AddListener(OnMyFavoriteProjectBtnClick);
            _cachedView.MyPlayHistoryBtn.Button.onClick.AddListener(OnProjectPlayHistoryBtnClick);

            _cachedView.ProjectCommentRemindBtn.Button.onClick.AddListener(OnProjectCommentRemindBtnClick);
            _cachedView.ProjectReplyRemindBtn.Button.onClick.AddListener(OnProjectReplyRemindBtnClick);
            _cachedView.ProjectRateRemindBtn.Button.onClick.AddListener(OnProjectRateRemindBtnClick);
            _cachedView.AnnounceBtn.Button.onClick.AddListener(OnAnnounceBtnClick);
            _cachedView.NewFollowerRemindBtn.Button.onClick.AddListener(OnNewFollowerRemindBtnClick);

            _cachedView.MyPublishedProjectBtn.ClearNewTip();
            _cachedView.MyFavoriteProjectBtn.ClearNewTip();
            ClearAllNotificationTip();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(SoyEngine.EMessengerType.OnAccountLoginStateChanged, OnAccountLoginStateChanged);
            RegisterEvent(EMessengerType.OnNotificationChanged, OnNotificationChanged);
        }

        private void RequestData()
        {
//            if(LocalUser.Instance.UserLegacy == null)
//            {
//                return;
//            }
//            if(_isRequest)
//            {
//                return;
//            }
//            User user = LocalUser.Instance.UserLegacy;
//            GameTimer timer = user.UserInfoRequestGameTimer;
//            if(timer.GetInterval() > UserInfoRequestInterval)
//            {
//                _isRequest = true;
//                Msg_CS_DAT_UserInfoDetail msg = new Msg_CS_DAT_UserInfoDetail();
//                msg.UserId = user.UserId;
//                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserInfoDetail>(SoyHttpApiPath.UserInfoDetail, msg, ret=>{
//                    _isRequest = false;
//                    UserManager.Instance.OnSyncUserData(ret, true);
//                    if(user == LocalUser.Instance.UserLegacy)
//                    {
//                        user.UserInfoRequestGameTimer.Reset();
//                        UpdateView();
//                    }
//                }, (code, msgStr)=>{
//                    _isRequest = false;
//                });
//            }
        }

        protected override void OnOpen(object parameter)
        {
            RequestData();
            UpdateView();
            base.OnOpen(parameter);
        }

        private void UpdateView()
        {
//            if(LocalUser.Instance.Account.HasLogin)
//            {
//                var user = LocalUser.Instance.UserLegacy;
//
//                _cachedView.UserInfoDock.SetActive(true);
//                _cachedView.LoginBtnDock.SetActive(false);
//
//                ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserHeadImg, user.HeadImgUrl, _cachedView.DefaultUserHeadTexture);
//                _cachedView.NickName.text = user.NickName;
//                if(user.Sex == ESex.S_None)
//                {
//                    _cachedView.SexImg.gameObject.SetActive(false);
//                }
//                else
//                {
//                    _cachedView.SexImg.gameObject.SetActive(true);
//                    _cachedView.SexImg.sprite = AppResourceManager.Instance.GetSprite(SpriteNameDefine.GetSexIcon(user.Sex));
//                }
//                _cachedView.Profile.text = user.Profile;
//                DictionaryTools.SetContentText(_cachedView.FollowListText, "关注 " + user.FollowCount);
//                DictionaryTools.SetContentText(_cachedView.FollowerListText, "粉丝 " + user.FollowerCount);
//                _cachedView.PlayerLevel.SetLevel(user.PlayerLevel);
//                _cachedView.PlayerLevel.SetExp(user.PlayerExp, UserLevelUtil.GetLevelTotalExp(user.PlayerLevel));
//                _cachedView.PlayerLevel.SetLevelProgress(UserLevelUtil.GetUserLevelProgress(user.PlayerLevel, user.PlayerExp));
//                _cachedView.CreatorLevel.SetLevel(user.CreatorLevel);
//                _cachedView.CreatorLevel.SetExp(user.CreatorExp, UserLevelUtil.GetLevelTotalExp(user.CreatorLevel));
//                _cachedView.CreatorLevel.SetLevelProgress(UserLevelUtil.GetUserLevelProgress(user.CreatorLevel, user.CreatorExp));
//                DictionaryTools.SetContentText(_cachedView.CurrencyText, string.Format("金币：{0}", user.GoldCoin));
//            }
//            else
//            {
//                _cachedView.UserInfoDock.SetActive(false);
//                _cachedView.LoginBtnDock.SetActive(true);
//                DictionaryTools.SetContentText(_cachedView.FollowListText, "关注 " + 0);
//                DictionaryTools.SetContentText(_cachedView.FollowerListText, "粉丝 " + 0);
//                _cachedView.PlayerLevel.SetLevel(0);
//                _cachedView.PlayerLevel.SetExp(0, 0);
//                _cachedView.PlayerLevel.SetLevelProgress(0);
//                _cachedView.CreatorLevel.SetLevel(0);
//                _cachedView.CreatorLevel.SetExp(0, 0);
//                _cachedView.CreatorLevel.SetLevelProgress(0);
//                DictionaryTools.SetContentText(_cachedView.CurrencyText, "未登录");
//                ClearAllNotificationTip();
//            }
        }

        private void ClearAllNotificationTip()
        {
            _cachedView.ProjectCommentRemindBtn.ClearNewTip();
            _cachedView.ProjectReplyRemindBtn.ClearNewTip();
            _cachedView.ProjectRateRemindBtn.ClearNewTip();
            _cachedView.AnnounceBtn.ClearNewTip();
            _cachedView.NewFollowerRemindBtn.ClearNewTip();
        }

        #endregion

        #region 事件处理
        private void OnAccountLoginStateChanged()
        {
            UpdateView();
        }

        private void OnNotificationChanged()
        {
        }

        private void OnLoginBtnClick()
        {
//            SocialGUIManager.Instance.OpenPopupUI<UICtrlLogin>();
        }

        private void OnRegisterBtnClick()
        {
//            SocialGUIManager.Instance.OpenPopupUI<UICtrlSignup>();
        }

        private void OnEditUserInfoClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlMyUserInfoModify>();
        }

        private void OnUserInfoClick()
        {
//            SocialGUIManager.Instance.OpenUI<UICtrlUserInfo>(LocalUser.Instance.UserLegacy);
        }

        private void OnFollowListBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlFollowedUserList>(LocalUser.Instance.UserLegacy);
        }

        private void OnFollowerListBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlFollowerUserList>(LocalUser.Instance.UserLegacy);
        }

        private void OnMyPublishedProjectBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlPublishedProjects>(LocalUser.Instance.UserLegacy);
        }

        private void OnMyRecordBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlUserRecords>(LocalUser.Instance.User);
        }

        private void OnMyFavoriteProjectBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlUserFavorite>();
        }

        private void OnProjectPlayHistoryBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlProjectPlayHistory>(LocalUser.Instance.User);
        }

        private void OnProjectCommentRemindBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlProjectCommentRemind>();
        }

        private void OnProjectReplyRemindBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlProjectReplyRemind>();
        }

        private void OnProjectRateRemindBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlProjectRateRemind>();
        }

        private void OnAnnounceBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlAnnounce>();
        }

        private void OnNewFollowerRemindBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
//            SocialGUIManager.Instance.OpenUI<UICtrlNewFollowerRemind>();
        }

        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "我的";
        }

        public UnityEngine.UI.Button GetRightButton()
        {
            return _cachedView.RightSettingButtonResource;
        }

//        public void OnRightButtonClick(UICtrlTitlebar titleBar)
//        {
//            SocialGUIManager.Instance.OpenUI<UICtrlSetting>();
//        }
        #endregion
    }
}
