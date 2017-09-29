using SoyEngine;

namespace GameA
{
    public class UPCtrlGameSettingInputKeys : UPCtrlInputKeysSettingBase<UICtrlGameSetting, UIViewGameSetting>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _settingColor = _cachedView.SettingColor;
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
        }
        
    }
}