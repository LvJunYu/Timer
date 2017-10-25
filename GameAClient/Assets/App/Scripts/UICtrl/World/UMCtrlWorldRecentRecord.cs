/********************************************************************
  ** Filename : UMCtrlWorldRecentRecord.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMCtrlWorldRecentRecord.cs
  ***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldRecentRecord : UMCtrlBase<UMViewWorldRecentRecord>, IDataItemRenderer
    {
        private CardDataRendererWrapper<Record> _wrapper;
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _wrapper.Content; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.PlayBtn.onClick.AddListener(OnCardClick);
        }

        protected override void OnDestroy()
        {
            _cachedView.PlayBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnCardClick()
        {
            _wrapper.FireOnClick();
        }

        public void Set(object obj)
        {
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }
            _wrapper = obj as CardDataRendererWrapper<Record>;
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged += RefreshView;
            }
            RefreshView();
        }

        public void RefreshView()
        {
            if (_wrapper == null)
            {
                Unload();
                return;
            }
            Record record = _wrapper.Content;
            UserInfoSimple user = record.UserInfo;
            DictionaryTools.SetContentText(_cachedView.UserName, user.NickName);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultUserIconTexture);
            DictionaryTools.SetContentText(_cachedView.DateTxt,
                DateTimeUtil.GetServerSmartDateStringByTimestampMillis(record.CreateTime));
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon,
                _cachedView.DefaultUserIconTexture);
        }
    }
}