using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlGameSettingInputKeys : UPCtrlInputKeysSettingBase<UICtrlGameSetting, UIViewGameSetting>
    {
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
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.RestoreDefaultBtn.onClick.AddListener(OnRestoreDefaultBtn);
            _cachedView.OKBtn_2.onClick.AddListener(OnOKBtn);
            _cachedView.RestoreDefaultBtn_2.onClick.AddListener(OnRestoreDefaultBtn);
            
            _cachedView.FullScreenToggle.onValueChanged.AddListener(OnFullScreenToggleValueChanged);
            _cachedView.ResolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownValueChanged);
        }

        public override void Open()
        {
            base.Open();
            UpdateScreenSettingView();
        }

        protected override void OnFullScreenToggleValueChanged(bool arg0)
        {
            base.OnFullScreenToggleValueChanged(arg0);
            _cachedView.ResolutionDropdown.options = _optionDatas;
            _cachedView.ResolutionDropdown.value = ScreenResolutionManager.Instance.SelectIndex;
        }

        private void UpdateScreenSettingView()
        {
            bool fullScreen = ScreenResolutionManager.Instance.FullScreen;
            _cachedView.WindowScreenToggle.isOn = !fullScreen;
            _cachedView.FullScreenToggle.isOn = fullScreen;
            OnFullScreenToggleValueChanged(fullScreen);
            RefreshOptions(ScreenResolutionManager.Instance.AllResolutionOptions);
            _cachedView.ResolutionDropdown.options = _optionDatas;
            _cachedView.ResolutionDropdown.value = ScreenResolutionManager.Instance.SelectIndex;
        }
    }
}