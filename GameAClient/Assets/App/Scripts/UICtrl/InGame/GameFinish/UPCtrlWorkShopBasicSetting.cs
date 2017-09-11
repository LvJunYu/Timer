using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopBasicSetting : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        private USCtrlGameSettingItem _playBGMusic;
        private USCtrlGameSettingItem _playSoundsEffects;
        private GameModeEdit _gameModeWorkshopEdit;
        private Project _curProject;
        private string _originalTitle;
        private string _originalDesc;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SaveBtn.onClick.AddListener(OnSaveBtn);
            _cachedView.TestBtn.onClick.AddListener(OnTestBtn);
            _cachedView.PublishBtn.onClick.AddListener(OnPublishBtn);
            _cachedView.TitleInputField.onEndEdit.AddListener(OnTitleEndEdit);
            _cachedView.DescInputField.onEndEdit.AddListener(OnDescEndEdit);
            _playBGMusic = new USCtrlGameSettingItem();
            _playBGMusic.Init(_cachedView.PlayBackGroundMusic);
            _playSoundsEffects = new USCtrlGameSettingItem();
            _playSoundsEffects.Init(_cachedView.PlaySoundsEffects);
        }

        private void OnDescEndEdit(string arg0)
        {
            if (arg0 != _originalDesc && _curProject != null)
            {
                _originalDesc = _curProject.Summary = arg0;
                _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                if (_gameModeWorkshopEdit != null)
                    _gameModeWorkshopEdit.NeedSave = true;
                Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _curProject);
            }
        }

        private void OnTitleEndEdit(string arg0)
        {
            if (arg0 != _originalTitle && _curProject != null)
            {
                _originalTitle = _curProject.Name = arg0;
                _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                if (_gameModeWorkshopEdit != null)
                    _gameModeWorkshopEdit.NeedSave = true;
                Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _curProject);
            }
        }

        private void OnPublishBtn()
        {
            if (null == _curProject) return;
            _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if(null == _gameModeWorkshopEdit) return;
            if (_gameModeWorkshopEdit.NeedSave)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存编辑的关卡");
                _gameModeWorkshopEdit.Save(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(_curProject);
                }, result =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("关卡保存失败");
                });
            }
            else
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(_curProject);
            }
        }

        private void OnTestBtn()
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

        private void OnSaveBtn()
        {
            _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (null != _gameModeWorkshopEdit)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存");
                _gameModeWorkshopEdit.Save(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    UpdateBtns();
                    SocialGUIManager.ShowPopupDialog("保存成功");
                }, result =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("保存失败");
                });
            }
        }

        public override void Open()
        {
            base.Open();
            _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (_gameModeWorkshopEdit != null)
                _curProject = _gameModeWorkshopEdit.Project;
            _cachedView.SettingPannel.SetActive(true);
            UpdateAll();
        }

        public void UpdateAll()
        {
            UpdateSettingItem();
            UpdateBtns();
            UpdateInfo();
            _originalTitle = _curProject.Name;
            _originalDesc = _curProject.Summary;
        }

        public override void Close()
        {
            base.Close();
            _cachedView.SettingPannel.SetActive(false);
        }

        private void OnClickMusicButton(bool isOn)
        {
            GameSettingData.Instance.PlayMusic = isOn;
        }

        private void OnClickSoundsEffectsButton(bool isOn)
        {
            GameSettingData.Instance.PlaySoundsEffects = isOn;
        }

        private void UpdateSettingItem()
        {
            _playBGMusic.SetData(GameSettingData.Instance.PlayMusic, OnClickMusicButton);
            _playSoundsEffects.SetData(GameSettingData.Instance.PlaySoundsEffects, OnClickSoundsEffectsButton);
        }

        private void UpdateBtns()
        {
            _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (_gameModeWorkshopEdit != null)
            {
                bool needSave = _gameModeWorkshopEdit.MapDirty;
                _cachedView.SaveBtn.gameObject.SetActive(needSave);
                _cachedView.SaveBtnFinished.SetActive(!needSave);
                bool needTest = !_gameModeWorkshopEdit.CheckCanPublish();
                _cachedView.TestBtn.gameObject.SetActive(needTest && !needSave);
                _cachedView.TestBtnDiable.SetActive(needSave);
                _cachedView.TestBtnFinished.SetActive(!needTest && !needSave);
                _cachedView.PublishBtn.gameObject.SetActive(!needSave && !needTest);
                _cachedView.PublishBtnDisable.SetActive(needSave || needTest);
            }
        }

        private void UpdateInfo()
        {
            _cachedView.TitleInputField.text = _curProject.Name;
            _cachedView.DescInputField.text = _curProject.Summary;
        }
    }
}