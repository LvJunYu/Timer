using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlProject : UMCtrlBase<UMViewProject>, IDataItemRenderer
    {
        private ECurUI _eCurUI;
        private CardDataRendererWrapper<Project> _wrapper;
        private int _index;
        public int Index { get; set; }
        private static string _newProject = "创建新关卡";
        private static string _countFormat = "({0})";

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
            bool emptyProject = _wrapper.Content == Project.EmptyProject;
            _cachedView.PublishedObj.SetActiveEx(false);
            _cachedView.AuthorObj.SetActiveEx(_eCurUI != ECurUI.Editing);
            _cachedView.DownloadObj.SetActiveEx(_eCurUI == ECurUI.Editing && !emptyProject &&
                                                _wrapper.Content.ParentId != 0);
            _cachedView.OriginalObj.SetActiveEx(_eCurUI == ECurUI.Editing && !emptyProject &&
                                                _wrapper.Content.ParentId == 0);
            _cachedView.BottomObj.SetActiveEx(_eCurUI != ECurUI.Editing);
            _cachedView.EditImg.SetActiveEx(_eCurUI == ECurUI.Editing && !emptyProject);
            _cachedView.NewEditObj.SetActiveEx(_eCurUI == ECurUI.Editing && emptyProject);
            if (emptyProject)
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
                _cachedView.TotalTxt.SetActiveEx(p.TotalCount > 0);
                DictionaryTools.SetContentText(_cachedView.TotalTxt, string.Format(_countFormat, p.TotalCount));
                DictionaryTools.SetContentText(_cachedView.AuthorTxt, p.UserInfo.NickName);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, p.IconPath,
                    _cachedView.DefaultCoverTexture);
            }
        }

        public void SetCurUI(ECurUI eCurUI)
        {
            _eCurUI = eCurUI;
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
        }

        public enum ECurUI
        {
            None,
            Editing,
            Published,
            Recommend,
            MaxScore,
            AllNewestProject,
            Follows,
            UserFavorite,
            UserPlayHistory,
            RankList,
            Search,
        }
    }
}