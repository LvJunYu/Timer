/********************************************************************
** Filename : BlueStone
** Author : Dong
** Date : 2017/3/16 星期四 上午 10:37:56
** Summary : BlueStone
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8001, Type = typeof(BlueStone))]
    public class BlueStone : CollectionBase
    {
        protected override void OnTrigger(UnitBase other)
        {
            if (other.IsActor)
            {
                other.WingCount += BattleDefine.MaxWingCount;
            }
            base.OnTrigger(other);
        }
    }
}
