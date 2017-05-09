/********************************************************************
** Filename : RedJet
** Author : Dong
** Date : 2017/4/7 星期五 下午 4:51:57
** Summary : RedJet
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 5016, Type = typeof(RedJet))]
    public class RedJet : Magic
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            InitAssetRotation();
            return true;
        }
    }
}
