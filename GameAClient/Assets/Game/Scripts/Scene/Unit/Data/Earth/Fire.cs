/********************************************************************
** Filename : Fire
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:33:46
** Summary : Fire
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4010, Type = typeof(Fire))]
    public class Fire : SkillBlock
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init("Run");
            return true;
        }

        protected override void CheckSkillHit(UnitBase other, Grid2D grid, EDirectionType eDirectionType)
        {
//            if (_colliderGrid.Intersects(grid))
            {
                OnEffect(other);
            }
        }

        public static void OnEffect(UnitBase other)
        {
            State state;
            if (!other.TryGetState(EStateType.Fire, out state))
            {
                other.AddStates(null, 21);
            }
        }
    }
}