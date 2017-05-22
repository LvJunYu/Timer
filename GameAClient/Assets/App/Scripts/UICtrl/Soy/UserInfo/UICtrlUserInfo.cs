  /********************************************************************
  ** Filename : UICtrlUserInfo.cs
  ** Author : quan
  ** Date : 2016/6/11 19:18
  ** Summary : UICtrlUserInfo.cs
  ***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Create)]
    public class UICtrlUserInfo : UISocialContentCtrlBase<UIViewUserInfo>, IUIWithTitle
    {
        #region 常量与字段
        private const long UserInfoRequestInterval = 5 * GameTimer.Hour2Ticks;
        private bool _isRequest = false;
        private User _user;
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
            _cachedView.FollowListBtn.onClick.AddListener(OnFollowListBtnClick);
            _cachedView.FollowerListBtn.onClick.AddListener(OnFollowerListBtnClick);
            _cachedView.TagGroup.AddButton(_cachedView.PublishedProjectBtn, OnPublishedProjectTagClick);
            _cachedView.TagGroup.AddButton(_cachedView.ProjectPlayHistoryBtn, OnProjectPlayHistoryTagClick);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtnClick);
            _cachedView.UnfollowBtn.onClick.AddListener(OnFollowBtnClick);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        private void RequestData()
        {
//            if(_isRequest)
//            {
//                return;
//            }
//            User user = _user;
//            GameTimer timer = user.UserInfoRequestGameTimer;
//            if(timer.GetInterval() > UserInfoRequestInterval)
//            {
//                _isRequest = true;
//                Msg_CA_RequestUserInfo msg = new Msg_CA_RequestUserInfo();
//                msg.UserGuid = user.UserGuid;
//                NetworkManager.AppHttpClient.SendWithCb<Msg_AC_UserInfo>(SoyHttpApiPath.GetUserInfo, msg, ret=>{
//                    _isRequest = false;
//                    if(ret.ResultCode == ECommonResultCode.CRC_Error)
//                    {
//                        LogHelper.Error("Request User Info failed");
//                        return;
//                    }
//                    UserManager.Instance.OnSyncUserData(ret.UserInfo, true);
//                    if(user == _user)
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
            User user = parameter as User;
            if(user == null)
            {
                LogHelper.Error("UICtrlUserInfo Open Error, param is null");
            }
//            if(_user != user)
//            {
//                _user = user;
//                _cachedView.SoyPublishedProjects.SetUser(_user);
////                _cachedView.SoyProjectPlayHistory.SetUser(_user);
//                ScrollToTop();
//                _cachedView.TagGroup.SelectIndex(0, true);
//            }
            RequestData();
            UpdateView();
            base.OnOpen(parameter);
        }

        protected override void OnDestroy()
        {
        }

        private void UpdateView()
        {
            var user = _user;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserHeadImg, user.HeadImgUrl, _cachedView.DefaultUserHeadTexture);
            _cachedView.NickName.text = user.NickName;
            if(user.Sex == ESex.S_None)
            {
                _cachedView.SexImg.gameObject.SetActive(false);
            }
            else
            {
                _cachedView.SexImg.gameObject.SetActive(true);
                _cachedView.SexImg.sprite = AppResourceManager.Instance.GetSprite(SpriteNameDefine.GetSexIcon(user.Sex));
            }
            _cachedView.Profile.text = user.Profile;
            if(LocalUser.Instance.UserGuid == _user.UserId)
            {
                _cachedView.FollowBtn.gameObject.SetActive(false);
                _cachedView.UnfollowBtn.gameObject.SetActive(false);
            }
            else
            {
                if(_user.FollowedByMe)
                {
                    _cachedView.FollowBtn.gameObject.SetActive(false);
                    _cachedView.UnfollowBtn.gameObject.SetActive(true);
                }
                else
                {
                    _cachedView.FollowBtn.gameObject.SetActive(true);
                    _cachedView.UnfollowBtn.gameObject.SetActive(false);
                }
            }
            DictionaryTools.SetContentText(_cachedView.FollowListText, "关注 " + user.FollowCount);
            DictionaryTools.SetContentText(_cachedView.FollowerListText, "粉丝 " + user.FollowerCount);
            _cachedView.PlayerLevel.SetLevel(user.PlayerLevel);
            _cachedView.PlayerLevel.SetExp(user.PlayerExp, UserLevelUtil.GetLevelTotalExp(user.PlayerLevel));
            _cachedView.PlayerLevel.SetLevelProgress(UserLevelUtil.GetUserLevelProgress(user.PlayerLevel, user.PlayerExp));
            _cachedView.CreatorLevel.SetLevel(user.CreatorLevel);
            _cachedView.CreatorLevel.SetExp(user.CreatorExp, UserLevelUtil.GetLevelTotalExp(user.CreatorLevel));
            _cachedView.CreatorLevel.SetLevelProgress(UserLevelUtil.GetUserLevelProgress(user.CreatorLevel, user.CreatorExp));
        }

        #endregion

        #region 事件处理

        private bool _isFollowStateRequest = false;
        private void OnFollowBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }
            if(_isFollowStateRequest)
            {
                return;
            }
            User user = _user;
            _isFollowStateRequest = true;
            user.UpdateFollowState(!_user.FollowedByMe, flag=>{
                _isFollowStateRequest = false;
                if(flag)
                {
                    if(user == _user)
                    {
                        UpdateView();
                    }
					if(LocalUser.Instance.User != null)
                    {
						LocalUser.Instance.User.FollowedListRequestTimer.Zero();
						LocalUser.Instance.User.UserInfoRequestGameTimer.Zero();
                    }
                    user.FollowerListRequestTimer.Zero();
                    user.UserInfoRequestGameTimer.Zero();
                }
            });
        }

        private void OnPublishedProjectTagClick(bool flag)
        {
//            _cachedView.SoyPublishedProjects.gameObject.SetActive(flag);
//            if(flag)
//            {
//                _cachedView.SoyPublishedProjects.Refresh();
//            }
        }

        private void OnProjectPlayHistoryTagClick(bool flag)
        {
//            _cachedView.SoyProjectPlayHistory.gameObject.SetActive(flag);
//            if(flag)
//            {
//                _cachedView.SoyProjectPlayHistory.Refresh();
//            }
        }

        private void OnFollowListBtnClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlFollowedUserList>(_user);
        }

        private void OnFollowerListBtnClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlFollowerUserList>(_user);
        }

        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return _user.NickName;
        }
        #endregion
    }
}
