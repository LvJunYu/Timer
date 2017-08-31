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
    }

    public class ActorBase : DynamicRigidbody
    {
        private static EInputType[] _skillInputs = new EInputType[3]{EInputType.Skill1, EInputType.Skill2, EInputType.Skill3};

        protected List<State> _currentStates = new List<State>();
        private Comparison<State> _comparisonState = SortState;

        protected EDieType _eDieType;

        /// <summary>
        /// 每一帧只检查一个水块
        /// </summary>
        protected bool _hasWaterCheckedInFrame;
        
        protected SkillCtrl _skillCtrl;
        
        public override EDieType EDieType
        {
            get { return _eDieType; }
        }

        public override bool IsActor
        {
            get { return true; }
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
            if (_skillCtrl != null)
            {
                _skillCtrl.Clear();
            }
            base.Clear();
        }

        protected override void OnLand()
        {
            base.OnLand();
            if (HasStateType(EStateType.Stun))
            {
                //落地时候移除掉猛犸象的晕眩
                RemoveStates(72);
            }
        }

        internal override void OnObjectDestroy()
        {
            //有特效
            if (_skillCtrl != null)
            {
                _skillCtrl.Clear();
            }
            RemoveAllStates();
            base.OnObjectDestroy();
        }

        public override void CheckStart()
        {
            base.CheckStart();
            _hasWaterCheckedInFrame = false;
        }

        protected override void UpdateData()
        {
            if (_input != null && CanMove)
            {
                UpdateInput();
            }
            if (_skillCtrl != null && CanAttack)
            {
                _skillCtrl.UpdateLogic();
            }
            if (_jumpTimer > 0)
            {
                _jumpTimer--;
            }
            if ((_jumpTimer == 0 && SpeedY > 0) || SpeedY < 0)
            {
                _jumpState = EJumpState.Fall;
            }
            for (int i = 0; i < _currentStates.Count; i++)
            {
                _currentStates[i].UpdateLogic();
            }
        }

        public void UpdateInput()
        {
            if (!PlayMode.Instance.SceneState.GameRunning)
            {
                return;
            }
            if (_curBanInputTime == 0 && !IsHoldingBox())
            {
                switch (_eClimbState)
                {
                    case EClimbState.None:
                        if (_input.GetKeyApplied(EInputType.Left))
                        {
                            SetFacingDir(EMoveDirection.Left);
                        }
                        else if (_input.GetKeyApplied(EInputType.Right))
                        {
                            SetFacingDir(EMoveDirection.Right);
                        }
                        break;
                    case EClimbState.Up://翻转
                        if (_input.GetKeyApplied(EInputType.Left))
                        {
                            SetFacingDir(EMoveDirection.Right);
                        }
                        else if (_input.GetKeyApplied(EInputType.Right))
                        {
                            SetFacingDir(EMoveDirection.Left);
                        }
                        break;
                }
            }
            CheckJump();
            CheckAssist();
            CheckSkill();
        }
        
        protected virtual void CheckJump()
        {
            _climbJump = false;
            if (_input.GetKeyDownApplied(EInputType.Jump))
            {
                //攀墙跳
                if (_eClimbState > EClimbState.None )
                {
                    _climbJump = true;
                    ExtraSpeed.y = 0;
                    _jumpLevel = 0;
                    _jumpState = EJumpState.Jump1;
                    if (_eClimbState == EClimbState.Left)
                    {
                        //按着下的时候 直接下来
                        if (_input.GetKeyApplied(EInputType.Down) && !_input.GetKeyApplied(EInputType.Right))
                        {
                            SpeedX = 0;
                            SpeedY = 0;
                        }
                        else
                        {
                            SpeedX = 100;
                            SpeedY = 120;
                            SetFacingDir(EMoveDirection.Right);
                        }
                    }
                    else if (_eClimbState == EClimbState.Right)
                    {
                        //按着下的时候 直接下来
                        if (_input.GetKeyApplied(EInputType.Down) && !_input.GetKeyApplied(EInputType.Left))
                        {
                            SpeedX = 0;
                            SpeedY = 0;
                        }
                        else
                        {
                            SpeedX = -100;
                            SpeedY = 120;
                            SetFacingDir(EMoveDirection.Left);
                        }
                    }
                    else if (_eClimbState == EClimbState.Up)
                    {
                        SpeedY = -10;
                    }
                    SetClimbState(EClimbState.None);
                }
                else if (_jumpLevel == -1)
                {
                    if (_stepY > 0)
                    {
                        ExtraSpeed.y = _stepY;
                        _stepY = 0;
                    }
                    _jumpLevel = 0;
                    SpeedY = _onClay ? 120 : 165;
                    _jumpState = EJumpState.Jump1;
                    _jumpTimer = 10;
                }
                else if (IsCharacterAbilityAvailable(ECharacterAbility.DoubleJump))
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
                            SpeedY = 165;
                            _jumpState = EJumpState.Jump2;
                        }
                        ExtraSpeed.y = 0;
                        _jumpTimer = 15;
                        _input.CurAppliedInputKeyAry[(int) EInputType.Jump] = false;
                    }
                }
            }
        }
        
        protected void CheckAssist()
        {
            if (_input.GetKeyUpApplied(EInputType.Assist))
            {
                OnBoxHoldingChanged();
            }
        }
        
        protected void CheckSkill()
        {
            var eShootDir = _curMoveDirection == EMoveDirection.Left ? EShootDirectionType.Left : EShootDirectionType.Right;
            if (_eClimbState == EClimbState.Up)
            {
                eShootDir = _curMoveDirection == EMoveDirection.Left ? EShootDirectionType.Right : EShootDirectionType.Left;
            }
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
            if (IsCharacterAbilityAvailable(ECharacterAbility.Shoot) && _skillCtrl != null)
            {
                for (int i = 0; i < _skillCtrl.CurrentSkills.Length; i++)
                {
                    var skill = _skillCtrl.CurrentSkills[i];
                    if (skill == null)
                    {
                        continue;
                    }
                    switch (skill.ECostType)
                    {
                            case ECostType.Paint:
                                if (_input.GetKeyApplied(_skillInputs[i]))
                                {
                                    if (_skillCtrl.Fire(i))
                                    {
                                        ChangeGunView(i);
                                    }
                                    return;
                                }
                                break;
                            case ECostType.Magic:
                            case ECostType.Rage:
                                if (IsPlayer)
                                {
                                    if (_input.GetKeyUpApplied(_skillInputs[i]))
                                    {
                                        if (_skillCtrl.Fire(i))
                                        {
                                            ChangeGunView(i);
                                        }
                                    }
                                }
                                else
                                {
                                    if (_input.GetKeyApplied(_skillInputs[i]))
                                    {
                                        if (_skillCtrl.Fire(i))
                                        {
                                            ChangeGunView(i);
                                        }
                                    }
                                }
                                break;
                    }
                }
            }
        }
        
        public virtual void ChangeGunView(int slot)
        {

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
                if (id == 0)
                {
                    continue;
                }
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
                    OnAddState(state);
                    _currentStates.Add(state);
                    _currentStates.Sort(_comparisonState);
                    continue;
                }
                PoolFactory<State>.Free(state);
            }
        }

        protected virtual void OnAddState(State state)
        {
   
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

        public void RemoveAllStates(bool deadRemove = false)
        {
            for (int i = _currentStates.Count - 1; i >= 0; i--)
            {
                var state = _currentStates[i];
                if (state != null)
                {
                    if (deadRemove && state.TableState.DeadRemove == 0)
                    {
                        continue;
                    }
                    state.OnRemoved();
                    PoolFactory<State>.Free(state);
                    _currentStates.Remove(state);
                }
            }
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
        }
        
        internal override void InSaw()
        {
            if (!_isAlive || IsInvincible)
            {
                return;
            }
            _eDieType = EDieType.Saw;
            OnDead();
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
        }

        protected override void OnDead ()
        {
            base.OnDead ();
            if (HasStateType(EStateType.Fire))
            {
                _eDieType = EDieType.Fire;
            }
            if (_animation != null)
            {
                RemoveAllStates(true);
                _animation.Reset();
                switch (_eDieType)
                {
                    case EDieType.None:
                        _animation.PlayOnce("Death");
                        break;
                    case EDieType.Lazer:
                        _animation.PlayOnce("DeathLazer");
                        break;
                    case EDieType.Water:
                        _animation.PlayOnce("DeathWater");
                        break;
                    case EDieType.Fire:
                        _animation.PlayOnce("DeathFire");
                        break;
                    case EDieType.Saw:
                        _animation.PlayOnce(IsPlayer ? "OnSawStart" : "Death");
                        break;
                }
            }
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