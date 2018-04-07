/********************************************************************
** Filename : Flower
** Author : Dong
** Date : 2017/5/2 星期二 下午 8:28:19
** Summary : Flower
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 7004, Type = typeof(Flower))]
    public class Flower : DecorationBase
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetSortingOrderBackground();
            return true;
        }
    }
}
