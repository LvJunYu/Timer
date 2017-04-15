//  /********************************************************************
//  ** Filename : UMCtrlProjectCommentSimple.cs
//  ** Author : quan
//  ** Date : 2016/7/28 17:25
//  ** Summary : UMCtrlProjectCommentSimple.cs
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
//    public class UMCtrlProjectCommentSimple : UMCtrlBase<UMViewProjectCommentSimple>
//    {
//        private const string CommentTemplate = "<color=#536f94>{0}</color>：{1}";
//        private const string ReplyTemplate = "<color=#536f94>{0}</color>回复<color=#536f94>{1}</color>：{2}";
//        private ProjectComment _content;
//
//        protected override void OnViewCreated()
//        {
//            base.OnViewCreated();
//        }
//
//        private void RefreshView()
//        {
////            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, _content.UserInfo.HeadImgUrl, _cachedView.DefaultTexture);
//            if(_content.TargetUserInfo == null)
//            {
//                DictionaryTools.SetContentText(_cachedView.Label, string.Format(CommentTemplate, _content.UserInfo.NickName, _content.Comment));
//            }
//            else
//            {
//                DictionaryTools.SetContentText(_cachedView.Label, string.Format(ReplyTemplate, _content.UserInfo.NickName, _content.TargetUserInfo.NickName, _content.Comment));
//            }
//        }
//
//        public void Set(ProjectComment data)
//        {
//            _content = data;
//            RefreshView();
//        }
//
//        public void Show()
//        {
//            _cachedView.gameObject.SetActive(true);
//        }
//
//        public void Hide()
//        {
//            _cachedView.gameObject.SetActive(false);
//        }
//    }
//}
