/********************************************************************
** Filename : RollerUnit
** Author : Dong
** Date : 2016/10/21 星期五 下午 5:20:58
** Summary : Roller
***********************************************************************/

using SoyEngine;
using Spine.Unity;

namespace GameA.Game
{
    [Unit(Id = 5005, Type = typeof (Roller))]
    public class Roller : BlockBase
    {
        private EMoveDirection _rollerDirection;

        public EMoveDirection RollerDirection
        {
            get { return _rollerDirection; }
        }

        public override void UpdateExtraData()
        {
            _rollerDirection = DataScene2D.Instance.GetUnitExtra(_guid).RollerDirection;
            if (_animation != null)
            {
                _animation.Init(_rollerDirection == EMoveDirection.Left ? "LeftRun" : "RightRun");
            }
            base.UpdateExtraData();
        }

        public override IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            IntVec2 deltaImpactPos = IntVec2.zero;
            var player = unit as PlayerBase;
            //推箱子的时候额外处理
            if (player != null && player.IsHoldingBox())
            {
                switch (_rollerDirection)
                {
                    case EMoveDirection.Right:
                        deltaImpactPos.x = (int) (player.CurMaxSpeedX * 0.8f);
                        break;
                    case EMoveDirection.Left:
                        deltaImpactPos.x = (int) (-player.CurMaxSpeedX * 0.8f);
                        break;
                }
                if (!_run || !UseMagic())
                {
                    return deltaImpactPos;
                }
                return deltaImpactPos + _deltaPos;
            }
            switch (_rollerDirection)
            {
                case EMoveDirection.Right:
                    deltaImpactPos.x = 50;
                    break;
                case EMoveDirection.Left:
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
            _animation.Init(_rollerDirection == EMoveDirection.Left ? "LeftRun" : "RightRun");
            return true;
        }
    }
}