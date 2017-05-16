/********************************************************************
** Filename : JetRed
** Author : Dong
** Date : 2017/4/7 星期五 下午 4:51:57
** Summary : JetRed
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 5016, Type = typeof(JetRed))]
    public class JetRed : Magic
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
