using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using PlayMode = GameA.Game.PlayMode;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlWorkShopSetting : UICtrlInGameBase<UIViewWorkShopSetting>
    {
        private UPCtrlWorkShopBasicSetting _upCtrlWorkShopBasicSetting;
        private UPCtrlWorkShopWinConditionSetting _upCtrlWorkShopWinConditionSetting;
        private UPCtrlWorkShopCommonSetting _upCtrlWorkShopCommonSetting;
        private UPCtrlWorkShopLevelSetting _upCtrlWorkShopLevelSetting;
        private UICtrlEdit.EMode _curMode;
        private bool _openGamePlaying;
        private GameModeEdit _gameModeWorkshopEdit;
        private string _originalTitle;
        private string _originalDesc;
        public Project CurProject;
        public FinishCondition CurCondition;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitFinishCondition();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.CloseBtn.onClick.AddListener(OnButtonCancleClick);
            _cachedView.Toggle01.onValueChanged.AddListener(Toggle01OnValueChanged);
            _cachedView.Toggle02.onValueChanged.AddListener(Toggle02OnValueChanged);

            _upCtrlWorkShopBasicSetting = new UPCtrlWorkShopBasicSetting();
            _upCtrlWorkShopBasicSetting.Init(this, _cachedView);
            _upCtrlWorkShopWinConditionSetting = new UPCtrlWorkShopWinConditionSetting();
            _upCtrlWorkShopWinConditionSetting.Init(this, _cachedView);
            _cachedView.SureBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.ExitBtn.onClick.AddListener(OnExitBtn);

            _upCtrlWorkShopCommonSetting = new UPCtrlWorkShopCommonSetting();
            _upCtrlWorkShopCommonSetting.Init(this, _cachedView);
            _upCtrlWorkShopLevelSetting = new UPCtrlWorkShopLevelSetting();
            _upCtrlWorkShopLevelSetting.Init(this, _cachedView);
            _cachedView.SureBtn_2.onClick.AddListener(OnCloseBtn);
            _cachedView.SureBtn_3.onClick.AddListener(OnCloseBtn);
            _cachedView.ExitBtn_2.onClick.AddListener(OnExitBtn);
            _cachedView.ExitBtn_3.onClick.AddListener(OnExitBtn);
            SetPlatform(CrossPlatformInputManager.Platform);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.DescInputField);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.DescInputField_2);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.TitleInputField);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.TitleInputField_2);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _curMode = (UICtrlEdit.EMode) parameter;
            if (GM2DGame.Instance != null) _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (_gameModeWorkshopEdit != null)
                CurProject = _gameModeWorkshopEdit.Project;
            _originalTitle = CurProject.Name;
            _originalDesc = CurProject.Summary;
            UpdateFinishCondition();
            //默认显示关卡页面
            _cachedView.Toggle01.isOn = true;
            Toggle01OnValueChanged(true);
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

        protected override void OnDestroy()
        {
            _upCtrlWorkShopBasicSetting.OnDestroy();
            _upCtrlWorkShopWinConditionSetting.OnDestroy();
            _upCtrlWorkShopCommonSetting.OnDestroy();
            _upCtrlWorkShopLevelSetting.OnDestroy();
            CurProject = null;
            CurCondition = null;
            base.OnDestroy();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            Messenger<KeyCode>.AddListener(EMessengerType.OnGetInputKeyCode, OnGetInputKeyCode);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.AppGameUI;
        }

        private void InitFinishCondition()
        {
            if (null == CurCondition)
            {
                CurCondition = new FinishCondition();
                CurCondition.SettingValue = new bool[(int) EWinCondition.WC_Max];
            }
        }

        private void UpdateFinishCondition()
        {
            for (EWinCondition i = 0; i < EWinCondition.WC_Max; i++)
            {
                CurCondition.SettingValue[(int) i] = EditMode.Instance.MapStatistics.HasWinCondition(i);
            }
            CurCondition.TimeLimit = EditMode.Instance.MapStatistics.TimeLimit;
            CurCondition.LifeCount = EditMode.Instance.MapStatistics.LifeCount;
        }

        private void Toggle02OnValueChanged(bool arg0)
        {
            if (arg0)
            {
                if (CrossPlatformInputManager.Platform == EPlatform.Moblie)
                {
                    _upCtrlWorkShopWinConditionSetting.Open();
                    _upCtrlWorkShopBasicSetting.Close();
                }
                else
                {
                    _upCtrlWorkShopCommonSetting.Open();
                    _upCtrlWorkShopLevelSetting.Close();
                }
            }
        }

        private void Toggle01OnValueChanged(bool arg0)
        {
            if (arg0)
            {
                if (CrossPlatformInputManager.Platform == EPlatform.Moblie)
                {
                    _upCtrlWorkShopBasicSetting.Open();
                    _upCtrlWorkShopWinConditionSetting.Close();
                }
                else
                {
                    _upCtrlWorkShopLevelSetting.Open();
                    _upCtrlWorkShopCommonSetting.Close();
                }
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

        private void SetPlatform(EPlatform ePlatform)
        {
            _cachedView.MobilePannel.SetActive(ePlatform == EPlatform.Moblie);
            _cachedView.PCPannel.SetActive(ePlatform == EPlatform.Standalone);
            if (ePlatform == EPlatform.Standalone)
            {
                _cachedView.Tap1Txt.text = _cachedView.Tap1Txt_2.text = "关卡设置";
                _cachedView.Tap2Txt.text = _cachedView.Tap2Txt_2.text = "常规设置";
            }
            else
            {
                _cachedView.Tap1Txt.text = _cachedView.Tap1Txt_2.text = "常规设置";
                _cachedView.Tap2Txt.text = _cachedView.Tap2Txt_2.text = "胜利条件";
            }
        }

        private void OnGetInputKeyCode(KeyCode keyCode)
        {
            if (_isOpen)
            {
                _upCtrlWorkShopCommonSetting.ChangeInputKey(keyCode);
            }
        }

        public void OnClickMusicButton(bool isOn)
        {
            GameSettingData.Instance.PlayMusic = isOn;
        }

        public void OnClickSoundsEffectsButton(bool isOn)
        {
            GameSettingData.Instance.PlaySoundsEffects = isOn;
        }

        public void OnDescEndEdit(string content)
        {
            if (content != _originalDesc && CurProject != null)
            {
                var testRes = CheckTools.CheckProjectDesc(content);
                if (testRes == CheckTools.ECheckProjectSumaryResult.Success)
                {
                    _originalDesc = CurProject.Summary = content;
                    _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                    if (_gameModeWorkshopEdit != null)
                        _gameModeWorkshopEdit.NeedSave = true;
                    Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, CurProject);
                }
                else
                {
                    SocialGUIManager.ShowCheckProjectDescRes(testRes);
                }
            }
        }

        public void OnTitleEndEdit(string content)
        {
            if (content != _originalTitle && CurProject != null)
            {
                var testRes = CheckTools.CheckProjectName(content);
                if (testRes == CheckTools.ECheckProjectNameResult.Success)
                {
                    _originalTitle = CurProject.Name = content;
                    _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                    if (_gameModeWorkshopEdit != null)
                        _gameModeWorkshopEdit.NeedSave = true;
                    Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, CurProject);
                }
                else
                {
                    SocialGUIManager.ShowCheckProjectNameRes(testRes);
                }
            }
        }

        public void OnPublishBtn()
        {
            if (null == CurProject) return;
            _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (null == _gameModeWorkshopEdit) return;
            if (_gameModeWorkshopEdit.NeedSave)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存编辑的关卡");
                _gameModeWorkshopEdit.Save(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(CurProject);
                }, result =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("关卡保存失败");
                });
            }
            else
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(CurProject);
            }
        }

        public void OnTestBtn()
        {
            _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (null != _gameModeWorkshopEdit)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
                GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                if (null != gameModeEdit)
                {
                    gameModeEdit.ChangeMode(GameModeEdit.EMode.EditTest);
                }
//                _gameModeWorkshopEdit.ChangeMode(GameModeEdit.EMode.EditTest);
            }
        }
    }
}