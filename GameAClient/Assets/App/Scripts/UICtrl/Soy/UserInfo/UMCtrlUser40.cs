﻿  /********************************************************************
  ** Filename : UMCtrlUser40.cs
  ** Author : quan
  ** Date : 2016/7/28 17:05
  ** Summary : UMCtrlUser40.cs
  ***********************************************************************/


using SoyEngine;

namespace GameA
{
    public class UMCtrlUser40 : UMCtrlBase<UMViewUser40>
    {
        private UserInfoSimple _content;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ItemButton.onClick.AddListener(OnItemClick);
        }

        private void RefreshView()
        {
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, _content.HeadImgUrl, _cachedView.DefaultTexture);
        }

        private void OnItemClick()
        {
//            SocialGUIManager.Instance.OpenUI<UICtrlUserInfo>(_content);
        }

        public void Set(UserInfoSimple data)
        {
            _content = data;
            RefreshView();
        }

        public void Show()
        {
            _cachedView.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _cachedView.gameObject.SetActive(false);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, ImageResourceManager.DefaultTextureGuid);
        }
    }
}
