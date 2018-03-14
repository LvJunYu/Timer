using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlUnitPropertyCycle : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private USCtrlSliderSetting _usCycleCdSetting;
        private USCtrlGameSettingItem _usCycleSetting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            var sliderView = _cachedView.CycleDock.GetComponentInChildren<USViewSliderSetting>();
            _usCycleCdSetting = new USCtrlSliderSetting();
            _usCycleCdSetting.Init(sliderView);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usCycleCdSetting, EAdvanceAttribute.CycleInterval,
                value => _mainCtrl.GetCurUnitExtra().CycleInterval = (byte) value);
            
            var cycleSettingView = _cachedView.CycleDock.GetComponentInChildren<USViewGameSettingItem>();
            _usCycleSetting = new USCtrlGameSettingItem();
            _usCycleSetting.Init(cycleSettingView);
        }

        public void RefreshView()
        {
            var unitExtra = _mainCtrl.GetCurUnitExtra();
            _usCycleCdSetting.SetEnable(unitExtra.TimerCirculation);
            _usCycleCdSetting.SetCur(unitExtra.CycleInterval);
            _usCycleSetting.SetData(_mainCtrl.EditData.UnitExtra.TimerCirculation,
                value =>
                {
                    _mainCtrl.GetCurUnitExtra().TimerCirculation = value;
                    _usCycleCdSetting.SetEnable(value);
                });
        }
    }
}