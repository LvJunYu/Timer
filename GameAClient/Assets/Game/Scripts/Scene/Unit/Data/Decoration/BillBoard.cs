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
    public class BillBoard : DecorationBase
    {
        private bool _trigger;
        private UnitBase _unit;
        private int _time;

        public override void UpdateLogic()
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
                        Messenger<IntVec3>.Broadcast(EMessengerType.OnTriggerBulletinBoardExit, _guid);
                    }
                }
            }
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

        protected override void Clear()
        {
            base.Clear();
            _time = 0;
            _trigger = false;
            _unit = null;
        }

        protected override void OnTrigger(UnitBase other)
        {
            if (!_trigger)
            {
                _trigger = true;
                _unit = other;
                _time = 0;
                if (_animation != null)
                {
                    _animation.PlayOnce("Start", 1, 1);
                }
                Messenger<IntVec3>.Broadcast(EMessengerType.OnTriggerBulletinBoardEnter, _guid);
            }
        }
    }
}