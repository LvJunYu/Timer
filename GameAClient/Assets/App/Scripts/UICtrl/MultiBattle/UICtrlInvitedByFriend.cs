
namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlInvitedByFriend : UICtrlAnimationBase<UIViewInvitedByFriend>
    {
        private EInviteType _inviteType;

        public EInviteType InviteType
        {
            get { return _inviteType; }
            set { _inviteType = value; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.CancelBtn.onClick.AddListener(OnCloseBtn);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshView();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        private void RefreshView()
        {
        }

        private void OnOKBtn()
        {
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlInviteFriend>();
        }
        
        public enum EInviteType
        {
            Room,
            Team
        }
    }
}