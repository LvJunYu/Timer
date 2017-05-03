/********************************************************************
** Filename : Tree
** Author : Dong
** Date : 2017/5/2 星期二 下午 8:27:43
** Summary : Tree
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 7002, Type = typeof(Tree))]
    public class Tree : DecorationBase
    {
        protected UnitView _grassView;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            return true;
        }
    }
}
