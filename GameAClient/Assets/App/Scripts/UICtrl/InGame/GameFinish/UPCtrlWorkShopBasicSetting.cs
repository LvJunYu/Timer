using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopBasicSetting : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        private USCtrlGameSettingItem _playBGMusic;
        private USCtrlGameSettingItem _playSoundsEffects;
        private GameModeWorkshopEdit _gameModeWorkshopEdit;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SaveBtn.onClick.AddListener(OnSaveBtn);
            _cachedView.TestBtn.onClick.AddListener(OnTestBtn);
            _cachedView.PublishBtn.onClick.AddListener(OnPublishBtn);
            _playBGMusic = new USCtrlGameSettingItem();
            _playBGMusic.Init(_cachedView.PlayBackGroundMusic);
            _playSoundsEffects = new USCtrlGameSettingItem();
            _playSoundsEffects.Init(_cachedView.PlaySoundsEffects);
        }

        private void OnPublishBtn()
        {
        }

        private void OnTestBtn()
        {
            if (null == _gameModeWorkshopEdit)
                _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeWorkshopEdit;
            if (null != _gameModeWorkshopEdit)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
                _gameModeWorkshopEdit.ChangeMode(GameModeEdit.EMode.EditTest);
            }
        }

        private void OnSaveBtn()
        {
            if (null == _gameModeWorkshopEdit)
                _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeWorkshopEdit;
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
            _cachedView.SettingPannel.SetActive(true);
            UpdateSettingItem();
            UpdateBtns();
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
            if (null == _gameModeWorkshopEdit)
                _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeWorkshopEdit;
            if (_gameModeWorkshopEdit != null)
            {
                _cachedView.SaveBtn.gameObject.SetActive(_gameModeWorkshopEdit.MapDirty);
                _cachedView.SaveBtnDiable.SetActive(!_gameModeWorkshopEdit.MapDirty);
                bool canPublish = _gameModeWorkshopEdit.CheckCanPublish();
                _cachedView.TestBtn.gameObject.SetActive(!canPublish);
                _cachedView.TestBtnDiable.SetActive(canPublish);
            }
        }
    }
}