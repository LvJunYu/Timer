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

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _rollerDirection = DataScene2D.Instance.GetUnitExtra(_guid).RollerDirection;
            return true;
        }

        public override void UpdateExtraData()
        {
            _rollerDirection = DataScene2D.Instance.GetUnitExtra(_guid).RollerDirection;
            base.UpdateExtraData();
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            return _animation.Init(_rollerDirection == EMoveDirection.Left ? "LeftRun" : "RightRun");
        }

        protected override void Clear()
        {
            base.Clear();
            switch (_rollerDirection)
            {
                case EMoveDirection.Right:
                    _deltaImpactPos.x = 50;
                    break;
                case EMoveDirection.Left:
                    _deltaImpactPos.x = -50;
                    break;
            }
        }
    }
}