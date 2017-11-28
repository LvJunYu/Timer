using SoyEngine.Proto;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlWorldProjectComment : UICtrlResManagedBase<UIViewWorldProjectComment>
    {
        private Project _project;
        private bool _onlyChangeView;
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.CancelBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.GoodTog.onValueChanged.AddListener(OnGoodTogValueChanged);
            _cachedView.BadTog.onValueChanged.AddListener(OnBadTogValueChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _project = parameter as Project;
            if (_project == null)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlWorldProjectComment>();
                return;
            }
            RefreshView();
        }

        protected override void OnClose()
        {
            _cachedView.CommentInput.text = string.Empty;
            base.OnClose();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI2;
        }

        private void RefreshView()
        {
        }

        private void OnOKBtn()
        {
            if (!string.IsNullOrEmpty(_cachedView.CommentInput.text))
            {
                _project.SendComment(_cachedView.CommentInput.text,
                    value => { SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>().Project.Request(); });
            }
            else
            {
                if (SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>().Project != null)
                {
                    SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>().Project.Request();
                }
            }
            SocialGUIManager.Instance.CloseUI<UICtrlWorldProjectComment>();
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWorldProjectComment>();
        }

        private void OnBadTogValueChanged(bool value)
        {
            if (_onlyChangeView) return;
            if (_project == null) return;
            if (value && _project.ProjectUserData.LikeState != EProjectLikeState.PLS_Unlike)
            {
                _project.UpdateLike(EProjectLikeState.PLS_Unlike);
            }
            else if (!value && _project.ProjectUserData.LikeState == EProjectLikeState.PLS_Unlike &&
                     !_cachedView.GoodTog.isOn)
            {
                _project.UpdateLike(EProjectLikeState.PLS_AllRight);
            }
        }

        private void OnGoodTogValueChanged(bool value)
        {
            if (_onlyChangeView) return;
            if (_project == null) return;
            if (value && _project.ProjectUserData.LikeState != EProjectLikeState.PLS_Like)
            {
                _project.UpdateLike(EProjectLikeState.PLS_Like);
            }
            else if (!value && _project.ProjectUserData.LikeState == EProjectLikeState.PLS_Like &&
                     !_cachedView.BadTog.isOn)
            {
                _project.UpdateLike(EProjectLikeState.PLS_AllRight);
            }
        }

        private void Clear()
        {
            _onlyChangeView = true;
            _cachedView.GoodTog.isOn = false;
            _cachedView.BadTog.isOn = false;
            _onlyChangeView = false;
        }
    }
}