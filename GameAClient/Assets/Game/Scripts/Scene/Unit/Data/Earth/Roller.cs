/********************************************************************
** Filename : RollerUnit
** Author : Dong
** Date : 2016/10/21 星期五 下午 5:20:58
** Summary : Roller
***********************************************************************/

using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5005, Type = typeof (Roller))]
    public class Roller : BlockBase
    {
        public override IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            IntVec2 deltaImpactPos = IntVec2.zero;
            var player = unit as PlayerBase;
            //推箱子的时候额外处理
            if (player != null && player.IsHoldingBox())
            {
                switch ((EDirectionType)Rotation)
                {
                    case EDirectionType.Right:
                        deltaImpactPos.x = (int) (player.CurMaxSpeedX * 0.8f);
                        break;
                    case EDirectionType.Left:
                        deltaImpactPos.x = (int) (-player.CurMaxSpeedX * 0.8f);
                        break;
                }
                if (_eActiveState != EActiveState.Active || !UseMagic())
                {
                    return deltaImpactPos;
                }
                return deltaImpactPos + _deltaPos;
            }
            switch ((EDirectionType)Rotation)
            {
                case EDirectionType.Right:
                    deltaImpactPos.x = 50;
                    break;
                case EDirectionType.Left:
                    deltaImpactPos.x = -50;
                    break;
            }
            return deltaImpactPos + _deltaPos;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init(Rotation == (int)EDirectionType.Left ? "LeftRun" : "RightRun");
            return true;
        }
    }
}