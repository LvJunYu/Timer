using GameA.Game;
using SoyEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlWorkShopSetting : UICtrlAnimationBase<UIViewWorkShopSetting>
    {
        private UPCtrlWorkShopBasicSetting _upCtrlWorkShopBasicSetting;
        private UPCtrlWorkShopWinConditionSetting _upCtrlWorkShopWinConditionSetting;

        private void WinConditionToggleOnValueChanged(bool arg0)
        {
            if (arg0)
            {
                _upCtrlWorkShopBasicSetting.Close();
                _upCtrlWorkShopWinConditionSetting.Open();
            }
        }

        private void BasicSettingToggleOnValueChanged(bool arg0)
        {
            if (arg0)
            {
                _upCtrlWorkShopWinConditionSetting.Close();
                _upCtrlWorkShopBasicSetting.Open();
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
        }
        
        private void OnExitBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
            //SocialApp.Instance.ReturnToApp();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "...");
            GM2DGame.Instance.QuitGame (
                () => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                },
                code => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                },
                true
            );
        }
        
        private void OnSureBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.AppGameUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _upCtrlWorkShopBasicSetting = new UPCtrlWorkShopBasicSetting();
            _upCtrlWorkShopBasicSetting.Init(this, _cachedView);
            _upCtrlWorkShopWinConditionSetting = new UPCtrlWorkShopWinConditionSetting();
            _upCtrlWorkShopWinConditionSetting.Init(this, _cachedView);
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.ExitBtn.onClick.AddListener(OnExitBtn);
            _cachedView.SureBtn.onClick.AddListener(OnSureBtn);
            _cachedView.BasicSettingToggle.onValueChanged.AddListener(BasicSettingToggleOnValueChanged);
            _cachedView.WinConditionToggle.onValueChanged.AddListener(WinConditionToggleOnValueChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _upCtrlWorkShopWinConditionSetting.Close();
            _upCtrlWorkShopBasicSetting.Open();
            GameRun.Instance.Pause();
        }

        protected override void OnClose()
        {
            GameSettingData.Instance.Save();
            if (PlayMode.Instance == null)
            {
                return;
            }
            GameRun.Instance.Continue();
            Messenger.Broadcast(EMessengerType.OnCloseGameSetting);
            base.OnClose();
        }

        protected override void SetAnimationType()
        {
            base.SetAnimationType();
            _animationType = EAnimationType.PopupFromUp;
        }
    }
}