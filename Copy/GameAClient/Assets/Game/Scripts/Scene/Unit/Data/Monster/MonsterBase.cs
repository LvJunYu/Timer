﻿/********************************************************************
** Filename : MonsterBase
** Author : Dong
** Date : 2017/4/18 星期二 下午 9:05:44
** Summary : MonsterBase
***********************************************************************/

using System;
using BehaviorDesigner.Runtime;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class MonsterBase : ActorBase
    {
        protected int _fireTimer;
        protected EClayOnWallDirection _eClayOnWallDirection;
        protected IntVec2 _clayPos;
        protected bool _isClayOnWall;
        protected UnitBase _attactTarget;
        protected MonsterCave _monsterCave;
        private int _lastGetTargetFrame;

        public override bool IsMonster
        {
            get { return true; }
        }

        public bool IsClayOnWall
        {
            get { return _isClayOnWall; }
            set
            {
                if (!value)
                {
                    _eClayOnWallDirection = EClayOnWallDirection.None;
                    _clayPos = IntVec2.zero;
                }

                _isClayOnWall = value;
            }
        }

        private bool CanAi
        {
            get
            {
                return _isAlive && !IsInState(EEnvState.Clay) && !IsInState(EEnvState.Stun) &&
                       !IsInState(EEnvState.Ice);
            }
        }

        public override bool CanAttack
        {
            get
            {
                return _isAlive && !IsInState(EEnvState.Clay) && !IsInState(EEnvState.Stun) &&
                       !IsInState(EEnvState.Ice);
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

//            _maxSpeedX = 40;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            // 游戏时怪物死亡后，在Destroy前切换场景，切换回来会重新生成View，此时判断不显示View
            if (!_isAlive && GameRun.Instance.IsPlaying)
            {
                _view.SetRendererEnabled(false);
            }
            else
            {
                CreateStatusBar();
                if (_statusBar != null)
                {
                    _statusBar.SetHPActive(true);
                }
            }

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
            _lastGetTargetFrame = -1000;
            if (_statusBar != null)
            {
                _statusBar.Reset();
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
                    if (SpeedX > 0 && _downUnits[0].ColliderGrid.XMax < ColliderGrid.XMax)
                    {
                        OnRightStampedEmpty();
                    }
                    else if (SpeedX < 0 && _downUnits[0].ColliderGrid.XMin > ColliderGrid.XMin)
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
                OnBeforeAi();
                if (_behaviour != null && CanAi)
                {
                    BehaviorManager.instance.Tick(_behaviour);
                }

                UpdateMonsterAI();
            }
        }

        protected virtual void OnBeforeAi()
        {
            SetInput(EInputType.Skill1, false);
        }

        public void ClayOnWall(EClayOnWallDirection eClayOnWallDirection)
        {
            if (IsInState(EEnvState.Clay))
            {
                RemoveStates(41);
            }

            _eClayOnWallDirection = eClayOnWallDirection;
            IsClayOnWall = true;
            _clayPos = CenterPos;
            Speed = IntVec2.zero;
            AddStates(null, 41);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            //判断黏在墙上若发生位移，则解除黏在墙上
            if (IsClayOnWall && (_clayPos - CenterPos).SqrMagnitude() >
                GM2DTools.WorldToTile(0.5f) * GM2DTools.WorldToTile(0.5f))
            {
                RemoveStates(41);
            }
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            if (unitExtra.CastRange > 0)
            {
                _attackRange = IntVec2.one * TableConvert.GetRange(unitExtra.CastRange);
            }
            else
            {
                _attackRange = IntVec2.one * TableConvert.GetRange(10);
            }

            if (unitExtra.MaxSpeedX > 0 && unitExtra.MaxSpeedX < ushort.MaxValue)
            {
                _maxSpeedX = unitExtra.MaxSpeedX;
            }
            else if (unitExtra.MaxSpeedX == ushort.MaxValue)
            {
                _maxSpeedX = 0;
            }
            else
            {
                _maxSpeedX = _tableUnit.MaxSpeed;
            }

            _injuredReduce = TableConvert.GetInjuredReduce(unitExtra.InjuredReduce);
            _curIncrease = TableConvert.GetCurIncrease(unitExtra.CureIncrease);
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
#if UNITY_EDITOR
            if (_path != null)
            {
                for (int i = 0; i < _path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(_path[i].x + 0.5f, _path[i].y + 0.5f),
                        new Vector3(_path[i + 1].x + 0.5f, _path[i + 1].y + 0.5f));
                }
            }
#endif
        }

        protected virtual void OnFire()
        {
            _fireTimer++;
            if (_fireTimer % 80 == 0)
            {
                SetInputByMoveDir(_moveDirection == EMoveDirection.Left ? EMoveDirection.Right : EMoveDirection.Left);
            }

            CheckWay();
            SetInput(EInputType.Skill1, false);
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
            _stampedEmpty = true;
        }

        protected virtual void OnLeftStampedEmpty()
        {
            _stampedEmpty = true;
        }

        protected bool CheckWay()
        {
            if (_hitUnits != null)
            {
                if (_moveDirection == EMoveDirection.Left && _hitUnits[(int) EDirectionType.Left] != null)
                {
                    _fireTimer = 0;
                    return SetInputByMoveDir(EMoveDirection.Right);
                }
                else if (_moveDirection == EMoveDirection.Right && _hitUnits[(int) EDirectionType.Right] != null)
                {
                    _fireTimer = 0;
                    return SetInputByMoveDir(EMoveDirection.Left);
                }
            }

            return false;
        }

        protected override void OnDead()
        {
            base.OnDead();
            Scene2DManager.Instance.GetCurScene2DEntity().RpgManger.AddKill(Id);
//            RpgTaskManger.Instance.AddKill(Id);
            Messenger<EDieType>.Broadcast(EMessengerType.OnMonsterDead, _eDieType);
        }

        public override void Dispose()
        {
            if (GameRun.Instance.IsPlaying)
            {
                var drops = GetUnitExtra().Drops;
                if (!drops.IsEmpty)
                {
                    PlayMode.Instance.CreateRuntimeUnit(drops.Get<ushort>(0), _curPos);
                }
            }

            if (_monsterCave != null)
            {
                _monsterCave.OnMonsterDestroy(this);
            }

            base.Dispose();
        }

        protected virtual void UpdateAttackTarget(UnitBase lastTarget = null)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti)
            {
                _attactTarget = PlayMode.Instance.MainPlayer;
                return;
            }

            if (lastTarget != null && lastTarget != this && lastTarget.IsAlive)
            {
                _attactTarget = lastTarget;
            }
            else
            {
                int curFrame = GameRun.Instance.LogicFrameCnt;
                if (curFrame - _lastGetTargetFrame > 10)
                {
                    _attactTarget = TeamManager.Instance.GetMonsterTarget(this, true);
                    _lastGetTargetFrame = curFrame;
                }
                else
                {
                    _attactTarget = TeamManager.Instance.GetMonsterTarget(this, false);
                }
            }
        }

        public override bool CanHarm(UnitBase unit)
        {
            if (!unit.IsActor)
            {
                return false;
            }

            return !IsSameTeam(unit.TeamId);
        }

        public override UnitExtraDynamic GetUnitExtra()
        {
            if (_monsterCave != null)
            {
                return _monsterCave.GetUnitExtra();
            }

            return base.GetUnitExtra();
        }

        public void SetCave(MonsterCave monsterCave)
        {
            _monsterCave = monsterCave;
            UpdateExtraData();
            _eActiveState = EActiveState.Active;
            _moveDirection = EMoveDirection.Left;
            //根据队伍刷新血条颜色
            if (_statusBar != null)
            {
                _statusBar.RefreshBar();
            }
        }
    }
}