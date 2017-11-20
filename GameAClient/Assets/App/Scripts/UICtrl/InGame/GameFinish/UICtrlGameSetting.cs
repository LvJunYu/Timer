using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using PlayMode = GameA.Game.PlayMode;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlGameSetting : UICtrlGenericBase<UIViewGameSetting>
    {
        private UPCtrlGameSettingInputKeys _upCtrlGameSettingInputKeys;
        private USCtrlGameSettingItem _showShadow;
        private USCtrlGameSettingItem _showRoute;
        private USCtrlGameSettingItem _playBGMusic;
        private USCtrlGameSettingItem _playSoundsEffects;
        private bool _openGamePlaying;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.AppGameUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<KeyCode>(EMessengerType.OnGetInputKeyCode, OnGetInputKeyCode);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            SetPlatform(CrossPlatformInputManager.Platform);
            _cachedView.LoginOut.onClick.AddListener(LoginOut);
            _cachedView.ChangePwdBtn.onClick.AddListener(OnChangePWDBtnClick);
            _cachedView.BindingBtn.onClick.AddListener(OnBindingBtnClick);

            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtn);
            _cachedView.ExitBtn.onClick.AddListener(OnExitBtn);
            _cachedView.RestartBtn.onClick.AddListener(OnRestartBtn);
            
            _cachedView.ReturnBtn_2.onClick.AddListener(OnReturnBtn);
            _cachedView.ExitBtn_2.onClick.AddListener(OnExitBtn);
            _cachedView.RestartBtn_2.onClick.AddListener(OnRestartBtn);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            UpdateSettingItem();
            if (CrossPlatformInputManager.Platform == EPlatform.Standalone)
            {
                _upCtrlGameSettingInputKeys.Open();
            }
            _cachedView.NickName.text = string.Format("账号：{0}", LocalUser.Instance.User.UserInfoSimple.NickName);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserHeadAvatar,
                LocalUser.Instance.User.UserInfoSimple.HeadImgUrl,
                _cachedView.DefaultUserHeadTexture);
            _openGamePlaying = false;
            if (GM2DGame.Instance != null)
            {
                if (GameRun.Instance.IsPlaying)
                {
                    GM2DGame.Instance.Pause();
                    _openGamePlaying = true;
                }
            }
            var isGuest = LocalUser.Instance.Account.IsGuest;
            _cachedView.BindingBtn.SetActiveEx(isGuest);
            _cachedView.ChangePwdBtn.SetActiveEx(!isGuest);
        }

        protected override void OnClose()
        {
            GameSettingData.Instance.Save();
            if (_upCtrlGameSettingInputKeys != null)
            {
                _upCtrlGameSettingInputKeys.Close();
            }
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

        private void SetPlatform(EPlatform ePlatform)
        {
            _cachedView.MobilePanel.SetActive(ePlatform == EPlatform.Moblie);
            _cachedView.PCPanel.SetActive(ePlatform == EPlatform.Standalone);
            _showShadow = new USCtrlGameSettingItem();
            _showRoute = new USCtrlGameSettingItem();
            _playBGMusic = new USCtrlGameSettingItem();
            _playSoundsEffects = new USCtrlGameSettingItem();
            if (ePlatform == EPlatform.Standalone)
            {
                _upCtrlGameSettingInputKeys = new UPCtrlGameSettingInputKeys();
                _upCtrlGameSettingInputKeys.Init(this, _cachedView);
                _showShadow.Init(_cachedView.ShowShadow_2);
                _showRoute.Init(_cachedView.ShowRoute_2);
                _playBGMusic.Init(_cachedView.PlayBackGroundMusic_2);
                _playSoundsEffects.Init(_cachedView.PlaySoundsEffects_2);
            }
            else
            {
                _showShadow.Init(_cachedView.ShowShadow);
                _showRoute.Init(_cachedView.ShowRoute);
                _playBGMusic.Init(_cachedView.PlayBackGroundMusic);
                _playSoundsEffects.Init(_cachedView.PlaySoundsEffects);
            }
        }

        private void LoginOut()
        {
            RemoteCommands.Logout(1,
                (ret) =>
                {
                    LocalUser.Instance.Account.Logout();
                    SocialApp.Instance.LoginAfterUpdateResComplete();
                    Close();
                }, null
            );
        }

        public void OnCloseBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
        }

        public void OnBindingBtnClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlBindingPhone>();
            SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
        }

        public void OnChangePWDBtnClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlChangePassword>();
            SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
        }

        public void ChangeToSettingAtHome()
        {
            _cachedView.BtnGroup1.SetActiveEx(true);
            _cachedView.BtnGroup2.SetActiveEx(false);
            _cachedView.BtnGroup1_2.SetActiveEx(true);
            _cachedView.BtnGroup2_2.SetActiveEx(false);
        }

        public void ChangeToSettingInGame()
        {
            _cachedView.BtnGroup1.SetActiveEx(false);
            _cachedView.BtnGroup2.SetActiveEx(true);
            _cachedView.BtnGroup1_2.SetActiveEx(false);
            _cachedView.BtnGroup2_2.SetActiveEx(true);
        }

        private void OnGetInputKeyCode(KeyCode keyCode)
        {
            if (_isOpen)
            {
                _upCtrlGameSettingInputKeys.ChangeInputKey(keyCode);
            }
        }

        private void UpdateSettingItem()
        {
            _playBGMusic.SetData(GameSettingData.Instance.PlayMusic, OnClickMusicButton);
            _playSoundsEffects.SetData(GameSettingData.Instance.PlaySoundsEffects, OnClickSoundsEffectsButton);
            _showShadow.SetData(GameSettingData.Instance.ShowPlayModeShadow, OnClickShowRuntimeShadow);
            _showRoute.SetData(GameSettingData.Instance.ShowEditModeShadow, OnClickShowEditShadow);
        }

        private void OnClickMusicButton(bool isOn)
        {
            GameSettingData.Instance.PlayMusic = isOn;
        }

        private void OnClickSoundsEffectsButton(bool isOn)
        {
            GameSettingData.Instance.PlaySoundsEffects = isOn;
        }

        private void OnClickShowRuntimeShadow(bool isOn)
        {
            GameSettingData.Instance.ShowPlayModeShadow = isOn;
        }

        private void OnClickShowEditShadow(bool isOn)
        {
            GameSettingData.Instance.ShowEditModeShadow = isOn;
        }

        private void OnExitBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
            GM2DGame.Instance.QuitGame(
                () => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                code => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                true
            );
        }

        private void OnReturnBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
        }

        private void OnRestartBtn()
        {
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                return;
            }

            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
            GM2DGame.Instance.GameMode.Restart(value =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                if (value)
                {
                    SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
                }
            }, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                CommonTools.ShowPopupDialog("启动失败", null,
                    new KeyValuePair<string, Action>("重试",
                        () => { CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnRestartBtn)); }),
                    new KeyValuePair<string, Action>("取消", () => { }));
            });
        }
    }
}