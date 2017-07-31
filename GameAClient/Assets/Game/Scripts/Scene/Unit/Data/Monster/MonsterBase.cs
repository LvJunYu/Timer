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

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _isMonster = true;
            _maxSpeedX = 30;
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _lastPos = _curPos;
            _input = _input ?? new PlayerInputBase(this);
            _input.Clear();
        }

        protected override void CalculateMotor()
        {
            base.CalculateMotor();
            if (HasStateType(EStateType.Fire))
            {
                _speedRatio *= SpeedFireRatio;
                OnFire();
            }
            else
            {
                UpdateMonsterAI();
            }
            if (_grounded)
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
        }

        protected virtual void OnFire()
        {
//            if (_curMoveDirection == EMoveDirection.Right)
//            {
//                SpeedX = Util.ConstantLerp(SpeedX, _curMaxSpeedX, _curFriction);
//            }
//            else
//            {
//                SpeedX = Util.ConstantLerp(SpeedX, -_curMaxSpeedX, _curFriction);
//            }
            //碰到墙壁转头
            CheckWay();
        }

        protected virtual void UpdateMonsterAI()
        {
        }

        protected override void UpdateDynamicView(float deltaTime)
        {
            base.UpdateDynamicView(deltaTime);
            if (_isStart && _isAlive)
            {
                UpdateMonsterView(deltaTime);
            }
            if (!_isAlive)
            {
                _dieTime ++;
                if (_dieTime == 100)
                {
                    PlayMode.Instance.DestroyUnit(this);
                }
            }
            _lastPos = _curPos;
        }

        protected virtual void UpdateMonsterView(float deltaTime)
        {
            if (_animation != null)
            {
                if (_speed.x == 0)
                {
                    if (_canMove)
                    {
                        _animation.PlayLoop("Idle");
                    }
                }
                else
                {
                    float speed = (int)(SpeedX * 1f);
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
