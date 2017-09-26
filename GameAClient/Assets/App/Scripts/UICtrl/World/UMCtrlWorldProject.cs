  /********************************************************************
  ** Filename : UMCtrlWorldProject.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMCtrlWorldProject.cs
  ***********************************************************************/

using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldProject : UMCtrlBase<UMViewWorldProject>, IDataItemRenderer
    {
        private CardDataRendererWrapper<Project> _wrapper;
        private Func<Project, string> _getTimeFunc;
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

        public Func<Project, string> GetTimeFunc
        {
            set { _getTimeFunc = value; }
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
            if(_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }
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
            string time;
            if (_getTimeFunc != null)
            {
                time = _getTimeFunc(p);
            }
            else
            {
                time = DateTimeUtil.GetServerSmartDateStringByTimestampMillis(p.CreateTime);
            }
            DictionaryTools.SetContentText(_cachedView.Time, time);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, p.IconPath, _cachedView.DefaultCoverTexture);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
        }
    }
}
