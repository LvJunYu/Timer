/********************************************************************
** Filename : Switch
** Author : Dong
** Date : 2017/3/16 星期四 下午 4:41:07
** Summary : Switch
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace GameA.Game
{
    [Unit(Id = 5101, Type = typeof(Switch))]
    public class Switch : UnitBase
    {
        protected List<UnitBase> _units; 

        protected override void Clear()
        {
            base.Clear();
            _units = null;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.Instance.GetSwitchedUnits(_guid);
        }

        public void OnHit()
        {
            if (_units != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnOtherSwitch();
                    }
                }
            }
        }
    }
}
