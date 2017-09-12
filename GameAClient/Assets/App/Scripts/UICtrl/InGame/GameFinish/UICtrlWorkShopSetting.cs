using GameA.Game;
using SoyEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlWorkShopSetting : UICtrlAnimationBase<UIViewWorkShopSetting>
    {
        private UPCtrlWorkShopBasicSetting _upCtrlWorkShopBasicSetting;
        private UPCtrlWorkShopWinConditionSetting _upCtrlWorkShopWinConditionSetting;
//        private UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting> _curCtrl;
        private UICtrlEdit.EMode _curMode;

        private void WinConditionToggleOnValueChanged(bool arg0)
        {
            if (arg0)
            {
                _upCtrlWorkShopBasicSetting.Close();
                _upCtrlWorkShopWinConditionSetting.Open();
//                _curCtrl = _upCtrlWorkShopWinConditionSetting;
            }
        }

        private void BasicSettingToggleOnValueChanged(bool arg0)
        {
            if (arg0)
            {
                _upCtrlWorkShopWinConditionSetting.Close();
                _upCtrlWorkShopBasicSetting.Open();
//                _curCtrl = _upCtrlWorkShopBasicSetting;
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
        }

        private void OnExitBtn()
        {
            //如果在测试状态，则先退出测试状态
            if (_curMode == UICtrlEdit.EMode.EditTest)
            {
                GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                if (null != gameModeEdit)
                    gameModeEdit.ChangeMode(GameModeEdit.EMode.Edit);
            }
            SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
            GM2DGame.Instance.QuitGame(
                () => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                code => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
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
            _curMode = (UICtrlEdit.EMode) parameter;
            //默认显示设置页面
            _cachedView.BasicSettingToggle.isOn = true;
            BasicSettingToggleOnValueChanged(true);
            if (GM2DGame.Instance != null)
            {
                GM2DGame.Instance.Pause();
            }
        }

        protected override void OnClose()
        {
            GameSettingData.Instance.Save();
            if (PlayMode.Instance == null)
            {
                return;
            }
            if (GM2DGame.Instance != null)
            {
                GM2DGame.Instance.Continue();
            }
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