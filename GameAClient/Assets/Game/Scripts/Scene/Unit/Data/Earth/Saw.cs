/********************************************************************
** Filename : Saw
** Author : Dong
** Date : 2017/1/5 星期四 下午 8:11:40
** Summary : Saw
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4005, Type = typeof(Saw))]
    public class Saw : BlockBase
    {
        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsActor && Rotation == (int) EDirectionType.Up)
            {
                OnEffect(other, EDirectionType.Up);
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsActor && Rotation == (int) EDirectionType.Down)
            {
                OnEffect(other, EDirectionType.Down);
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsActor && Rotation == (int) EDirectionType.Left)
            {
                OnEffect(other, EDirectionType.Left);
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsActor && Rotation == (int) EDirectionType.Right)
            {
                OnEffect(other, EDirectionType.Right);
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        private void OnEffect(UnitBase other, EDirectionType eDirectionType)
        {
            other.OnHpChanged(-99999);
        }
    }
}
