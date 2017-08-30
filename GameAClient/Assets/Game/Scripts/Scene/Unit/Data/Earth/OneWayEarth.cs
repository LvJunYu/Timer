/********************************************************************
** Filename : FakeEarth
** Author : Dong
** Date : 2017/1/5 星期四 下午 8:43:48
** Summary : FakeEarth
***********************************************************************/

using System;
using System.Collections;
using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4002, Type = typeof(OneWayEarth))]
    public class OneWayEarth : BlockBase
    {
        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            return false;
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            return false;
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            return false;
        }
    }
}
