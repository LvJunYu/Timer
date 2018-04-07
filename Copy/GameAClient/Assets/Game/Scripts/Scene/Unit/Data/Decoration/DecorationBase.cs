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
            SetSortingOrderFrontest();
            return true;
        }
     
        public override void OnIntersect(UnitBase other)
        {
            if (other.IsMain)
            {
                OnTrigger(other);
            }
        }

        protected virtual void OnTrigger(UnitBase other)
        {

        }
    }
}
