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

        public override void OnTriggerStart(UnitBase other)
        {
            //LogHelper.Debug("OnTriggerStart {0}", ToString() + "~" + _trans.GetInstanceID());
            if (_units != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnSwitchPressStart(this);
                    }
                }
            }
        }

        public override void OnTriggerEnd()
        {
            //LogHelper.Debug("OnTriggerEnd {0}", ToString());
            if (_units != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnSwitchPressEnd(this);
                    }
                }
            }
        }
    }
}
