using System.Collections.Generic;

namespace GameA.Game
{
    [Unit(Id = 8108, Type = typeof(Timer))]
    public class Timer : SwitchUnit
    {
        protected List<UnitBase> _units;

        public override int SwitchTriggerId
        {
            get { return UnitDefine.TimerTriggerPressId; }
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.CurScene.GetControlledUnits(_guid);
        }

        public override void OnTriggerChanged(EActiveState value)
        {
            if (_units != null && _switchTrigger != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        if (_switchTrigger.Trigger == EActiveState.Active)
                        {
                            unit.OnSwitchPressStart(this);
                        }
                        else if (_switchTrigger.Trigger == EActiveState.Deactive)
                        {
                            unit.OnSwitchPressEnd(this);
                        }
                    }
                }
            }
        }
    }
}