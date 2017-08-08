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
                var min = new IntVec2(other.ColliderGrid.XMax + 1, other.CenterPos.y);
                var grid = new Grid2D(min.x, min.y, min.x + ConstDefineGM2D.ServerTileScale, min.y);
                if (_colliderGrid.Intersects(grid))
                {
                    OnEffect(other, EDirectionType.Left);
                }
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsActor)
            {
                var min = new IntVec2(other.ColliderGrid.XMin - 1, other.CenterPos.y);
                var grid = new Grid2D(min.x, min.y, min.x + ConstDefineGM2D.ServerTileScale, min.y);
                if (_colliderGrid.Intersects(grid))
                {
                    OnEffect(other, EDirectionType.Right);
                }
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsActor)
            {
                var min = new IntVec2(other.CenterPos.x, other.ColliderGrid.YMax + 1);
                var grid = new Grid2D(min.x, min.y, min.x, min.y + ConstDefineGM2D.ServerTileScale);
                if (_colliderGrid.Intersects(grid))
                {
                    OnEffect(other, EDirectionType.Down);
                }
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
