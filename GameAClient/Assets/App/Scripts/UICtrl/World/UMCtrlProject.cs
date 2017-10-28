using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlProject : UMCtrlBase<UMViewProject>, IDataItemRenderer
    {
        private EFunc _efunc;
        private CardDataRendererWrapper<Project> _wrapper;
        private int _index;
        public int Index { get; set; }
        private bool _newEdit;

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
            if (_wrapper != null)
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
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }
            _wrapper = obj as CardDataRendererWrapper<Project>;
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged += RefreshView;
                if (_wrapper.Content != null && _efunc == EFunc.Editing)
                {
                    _newEdit = _wrapper.Content == Project.NewEditProject;
                }
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
            _cachedView.BottomObj.SetActive(_efunc == EFunc.Published);
            _cachedView.EditImg.SetActive(_efunc == EFunc.Editing && !_newEdit);
            _cachedView.NewEditObj.SetActive(_efunc == EFunc.Editing && _newEdit);
            if (!_newEdit)
            {
                Project p = _wrapper.Content;
                DictionaryTools.SetContentText(_cachedView.PlayCountTxt, p.PlayCount.ToString());
                DictionaryTools.SetContentText(_cachedView.CommentCountTxt, p.TotalCommentCount.ToString());
                DictionaryTools.SetContentText(_cachedView.Title, p.Name);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, p.IconPath,
                    _cachedView.DefaultCoverTexture);
            }
            else
            {
                DictionaryTools.SetContentText(_cachedView.Title, string.Empty);
            }
        }

        public void SetMode(EFunc eFunc)
        {
            _efunc = eFunc;
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
        }

        public enum EFunc
        {
            Published,
            Editing,
        }
    }
}