/********************************************************************
** Filename : DecorationBase
** Author : Dong
** Date : 2017/5/3 星期三 上午 11:43:50
** Summary : DecorationBase
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public class DecorationBase : UnitBase
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _zOffset = 1f;
            return true;
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            OnTrigger();
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            OnTrigger();
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            OnTrigger();
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            OnTrigger();
            return base.OnDownHit(other, ref y, checkOnly);
        }

        protected virtual void OnTrigger()
        {

        }
    }
}
