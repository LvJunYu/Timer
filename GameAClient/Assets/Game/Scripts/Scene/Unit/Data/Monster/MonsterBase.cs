﻿/********************************************************************
** Filename : MonsterBase
** Author : Dong
** Date : 2017/4/18 星期二 下午 9:05:44
** Summary : MonsterBase
***********************************************************************/

using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class MonsterBase : ActorBase
    {
        protected int _fireTimer;
        protected EClayOnWallDirection _eClayOnWallDirection;
        protected IntVec2 _hitPos;
        protected bool _isClayOnWall;

        public bool IsClayOnWall
        {
            get { return _isClayOnWall; }
            set
            {
                if (!value)
                {
                    _eClayOnWallDirection = EClayOnWallDirection.None;
                    _hitPos = IntVec2.zero;
                }
                _isClayOnWall = value;
            }
        }

        public EClayOnWallDirection EClayOnWallDirection
        {
            get { return _eClayOnWallDirection; }
        }

        public override bool CanMove
        {
            get
            {
                return _isAlive && !IsInState(EEnvState.Clay) && !IsInState(EEnvState.Stun) &&
                       !IsInState(EEnvState.Ice);
            }
        }

        protected override bool IsCheckClimb()
        {
            return false;
        }

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _isMonster = true;
//            _maxSpeedX = 40;
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

        internal override void OnPlay()
        {
            base.OnPlay();
            if (_tableUnit.SkillId > 0)
            {
                _skillCtrl = new SkillCtrl(this);
                _skillCtrl.SetSkill(_tableUnit.SkillId);
            }
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

        public void ClayOnWall(EClayOnWallDirection eClayOnWallDirection)
        {
            if (IsInState(EEnvState.Clay))
            {
                RemoveStates(41);
            }
            _eClayOnWallDirection = eClayOnWallDirection;
            IsClayOnWall = true;
            _hitPos = CenterPos;
            Speed = IntVec2.zero;
            AddStates(41);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            //判断黏在墙上若发生位移，则解除黏在墙上
            if (IsClayOnWall && (_hitPos - CenterPos).SqrMagnitude() >
                GM2DTools.WorldToTile(0.5f) * GM2DTools.WorldToTile(0.5f))
            {
                RemoveStates(41);
            }
        }

        public override UnitExtra UpdateExtraData()
        {
            var unitExtra = base.UpdateExtraData();
            if (unitExtra.MaxSpeedX > 0)
            {
                _maxSpeedX = unitExtra.MaxSpeedX;
            }
            else if (unitExtra.MaxSpeedX == -1)
            {
                _maxSpeedX = 0;
            }
            else
            {
                _maxSpeedX = 40;
            }
            return unitExtra;
        }

        protected override void CaculateGravity()
        {
            if (IsInState(EEnvState.Clay) && _isClayOnWall)
            {
//                SpeedY = 0;
            }
            else
            {
                base.CaculateGravity();
            }
        }

        protected override void CaculateSpeedX(bool air)
        {
            if (IsInState(EEnvState.Clay) && _isClayOnWall)
            {
//                SpeedX = 0;
            }
            else
            {
                base.CaculateSpeedX(air);
            }
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            if (_eActiveState != EActiveState.Active)
            {
                SetInput(EInputType.Right, false);
                SetInput(EInputType.Left, false);
                SetInput(EInputType.Skill1, false);
                SpeedX = 0;
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
                _dieTime++;
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
                        _animation.PlayLoop(Idle);
                    }
                }
                else
                {
                    _animation.PlayLoop(Run, Mathf.Clamp(Math.Abs(SpeedX), 30, 200) * deltaTime);
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
                if (_moveDirection == EMoveDirection.Left && _hitUnits[(int) EDirectionType.Left] != null)
                {
                    _fireTimer = 0;
                    return ChangeWay(EMoveDirection.Right);
                }
                else if (_moveDirection == EMoveDirection.Right && _hitUnits[(int) EDirectionType.Right] != null)
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

        protected override void OnDead()
        {
            base.OnDead();
            Messenger<EDieType>.Broadcast(EMessengerType.OnMonsterDead, _eDieType);
        }

        internal override void OnObjectDestroy()
        {
            var drops = GetUnitExtra().Drops;
            base.OnObjectDestroy();
            if (GameRun.Instance.IsPlaying)
            {
                if (drops != null && drops.Count > 0)
                {
                    PlayMode.Instance.CreateRuntimeUnit(drops[0], _curPos);
                    PlayMode.Instance.CreateRuntimeUnit(drops[1], _curPos);
                }
            }
        }

        protected void SetInput(EInputType eInputType, bool value)
        {
            _input.CurAppliedInputKeyAry[(int) eInputType] = value;
        }
    }
}