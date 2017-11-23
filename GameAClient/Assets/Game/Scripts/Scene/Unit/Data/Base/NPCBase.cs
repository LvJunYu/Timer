using SoyEngine;

namespace GameA.Game
{
//    [Unit(Id = 7201, Type = typeof(NPCBase))]
    public class NPCBase : ActorBase
    {
        private bool _trigger;
        private UnitBase _unit;
        private int _time;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsMain)
            {
                OnTrigger(other);
            }
        }

        public override void UpdateLogic()
        {
            if (_trigger)
            {
                _time++;
                if (!_colliderGrid.Intersects(_unit.ColliderGrid) && _time >= 100)
                {
                    _trigger = false;
                    _unit = null;
//                    Messenger.Broadcast(EMessengerType.OnTriggerBulletinBoardExit);
                }
            }
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
//            _animation.Init("Run");
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _time = 0;
            _trigger = false;
            _unit = null;
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            if (!_trigger)
            {
                _trigger = true;
                _unit = other;
                _time = 0;
//                if (_animation != null)
//                {
//                    _animation.PlayOnce("Start", 1, 1);
//                }
//                Messenger<IntVec3>.Broadcast(EMessengerType.OnTriggerBulletinBoardEnter, _guid);
            }
        }

        protected override bool IsCheckClimb()
        {
            return false;
        }
    }
}