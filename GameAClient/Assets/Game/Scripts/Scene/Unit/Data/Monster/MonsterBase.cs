/********************************************************************
** Filename : MonsterBase
** Author : Dong
** Date : 2017/4/18 星期二 下午 9:05:44
** Summary : MonsterBase
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class MonsterBase : ActorBase
    {
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
            _maxSpeedX = 40;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            CreateStatusBar();
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _input = _input ?? new InputBase();
            _input.Clear();
            _fireTimer = 0;
            if (_statusBar != null)
            {
                _statusBar.SetHPActive(false);
            }
        }

        protected override void CalculateSpeedRatio()
        {
            base.CalculateSpeedRatio();
            if (HasStateType(EStateType.Fire))
            {
                _speedRatio *= SpeedFireRatio;
                OnFire();
            }
        }

        protected override void AfterCheckGround()
        {
            if (_grounded)
            {
                // 判断左右踩空
                if (_downUnits.Count == 1 && !_downUnits[0].IsActor)
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
            if (HasStateType(EStateType.Fire))
            {
                OnFire();
            }
            else
            {
                _fireTimer = 0;
                UpdateMonsterAI();
            }
        }

        protected virtual void UpdateMonsterAI()
        {
        }

        protected virtual void OnFire()
        {
            _fireTimer++;
            if (_fireTimer % 80 == 0)
            {
                ChangeWay(_moveDirection == EMoveDirection.Left ? EMoveDirection.Right : EMoveDirection.Left);
            }
            CheckWay();
        }

        protected override void UpdateDynamicView(float deltaTime)
        {
            base.UpdateDynamicView(deltaTime);
            if (_isAlive)
            {
                UpdateMonsterView(deltaTime);
            }
            else
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
                    if (CanMove)
                    {
                        _animation.PlayLoop("Idle");
                    }
                }
                else
                {
                    _animation.PlayLoop("Run", Mathf.Clamp(Math.Abs(SpeedX), 30, 200) * deltaTime);
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
                if (_moveDirection == EMoveDirection.Left && _hitUnits[(int)EDirectionType.Left] != null)
                {
                    _fireTimer = 0;
                    return ChangeWay(EMoveDirection.Right);
                }
                else if (_moveDirection == EMoveDirection.Right && _hitUnits[(int)EDirectionType.Right] != null)
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
