
namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlChatInGame : UICtrlInGameBase<UIViewChatInGame>
    {
        private USCtrlChatInGame _chat;
        
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.OpenBtn.onClick.AddListener(OnOpenBtn);
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);

            _chat = new USCtrlChatInGame();
            _chat.ResScenary = ResScenary;
            _chat.Init(_cachedView.InGameChat);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefrshView();
        }

        protected override void OnClose()
        {
            _chat.Close();
            base.OnClose();
        }

        private void RefrshView()
        {
           
        }

        private void OnCloseBtn()
        {
        }

        private void OnOpenBtn()
        {
        }
    }
}