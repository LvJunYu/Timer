using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlProjectGMCanditate : UMCtrlBase<UMViewGMProjectOfficalRecommend>, IDataItemRenderer
    {
        private CardDataRendererWrapper<WorldOfficialRecommendPrepareProject> _wrapper;
        private int _index;
        public int Index { get; set; }
        private const string NoName = "未命名";
        private GridDataScroller _gridDataScroller;

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _wrapper.Content.ProjectData; }
        }

        private Vector2 _orgPos = Vector2.zero;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SelectBtn.onClick.AddListener(OnSelectBtn);
            _cachedView.UnSelectBtn.onClick.AddListener(OnUnSelectBtn);
            _orgPos = _cachedView.ProjectRect.anchoredPosition;
        }

        protected override void OnDestroy()
        {
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
            _cachedView.ProjectRect.anchoredPosition = _orgPos;
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }

            _wrapper = obj as CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>;
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged += RefreshView;
            }

            RefreshView();
        }

        public void Unload()
        {
            Debug.Log("Unload");
        }


        public void RefreshView()
        {
            if (_wrapper == null)
            {
                return;
            }

            RefreshProjectView();
        }

        private void RefreshProjectView()
        {
            bool emptyProject = _wrapper.Content.ProjectData == null;
            if (!emptyProject)
            {
                _cachedView.ProjectObj.SetActiveEx(true);
                _cachedView.SelectBtn.SetActiveEx(true);
                _cachedView.UnSelectBtn.SetActiveEx(false);
                Project p = _wrapper.Content.ProjectData;
                if (string.IsNullOrEmpty(p.Name))
                {
                    DictionaryTools.SetContentText(_cachedView.TileText, NoName);
                }
                else
                {
                    DictionaryTools.SetContentText(_cachedView.TileText, p.Name);
                }

                ImageResourceManager.Instance.SetDynamicImage(_cachedView.ProjectBgImage, p.IconPath,
                    _cachedView.DefualtTexture);
            }
        }

        private void OnSelectBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlWorld>()
                .OnSelectGMCanditateProject(_wrapper.Content.ProjectData.MainId);
            _cachedView.UnSelectBtn.SetActiveEx(true);
        }

        private void OnUnSelectBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlWorld>()
                .OnUnSelectGMCanditateProject(_wrapper.Content.ProjectData.MainId);
            _cachedView.UnSelectBtn.SetActiveEx(false);
        }
    }
}