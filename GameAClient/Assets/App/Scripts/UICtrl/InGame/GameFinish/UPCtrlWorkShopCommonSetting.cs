using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorkShopCommonSetting : UPCtrlInputKeysSettingBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        private USCtrlGameSettingItem _playBGMusic_2;
        private USCtrlGameSettingItem _playSoundsEffects_2;
        
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            if ((int) EInputKey.Max != _cachedView.UsInputKeyViews.Length)
            {
                LogHelper.Error("EInputKey.Max != _cachedView.UMInputKeyViews.Length");
                return;
            }
            _usCtrls = new USCtrlInputKeySetting[(int) EInputKey.Max];
            for (int i = 0; i < _usCtrls.Length; i++)
            {
                _usCtrls[i] = new USCtrlInputKeySetting();
                _usCtrls[i].Init(_cachedView.UsInputKeyViews[i]);
                _usCtrls[i].InitInputKey((EInputKey) i); //注意：枚举中的排序必须和数组中排序相同
                _usCtrls[i].AddBtnCallBack(StartSettingInputKey);
            }
            _cachedView.SureBtn_2.onClick.AddListener(OnOKBtn);
            _cachedView.SureBtn_3.onClick.AddListener(OnOKBtn);
            _cachedView.RestoreDefaultBtn.onClick.AddListener(OnRestoreDefaultBtn);
            _playBGMusic_2 = new USCtrlGameSettingItem();
            _playBGMusic_2.Init(_cachedView.PlayBackGroundMusic_2);
            _playSoundsEffects_2 = new USCtrlGameSettingItem();
            _playSoundsEffects_2.Init(_cachedView.PlaySoundsEffects_2);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.CommonSettingPanel.SetActive(true);
            UpdateSettingItem();
        }

        public override void Close()
        {
            base.Close();
            _cachedView.CommonSettingPanel.SetActive(false);
        }

        private void UpdateSettingItem()
        {
            _playBGMusic_2.SetData(GameSettingData.Instance.PlayMusic, _mainCtrl.OnClickMusicButton);
            _playSoundsEffects_2.SetData(GameSettingData.Instance.PlaySoundsEffects, _mainCtrl.OnClickSoundsEffectsButton);
        }
        
    }
}