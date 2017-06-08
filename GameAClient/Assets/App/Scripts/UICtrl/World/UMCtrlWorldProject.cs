  /********************************************************************
  ** Filename : UMCtrlWorldProject.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMCtrlWorldProject.cs
  ***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldProject : UMCtrlBase<UMViewWorldProject>, IDataItemRenderer
    {
        private CardDataRendererWrapper<Project> _wrapper;
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
            get { return _wrapper.Content; }
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
            _wrapper.FireOnClick();
        }

        public void Set(object obj)
        {
            if(_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }
            _wrapper = obj as CardDataRendererWrapper<Project>;
            if(_wrapper != null)
            {
                _wrapper.OnDataChanged += RefreshView;
            }
            RefreshView();
        }

        public void RefreshView()
        {
            if(_wrapper == null)
            {
                Unload();
                return;
            }
            Project p = _wrapper.Content;
            DictionaryTools.SetContentText(_cachedView.Title, p.Name);
            if (false)//subtitle
            {
                _cachedView.SubTitle.gameObject.SetActive(true);
                DictionaryTools.SetContentText(_cachedView.SubTitle, String.Empty);
            }
            else
            {
                _cachedView.SubTitle.gameObject.SetActive(false);
            }
            DictionaryTools.SetContentText(_cachedView.PlayCount, p.ExtendData.PlayCount.ToString());
            DictionaryTools.SetContentText(_cachedView.LikeCount, p.ExtendData.LikeCount.ToString());
            DictionaryTools.SetContentText(_cachedView.CompleteRate, GameATools.GetCompleteRateString(p.CompleteRate));
            DictionaryTools.SetContentText(_cachedView.CommentCount, p.ExtendData.CommentCount.ToString());
            DictionaryTools.SetContentText(_cachedView.PublishTime, DateTimeUtil.GetServerSmartDateStringByTimestampMillis(p.PublishTime));
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, p.IconPath, _cachedView.DefaultCoverTexture);
            _cachedView.SeletedMark.SetActiveEx (_wrapper.IsSelected);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
        }
    }
}
