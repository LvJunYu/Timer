/********************************************************************
** Filename : Fire
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:33:46
** Summary : Fire
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 4010, Type = typeof(Fire))]
    public class Fire : BlockBase
    {
        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            OnDamage(other, checkOnly);
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            OnDamage(other, checkOnly);
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            OnDamage(other, checkOnly);
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            OnDamage(other, checkOnly);
            return base.OnRightHit(other, ref x, checkOnly);
        }

        private void OnDamage(UnitBase other, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.IsHero)
                {
                    other.OnDamage();
                }
            }
        }
    }
}
