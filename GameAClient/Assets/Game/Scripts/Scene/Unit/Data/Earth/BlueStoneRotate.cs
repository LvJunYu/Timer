/********************************************************************
** Filename : BlueStoneRotate
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:35:30
** Summary : BlueStoneRotate
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4103, Type = typeof(BlueStoneRotate))]
    public class BlueStoneRotate : Magic
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init(((EDirectionType)Rotation).ToString());
            return true;
        }
    }
}
