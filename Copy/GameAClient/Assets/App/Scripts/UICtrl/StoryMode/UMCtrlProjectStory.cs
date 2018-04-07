using SoyEngine;

namespace GameA
{
    public class UMCtrlProjectStory : UMCtrlBase<UMViewProjectStory>
    {
        private Project _project;
        private int _index;
        private const string NoName = "未命名";

        public void Set(Project obj)
        {
            _cachedView.PlayBtn.onClick.RemoveAllListeners();
            _project = obj;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_project == null)
            {
                return;
            }

            RefreshProjectView();
        }

        private void RefreshProjectView()
        {
            bool emptyProject = _project == null;
            if (!emptyProject)
            {
                Project p = _project;
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
                _cachedView.PlayBtn.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            if (_project == null)
            {
                return;
            }

            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(_project);
        }
    }
}