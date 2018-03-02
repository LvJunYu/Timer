using System.Collections.Generic;

namespace GameA.Game
{
    [Unit(Id = 8201, Type = typeof(TimerTriggerPress))]
    public class TimerTriggerPress : SwitchTrigger
    {
        private const string TimerTriggerOn = "M1TimerTriggerOn";
        private const string TimerTriggerOff = "M1TimerTriggerOff";
        protected List<UnitBase> _gridCheckUnits = new List<UnitBase>();

        protected override void Clear()
        {
            base.Clear();
            _gridCheckUnits.Clear();
        }

        public void OnGridCheckEnter(UnitBase other)
        {
            if (_gridCheckUnits.Contains(other))
            {
                return;
            }

            _gridCheckUnits.Add(other);
            SetTrigger(EActiveState.Active);
        }

        public void OnGridCheckExit(UnitBase other)
        {
            _gridCheckUnits.Remove(other);
            if (_gridCheckUnits.Count == 0 && _units.Count == 0)
            {
                SetTrigger(EActiveState.Deactive);
            }
        }

        protected override void OnTrigger(UnitBase other)
        {
            if (other == _switchUnit || !UnitDefine.CanTrigger(other) || _units.Contains(other))
            {
                return;
            }

            _units.Add(other);
            SetTrigger(EActiveState.Deactive);
        }

        public override void UpdateLogic()
        {
            if (_trigger == EActiveState.Deactive)
            {
                if (_units.Count > 0)
                {
                    for (int i = _units.Count - 1; i >= 0; i--)
                    {
                        if (_units[i] == null || !_colliderGrid.Intersects(_units[i].ColliderGrid) ||
                            !_units[i].IsAlive)
                        {
                            _units.RemoveAt(i);
                        }
                    }
                }

                if (_gridCheckUnits.Count == 0 && _units.Count == 0)
                {
                    SetTrigger(EActiveState.Active);
                }
            }
        }

        protected override void InitAssetRotation(bool loop = false)
        {
            if (_animation == null)
            {
                if (_trigger == EActiveState.Active)
                {
                    _assetPath = TimerTriggerOn;
                }
                else
                {
                    _assetPath = TimerTriggerOff;
                }
            }
            else
            {
                _animation.Init(((EDirectionType) Rotation).ToString(), loop);
            }
        }

        protected override void ChangView()
        {
            if (_view != null)
            {
                if (_trigger == EActiveState.Active)
                {
                    _view.ChangeView(TimerTriggerOn);
                }
                else
                {
                    _view.ChangeView(TimerTriggerOff);
                }
            }
        }
    }
}