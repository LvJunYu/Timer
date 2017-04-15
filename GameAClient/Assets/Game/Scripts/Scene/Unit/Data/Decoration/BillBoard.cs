/********************************************************************
** Filename : BillBoard
** Author : Dong
** Date : 2016/10/27 星期四 下午 2:10:24
** Summary : BillBoard
***********************************************************************/

using SoyEngine;
using Spine.Unity;

namespace GameA.Game
{
    [Unit(Id = 7001, Type = typeof (BillBoard))]
    public class BillBoard : UnitBase
    {
        private SkeletonAnimation _animation;
        private bool _trigger;
        private UnitBase _unit;
        private int _time;

        public IntVec3 GetGuid()
        {
            return _guid;
        }

        public void UpdateLogic(float deltaTime)
        {
            if (_trigger)
            {
                _time++;
                if (!_colliderGrid.Intersects(_unit.ColliderGrid))
                {
                    if (_time >= 100)
                    {
                        _trigger = false;
                        _unit = null;
                        Messenger.Broadcast(EMessengerType.OnTriggerBulletinBoardExit);
                        _animation.state.SetAnimation(0, "Run", true);
                    }
                }
            }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _animation = _trans.GetComponent<SkeletonAnimation>();
            return true;
        }

        internal override void OnPlay()
        {
            _animation.state.SetAnimation(0, "Run", true);
            _time = 0;
            base.OnPlay();
        }

        internal override void Reset()
        {
            _trigger = false;
            _unit = null;
            _animation.Reset();
            base.Reset();
        }

        public override void OnHit(UnitBase other)
        {
            if (!other.IsMain)
            {
                return;
            }
            if (!_trigger)
            {
                _trigger = true;
                _unit = other;
                _animation.state.SetAnimation(1, "Start", false);
                Messenger<IntVec3>.Broadcast(EMessengerType.OnTriggerBulletinBoardEnter, _guid);
                _time = 0;
            }
        }
    }
}