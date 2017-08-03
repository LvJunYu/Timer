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
        protected int _fireTimer;

        protected override bool IsCheckClimb()
        {
            return false;
        }

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

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _view.StatusBar.ShowHP();
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _lastPos = _curPos;
            _input = _input ?? new InputBase();
            _input.Clear();
            _fireTimer = 0;
        }

        protected override void CalculateMotor()
        {
            base.CalculateMotor();
            if (HasStateType(EStateType.Fire))
            {
                _speedRatio *= SpeedFireRatio;
                OnFire();
                ChangeState(EMonsterState.None);
            }
            else
            {
                _fireTimer = 0;
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

        protected virtual void ChangeState(EMonsterState state)
        {
            //LogHelper.Debug("ChangeState : {0}", _eState);
        }

        protected virtual void UpdateMonsterAI()
        {
        }

        protected virtual void OnFire()
        {
            _fireTimer++;
            if (_fireTimer % 80 == 0)
            {
                ChangeWay(_curMoveDirection == EMoveDirection.Left ? EMoveDirection.Right : EMoveDirection.Left);
            }
            CheckWay();
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
                    _fireTimer = 0;
                    return ChangeWay(EMoveDirection.Right);
                }
                else if (_curMoveDirection == EMoveDirection.Right && _hitUnits[(int)EDirectionType.Right] != null)
                {
                    _fireTimer = 0;
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
            LogHelper.Debug("ChangeWay {0}", eMoveDirection);
            SetInput(eMoveDirection == EMoveDirection.Right ? EInputType.Right : EInputType.Left, true);
            SetInput(eMoveDirection == EMoveDirection.Right ? EInputType.Left : EInputType.Right, false);
            return true;
        }

        protected override void OnDead ()
        {
            base.OnDead ();
            Messenger<EDieType>.Broadcast (EMessengerType.OnMonsterDead, _eDieType);
        }

        protected void SetInput(EInputType eInputType, bool value)
        {
            _input.CurAppliedInputKeyAry[(int)eInputType] = value;
        }
    }
}
