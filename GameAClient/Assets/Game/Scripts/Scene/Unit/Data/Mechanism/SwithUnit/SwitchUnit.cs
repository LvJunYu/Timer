/********************************************************************
** Filename : SwitchUnit
** Author : Dong
** Date : 2017/5/8 星期一 下午 2:38:33
** Summary : SwitchUnit
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace GameA.Game
{
    public class SwitchUnit : BlockBase
    {
        protected bool _useSelfSwitch;

        protected override bool OnInit()
        {
            // 判断是否被按压式开关控制，如果是，则自己的就不起作用了。
            return base.OnInit();
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && _useSelfSwitch && _unitDesc.Rotation == (int)EDirectionType.Up)
            {
                OnSelfSwitch(other);
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && _useSelfSwitch && _unitDesc.Rotation == (int)EDirectionType.Down)
            {
                OnSelfSwitch(other);
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && _useSelfSwitch && _unitDesc.Rotation == (int)EDirectionType.Right)
            {
                OnSelfSwitch(other);
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && _useSelfSwitch && _unitDesc.Rotation == (int)EDirectionType.Left)
            {
                OnSelfSwitch(other);
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        protected virtual void OnSelfSwitch(UnitBase other)
        {
            //角色和箱子触发按压开关，还有激光
            if (other.IsHero || other is Box)
            {
                _run = !_run;
                OnTrigger();
            }
        }

        internal override void OnOtherSwitch()
        {
            base.OnOtherSwitch();
            OnTrigger();
        }

        protected virtual void OnTrigger()
        {
        }
    }
}
