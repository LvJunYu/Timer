using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopBasicSetting : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        private USCtrlGameSettingItem _playBGMusic;
        private USCtrlGameSettingItem _playSoundsEffects;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _playBGMusic = new USCtrlGameSettingItem();
            _playBGMusic.Init(_cachedView.PlayBackGroundMusic);
            _playSoundsEffects = new USCtrlGameSettingItem();
            _playSoundsEffects.Init(_cachedView.PlaySoundsEffects);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.SettingPannel.SetActive(true);
            UpdateAll();
        }

        public void UpdateAll()
        {
            UpdateSettingItem();
            UpdateBtns();
            UpdateInfo();
        }

        public override void Close()
        {
            base.Close();
            _cachedView.SettingPannel.SetActive(false);
        }

        private void UpdateSettingItem()
        {
            _playBGMusic.SetData(GameSettingData.Instance.PlayMusic, _mainCtrl.OnClickMusicButton);
            _playSoundsEffects.SetData(GameSettingData.Instance.PlaySoundsEffects,
                _mainCtrl.OnClickSoundsEffectsButton);
        }

        private void UpdateBtns()
        {
            var gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (gameModeWorkshopEdit != null)
            {
//                bool needSave = _gameModeWorkshopEdit.MapDirty;
//                _cachedView.SaveBtn.gameObject.SetActive(needSave);
//                _cachedView.SaveBtnFinished.SetActive(!needSave);
                bool canPublish = gameModeWorkshopEdit.CheckCanPublish();
                _cachedView.TestBtn.gameObject.SetActive(!canPublish);
//                _cachedView.TestBtnDiable.SetActive(needSave);
                _cachedView.TestBtnFinished.SetActive(canPublish);
                _cachedView.PublishBtn.gameObject.SetActive(canPublish);
                _cachedView.PublishBtnDisable.SetActive(!canPublish);
            }
        }

        private void UpdateInfo()
        {
            _cachedView.TitleInputField.text = _mainCtrl.CurProject.Name;
            _cachedView.DescInputField.text = _mainCtrl.CurProject.Summary;
        }
    }
}