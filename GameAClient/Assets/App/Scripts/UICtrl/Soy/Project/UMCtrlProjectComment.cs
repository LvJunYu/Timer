//  /********************************************************************
//  ** Filename : UMCtrlProjectComment.cs
//  ** Author : quan
//  ** Date : 2016/6/12 2:17
//  ** Summary : UMCtrlProjectComment.cs
//  ***********************************************************************/
//
//
//using System;
//using System.Collections;
//using SoyEngine;
//using UnityEngine.UI;
//using UnityEngine;
//
//namespace GameA
//{
//    public class UMCtrlProjectComment : UMCtrlBase<UMViewProjectComment>, IDataItemRenderer
//    {
//        private const string CommentTemplate = "{0}";
//        private const string ReplyTemplate = "回复<color=#536f94>{0}</color>：{1}";
//        private ProjectComment _content;
//        private int _index;
//        private Action<UMCtrlProjectComment> _itemClickCallback;
//
//        public ProjectComment Content
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
//            if(_content.TargetUserInfo == null)
//            {
//                DictionaryTools.SetContentText(_cachedView.Content, string.Format(CommentTemplate,  _content.Comment));
//            }
//            else
//            {
//                DictionaryTools.SetContentText(_cachedView.Content, string.Format(ReplyTemplate, _content.TargetUserInfo.NickName, _content.Comment));
//            }
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
//        public void SetCallback(Action<UMCtrlProjectComment> callback)
//        {
//            _itemClickCallback = callback;
//        }
//
//        #region IDataItemRenderer implementation
//        public void Set(object data)
//        {
//            _content = data as ProjectComment;
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
