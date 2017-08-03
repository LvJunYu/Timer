/********************************************************************
** Filename : ActorBase
** Author : Dong
** Date : 2017/5/20 星期六 上午 10:51:33
** Summary : ActorBase
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public enum EDieType
    {
        None,
        Lazer,
        Water,
        Fire,
        Saw,
        OutofMap,
    }

    public class ActorBase : DynamicRigidbody
    {
        protected List<State> _currentStates = new List<State>();
        private Comparison<State> _comparisonState = SortState;

        protected EDieType _eDieType;

        /// <summary>
        /// 每一帧只检查一个水块
        /// </summary>
        protected bool _hasWaterCheckedInFrame;
        
        protected int _curMaxSpeedX;
        
        protected SkillCtrl _skillCtrl;
 
        public override EDieType EDieType
        {
            get { return _eDieType; }
        }

        public override bool IsActor
        {
            get { return true; }
        }

        public int CurMaxSpeedX
        {
            get { return _curMaxSpeedX; }
        }
        
        
        protected override bool IsCheckGround()
        {
            return true;
        }

        protected override bool IsCheckClimb()
        {
            return true;
        }

        protected override bool IsUpdateSpeedY()
        {
            return true;
        }

        protected override void Clear()
        {
            RemoveAllStates();
            _canFanCross = true;
            _eDieType = EDieType.None;
            base.Clear();
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            Clear();
        }

        public override void CheckStart()
        {
            base.CheckStart();
            _hasWaterCheckedInFrame = false;
        }

        public override void UpdateLogic()
        {
            if (_isAlive && _isStart && !_isFreezed)
            {
                if (_stunTimer > 0)
                {
                    _stunTimer--;
                }
                if (_stunTimer <= 0)
                {
                    if (_input != null)
                    {
                        UpdateInputLogic();
                    }
                    if (_skillCtrl != null)
                    {
                        _skillCtrl.UpdateLogic();
                    }
                }
                for (int i = 0; i < _currentStates.Count; i++)
                {
                    _currentStates[i].UpdateLogic();
                }
            }
            base.UpdateLogic();
        }

        public void UpdateInputLogic()
        {
            if (!PlayMode.Instance.SceneState.GameRunning)
            {
                return;
            }
            if (_curBanInputTime == 0 && !IsHoldingBox())
            {
                if (_input.GetKeyApplied(EInputType.Left))
                {
                    if (_curMoveDirection != EMoveDirection.Left)
                    {
                        SetFacingDir(EMoveDirection.Left);
                    }
                }
                else if (_input.GetKeyApplied(EInputType.Right))
                {
                    if (_curMoveDirection != EMoveDirection.Right)
                    {
                        SetFacingDir(EMoveDirection.Right);
                    }
                }
            }
            CheckJump();
            CheckAssist();
            CheckSkill();
        }

        protected virtual void CheckJump()
        {
            _climbJump = false;
            if (_input.GetKeyApplied(EInputType.Jump))
            {
                //攀墙跳
                if (_eClimbState > EClimbState.None)
                {
                    _climbJump = true;
                    _curBanInputTime = BattleDefine.WallJumpBanInputTime;
                    ExtraSpeed.y = 0;
                    _jumpLevel = 0;
                    _jumpState = EJumpState.Jump1;
                    if (_eClimbState == EClimbState.Left)
                    {
                        SpeedX = 120;
                        SetFacingDir(EMoveDirection.Right);
                    }
                    else if (_eClimbState == EClimbState.Right)
                    {
                        SpeedX = -120;
                        SetFacingDir(EMoveDirection.Left);
                    }
                }
                else if (_jumpLevel == -1)
                {
                    if (_stepY > 0)
                    {
                        ExtraSpeed.y = _stepY;
                        _stepY = 0;
                    }
                    _jumpLevel = 0;
                    SpeedY = OnClay ? 100 : 150;
                    _jumpState = EJumpState.Jump1;
                    _jumpTimer = 10;
                }
                else if (!_input.GetKeyLastApplied(EInputType.Jump) && IsCharacterAbilityAvailable(ECharacterAbility.DoubleJump))
                {
                    if (_jumpLevel == 0 || _jumpLevel == 2)
                    {
                        if (WingCount > 0)
                        {
                            WingCount--;
                            _jumpLevel = 2;
                            SpeedY = 120;
                        }
                        else
                        {
                            _jumpLevel = 1;
                            SpeedY = 150;
                            _jumpState = EJumpState.Jump2;
                        }
                        ExtraSpeed.y = 0;
                        _jumpTimer = 15;
                        _input.CurAppliedInputKeyAry[(int) EInputType.Jump] = false;
                    }
                }
            }
            if (_jumpTimer > 0)
            {
                _jumpTimer--;
            }
            if ((_jumpTimer == 0 && SpeedY > 0) || SpeedY < 0)
            {
                _jumpState = EJumpState.Fall;
            }
        }
        
        protected void CheckAssist()
        {
            switch (_littleSkillState)
            {
                case ELittleSkillState.HoldBox:
                    {
                        if (_input.GetKeyUpApplied(EInputType.Assist))
                        {
                            OnBoxHoldingChanged();
                        }
                    }
                    break;
                case ELittleSkillState.Quicken:
                    {
        
                    }
                    break;
            }
        }
        
        protected void CheckSkill()
        {
            var eShootDir = _curMoveDirection == EMoveDirection.Left ? EShootDirectionType.Left : EShootDirectionType.Right;
            if (_input.GetKeyApplied(EInputType.Left))
            {
                eShootDir = EShootDirectionType.Left;
                if (_input.GetKeyApplied(EInputType.Down))
                {
                    eShootDir = EShootDirectionType.LeftDown;
                }
                else if (_input.GetKeyApplied(EInputType.Up))
                {
                    eShootDir = EShootDirectionType.LeftUp;
                }
            }
            else if (_input.GetKeyApplied(EInputType.Right))
            {
                eShootDir = EShootDirectionType.Right;
                if (_input.GetKeyApplied(EInputType.Down))
                {
                    eShootDir = EShootDirectionType.RightDown;
                }
                else if (_input.GetKeyApplied(EInputType.Up))
                {
                    eShootDir = EShootDirectionType.RightUp;
                }
            }
            else if (_input.GetKeyApplied(EInputType.Down))
            {
                eShootDir = EShootDirectionType.Down;
            }
            else if (_input.GetKeyApplied(EInputType.Up))
            {
                eShootDir = EShootDirectionType.Up;
            }
            _shootAngle = (int)eShootDir;
            if (IsCharacterAbilityAvailable(ECharacterAbility.Shoot))
            {
                if (_input.GetKeyApplied(EInputType.Skill1))
                {
                    _skillCtrl.Fire(0);
                }
                if (_input.GetKeyDownApplied(EInputType.Skill2))
                {
                    _skillCtrl.Fire(1);
                }
                if (_input.GetKeyDownApplied(EInputType.Skill3))
                {
                    _skillCtrl.Fire(2);
                }
            }
        }
        
        public void ChangeLittleSkillState(ELittleSkillState eLittleSkillState)
        {
            if (_littleSkillState == eLittleSkillState)
            {
                return;
            }
            _littleSkillState = eLittleSkillState;
            Messenger<ELittleSkillState>.Broadcast(EMessengerType.OnLittleSkillChanged, _littleSkillState);
        }

        private bool IsCharacterAbilityAvailable(ECharacterAbility eCharacterAbility)
        {
            if (!IsMain)
            {
                return true;
            }
            return GM2DGame.Instance.GameMode.IsPlayerCharacterAbilityAvailable(this, eCharacterAbility);
        }
        
        public override void AddStates(params int[] ids)
        {
            if (!_isAlive)
            {
                return;
            }
            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var tableState = TableManager.Instance.GetState(id);
                if (tableState == null)
                {
                    continue;
                }
                //如果是减益buff 当前无敌时跳过。
                if (tableState.IsBuff == 0 && IsInvincible)
                {
                    continue;
                }
                //如果已存在，判断叠加属性
                State state;
                if (TryGetState(id, out state))
                {
                    ++state;
                    continue;
                }
                //如果不存在，判断是否同类替换
                if (tableState.IsReplace == 1)
                {
                    RemoveStateByType((EStateType)tableState.StateType);
                }
                state = PoolFactory<State>.Get();
                if (state.OnAttached(tableState, this))
                {
                    _currentStates.Add(state);
                    _currentStates.Sort(_comparisonState);
                    continue;
                }
                PoolFactory<State>.Free(state);
            }
        }

        public override void RemoveStates(params int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                State state;
                if (TryGetState(ids[i], out state))
                {
                    --state;
                }
            }
        }

        public override void RemoveState(State state)
        {
            if (_currentStates.Contains(state))
            {
                if (state.OnRemoved())
                {
                    _currentStates.Remove(state);
                    PoolFactory<State>.Free(state);
                }
            }
        }

        public bool TryGetState(int id, out State state)
        {
            for (int i = 0; i < _currentStates.Count; i++)
            {
                if (_currentStates[i].TableState.Id == id)
                {
                    state = _currentStates[i];
                    return true;
                }
            }
            state = null;
            return false;
        }

        public void RemoveStateByType(EStateType stateType)
        {
            for (int i = _currentStates.Count - 1; i >= 0; i--)
            {
                if (_currentStates[i].TableState.StateType == (int)stateType)
                {
                    RemoveState(_currentStates[i]);
                }
            }
        }
        
        public override bool TryGetState(EStateType stateType, out State state)
        {
            for (int i = _currentStates.Count - 1; i >= 0; i--)
            {
                if (_currentStates[i].TableState.StateType == (int)stateType)
                {
                    state = _currentStates[i];
                    return true;
                }
            }
            state = null;
            return false;
        }

        public bool HasStateType(EStateType stateType)
        {
            State state;
            return TryGetState(stateType, out state);
        }

        public void RemoveAllStates()
        {
            for (int i = 0; i < _currentStates.Count; i++)
            {
                _currentStates[i].OnRemoved();
                PoolFactory<State>.Free(_currentStates[i]);
            }
            _currentStates.Clear();
        }

        public override void RemoveAllDebuffs()
        {
            for (int i = _currentStates.Count - 1; i >= 0; i--)
            {
                if (_currentStates[i].TableState.IsBuff == 0)
                {
                    RemoveState(_currentStates[i]);
                }
            }
        }

        private static int SortState(State one, State other)
        {
            int v = one.TableState.StatePriority.CompareTo(other.TableState.StatePriority);
            if (v == 0)
            {
                v = one.TableState.StateTypePriority.CompareTo(other.TableState.StateTypePriority);
            }
            return v;
        }

        internal override void InLazer()
        {
            if (!_isAlive || IsInvincible)
            {
                return;
            }
            _eDieType = EDieType.Lazer;
            OnDead();
            if (_animation != null)
            {
                _animation.PlayOnce("DeathLazer");
            }
        }
        
        internal override void InSaw()
        {
            if (!_isAlive || IsInvincible)
            {
                return;
            }
            _eDieType = EDieType.Saw;
            OnDead();
            if (_animation != null)
            {
                _animation.PlayOnce("OnSawStart");
            }
        }
   
        internal override void InWater()
        {
            //每一帧只检测一个水。
            if (_hasWaterCheckedInFrame)
            {
                return;
            }
            _hasWaterCheckedInFrame = true;
            if (HasStateType(EStateType.Fire))
            {
                //跳出水里
                Speed = IntVec2.zero;
                ExtraSpeed.y = 240;
                RemoveStateByType(EStateType.Fire);
                return;
            }
            if (!_isAlive || IsInvincible)
            {
                return;
            }
            _eDieType = EDieType.Water;
            OnDead();
            if (_animation != null)
            {
                _animation.PlayOnce("DeathWater");
            }
        }
        
        internal override void InFan(UnitBase fanUnit, IntVec2 force)
        {
            if (_fanForces.ContainsKey(fanUnit.Guid))
            {
                _fanForces[fanUnit.Guid] = force;
            }
            else
            {
                _fanForces.Add(fanUnit.Guid, force);
            }
            _fanForce= IntVec2.zero;
            var iter = _fanForces.GetEnumerator();
            while (iter.MoveNext())
            {
                _fanForce += iter.Current.Value;
            }
            ExtraSpeed.x = _fanForce.x;
        }

        internal override void OutFan(UnitBase fanUnit)
        {
            _fanForces.Remove(fanUnit.Guid);
            _fanForce = IntVec2.zero;
            if (_fanForces.Count > 0)
            {
                var iter = _fanForces.GetEnumerator();
                while (iter.MoveNext())
                {
                    _fanForce += iter.Current.Value;
                }
            }
        }

        protected override bool CheckOutOfMap()
        {
            if (base.CheckOutOfMap())
            {
                _eDieType = EDieType.OutofMap;
                return true;
            }
            return false;
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (_isAlive && unit.IsAlive)
            {
                State state;
                if (TryGetState(EStateType.Fire, out state))
                {
                    unit.AddStates(state.TableState.Id);
                }
            }
        }
    }
}