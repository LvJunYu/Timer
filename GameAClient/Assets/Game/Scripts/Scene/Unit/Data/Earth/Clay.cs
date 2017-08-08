/********************************************************************
** Filename : Clay
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:40:40
** Summary : Clay
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4011, Type = typeof(Clay))]
    public class Clay : SkillBlock
    {
        public override bool CanClimbed
        {
            get { return true; }
        }

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
            if (_colliderGrid.Intersects(grid))
            {
                OnEffect(other, eDirectionType);
            }
        }

        public static void OnEffect(UnitBase other, EDirectionType eDirectionType)
        {
            switch (eDirectionType)
            {
                case EDirectionType.Up:
                    other.SetStepOnClay();
                    break;
                case EDirectionType.Down:
                    other.SetClimbState(EClimbState.Up);
                    break;
                case EDirectionType.Left:
                    other.SetClimbState(EClimbState.Right);
                    break;
                case EDirectionType.Right:
                    other.SetClimbState(EClimbState.Left);
                    break;
            }
        }
    }
}
