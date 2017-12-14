﻿using System;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2003, Type = typeof(MonsterExplode))]
    public class MonsterExplode : MonsterAI_2
    {
        private float _viewDistance = 15 * ConstDefineGM2D.ServerTileScale;

        public override bool CanDashBrick
        {
            get { return _eMonsterState == EMonsterState.Chase; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _intelligenc = 3;
            return true;
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (unit.IsPlayer && CanHarm(unit))
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
            if (GameRun.Instance.LogicFrameCnt % 5 == 0 && CanMove)
            {
                var units = ColliderScene2D.RaycastAllReturnUnits(CenterPos,
                    _moveDirection == EMoveDirection.Right ? Vector2.right : Vector2.left, _viewDistance,
                    EnvManager.MonsterViewLayer);
                bool target = false;
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive && unit.TableUnit.IsViewBlock == 1 && !unit.CanCross)
                    {
                        if (unit.IsPlayer && CanHarm(unit))
                        {
                            target = true;
                            if (_eMonsterState != EMonsterState.Chase)
                            {
                                ChangeState(EMonsterState.Bang);
                                _timerDetectStay = 100 + _timerBang;
                            }
                        }
                        break;
                    }
                }
                if ((units.Count == 0 || !target) && _eMonsterState == EMonsterState.Chase && _timerDetectStay == 0)
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
                        _animation.PlayLoop(Idle);
                    }
                }
                else
                {
                    _animation.PlayLoop(_eMonsterState == EMonsterState.Chase ? Attack : Run,
                        Mathf.Clamp(Math.Abs(SpeedX), 30, 200) * deltaTime);
                }
            }
        }
    }
}