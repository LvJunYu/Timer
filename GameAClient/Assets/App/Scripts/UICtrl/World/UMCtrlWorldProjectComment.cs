  /********************************************************************
  ** Filename : UMCtrlWorldProjectComment.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMCtrlWorldProjectComment.cs
  ***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldProjectComment : UMCtrlBase<UMViewWorldProjectComment>, IDataItemRenderer
    {
        private ProjectComment _content;
        private int _index;
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

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _content; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Button.onClick.AddListener(OnCardClick);
        }

        protected override void OnDestroy()
        {
            _cachedView.Button.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnCardClick()
        {
        }

        public void Set(object obj)
        {
            _content = obj as ProjectComment;
            RefreshView();
        }

        public void RefreshView()
        {
            if(_content == null)
            {
                Unload();
                return;
            }
            ProjectComment data = _content;
            UserInfoSimple user = data.UserInfo;
            DictionaryTools.SetContentText(_cachedView.UserName, user.NickName);
            DictionaryTools.SetContentText(_cachedView.UserLevel, GameATools.GetLevelString(user.LevelData.PlayerLevel));
            DictionaryTools.SetContentText(_cachedView.CreateTime, DateTimeUtil.GetServerSmartDateStringByTimestampMillis(data.CreateTime));
            DictionaryTools.SetContentText(_cachedView.Content, data.Comment);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl, _cachedView.DefaultIconTexture);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultIconTexture);
        }
    }
}
