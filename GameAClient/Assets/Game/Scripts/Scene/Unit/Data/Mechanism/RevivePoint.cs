/********************************************************************
** Filename : RevivePoint
** Author : Dong
** Date : 2016/10/29 星期六 上午 11:26:57
** Summary : RevivePoint
***********************************************************************/

using SoyEngine;
using Spine.Unity;

namespace GameA.Game
{
    [Unit(Id = 5002, Type = typeof(RevivePoint))]
    public class RevivePoint : BlockBase
    {
        private bool _trigger;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            Messenger<IntVec3>.AddListener(EMessengerType.OnRespawnPointTrigger, OnRespawnPointTrigger);
            return true;
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

        private void OnRespawnPointTrigger(IntVec3 guid)
        {
            if (_guid == guid)
            {
                return;
            }
            if (!_trigger)
            {
                return;
            }
            if (_trans != null)
            {
                _animation.Reset();
                _animation.PlayLoop("Run");
            }
            _trigger = false;
        }

        internal override void Reset()
        {
            base.Reset();
            _animation.Reset();
            _trigger = false;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                if (other.IsMain)
                {
                    if (!_trigger)
                    {
                        _trigger = true;
                        other.OnRevivePos(_curPos);
                        _animation.PlayOnce("Start").Complete +=
                            (state, index, count) => _animation.PlayLoop("End");
                        Messenger<IntVec3>.Broadcast(EMessengerType.OnRespawnPointTrigger, _guid);
                    }
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        internal override void OnDispose()
        {
            Messenger<IntVec3>.RemoveListener(EMessengerType.OnRespawnPointTrigger, OnRespawnPointTrigger);
            base.OnDispose();
        }
    }
}