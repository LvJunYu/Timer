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
        private static string _newProject = "创建新关卡";
        private bool _emptyProject;

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
                _emptyProject = _wrapper.Content != null && _efunc == EFunc.Editing &&
                                _wrapper.Content == Project.EmptyProject;
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
            _cachedView.DownloadObj.SetActive(!_emptyProject && _efunc == EFunc.Editing &&
                                              _wrapper.Content.ParentId != 0);
            _cachedView.OriginalObj.SetActive(!_emptyProject && _efunc == EFunc.Editing &&
                                              _wrapper.Content.ParentId == 0);
            _cachedView.BottomObj.SetActive(_efunc == EFunc.Published);
            _cachedView.EditImg.SetActive(!_emptyProject && _efunc == EFunc.Editing);
            _cachedView.NewEditObj.SetActive(_emptyProject);
            if (_emptyProject)
            {
                DictionaryTools.SetContentText(_cachedView.Title, _newProject);
            }
            else
            {
                Project p = _wrapper.Content;
                DictionaryTools.SetContentText(_cachedView.PlayCountTxt, p.PlayCount.ToString());
                DictionaryTools.SetContentText(_cachedView.CommentCountTxt, p.TotalCommentCount.ToString());
                DictionaryTools.SetContentText(_cachedView.Title, p.Name);
                DictionaryTools.SetContentText(_cachedView.PraiseScoreTxt, p.ScoreFormat);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, p.IconPath,
                    _cachedView.DefaultCoverTexture);
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