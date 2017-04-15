  /********************************************************************
  ** Filename : UMCtrlRecentPlayedProjectUser.cs
  ** Author : quan
  ** Date : 2016/7/28 17:25
  ** Summary : UMCtrlRecentPlayedProjectUser.cs
  ***********************************************************************/ 

using System;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class UMCtrlRecentPlayedProjectUser : UMCtrlBase<UMViewRecentPlayedProjectUser>
    {
        private Project.PlayedProjectUser _content;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ItemBtn.onClick.AddListener(OnItemClick);
        }

        private void RefreshView()
        {
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, _content.User.HeadImgUrl, _cachedView.DefaultTexture);
            DictionaryTools.SetContentText(_cachedView.NickName, _content.User.NickName);

            _cachedView.CreateTime.text = DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_content.LastPlayTime);
        }

        public void Set(Project.PlayedProjectUser data)
        {
            _content = data;
            RefreshView();
        }

        private void OnItemClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlUserInfo>(_content.User);
        }

        public void Show()
        {
            _cachedView.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _cachedView.gameObject.SetActive(false);
        }
    }
}
