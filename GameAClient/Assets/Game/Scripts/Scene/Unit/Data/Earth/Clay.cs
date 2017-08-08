/********************************************************************
** Filename : Clay
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:40:40
** Summary : Clay
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 4011, Type = typeof(Clay))]
    public class Clay : BlockBase
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

        public override bool StepOnClay()
        {
            return true;
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsActor)
            {
                OnEffect(other, EDirectionType.Left);
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsActor)
            {
                OnEffect(other, EDirectionType.Right);
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsActor)
            {
                OnEffect(other, EDirectionType.Down);
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public static void OnEffect(UnitBase other, EDirectionType eDirectionType)
        {
            switch (eDirectionType)
            {
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
