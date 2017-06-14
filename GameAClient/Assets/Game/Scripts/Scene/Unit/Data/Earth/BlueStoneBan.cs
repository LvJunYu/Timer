/********************************************************************
** Filename : BlueStoneBan
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:35:08
** Summary : BlueStoneBan
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 4102, Type = typeof(BlueStoneBan))]
    public class BlueStoneBan : UnitBase
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetSortingOrderBack();
            return true;
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (checkOnly)
            {
                if (other is Magic)
                {
                    return true;
                }
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (checkOnly)
            {
                if (other is Magic)
                {
                    return true;
                }
            } 
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (checkOnly)
            {
                if (other is Magic)
                {
                    return true;
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (checkOnly)
            {
                if (other is Magic)
                {
                    return true;
                }
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }
    }
}
