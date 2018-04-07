/********************************************************************
** Filename : SwitchPress
** Author : Dong
** Date : 2017/5/8 星期一 下午 2:54:54
** Summary : SwitchPress
***********************************************************************/

using System.Collections.Generic;

namespace GameA.Game
{
    [Unit(Id = 8105, Type = typeof(SwitchPress))]
    public class SwitchPress : SwitchUnit
    {
        protected List<UnitBase> _units;
        
        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.CurScene.GetControlledUnits(_guid);
        }

        public override void OnTriggerChanged(EActiveState value)
        {
            base.OnTriggerChanged(value);
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
