using SoyEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlProjectDetail : UICtrlAnimationBase<UIViewProjectDetail>
    {
        #region Fields

        private Project _project;
        private UPCtrlProjectInfo _projectInfoPanel;
        private UPCtrlProjectExtra _extraPanel;

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            var project = parameter as Project;
            SetProject(project);
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.Trans, EAnimationType.Fade);
        }

        public void SetProject(Project project)
        {
            _projectInfoPanel.SetData(project);
            _extraPanel.SetData(project);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI2;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
        }

        private void OnProjectDataChanged(long projectId)
        {
            if (!_isOpen)
            {
                return;
            }
            _projectInfoPanel.OnChangeHandler(projectId);
        }

        private void OnChangeToApp()
        {
            if (!_isOpen)
            {
                return;
            }
            _projectInfoPanel.OnChangeToApp();
            _extraPanel.OnChangeToAppMode();
        }


        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.BgBtn.onClick.AddListener(OnCloseBtn);
            _projectInfoPanel = new UPCtrlProjectInfo();
            _projectInfoPanel.Init(this, _cachedView);

            _extraPanel = new UPCtrlProjectExtra();
            _extraPanel.Init(this, _cachedView);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<long>(EMessengerType.OnProjectDataChanged, OnProjectDataChanged);
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnChangeToApp);
        }

        #endregion
    }
}