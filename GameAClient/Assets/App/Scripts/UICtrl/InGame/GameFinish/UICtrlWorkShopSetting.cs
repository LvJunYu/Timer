using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using PlayMode = GameA.Game.PlayMode;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlWorkShopSetting : UICtrlInGameBase<UIViewWorkShopSetting>
    {
        public enum EPlatform
        {
            Moblie,
            Standalone
        }
        
        private UPCtrlWorkShopBasicSetting _upCtrlWorkShopBasicSetting;
        private UPCtrlWorkShopWinConditionSetting _upCtrlWorkShopWinConditionSetting;
        private UICtrlEdit.EMode _curMode;
        private UPCtrlWorkShopCommonSetting _upCtrlWorkShopCommonSetting;
        private UPCtrlWorkShopLevelSetting _upCtrlWorkShopLevelSetting;

        private bool _openGamePlaying;

        private void Toggle02OnValueChanged(bool arg0)
        {
            if (arg0)
            {
                _upCtrlWorkShopBasicSetting.Close();
                _upCtrlWorkShopWinConditionSetting.Open();
                _upCtrlWorkShopCommonSetting.Close();
                _upCtrlWorkShopLevelSetting.Open();
            }
        }

        private void Toggle01OnValueChanged(bool arg0)
        {
            if (arg0)
            {
                //如果在测试状态，则先退出测试状态
//                if (_curMode == UICtrlEdit.EMode.EditTest)
//                {
//                    GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
//                    if (null != gameModeEdit)
//                        gameModeEdit.ChangeMode(GameModeEdit.EMode.Edit);
//                }
                _upCtrlWorkShopBasicSetting.Open();
                _upCtrlWorkShopWinConditionSetting.Close();
                _upCtrlWorkShopCommonSetting.Open();
                _upCtrlWorkShopLevelSetting.Close();
            }
        }
        
        private void OnButtonCancleClick()
        {
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.WindowClosed);
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

        private void UpdatePlatform()
        {
            if (RuntimeConfig.Instance.UseDebugMobileInput && Application.isEditor)
            {
                SetPlatform(EPlatform.Moblie);
                return;
            }
            if (Application.isEditor)
            {
                SetPlatform(EPlatform.Standalone);
            }
            else
            {
#if MOBILE_INPUT
                SetPlatform(EPlatform.Moblie);
#else
                SetPlatform(EPlatform.Standalone);
#endif
            }
        }

        private void SetPlatform(EPlatform ePlatform)
        {
            _cachedView.MobilePannel.SetActive(ePlatform == EPlatform.Moblie);
            _cachedView.PCPannel.SetActive(ePlatform == EPlatform.Standalone);
        }
        
        private void OnGetInputKeyCode(KeyCode keyCode)
        {
            if (_isOpen)
            {
                _upCtrlWorkShopCommonSetting.ChangeInputKey(keyCode);
            }
        }
        
        protected override void InitEventListener()
        {
            base.InitEventListener();
            Messenger<KeyCode>.AddListener(EMessengerType.OnGetInputKeyCode,OnGetInputKeyCode);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.AppGameUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.BasicSettingToggle.onValueChanged.AddListener(Toggle01OnValueChanged);
            _cachedView.WinConditionToggle.onValueChanged.AddListener(Toggle02OnValueChanged);
            
            _upCtrlWorkShopBasicSetting = new UPCtrlWorkShopBasicSetting();
            _upCtrlWorkShopBasicSetting.Init(this, _cachedView);
            _upCtrlWorkShopWinConditionSetting = new UPCtrlWorkShopWinConditionSetting();
            _upCtrlWorkShopWinConditionSetting.Init(this, _cachedView);
            _cachedView.SureBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.ExitBtn.onClick.AddListener(OnExitBtn);
           
            _upCtrlWorkShopCommonSetting = new UPCtrlWorkShopCommonSetting();
            _upCtrlWorkShopCommonSetting.Init(this,_cachedView);
            _upCtrlWorkShopLevelSetting = new UPCtrlWorkShopLevelSetting();
            _upCtrlWorkShopLevelSetting.Init(this,_cachedView);
            _cachedView.SureBtn_2.onClick.AddListener(OnCloseBtn);
            _cachedView.SureBtn_3.onClick.AddListener(OnCloseBtn);
            _cachedView.ExitBtn_2.onClick.AddListener(OnExitBtn);
            _cachedView.ExitBtn_3.onClick.AddListener(OnExitBtn);
        }

        protected override void OnDestroy()
        {
            _upCtrlWorkShopBasicSetting.OnDestroy();
            _upCtrlWorkShopWinConditionSetting.OnDestroy();
            base.OnDestroy();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            UpdatePlatform();
            _curMode = (UICtrlEdit.EMode) parameter;
            //默认显示设置页面
            _cachedView.BasicSettingToggle.isOn = true;
            Toggle01OnValueChanged(true);
            //每次打开时更新胜利条件
            _upCtrlWorkShopWinConditionSetting.UpdateData();
            _upCtrlWorkShopLevelSetting.UpdateData();
            _openGamePlaying = false;
            if (GM2DGame.Instance != null)
            {
                if (GameRun.Instance.IsPlaying)
                {
                    GM2DGame.Instance.Pause();
                    _openGamePlaying = true;
                }
            }
        }

        protected override void OnClose()
        {
            _upCtrlWorkShopCommonSetting.Close();
            GameSettingData.Instance.Save();
            if (PlayMode.Instance == null)
            {
                return;
            }
            if (GM2DGame.Instance != null && _openGamePlaying)
            {
                GM2DGame.Instance.Continue();
                _openGamePlaying = false;
            }
            Messenger.Broadcast(EMessengerType.OnCloseGameSetting);
            base.OnClose();
        }
        
        public void OnClickMusicButton(bool isOn)
        {
            GameSettingData.Instance.PlayMusic = isOn;
        }

        public void OnClickSoundsEffectsButton(bool isOn)
        {
            GameSettingData.Instance.PlaySoundsEffects = isOn;
        }
    }
}