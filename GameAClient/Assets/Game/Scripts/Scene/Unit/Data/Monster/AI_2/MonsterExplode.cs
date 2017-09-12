using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2003, Type = typeof(MonsterExplode))]
    public class MonsterExplode : MonsterAI_2
    {
        private float _viewDistance = 10 * ConstDefineGM2D.ServerTileScale;

        protected override void Clear()
        {
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this);
            _skillCtrl.SetSkill(103);
            base.Clear();
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (unit.IsMain)
            {
                OnDead();
                return;
            }
            if (eDirectionType == EDirectionType.Left || eDirectionType == EDirectionType.Right)
            {
                _timerDetectStay = 0;
            }
            base.Hit(unit, eDirectionType);
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsMain)
            {
                OnDead();
            }
        }

        protected override void OnDead()
        {
            _skillCtrl.Fire(0);
            base.OnDead();
        }

        protected override void CalculateSpeedRatio()
        {
            base.CalculateSpeedRatio();
            if (_eMonsterState == EMonsterState.Chase)
            {
                _speedRatio *= SpeedChaseRatio;
            }
        }

        protected override void UpdateMonsterAI()
        {
            if (GameRun.Instance.LogicFrameCnt % 5 == 0)
            {
                var units = ColliderScene2D.RaycastAllReturnUnits(CenterPos,
                    _moveDirection == EMoveDirection.Right ? Vector2.right : Vector2.left, _viewDistance,
                    EnvManager.MonsterViewLayer);
                bool isMain = false;
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive && !unit.CanLazerCross)
                    {
                        if (unit.IsMain)
                        {
                            isMain = true;
                            if (_eMonsterState != EMonsterState.Chase)
                            {
                                ChangeState(EMonsterState.Bang);
                                _timerDetectStay = 100 + _timerBang;
                            }
                        }
                        break;
                    }
                }
                if ((units.Count == 0 || !isMain) && _eMonsterState == EMonsterState.Chase && _timerDetectStay == 0)
                {
                    ChangeState(EMonsterState.Run);
                }
            }
            base.UpdateMonsterAI();
        }

        protected override void UpdateMonsterView(float deltaTime)
        {
            if (_animation != null)
            {
                if (_speed.x == 0)
                {
                    if (CanMove)
                    {
                        _animation.PlayLoop("Idle");
                    }
                }
                else
                {
                    _animation.PlayLoop(_eMonsterState == EMonsterState.Chase ? "Attack" : "Run",
                        Mathf.Clamp(Math.Abs(SpeedX), 30, 200) * deltaTime);
                }
            }
        }
    }
}