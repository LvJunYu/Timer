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

        private UPCtrlWorkShopNetBattleBasic _upCtrlWorkShopNetBattleBasic;
        private UPCtrlWorkShopNetBattleWinCondition _upCtrlWorkShopNetBattleWinCondition;
        private UPCtrlWorkShopNetBattlePlayerSetting _upCtrlWorkShopNetBattlePlayerSetting;
        private UPCtrlWorkShopLevelSetting _upCtrlWorkShopLevelSetting;
        private UPCtrlWorkShopCommonSetting _upCtrlWorkShopCommonSetting;

        private EMenu _curMenu = EMenu.None;
        private UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting> _curMenuCtrl;
        private UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>[] _menuCtrlArray;

        private UICtrlEdit.EMode _curMode;
        private bool _openGamePlaying;
        private string _originalTitle;
        private string _originalDesc;
        public Project CurProject;
        public FinishCondition CurCondition;

        public bool IsMulti
        {
            get { return _curMode == UICtrlEdit.EMode.MultiEdit || _curMode == UICtrlEdit.EMode.MultiEditTest; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitWinCondition();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.CloseBtn.onClick.AddListener(OnButtonCancleClick);
            _cachedView.Toggle01.onValueChanged.AddListener(Toggle01OnValueChanged);
            _cachedView.Toggle02.onValueChanged.AddListener(Toggle02OnValueChanged);
            _cachedView.ExitBtn.onClick.AddListener(OnRetunToAppBtn);
            _cachedView.ExitBtn_2.onClick.AddListener(OnRetunToAppBtn);
            _cachedView.ExitBtn_3.onClick.AddListener(OnRetunToAppBtn);
            _cachedView.SureBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.SureBtn_2.onClick.AddListener(OnCloseBtn);
            _cachedView.SureBtn_3.onClick.AddListener(OnCloseBtn);
            _cachedView.SureBtn.onClick.AddListener(OnSure);
            _cachedView.SureBtn_2.onClick.AddListener(OnSure);
            _cachedView.SureBtn_3.onClick.AddListener(OnSure);
            _cachedView.TestBtn.onClick.AddListener(OnTestBtn);
            _cachedView.TestBtn_2.onClick.AddListener(OnTestBtn);
            _cachedView.PublishBtn.onClick.AddListener(OnPublishBtn);
            _cachedView.PublishBtn_2.onClick.AddListener(OnPublishBtn);
            _cachedView.TitleInputField.onEndEdit.AddListener(OnTitleEndEdit);
            _cachedView.TitleInputField_2.onEndEdit.AddListener(OnTitleEndEdit);
            _cachedView.TitleInputField_3.onEndEdit.AddListener(OnTitleEndEdit);
            _cachedView.DescInputField.onEndEdit.AddListener(OnDescEndEdit);
            _cachedView.DescInputField_2.onEndEdit.AddListener(OnDescEndEdit);
            _cachedView.DescInputField_3.onEndEdit.AddListener(OnDescEndEdit);

            _upCtrlWorkShopBasicSetting = new UPCtrlWorkShopBasicSetting();
            _upCtrlWorkShopBasicSetting.Init(this, _cachedView);
            _upCtrlWorkShopWinConditionSetting = new UPCtrlWorkShopWinConditionSetting();
            _upCtrlWorkShopWinConditionSetting.Init(this, _cachedView);
            _menuCtrlArray = new UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>[(int) EMenu.Max];
            _upCtrlWorkShopNetBattleBasic = new UPCtrlWorkShopNetBattleBasic();
            _upCtrlWorkShopNetBattleBasic.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.MultiBasic] = _upCtrlWorkShopNetBattleBasic;
            _upCtrlWorkShopNetBattleWinCondition = new UPCtrlWorkShopNetBattleWinCondition();
            _upCtrlWorkShopNetBattleWinCondition.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.MultiWinCondition] = _upCtrlWorkShopNetBattleWinCondition;
            _upCtrlWorkShopNetBattlePlayerSetting = new UPCtrlWorkShopNetBattlePlayerSetting();
            _upCtrlWorkShopNetBattlePlayerSetting.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.MultiPlayerSetting] = _upCtrlWorkShopNetBattlePlayerSetting;
            _upCtrlWorkShopLevelSetting = new UPCtrlWorkShopLevelSetting();
            _upCtrlWorkShopLevelSetting.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.StandaloneLevelSetting] = _upCtrlWorkShopLevelSetting;
            _upCtrlWorkShopCommonSetting = new UPCtrlWorkShopCommonSetting();
            _upCtrlWorkShopCommonSetting.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.CommonSetting] = _upCtrlWorkShopCommonSetting;
            for (int i = 0; i < _cachedView.Togs.Length; i++)
            {
                var inx = i;
                _cachedView.Togs[i].onValueChanged.AddListener(value =>
                {
                    if (value)
                    {
                        ChangeMenu((EMenu) inx);
                    }
                });
                if (i < _menuCtrlArray.Length && null != _menuCtrlArray[i])
                {
                    _menuCtrlArray[i].Close();
                }
            }

            SetPlatform(CrossPlatformInputManager.Platform);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.DescInputField);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.DescInputField_2);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.DescInputField_3);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.TitleInputField);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.TitleInputField_2);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.TitleInputField_3);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _curMode = (UICtrlEdit.EMode) parameter;
            var gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (gameModeWorkshopEdit != null)
            {
                CurProject = gameModeWorkshopEdit.Project;
            }
            else
            {
                LogHelper.Error("GM2DGame.Instance.GameMode is null");
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
                return;
            }
            if (IsMulti)
            {
                _cachedView.SureBtn_2Txt.text = "发 布";
                _cachedView.SureBtn_3Txt.text = "发 布";
            }
            else
            {
                _cachedView.SureBtn_2Txt.text = "确 定";
                _cachedView.SureBtn_3Txt.text = "确 定";
            }
            RefreshWinCondition();
            RefreshView();
            //游戏暂停
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
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            GameSettingData.Instance.Save();
            base.OnClose();
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
        }

        protected override void OnDestroy()
        {
            _upCtrlWorkShopBasicSetting.OnDestroy();
            _upCtrlWorkShopWinConditionSetting.OnDestroy();
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                _menuCtrlArray[i].OnDestroy();
            }
            CurProject = null;
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

        private void InitWinCondition()
        {
            if (null == CurCondition)
            {
                CurCondition = new FinishCondition();
                CurCondition.SettingValue = new bool[(int) EWinCondition.WC_Max];
            }
        }

        private void RefreshWinCondition()
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
                _upCtrlWorkShopWinConditionSetting.Open();
                _upCtrlWorkShopBasicSetting.Close();
            }
        }

        private void Toggle01OnValueChanged(bool arg0)
        {
            if (arg0)
            {
                _upCtrlWorkShopBasicSetting.Open();
                _upCtrlWorkShopWinConditionSetting.Close();
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

        private void OnRetunToAppBtn()
        {
            //如果在测试状态，则先退出测试状态
            if (_curMode == UICtrlEdit.EMode.EditTest || _curMode == UICtrlEdit.EMode.MultiEditTest)
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

        private void RefreshView()
        {
            _cachedView.Togs[(int) EMenu.MultiBasic].SetActiveEx(IsMulti);
            _cachedView.Togs[(int) EMenu.MultiWinCondition].SetActiveEx(IsMulti);
            _cachedView.Togs[(int) EMenu.MultiPlayerSetting].SetActiveEx(IsMulti);
            _cachedView.Togs[(int) EMenu.StandaloneLevelSetting].SetActiveEx(!IsMulti);
//            UpdateFinishCondition();
            if (IsMulti)
            {
                _curMenu = EMenu.MultiBasic;
            }
            else
            {
                _curMenu = EMenu.StandaloneLevelSetting;
            }
            _cachedView.Togs[(int)_curMenu].isOn = true;
            ChangeMenu(_curMenu);
        }

        private void SetPlatform(EPlatform ePlatform)
        {
            _cachedView.MobilePannel.SetActive(ePlatform == EPlatform.Moblie);
            _cachedView.PCPannel.SetActive(ePlatform == EPlatform.PC);
        }

        private void ChangeMenu(EMenu menu)
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            _curMenu = menu;
            var inx = (int) _curMenu;
            if (inx < _menuCtrlArray.Length)
            {
                _curMenuCtrl = _menuCtrlArray[inx];
            }
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Open();
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
            if (content != CurProject.Summary)
            {
                var testRes = CheckTools.CheckProjectDesc(content);
                if (testRes == CheckTools.ECheckProjectSumaryResult.Success)
                {
                    CurProject.Summary = content;
                    var gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                    if (gameModeEdit != null)
                    {
                        gameModeEdit.NeedSave = true;
                    }
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
            if (content != CurProject.Name)
            {
                var testRes = CheckTools.CheckProjectName(content);
                if (testRes == CheckTools.ECheckProjectNameResult.Success)
                {
                    CurProject.Name = content;
                    var gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                    if (gameModeEdit != null)
                    {
                        gameModeEdit.NeedSave = true;
                    }
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
            var gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (null == gameModeWorkshopEdit) return;
            if (gameModeWorkshopEdit.NeedSave)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存编辑的关卡");
                gameModeWorkshopEdit.Save(() =>
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
            var gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (null != gameModeEdit)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
                gameModeEdit.ChangeMode(GameModeEdit.EMode.EditTest);
            }
        }
        
        public void OnSure()
        {
            //多人是发布
            if (IsMulti)
            {
                OnPublishBtn();
                return;
            }
            //单人保存胜利条件
            for (EWinCondition i = 0; i < EWinCondition.WC_Max; i++)
            {
                EditMode.Instance.MapStatistics.SetWinCondition(i, CurCondition.SettingValue[(int) i]);
            }
            EditMode.Instance.MapStatistics.TimeLimit = CurCondition.TimeLimit;
            EditMode.Instance.MapStatistics.LifeCount = CurCondition.LifeCount;
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.WindowClosed);
        }

        public enum EMenu
        {
            None = -1,
            MultiBasic,
            MultiWinCondition,
            MultiPlayerSetting,
            StandaloneLevelSetting,
            CommonSetting,
            Max
        }
    }
}