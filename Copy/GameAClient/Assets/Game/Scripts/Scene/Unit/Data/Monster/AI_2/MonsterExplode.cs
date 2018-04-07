using System;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2003, Type = typeof(MonsterExplode))]
    public class MonsterExplode : MonsterAI_2
    {
        public override bool CanDashBrick
        {
            get { return _actorState == EActorState.Chase; }
        }

        public override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (unit.IsPlayer && CanHarm(unit))
            {
                OnDead();
                return;
            }

            base.Hit(unit, eDirectionType);
        }

        public override void OnIntersect(UnitBase other)
        {
            if (CanHarm(other) && other.IsPlayer)
            {
                OnDead();
            }
        }

        protected override void OnDead()
        {
            _skillCtrl.Fire(0);
            base.OnDead();
        }

        protected override void UpdateMonsterView(float deltaTime)
        {
            if (_animation != null)
            {
                if (_speed.x == 0)
                {
                    if (CanMove)
                    {
                        _animation.PlayLoop(Idle);
                    }
                }
                else
                {
                    _animation.PlayLoop(_actorState == EActorState.Chase ? Attack : Run,
                        Mathf.Clamp(Math.Abs(SpeedX), 30, 200) * deltaTime);
                }
            }
        }

        protected override void CalculateSpeedRatio()
        {
            base.CalculateSpeedRatio();
            if (_actorState == EActorState.Chase)
            {
                _speedRatio *= SpeedChaseRatio;
            }
        }

        public override void DoChase(UnitBase target, out bool reachDes)
        {
            SetState(EActorState.Chase);
            reachDes = false;
            SetInputByMoveDir(_moveDirection);
        }
    }
}