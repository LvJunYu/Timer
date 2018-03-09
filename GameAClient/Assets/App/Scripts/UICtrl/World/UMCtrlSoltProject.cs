using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlSoltProject : UMCtrlBase<UMViewSoltProject>, IDataItemRenderer
    {
        private UMCtrlProject.ECurUI _eCurUI;
        private CardDataRendererWrapper<UserSelfRecommendProject> _wrapper;
        private int _index;
        private const string NewProject = "创建新关卡";
        private const string NoName = "未命名";
        private const string CountFormat = "({0})";
        private UserSelfRecommendProject _soltData;
        public int Index { get; set; }
        public object Data { get; private set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }


        public void Set(object obj)
        {
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }

            _wrapper = obj as CardDataRendererWrapper<UserSelfRecommendProject>;
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

            bool emptyProject = _wrapper.Content.ProjectData == Project.EmptyProject;
            _cachedView.ProjectView.PublishedObj.SetActiveEx(
                _eCurUI == UMCtrlProject.ECurUI.Editing && _wrapper.Content.ProjectData.MainId != 0);
            _cachedView.ProjectView.SingleObj.SetActiveEx(
                _wrapper.Content.ProjectData.ProjectType == EProjectType.PT_Single && !emptyProject);
            _cachedView.ProjectView.CooperationObj.SetActiveEx(
                _wrapper.Content.ProjectData.ProjectType == EProjectType.PT_Cooperation &&
                !emptyProject);
            _cachedView.ProjectView.CompeteObj.SetActiveEx(
                _wrapper.Content.ProjectData.ProjectType == EProjectType.PT_Compete &&
                !emptyProject);
            _cachedView.ProjectView.AuthorObj.SetActiveEx(_eCurUI != UMCtrlProject.ECurUI.Editing);
//            _cachedView.DownloadObj.SetActiveEx(false);
//            _cachedView.OriginalObj.SetActiveEx(false);
            _cachedView.ProjectView.BottomObj.SetActiveEx(
                _eCurUI != UMCtrlProject.ECurUI.Editing && _eCurUI != UMCtrlProject.ECurUI.Download);
            _cachedView.ProjectView.EditImg.SetActiveEx(_eCurUI == UMCtrlProject.ECurUI.Editing && !emptyProject);
            _cachedView.ProjectView.NewEditObj.SetActiveEx(_eCurUI == UMCtrlProject.ECurUI.Editing && emptyProject);
            if (emptyProject)
            {
                DictionaryTools.SetContentText(_cachedView.ProjectView.Title, NewProject);
            }
            else
            {
                Project p = _wrapper.Content.ProjectData;
                DictionaryTools.SetContentText(_cachedView.ProjectView.PlayCountTxt, p.PlayCount.ToString());
                DictionaryTools.SetContentText(_cachedView.ProjectView.CommentCountTxt,
                    p.TotalCommentCount.ToString());
                if (string.IsNullOrEmpty(p.Name))
                {
                    DictionaryTools.SetContentText(_cachedView.ProjectView.Title, NoName);
                }
                else
                {
                    DictionaryTools.SetContentText(_cachedView.ProjectView.Title, p.Name);
                }

                DictionaryTools.SetContentText(_cachedView.ProjectView.PraiseScoreTxt, p.ScoreFormat);
                _cachedView.ProjectView.TotalTxt.SetActiveEx(p.TotalCount > 0);
                DictionaryTools.SetContentText(_cachedView.ProjectView.TotalTxt,
                    string.Format(CountFormat, p.TotalCount));
                DictionaryTools.SetContentText(_cachedView.ProjectView.AuthorTxt, p.UserInfo.NickName);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.ProjectView.Cover, p.IconPath,
                    _cachedView.ProjectView.DefaultCoverTexture);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.ProjectView.HeadRawImage,
                    p.UserInfo.HeadImgUrl,
                    _cachedView.ProjectView.DefaultCoverTexture);
            }
        }

        public void SetCurUI(UMCtrlProject.ECurUI eCurUI)
        {
            _eCurUI = eCurUI;
        }

        public void Unload()
        {
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}