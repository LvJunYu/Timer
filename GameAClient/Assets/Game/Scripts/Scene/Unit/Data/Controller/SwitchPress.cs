/********************************************************************
** Filename : SwitchPress
** Author : Dong
** Date : 2017/5/8 星期一 下午 2:54:54
** Summary : SwitchPress
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 8105, Type = typeof(SwitchPress))]
    public class SwitchPress : SwitchUnit
    {
        protected List<UnitBase> _units;
        
        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.Instance.GetControlledUnits(_guid);
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            if (_units != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        if (_eActiveState == EActiveState.Active)
                        {
                            unit.OnSwitchPressStart(this);
                        }
                        else if (_eActiveState == EActiveState.Deactive)
                        {
                            unit.OnSwitchPressEnd(this);
                        }
                    }
                }
            }
        }
    }
}
