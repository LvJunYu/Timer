/********************************************************************
** Filename : Clay
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:40:40
** Summary : Clay
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 4011, Type = typeof(Clay))]
    public class Clay : BlockBase
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _canClimbed = true;
            return true;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            //速度变慢 跳的不高。TODO 
            return base.OnUpHit(other, ref y, checkOnly);
        }
    }
}
