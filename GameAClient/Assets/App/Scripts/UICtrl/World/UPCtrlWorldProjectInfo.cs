/********************************************************************
** Filename : UPCtrlWorldProjectInfo.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UPCtrlWorldProjectInfo.cs
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace GameA
{
    public class UPCtrlWorldProjectInfo : UPCtrlBase<UICtrlWorld, UIViewWorld>
    {
        #region 常量与字段
        private Project _content;
        private bool _isRequestFavorite = false;
        #endregion

        #region 属性

        #endregion

        #region 方法
        public void SetData(Project project)
        {
            _content = project;
            _isRequestFavorite = false;
            if (_content == null)
            {
                SetViewEmpty();
            }
            else
            {
                RefreshView();
                RequestData();
            }
        }
        #region private
        #endregion private
        private void SetViewEmpty()
        {
            DictionaryTools.SetContentText(_cachedView.TitleText, "-");
            DictionaryTools.SetContentText(_cachedView.SubTitleText, "-");
            DictionaryTools.SetContentText(_cachedView.UserNickNameText, "-");
            DictionaryTools.SetContentText(_cachedView.UserLevelText, "");
            DictionaryTools.SetContentText(_cachedView.PlayCountText, "-");
            DictionaryTools.SetContentText(_cachedView.LikeCountText, "-");
            DictionaryTools.SetContentText(_cachedView.CompleteRateText, "-");
            DictionaryTools.SetContentText(_cachedView.Desc, "-");
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultCoverTexture);
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
            _cachedView.FavoriteBtn.gameObject.SetActive(false);
            _cachedView.UnfavoriteBtn.gameObject.SetActive(false);
        }

        private void RefreshView()
        {
            Project p = _content;
            UserInfoSimple u = _content.UserInfo;
            DictionaryTools.SetContentText(_cachedView.TitleText, p.Name);
            DictionaryTools.SetContentText(_cachedView.SubTitleText, "");
            DictionaryTools.SetContentText(_cachedView.UserNickNameText, u.NickName);
            DictionaryTools.SetContentText(_cachedView.UserLevelText, GameATools.GetLevelString(u.LevelData.PlayerLevel));
            DictionaryTools.SetContentText(_cachedView.PlayCountText, p.ExtendReady ? p.PlayCount.ToString() : "-");
            DictionaryTools.SetContentText(_cachedView.LikeCountText, p.ExtendReady ? p.LikeCount.ToString() : "-");
            DictionaryTools.SetContentText(_cachedView.CompleteRateText, p.ExtendReady ? GameATools.GetCompleteRateString(p.CompleteRate) : "-");
            DictionaryTools.SetContentText(_cachedView.Desc, p.Summary);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, u.HeadImgUrl, _cachedView.DefaultCoverTexture);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, p.IconPath, _cachedView.DefaultCoverTexture);
            RefreshFavoriteBtnView();
        }

        private void RefreshFavoriteBtnView()
        {
            bool favorite = _content.ProjectUserData != null && _content.ProjectUserData.Favorite;
            _cachedView.FavoriteBtn.gameObject.SetActive(!favorite);
            _cachedView.UnfavoriteBtn.gameObject.SetActive(favorite);
        }

        private void RequestData()
        {
            if (_content == null)
            {
                return;
            }
            Project requestP = _content;
            requestP.Request(requestP.ProjectId, () => {
                if (_content != null && _content.ProjectId == requestP.ProjectId) {
                    RefreshView();
                }
            }, code=>{

            });
        }

        private void RequestUpdateFavorite(bool favorite)
        {
            if (_content == null)
            {
                return;
            }
            if (_isRequestFavorite)
            {
                return;
            }
            Project requestP = _content;
            requestP.UpdateFavorite(favorite, () => {
                if (_content != null && _content.ProjectId == requestP.ProjectId) {
                    _isRequestFavorite = false;
                    RefreshFavoriteBtnView();
                }
            }, code=>{
                if (_content != null && _content.ProjectId == requestP.ProjectId) {
                    _isRequestFavorite = false;
                    RefreshFavoriteBtnView();
                }
            });
        }

        private void PlayProject()
        {
            if (_content == null)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, string.Format ("请求进入关卡"));

            _content.RequestPlay (() => {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);

                GameManager.Instance.RequestPlay (_content);
                SocialGUIManager.Instance.ChangeToGameMode ();
            }, (error) => {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                SocialGUIManager.ShowPopupDialog("进入关卡失败");
            });
        }
        #region 接口
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.FavoriteBtn.onClick.AddListener(OnFavoriteBtnClick);
            _cachedView.UnfavoriteBtn.onClick.AddListener(OnUnFavoriteBtnClick);
            _cachedView.PlayBtn.onClick.AddListener(OnPlayBtnClick);
        }

        private void OnFavoriteBtnClick()
        {
            RequestUpdateFavorite(true);
        }

        private void OnUnFavoriteBtnClick()
        {
            RequestUpdateFavorite(false);
        }

        private void OnPlayBtnClick()
        {
            PlayProject();
        }
        #endregion 接口

        #endregion

    }
}
