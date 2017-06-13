/********************************************************************
** Filename : MonsterBase
** Author : Dong
** Date : 2017/4/18 星期二 下午 9:05:44
** Summary : MonsterBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class MonsterBase : ActorBase
    {
        protected IntVec2 _lastPos;
        protected int _monsterSpeed;
        protected int _curMaxSpeedX;
        protected int _curFriction;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _isMonster = true;
            _monsterSpeed = 30;
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _lastPos = _curPos;
            _curMaxSpeedX = _monsterSpeed;
        }

        public override void UpdateLogic()
        {
            if (_isAlive && _isStart && !_isFreezed)
            {
                bool air = false;
                _curFriction = _friction;
                if (SpeedY != 0)
                {
                    air = true;
                }
                if (!air)
                {
                    _onClay = false;
                    bool downExist = false;
                    int deltaX = int.MaxValue;
                    var units = EnvManager.RetriveDownUnits(this);
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        int ymin = 0;
                        if (unit != null && unit.IsAlive && CheckOnFloor(unit) && unit.OnUpHit(this, ref ymin, true))
                        {
                            downExist = true;
                            _grounded = true;
                            _downUnits.Add(unit);
                            if (unit.Friction > _curFriction)
                            {
                                _curFriction = unit.Friction;
                            }
                            var edge = unit.GetUpEdge(this);
                            if (unit.StepOnClay() || edge.ESkillType == ESkillType.Clay)
                            {
                                _onClay = true;
                            }
                            else if (unit.StepOnIce() || edge.ESkillType == ESkillType.Ice)
                            {
                                _onIce = true;
                            }
                            var delta = Math.Abs(CenterPos.x - unit.CenterPos.x);
                            if (deltaX > delta)
                            {
                                deltaX = delta;
                                _downUnit = unit;
                            }
                        }
                    }
                    if (!downExist)
                    {
                        air = true;
                    }
                }
                if (air && _grounded)
                {
                    Speed += _lastExtraDeltaPos;
                    _grounded = false;
                }
                _curMaxSpeedX = _monsterSpeed;
                if (_onClay)
                {
                    _curFriction = 30;
                    _curMaxSpeedX = (int)(_curMaxSpeedX * SpeedClayRatio);
                }
                else if (_onIce)
                {
                    _curFriction = 1;
                }
                if (_eDieType == EDieType.Fire)
                {
                    OnFire();
                }
                else
                {
                    _fireTimer = 0;
                    UpdateMonsterAI();
                }
                if (!air)
                {
                    // 判断左右踩空
                    if (_downUnits.Count == 1)
                    {
                        if (SpeedX > 0 && _downUnits[0].ColliderGrid.XMax < _colliderGrid.XMax)
                        {
                            OnRightStampedEmpty();
                        }
                        else if (SpeedX < 0 && _downUnits[0].ColliderGrid.XMin > _colliderGrid.XMin)
                        {
                            OnLeftStampedEmpty();
                        }
                    }
                }
                if (!_grounded)
                {
                    SpeedY -= 12;
                    if (SpeedY < -120)
                    {
                        SpeedY = -120;
                    }
                }
            }
        }

        protected virtual void OnFire()
        {
            _curMaxSpeedX = (int)(_curMaxSpeedX * SpeedFireRatio);
            if (_curMoveDirection == EMoveDirection.Right)
            {
                SpeedX = Util.ConstantLerp(SpeedX, _curMaxSpeedX, _curFriction);
            }
            else
            {
                SpeedX = Util.ConstantLerp(SpeedX, -_curMaxSpeedX, _curFriction);
            }
            _fireTimer++;
            //碰到墙壁转头
            CheckWay();
            //每隔转头
            if (_fireTimer == 80)
            {
                ChangeWay(_curMoveDirection == EMoveDirection.Right ? EMoveDirection.Left : EMoveDirection.Right);
            }
            //4秒后还是这个状态挂掉
            else if (_fireTimer == 200)
            {
                OnDead();
                if (_animation != null)
                {
                    _animation.Reset();
                    _animation.PlayOnce("DeathFire");
                }
            }
        }

        protected virtual void UpdateMonsterAI()
        {
        }

        public override void UpdateView(float deltaTime)
        {
            if (_isStart && _isAlive)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                LimitPos();
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
                if (OutOfMap())
                {
                    return;
                }
                UpdateMonsterView(deltaTime);
                _lastGrounded = _grounded;
                _lastPos = _curPos;
            }
            if (!_isAlive)
            {
                _dieTime ++;
                if (_dieTime == 100)
                {
                    PlayMode.Instance.DestroyUnit(this);
                }
            }
        }

        protected virtual void UpdateMonsterView(float deltaTime)
        {
            if (_animation != null)
            {
                if (_speed.x == 0)
                {
                    if (_canMotor)
                    {
                        _animation.PlayLoop("Idle");
                    }
                }
                else
                {
                    float speed = (int)(SpeedX * 0.7f);
                    speed = Math.Abs(speed);
                    speed = Mathf.Clamp(speed, 30, 100) * deltaTime;
                    _animation.PlayLoop("Run", speed);
                }
            }
        }

        protected virtual void OnRightStampedEmpty()
        {
        }

        protected virtual void OnLeftStampedEmpty()
        {
        }

        protected bool CheckWay()
        {
            if (_hitUnits != null)
            {
                if (_curMoveDirection == EMoveDirection.Left && _hitUnits[(int)EDirectionType.Left] != null)
                {
                    return ChangeWay(EMoveDirection.Right);
                }
                else if (_curMoveDirection == EMoveDirection.Right && _hitUnits[(int)EDirectionType.Right] != null)
                {
                    return ChangeWay(EMoveDirection.Left);
                }
            }
            return false;
        }

        public override bool ChangeWay(EMoveDirection eMoveDirection)
        {
            if (!_isMonster)
            {
                return false;
            }
            if (_curMaxSpeedX != 0)
            {
                SpeedX = eMoveDirection == EMoveDirection.Right ? _curMaxSpeedX : -_curMaxSpeedX;
            }
            SetFacingDir(eMoveDirection);
            return true;
        }

        protected override void OnDead ()
        {
            base.OnDead ();
            Messenger<EDieType>.Broadcast (EMessengerType.OnMonsterDead, _eDieType);
        }
    }
}
