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
    public class Switch : BlockBase
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
            _units = DataScene2D.Instance.GetControlledUnits(_guid);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other is BulletBase)
            {
                OnTrigger();
                base.OnUpHit(other, ref y, checkOnly);
            }
            return false;
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other is BulletBase)
            {
                OnTrigger();
                return base.OnDownHit(other, ref y, checkOnly);
            }
            return false;
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other is BulletBase)
            {
                OnTrigger();
                return base.OnLeftHit(other, ref x, checkOnly);
            }
            return false;
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other is BulletBase)
            {
                OnTrigger();
                return base.OnRightHit(other, ref x, checkOnly);
            }
            return false;
        }

        protected void OnTrigger()
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
