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
        #endregion

        #region 属性

        #endregion

        #region 方法
        public RectTransform GetRoot()
        {
            return _cachedView.InfoPanel;
        }

        public void SetData(Project project)
        {
            _content = project;
            if (_content == null)
            {
                SetViewEmpty();
            }
            else
            {
                RefreshView();
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
            DictionaryTools.SetContentText(_cachedView.PlayCountText, p.ExtendReady ? p.ExtendData.PlayCount.ToString() : "-");
            DictionaryTools.SetContentText(_cachedView.LikeCountText, p.ExtendReady ? p.ExtendData.LikeCount.ToString() : "-");
            DictionaryTools.SetContentText(_cachedView.CompleteRateText, p.ExtendReady ? GameATools.GetCompleteRateString(p.CompleteRate) : "-");
            DictionaryTools.SetContentText(_cachedView.Desc, p.Summary);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, u.HeadImgUrl, _cachedView.DefaultCoverTexture);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, p.IconPath, _cachedView.DefaultCoverTexture);
            bool favorite = p.ProjectUserData != null && p.ProjectUserData.Favorite;
            _cachedView.FavoriteBtn.gameObject.SetActive(!favorite);
            _cachedView.UnfavoriteBtn.gameObject.SetActive(favorite);
        }
        #region 接口

        #endregion 接口

        #endregion

    }
}
