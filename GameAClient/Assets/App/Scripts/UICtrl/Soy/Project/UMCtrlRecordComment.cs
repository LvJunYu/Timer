//  /********************************************************************
//  ** Filename : UMCtrlRecordComment.cs
//  ** Author : quan
//  ** Date : 1/9/2017 8:09 PM
//  ** Summary : UMCtrlRecordComment.cs
//  ***********************************************************************/
//
//using System;
//using System.Collections;
//using SoyEngine;
//using UnityEngine.UI;
//using UnityEngine;
//
//namespace GameA
//{
//    public class UMCtrlRecordComment : UMCtrlBase<UMViewRecordComment>, IDataItemRenderer
//    {
//        private const string CommentTemplate = "{0}";
//        private RecordComment _content;
//        private int _index;
//        private Action<UMCtrlRecordComment> _itemClickCallback;
//
//        public RecordComment Content
//        {
//            get { return _content; }
//        }
//
//        protected override void OnViewCreated()
//        {
//            base.OnViewCreated();
//            _cachedView.UserBtn.onClick.AddListener(OnIconClick);
//            _cachedView.ItemButton.onClick.AddListener(OnItemClick);
//        }
//
//        private void RefreshView()
//        {
//            if(_content == null)
//            {
//                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultTexture);
//                return;
//            }
//            DictionaryTools.SetContentText(_cachedView.NickName, _content.UserInfo.NickName);
//            DictionaryTools.SetContentText(_cachedView.CreateTime, DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_content.CreateTime));
//            DictionaryTools.SetContentText(_cachedView.Content, string.Format(CommentTemplate,  _content.Comment));
//            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, _content.UserInfo.HeadImgUrl, _cachedView.DefaultTexture);
//        }
//
//
//        private void OnIconClick()
//        {
//            SocialGUIManager.Instance.OpenUI<UICtrlUserInfo>(_content.UserInfo);
//        }
//
//        private void OnItemClick()
//        {
//            if(_itemClickCallback != null)
//            {
//                _itemClickCallback.Invoke(this);
//            }
//        }
//
//        public void SetCallback(Action<UMCtrlRecordComment> callback)
//        {
//            _itemClickCallback = callback;
//        }
//
//        #region IDataItemRenderer implementation
//        public void Set(object data)
//        {
//            _content = data as RecordComment;
//            RefreshView();
//        }
//
//        public void Unload()
//        {
//            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultTexture);
//        }
//
//        public RectTransform Transform
//        {
//            get
//            {
//                return _cachedView.Trans;
//            }
//        }
//        public int Index
//        {
//            get
//            {
//                return _index;
//            }
//            set
//            {
//                _index = value;
//            }
//        }
//        public object Data
//        {
//            get
//            {
//                return _content;
//            }
//        }
//
//        #endregion
//    }
//}
