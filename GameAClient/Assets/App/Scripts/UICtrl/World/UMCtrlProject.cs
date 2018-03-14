using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlProject : UMCtrlBase<UMViewProject>, IDataItemRenderer
    {
        private ECurUI _eCurUI;
        private CardDataRendererWrapper<Project> _wrapper;
        private int _index;
        public int Index { get; set; }
        private const string NewProject = "创建新关卡";
        private const string NoName = "未命名";
        private const string CountFormat = "({0})";

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
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
        }

        protected override void OnDestroy()
        {
            _cachedView.Button.onClick.RemoveAllListeners();
            _cachedView.HeadBtn.onClick.RemoveAllListeners();
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
            _cachedView.PublishedObj.SetActiveEx(_eCurUI == ECurUI.Editing && _wrapper.Content.MainId != 0);
            _cachedView.SingleObj.SetActiveEx(_wrapper.Content.ProjectType == EProjectType.PT_Single && !emptyProject);
            _cachedView.CooperationObj.SetActiveEx(_wrapper.Content.ProjectType == EProjectType.PT_Cooperation &&
                                                   !emptyProject);
            _cachedView.CompeteObj.SetActiveEx(_wrapper.Content.ProjectType == EProjectType.PT_Compete &&
                                               !emptyProject);
            _cachedView.AuthorObj.SetActiveEx(_eCurUI != ECurUI.Editing);
//            _cachedView.DownloadObj.SetActiveEx(false);
//            _cachedView.OriginalObj.SetActiveEx(false);
            _cachedView.BottomObj.SetActiveEx(_eCurUI != ECurUI.Editing && _eCurUI != ECurUI.Download);
            _cachedView.EditImg.SetActiveEx(_eCurUI == ECurUI.Editing && !emptyProject);
            _cachedView.NewEditObj.SetActiveEx(_eCurUI == ECurUI.Editing && emptyProject);
            if (emptyProject)
            {
                DictionaryTools.SetContentText(_cachedView.Title, NewProject);
            }
            else
            {
                Project p = _wrapper.Content;
                DictionaryTools.SetContentText(_cachedView.PlayCountTxt, p.PlayCount.ToString());
                DictionaryTools.SetContentText(_cachedView.CommentCountTxt, p.TotalCommentCount.ToString());
                if (string.IsNullOrEmpty(p.Name))
                {
                    DictionaryTools.SetContentText(_cachedView.Title, NoName);
                }
                else
                {
                    DictionaryTools.SetContentText(_cachedView.Title, p.Name);
                }

                DictionaryTools.SetContentText(_cachedView.PraiseScoreTxt, p.ScoreFormat);
                _cachedView.TotalTxt.SetActiveEx(p.TotalCount > 0);
                DictionaryTools.SetContentText(_cachedView.TotalTxt, string.Format(CountFormat, p.TotalCount));
                DictionaryTools.SetContentText(_cachedView.AuthorTxt, p.UserInfo.NickName);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, p.IconPath,
                    _cachedView.DefaultCoverTexture);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadRawImage, p.UserInfo.HeadImgUrl,
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
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadRawImage,
                _cachedView.DefaultCoverTexture);
        }

        private void OnHeadBtn()
        {
            if (_wrapper != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_wrapper.Content.UserInfoDetail);
            }
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
            Multi,
            Download
        }
    }
}