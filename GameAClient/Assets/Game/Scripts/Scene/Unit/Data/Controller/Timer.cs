using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 8108, Type = typeof(Timer))]
    public class Timer : SwitchUnit
    {
        protected List<UnitBase> _units;
        private bool _random;
        private bool _circulation;
        private int _second;
        private int _minSecond;
        private int _maxSecond;
        private int _timer;
        private int _endTimer;
        private bool _run;

        public override int SwitchTriggerId
        {
            get { return UnitDefine.TimerTriggerPressId; }
        }

        public override bool UseMagic()
        {
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.CurScene.GetControlledUnits(_guid);
            OnActiveStateChanged();
        }

        protected override void Clear()
        {
            _timer = 0;
            _endTimer = 0;
            _run = false;
            ShowTime(false);
            base.Clear();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_run)
            {
                if (_timer > 0)
                {
                    _timer--;
                    ShowTime(true);
                }
                else
                {
                    if (_endTimer == 0)
                    {
                        DoControl();
                        _endTimer = 49;
                    }
                }

                if (_endTimer > 0)
                {
                    _endTimer--;
                    if (_endTimer == 0)
                    {
                        SetActiveState(EActiveState.Deactive);
                        if (_circulation)
                        {
                            SetActiveState(EActiveState.Active);
                            _run = true;
                        }
                    }
                }
            }
        }

        private void ShowTime(bool show)
        {
            if (_view != null)
            {
                _view.SetTimeRenderer(show, Mathf.CeilToInt(_timer / (float) ConstDefineGM2D.FixedFrameCount));
            }
        }

        public override UnitExtraDynamic UpdateExtraData()
        {
            var unitExtra = base.UpdateExtraData();
            _random = unitExtra.TimerRandom;
            _circulation = unitExtra.TimerCirculation;
            _second = unitExtra.TimerSecond;
            _minSecond = unitExtra.TimerMinSecond;
            _maxSecond = unitExtra.TimerMaxSecond;
            return unitExtra;
        }

        public override void OnTriggerChanged(EActiveState value)
        {
//            if (value == EActiveState.Deactive && _eActiveState == EActiveState.Deactive)
//            {
//                SetActiveState(EActiveState.Active);
//            }

            if (_endTimer > 0 || _eActiveState == EActiveState.Deactive)
            {
                return;
            }

            _run = value == EActiveState.Active;
        }

        protected override void OnActiveStateChanged()
        {
            if (_eActiveState == EActiveState.Active)
            {
                _timer = (_random ? GetRandomSecond() : _second) * ConstDefineGM2D.FixedFrameCount;
                _endTimer = 0;
                ShowTime(true);
                _run = true;
            }
            else if (_eActiveState == EActiveState.Deactive)
            {
                ShowTime(false);
                _run = false;
            }
        }

        private int GetRandomSecond()
        {
            var delta = _maxSecond + 1 - _minSecond;
            if (delta > 0)
            {
                int curFrame = GameRun.Instance.LogicFrameCnt;
                return curFrame % delta + _minSecond;
            }

            LogHelper.Error("max is less than min");
            return _maxSecond;
        }

        private void DoControl()
        {
            if (_units != null && _switchTrigger != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnCtrlBySwitch();
                    }
                }
            }
        }
    }
}