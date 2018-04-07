using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlUnitPropertyAdvanceTimer : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private USCtrlSliderSetting _usTimerSecondSetting;
        private USCtrlSliderSetting _usTimerMinSecondSetting;
        private USCtrlSliderSetting _usTimerMaxSecondSetting;
        private USCtrlGameSettingItem _usTimerRandomSetting;
        private USCtrlGameSettingItem _usTimerCirculationSetting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _usTimerSecondSetting = new USCtrlSliderSetting();
            _usTimerMinSecondSetting = new USCtrlSliderSetting();
            _usTimerMaxSecondSetting = new USCtrlSliderSetting();
            _usTimerSecondSetting.Init(_cachedView.TimerSecondSetting);
            _usTimerMinSecondSetting.Init(_cachedView.TimerMinSecondSetting);
            _usTimerMaxSecondSetting.Init(_cachedView.TimerMaxSecondSetting);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usTimerSecondSetting, EAdvanceAttribute.TimerSecond,
                value => _mainCtrl.GetCurUnitExtra().TimerSecond = (byte) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usTimerMinSecondSetting, EAdvanceAttribute.TimerMinSecond,
                value =>
                {
                    _mainCtrl.GetCurUnitExtra().TimerMinSecond = (byte) value;
                    if (value > _mainCtrl.GetCurUnitExtra().TimerMaxSecond)
                    {
                        _usTimerMaxSecondSetting.SetCur(value, false);
                    }
                });
            UnitExtraHelper.SetUSCtrlSliderSetting(_usTimerMaxSecondSetting, EAdvanceAttribute.TimerMaxSecond,
                value =>
                {
                    _mainCtrl.GetCurUnitExtra().TimerMaxSecond = (byte) value;
                    if (value < _mainCtrl.GetCurUnitExtra().TimerMinSecond)
                    {
                        _usTimerMinSecondSetting.SetCur(value, false);
                    }
                });
            _usTimerRandomSetting = new USCtrlGameSettingItem();
            _usTimerRandomSetting.Init(_cachedView.TimerRandomSetting);
            _usTimerCirculationSetting = new USCtrlGameSettingItem();
            _usTimerCirculationSetting.Init(_cachedView.TimerCirculationSetting);
        }

        public override void Open()
        {
            base.Open();
            RefreshView();
        }

        public override void Close()
        {
            base.Close();
            RefreshView();
        }

        private void RefreshView()
        {
            _usTimerSecondSetting.SetEnable(_isOpen && !_mainCtrl.EditData.UnitExtra.IsRandom);
            _usTimerMinSecondSetting.SetEnable(_isOpen && _mainCtrl.EditData.UnitExtra.IsRandom);
            _usTimerMaxSecondSetting.SetEnable(_isOpen && _mainCtrl.EditData.UnitExtra.IsRandom);
            _usTimerRandomSetting.SetEnable(_isOpen);
            _usTimerCirculationSetting.SetEnable(_isOpen);
            if (_isOpen)
            {
                _usTimerSecondSetting.SetCur(_mainCtrl.EditData.UnitExtra.TimerSecond);
                _usTimerMinSecondSetting.SetCur(_mainCtrl.EditData.UnitExtra.TimerMinSecond);
                _usTimerMaxSecondSetting.SetCur(_mainCtrl.EditData.UnitExtra.TimerMaxSecond);
                _usTimerRandomSetting.SetData(_mainCtrl.EditData.UnitExtra.IsRandom, value =>
                {
                    _usTimerSecondSetting.SetEnable(!value);
                    _usTimerMinSecondSetting.SetEnable(value);
                    _usTimerMaxSecondSetting.SetEnable(value);
                    _mainCtrl.EditData.UnitExtra.IsRandom = value;
                });
                _usTimerCirculationSetting.SetData(_mainCtrl.EditData.UnitExtra.TimerCirculation,
                    value => { _mainCtrl.EditData.UnitExtra.TimerCirculation = value; });
            }
        }
    }
}