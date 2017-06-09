  /********************************************************************
  ** Filename : UMCtrlRelationUser.cs
  ** Author : quan
  ** Date : 2016/6/10 10:07
  ** Summary : UMCtrlRelationUser.cs
  ***********************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlRelationUser : UMCtrlBase<UMViewRelationUser>, IDataItemRenderer
    {
        private int _index;
        private User _content;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ItemBtn.onClick.AddListener(OnClick);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtnClick);
            _cachedView.UnfollowBtn.onClick.AddListener(OnFollowBtnClick);
        }

        private void RefreshView()
        {
            if(_content == null)
            {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultTexture);
                return;
            }
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, _content.HeadImgUrl, _cachedView.DefaultTexture);
            DictionaryTools.SetContentText(_cachedView.UserNickName, _content.NickName);
            if(_content.Sex == ESex.S_None)
            {
                _cachedView.SexImg.gameObject.SetActive(false);
            }
            else
            {
                _cachedView.SexImg.gameObject.SetActive(true);
                _cachedView.SexImg.sprite = AppResourceManager.Instance.GetSprite(SpriteNameDefine.GetSexIcon(_content.Sex));
            }

            if(LocalUser.Instance.UserGuid == _content.UserId)
            {
                _cachedView.FollowBtn.gameObject.SetActive(false);
                _cachedView.UnfollowBtn.gameObject.SetActive(false);
            }
            else
            {
                if(_content.FollowedByMe)
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
        }


        private void OnClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlUserInfo>(_content);
        }

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
            User user = _content;
            _isFollowStateRequest = true;
            user.UpdateFollowState(!_content.FollowedByMe, flag=>{
                _isFollowStateRequest = false;
                if(flag)
                {
                    if(user == _content)
                    {
                        RefreshView();
                    }
//                    if(LocalUser.Instance.UserLegacy != null)
//                    {
//                        LocalUser.Instance.UserLegacy.FollowedListRequestTimer.Zero();
//                        LocalUser.Instance.UserLegacy.UserInfoRequestGameTimer.Zero();
//                    }
                    user.FollowerListRequestTimer.Zero();
                    user.UserInfoRequestGameTimer.Zero();
                }
            });
        }

        #region IDataItemRenderer implementation
        public void Set(object data)
        {
            _content = data as User;
            RefreshView();
        }
        public void Unload()
        {
        }
        public RectTransform Transform
        {
            get
            {
                return _cachedView.Trans;
            }
        }
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }
        public object Data
        {
            get
            {
                return _content;
            }
        }
        #endregion
    }
}

