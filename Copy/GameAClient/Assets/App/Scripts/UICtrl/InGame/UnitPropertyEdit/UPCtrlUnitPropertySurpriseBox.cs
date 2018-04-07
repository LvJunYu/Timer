using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlUnitPropertySurpriseBox : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private USCtrlSliderSetting _usSurpriseBoxIntervalSetting;
        private USCtrlSliderSetting _usSurpriseBoxMaxCountSetting;
        private USCtrlGameSettingItem _usSurpriseBoxRandomSetting;
        private USCtrlGameSettingItem _usSurpriseBoxLimitSetting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _usSurpriseBoxIntervalSetting = new USCtrlSliderSetting();
            _usSurpriseBoxMaxCountSetting = new USCtrlSliderSetting();
            _usSurpriseBoxIntervalSetting.Init(_cachedView.SurpriseBoxIntervalSetting);
            _usSurpriseBoxMaxCountSetting.Init(_cachedView.SurpriseBoxMaxCountSetting);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usSurpriseBoxIntervalSetting, EAdvanceAttribute.SurpriseBoxInterval,
                value => _mainCtrl.EditData.UnitExtra.SurpriseBoxInterval = (byte) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usSurpriseBoxMaxCountSetting, EAdvanceAttribute.SurpriseBoxMaxCount,
                value => _mainCtrl.EditData.UnitExtra.SurpriseBoxMaxCount = (byte) value);
            _usSurpriseBoxRandomSetting = new USCtrlGameSettingItem();
            _usSurpriseBoxRandomSetting.Init(_cachedView.SurpriseBoxRandomSetting);
            _usSurpriseBoxLimitSetting = new USCtrlGameSettingItem();
            _usSurpriseBoxLimitSetting.Init(_cachedView.SurpriseBoxLimitSetting);
        }

        public void RefreshView()
        {
            _usSurpriseBoxMaxCountSetting.SetEnable(_mainCtrl.EditData.UnitExtra.SurpriseBoxCountLimit);
            _usSurpriseBoxIntervalSetting.SetCur(_mainCtrl.EditData.UnitExtra.SurpriseBoxInterval);
            _usSurpriseBoxMaxCountSetting.SetCur(_mainCtrl.EditData.UnitExtra.SurpriseBoxMaxCount);
            _usSurpriseBoxRandomSetting.SetData(_mainCtrl.EditData.UnitExtra.IsRandom,
                value => { _mainCtrl.EditData.UnitExtra.IsRandom = value; });
            _usSurpriseBoxLimitSetting.SetData(_mainCtrl.EditData.UnitExtra.SurpriseBoxCountLimit,
                value =>
                {
                    _mainCtrl.EditData.UnitExtra.SurpriseBoxCountLimit = value;
                    _usSurpriseBoxMaxCountSetting.SetEnable(value);
                });
        }
    }
}