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
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetSortingOrderBack();
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            InitAssetRotation(true);
            return true;
        }
    }
}
