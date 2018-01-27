using GameA.Game;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlCooperation : UICtrlAnimationBase<UIViewCooperation>
    {
        private bool _hasRequested;
        
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtn);
            _cachedView.QuickJoinBtn.onClick.AddListener(OnQuickJoinBtn);
        }

        private void OnQuickJoinBtn()
        {
            RoomManager.Instance.SendRequestQuickPlay();
        }

        private void OnReturnBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlCooperation>();
        }
    }
}