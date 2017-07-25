/********************************************************************
** Filename : Grass
** Author : Dong
** Date : 2017/5/2 星期二 下午 8:28:09
** Summary : Grass
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 7003, Type = typeof(Grass))]
    public class Grass : DecorationBase
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
