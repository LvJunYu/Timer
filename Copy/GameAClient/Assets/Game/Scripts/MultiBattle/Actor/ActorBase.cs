/********************************************************************
** Filename : ActorBase
** Author : Dong
** Date : 2017/5/20 星期六 上午 10:51:33
** Summary : ActorBase
***********************************************************************/

using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    public enum EDieType
    {
        None,
        Lazer,
        Water,
        Fire,
        Saw,
        TigerEat //老虎吃掉
    }

    public enum EKillerType
    {
        None,
        Trap,
        Monster,
        Drowned //淹死
    }

    public enum EClayOnWallDirection
    {
        None,
        Down,
        Left,
        Right
    }

    public class ActorBase : DynamicRigidbody
    {
        protected const string Death = "Death";
        protected const string DeathLazer = "DeathLazer";
        protected const string DeathWater = "DeathWater";
        protected const string DeathFire = "DeathFire";
        protected const string OnSawStart = "OnSawStart";
        protected const string Idle = "Idle";
        public const string Run = "Run";
        protected const string Attack = "Attack";
        protected const int _breakFlagDuration = 5 * ConstDefineGM2D.FixedFrameCount; //使坏标记持续时间，用于击杀者判断
        protected PlayerBase _curBreaker; //使坏的人，用于击杀者判断
        protected int _breakFrame; //使坏的帧数，用于击杀判断
        protected BehaviorTree _behaviour;
        protected UnitBase _hitUnit;
        protected UnitBase _attackUnit;
        protected bool _stampedEmpty;
        protected EActorState _lastActorState;
        protected EActorState _actorState;
        private static EInputType[] _skillInputs = {EInputType.Skill1, EInputType.Skill2, EInputType.Skill3};

        protected List<State> _currentStates = new List<State>();
        private Comparison<State> _comparisonState = SortState;

        protected EDieType _eDieType;

        /// <summary>
        /// 每一帧只检查一个水块
        /// </summary>
        protected bool _hasWaterCheckedInFrame;

        protected SkillCtrl _skillCtrl;

        private int _damageFrame;
        private int _hpStayTimer;

        protected StatusBar _statusBar;
        protected TextBar _textBar;
        protected int _jumpAbility;
        protected IntVec2 _attackRange;
        protected float _injuredReduce;
        protected float _curIncrease;

        public override EDieType EDieType
        {
            get { return _eDieType; }
        }

        public override bool IsActor
        {
            get { return true; }
        }

        protected override bool IsInWater
        {
            get { return _eDieType == EDieType.Water; }
        }

        public UnitBase HitUnit
        {
            get { return _hitUnit; }
        }

        public bool StampedEmpty
        {
            get { return _stampedEmpty; }
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
            _curBreaker = null;
            _breakFrame = 0;
            _eDieType = EDieType.None;
            _damageFrame = 0;
            if (_view != null)
            {
                _view.SetDamageShaderValue("Value", 0);
            }

            _hpStayTimer = 0;
            _skillCtrl = null;
            _hitUnit = null;
            _attackUnit = null;
            _stampedEmpty = false;
            if (_behaviour != null)
            {
                _behaviour.DisableBehavior(false);
            }

            _lastMonsterPos = _curPos;
            _path.Clear();
            _stuckTimer = 0;
            _reSeekTimer = 0;
            _currentNodeId = 0;
            base.Clear();
        }

        protected override void OnLand()
        {
            base.OnLand();
            if (GameModeBase.DebugEnable())
            {
                GameModeBase.WriteDebugData(string.Format("Type = {1}, Actor {0} OnLand ", Guid, GetType().Name));
            }

            if (HasStateType(EStateType.Stun))
            {
                //落地时候移除掉猛犸象的晕眩
                RemoveStates(72);
            }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }

            if (!string.IsNullOrEmpty(_tableUnit.BehaviorName))
            {
                _behaviour = BehaviorTreePool.Instance.Get(_tableUnit.BehaviorName);
                _behaviour.SetVariableValue("Owner", this);
            }

            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            if (_behaviour != null)
            {
                _behaviour.EnableBehavior();
            }

            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            if (_behaviour != null)
            {
                _behaviour.EnableBehavior();
            }
        }

        public override void Dispose()
        {
            if (_behaviour != null)
            {
                BehaviorTreePool.Instance.Free(_behaviour);
                _behaviour = null;
            }

            base.Dispose();
        }

        internal override void OnViewDispose()
        {
            if (_statusBar != null)
            {
                Object.Destroy(_statusBar.gameObject);
                _statusBar = null;
            }

            if (_textBar != null)
            {
                Object.Destroy(_textBar.gameObject);
                _textBar = null;
            }

            RemoveAllStates();
            if (_behaviour != null)
            {
                _behaviour.DisableBehavior(true);
            }

            base.OnViewDispose();
        }

        public override void BeforeLogic()
        {
            base.BeforeLogic();
            _hasWaterCheckedInFrame = false;
            _hitUnit = null;
            _attackUnit = null;
            _stampedEmpty = false;
        }

        public override void UpdateLogic()
        {
            if (_eActiveState != EActiveState.Active)
            {
                return;
            }

            base.UpdateLogic();
            //死亡时也要变色
            CheckShowDamage();
        }

        protected override void UpdateData()
        {
            if (_eActiveState == EActiveState.Active && _input != null && CanMove && !IsInState(EEnvState.Ice))
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

            if (_dropClimbTimer > 0)
            {
                _dropClimbTimer--;
            }

            for (int i = 0; i < _currentStates.Count; i++)
            {
                _currentStates[i].UpdateLogic();
            }
        }

        public virtual void UpdateInput()
        {
            if (!PlayMode.Instance.SceneState.GameRunning)
            {
                return;
            }

            if (_curBanInputTime == 0 && !IsHoldingBox())
            {
                switch (ClimbState)
                {
                    case EClimbState.None:
                    case EClimbState.ClimbLikeLadder:
                        if (_input.GetKeyApplied(EInputType.Left))
                        {
                            SetFacingDir(EMoveDirection.Left);
                        }
                        else if (_input.GetKeyApplied(EInputType.Right))
                        {
                            SetFacingDir(EMoveDirection.Right);
                        }

                        break;
                    case EClimbState.Up: //翻转
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
                if (ClimbState > EClimbState.None)
                {
                    ExtraSpeed.y = 0;
                    _jumpLevel = 0;
                    _jumpState = EJumpState.Jump1;
                    switch (ClimbState)
                    {
                        case EClimbState.Left:
                            _climbJump = true;
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

                            break;
                        case EClimbState.Right:
                            _climbJump = true;
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

                            break;
                        case EClimbState.Up:
                            _climbJump = true;
                            SpeedY = -10;
                            break;
                        case EClimbState.ClimbLikeLadder:
                            if (_input.GetKeyApplied(EInputType.Down))
                            {
                                SpeedX = 0;
                                SpeedY = 0;
                                _dropClimbTimer = 15;
                            }
                            else
                            {
                                SpeedY = 120;
                                if (_input.GetKeyApplied(EInputType.Up))
                                {
                                    _dropClimbTimer = 15;
                                }
                            }

                            break;
                        case EClimbState.Rope:
                            //按着下的时候 直接下来
                            if (_input.GetKeyApplied(EInputType.Down))
                            {
                                SpeedX = 0;
                                SpeedY = 0;
                            }
                            else
                            {
                                Speed = _curClimbUnit.Speed * 2;
                                if (SpeedX > 0)
                                {
                                    SpeedX += 10;
                                }
                                else if (SpeedX < 0)
                                {
                                    SpeedX -= 10;
                                }

                                SpeedX = Mathf.Clamp(SpeedX, -160, 160);

                                SpeedY += 120;
                                if (SpeedY > 0)
                                {
                                    SpeedY = Mathf.Clamp(SpeedY, 120, 250);
                                }

                                RopeJoint ropeJoint = _curClimbUnit as RopeJoint;
                                if (ropeJoint != null)
                                {
                                    ropeJoint.JumpAwayRope(this as PlayerBase);
                                }
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
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
                    SpeedY = _onClay ? 120 : _jumpAbility;
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
                            SpeedY = _jumpAbility;
                            _jumpState = EJumpState.Jump2;
                        }

                        ExtraSpeed.y = 0;
                        _jumpTimer = 15;
                        _input.CurAppliedInputKeyAry[(int) EInputType.Jump] = false;
                    }
                }
            }

            //如果掉下梯子时按上下则重新上梯子
            if (_dropClimbTimer > 0 && _input.GetKeyUpApplied(EInputType.Down) ||
                _input.GetKeyUpApplied(EInputType.Up))
            {
                _dropClimbTimer = 0;
            }
        }

        protected virtual void CheckAssist()
        {
            if (_input.GetKeyUpApplied(EInputType.Assist))
            {
                OnBoxHoldingChanged();
                Scene2DManager.Instance.GetCurScene2DEntity().RpgManger.AssitConShowDiaEvent();
//                RpgTaskManger.Instance.AssitConShowDiaEvent();
            }
        }

        protected void CheckSkill()
        {
            var eShootDir = _moveDirection == EMoveDirection.Left
                ? EShootDirectionType.Left
                : EShootDirectionType.Right;
            if (ClimbState == EClimbState.Up)
            {
                eShootDir = _moveDirection == EMoveDirection.Left
                    ? EShootDirectionType.Right
                    : EShootDirectionType.Left;
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

            _angle = (int) eShootDir;
            if (IsCharacterAbilityAvailable(ECharacterAbility.Shoot) && _skillCtrl != null)
            {
                for (int i = 0; i < _skillCtrl.CurrentSkills.Length; i++)
                {
                    var skill = _skillCtrl.CurrentSkills[i];
                    if (skill == null)
                    {
                        continue;
                    }

                    switch (skill.EWeaponInputType)
                    {
                        case EWeaponInputType.GetKey:
                            if (_input.GetKeyApplied(_skillInputs[i]))
                            {
                                if (_skillCtrl.Fire(i) && skill.SkillType == ESkillType.Normal)
                                {
                                    ChangeGunView(i, eShootDir);
                                }

                                return;
                            }

                            break;
                        case EWeaponInputType.GetKeyUp:
                            if (_input.GetKeyUpApplied(_skillInputs[i]))
                            {
                                if (_skillCtrl.Fire(i) && skill.SkillType == ESkillType.Normal)
                                {
                                    ChangeGunView(i, eShootDir);
                                }
                            }

                            break;
                    }
                }
            }
        }

        public virtual void ChangeGunView(int slot, EShootDirectionType? eShootDir)
        {
        }

        private bool IsCharacterAbilityAvailable(ECharacterAbility eCharacterAbility)
        {
            if (!IsPlayer)
            {
                return true;
            }

            return GM2DGame.Instance.GameMode.IsPlayerCharacterAbilityAvailable(this, eCharacterAbility);
        }

        private void CheckKiller()
        {
            if (_curBreaker != null && _breakFrame > 0 &&
                GameRun.Instance.LogicFrameCnt - _breakFrame < _breakFlagDuration)
            {
                if (IsMonster)
                {
                    Messenger<UnitBase>.Broadcast(EMessengerType.OnMonsterDead, _curBreaker);
                }
                else if (IsPlayer)
                {
                    Messenger<PlayerBase, UnitBase>.Broadcast(EMessengerType.OnPlayerDead, this as PlayerBase,
                        _curBreaker);
                }

                _curBreaker = null;
                _breakFrame = 0;
            }
        }

        public void AddBreaker(PlayerBase player)
        {
            _curBreaker = player;
            _breakFrame = GameRun.Instance.LogicFrameCnt;
        }

        /// sender是状态的施与者，用于计算击杀
        public override void AddStates(UnitBase sender, params int[] ids)
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

                //记录debuff施与者，用于计算击杀
                if (tableState.IsBuff == 0 && sender != null && !IsSameTeam(sender.TeamId))
                {
                    var player = sender as PlayerBase;
                    if (player != null)
                    {
                        AddBreaker(player);
                    }
                }
                else
                {
                    _breakFrame = 0;
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
                    RemoveStateByType((EStateType) tableState.StateType);
                }

                state = PoolFactory<State>.Get();
                if (state.OnAttached(tableState, this, sender))
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
                if (_currentStates[i].TableState.StateType == (int) stateType)
                {
                    RemoveState(_currentStates[i]);
                }
            }
        }

        public override bool TryGetState(EStateType stateType, out State state)
        {
            for (int i = _currentStates.Count - 1; i >= 0; i--)
            {
                if (_currentStates[i].TableState.StateType == (int) stateType)
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
            CheckKiller();
            OnDead();
            if (IsMain)
            {
                Messenger<EKillerType>.Broadcast(EMessengerType.OnMainPlayerDead, EKillerType.Trap);
            }
        }

        internal override void InSaw()
        {
            if (!_isAlive || IsInvincible)
            {
                return;
            }

            _eDieType = EDieType.Saw;
            CheckKiller();
            OnDead();
            if (IsMain)
            {
                Messenger<EKillerType>.Broadcast(EMessengerType.OnMainPlayerDead, EKillerType.Trap);
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
            CheckKiller();
            OnDead();
            if (IsMain)
            {
                Messenger<EKillerType>.Broadcast(EMessengerType.OnMainPlayerDead, EKillerType.Drowned);
            }
        }

        protected override void OnDead()
        {
            base.OnDead();
            if (_statusBar != null)
            {
                _statusBar.SetHPActive(false);
            }

            if (_textBar != null)
            {
                _textBar.SetEnable(false);
            }

            if (HasStateType(EStateType.Fire))
            {
                _eDieType = EDieType.Fire;
            }

            State state;
            if (TryGetState(73, out state))
            {
                _eDieType = EDieType.TigerEat;
            }

            if (_animation != null)
            {
                RemoveAllStates(true);
                _animation.Reset();
                _animation.PlayOnce(DeathName(_eDieType));
                if (IsMain)
                {
                    GM2DGame.Instance.GameMode.RecordAnimation(DeathName(_eDieType), false);
                }
            }
        }

        protected virtual string DeathName(EDieType dieType)
        {
            switch (_eDieType)
            {
                case EDieType.Lazer:
                    return _animation.HasAnimation(DeathLazer) ? DeathLazer : Death;
                case EDieType.Water:
                    return _animation.HasAnimation(DeathWater) ? DeathWater : Death;
                case EDieType.Fire:
                    return _animation.HasAnimation(DeathFire) ? DeathFire : Death;
                case EDieType.Saw:
                    return _animation.HasAnimation(OnSawStart) ? OnSawStart : Death;
                default:
                    return Death;
            }
        }

        public override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (eDirectionType != EDirectionType.Down && eDirectionType != EDirectionType.Up)
            {
                _hitUnit = unit;
            }

            if (_isAlive && unit.IsAlive)
            {
                State state;
                if (TryGetState(EStateType.Fire, out state))
                {
                    unit.AddStates(state.Sender, state.TableState.Id);
                }
            }
        }

        public override void OnHpChanged(int hpChanged, UnitBase killer = null)
        {
            if (!_isAlive || !PlayMode.Instance.SceneState.GameRunning)
            {
                return;
            }

            if (hpChanged < 0)
            {
                //无敌时候不管用
                if (IsInvincible)
                {
                    return;
                }

                _damageFrame = BattleDefine.DamageDurationFrame;
//                if (_isMonster)
//                {
//                    _hpStayTimer = BattleDefine.HpStayTime;
//                    if (_statusBar != null)
//                    {
//                        _statusBar.SetHPActive(true);
//                    }
//                }
                hpChanged = (int) (hpChanged * _injuredReduce);
                if (killer != null)
                {
                    _attackUnit = killer;
                }
            }
            else
            {
                hpChanged = (int) (hpChanged * _curIncrease);
            }

            _hp += hpChanged;
            _hp = Mathf.Clamp(_hp, 0, _maxHp);
            if (_hp == 0)
            {
                OnDead();
                if (killer != null)
                {
                    if (IsMonster)
                    {
                        Messenger<UnitBase>.Broadcast(EMessengerType.OnMonsterDead, killer);
                    }
                    else if (IsPlayer)
                    {
                        Messenger<PlayerBase, UnitBase>.Broadcast(EMessengerType.OnPlayerDead, this as PlayerBase,
                            killer);
                    }
                }
            }

            if (_statusBar != null)
            {
                _statusBar.SetHP(hpChanged > 0 ? EHPModifyCase.Heal : EHPModifyCase.Hit, _hp, _maxHp);
            }
        }

        protected void CheckShowDamage()
        {
            if (_damageFrame > 0)
            {
                _damageFrame--;
                if (_view != null)
                {
                    _view.SetDamageShaderValue("Value", _damageFrame / (float) BattleDefine.DamageDurationFrame);
                }
            }

            if (_hpStayTimer > 0)
            {
                _hpStayTimer--;
                if (_hpStayTimer == 0 && _statusBar != null)
                {
                    _statusBar.SetHPActive(false);
                }
            }
        }

        protected virtual void CreateStatusBar()
        {
            if (null != _statusBar)
            {
                return;
            }

            GameObject statusBarObj =
                Object.Instantiate(JoyResManager.Instance.GetPrefab(EResType.ParticlePrefab, "StatusBar")) as
                    GameObject;
            if (null != statusBarObj)
            {
                _statusBar = statusBarObj.GetComponent<StatusBar>();
                CommonTools.SetParent(statusBarObj.transform, _trans);
            }

            if (_statusBar != null)
            {
                _statusBar.SetOwner(this);
            }
        }

        protected virtual void CreateTextBar()
        {
            if (null != _textBar)
            {
                return;
            }

            GameObject textObj =
                Object.Instantiate(JoyResManager.Instance.GetPrefab(EResType.ParticlePrefab, "TextBar")) as
                    GameObject;
            if (null != textObj)
            {
                _textBar = textObj.GetComponent<TextBar>();
                CommonTools.SetParent(textObj.transform, _trans);
            }
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!CheckProjectileHit(other))
            {
                return false;
            }

            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!CheckProjectileHit(other))
            {
                return false;
            }

            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!CheckProjectileHit(other))
            {
                return false;
            }

            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!CheckProjectileHit(other))
            {
                return false;
            }

            return base.OnRightHit(other, ref x, checkOnly);
        }

        //检测子弹穿过
        private bool CheckProjectileHit(UnitBase other)
        {
            if (UnitDefine.UseProjectileBullet(other.Id))
            {
                var projectile = other as ProjectileBase;
                if (projectile != null && !projectile.Skill.Owner.CanHarm(this))
                {
                    return false;
                }
            }

            return true;
        }

        protected void SetInput(EInputType eInputType, bool value)
        {
            _input.CurAppliedInputKeyAry[(int) eInputType] = value;
        }

        protected virtual void SetState(EActorState eActorState)
        {
            if (_actorState == eActorState)
            {
                return;
            }

            _lastActorState = _actorState;
            _actorState = eActorState;
        }

        //-----------------------寻路
        protected const int MaxStuckFrames = 30;
        protected const int MaxReSeekFrames = 5;
        protected List<IntVec2> _path = new List<IntVec2>();
        protected IntVec2 _lastMonsterPos;
        protected int _stuckTimer;
        protected int _reSeekTimer;
        protected int _currentNodeId;
        protected static IntVec2 PathRange = new IntVec2(3, 2);

        private bool CheckStuck(UnitBase unit)
        {
            if (_curPos != _lastMonsterPos)
            {
                _stuckTimer = 0;
            }
            else
            {
                ++_stuckTimer;
                if (_stuckTimer > MaxStuckFrames)
                {
                    FindPath(unit);
                    return true;
                }
            }

            _lastMonsterPos = _curPos;
            return false;
        }

        private bool CheckTargetFarAway(UnitBase unit)
        {
            IntVec2 distance = _path[_path.Count - 1] - unit.CurPos / ConstDefineGM2D.ServerTileScale;
            if (Mathf.Abs(distance.x) <= PathRange.x && Mathf.Abs(distance.y) <= PathRange.y)
            {
                _reSeekTimer = 0;
            }
            else
            {
                ++_reSeekTimer;
                if (_reSeekTimer > MaxReSeekFrames)
                {
                    FindPath(unit);
                    return true;
                }
            }

            return false;
        }

        private void ReachCurDest(out bool reachDes)
        {
            int lastNodeId = _currentNodeId;
            _currentNodeId++;
            if (_currentNodeId >= _path.Count)
            {
                reachDes = true;
                _path.Clear();
                return;
            }

            reachDes = false;
            if (_grounded)
            {
                SpeedY = GetJumpSpeedForNode(lastNodeId);
                if (SpeedY != 0)
                {
                    //起跳瞬间清除惯性
                    ClearSpeedX();
                }
            }
        }

        private void CalculateAISpeedX(IntVec2 curPos, IntVec2 destPos)
        {
            var relPosX = destPos.x - curPos.x;
            _curMaxSpeedX = Mathf.Min(Mathf.Abs(relPosX), _curMaxSpeedX);
            _moveDirection = relPosX > 0 ? EMoveDirection.Right : EMoveDirection.Left;
            SetInputByMoveDir(_moveDirection);
        }

        public void ClearPath()
        {
            _path.Clear();
        }

        public void CheckPath(UnitBase unit)
        {
            if (_path.Count <= 1)
            {
                FindPath(unit);
            }
        }

        public void FindPath(UnitBase unit)
        {
            _lastMonsterPos = _curPos;
            _path.Clear();
            _stuckTimer = 0;
            _reSeekTimer = 0;
            _currentNodeId = 0;
            var path = ColliderScene2D.CurScene.FindPath(this, unit, 3);
            if (path != null && path.Count > 1)
            {
                _currentNodeId = 1;
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    _path.Add(path[i]);
                }

                if (_grounded)
                {
                    SpeedY = GetJumpSpeedForNode(0);
                }
            }
            else
            {
                SetInput(EInputType.Left, false);
                SetInput(EInputType.Right, false);
            }

            if (GameModeBase.DebugEnable())
            {
                GameModeBase.WriteDebugData(string.Format("Type = {1}, FindPath {0}", _path.Count, GetType().Name));
            }
        }

        private int GetJumpSpeedForNode(int lastNodeId)
        {
            int currentNodeId = lastNodeId + 1;
            if (_path[currentNodeId].y - _path[lastNodeId].y > 0 && _grounded)
            {
                int jumpHeight = 1;
                for (int i = currentNodeId; i < _path.Count; ++i)
                {
                    if (_path[i].y - _path[lastNodeId].y >= jumpHeight)
                    {
                        jumpHeight = _path[i].y - _path[lastNodeId].y;
                    }

                    if (_path[i].y - _path[lastNodeId].y < jumpHeight ||
                        ColliderScene2D.CurScene.IsGround(_path[i].x, _path[i].y - 1))
                    {
                        return GetJumpSpeed(jumpHeight);
                    }
                }
            }

            return 0;
        }

        private int GetJumpSpeed(int jumpHeight)
        {
            if (jumpHeight <= 0)
            {
                return 0;
            }

            switch (jumpHeight)
            {
                case 1:
                    return 140;
                case 2:
                    return 180;
                case 3:
                    return 220;
            }

            return 0;
        }

        private void ClearSpeedX()
        {
            SpeedX = 0;
            SetInput(EInputType.Left, false);
            SetInput(EInputType.Right, false);
        }

        private void GetContext(ref IntVec2 pathPos, out IntVec2 currentDest, out IntVec2 nextDest,
            out bool destOnGround, out bool reachedX, out bool reachedY)
        {
            int halfSize = (int) (0.5f * ConstDefineGM2D.ServerTileScale);
            currentDest = _path[_currentNodeId] * ConstDefineGM2D.ServerTileScale;
            currentDest.x += halfSize;
            nextDest = currentDest;
            if (_path.Count > _currentNodeId + 1)
            {
                nextDest = _path[_currentNodeId + 1] * ConstDefineGM2D.ServerTileScale;
                nextDest.x += halfSize;
            }

            destOnGround = false;
            if (ColliderScene2D.CurScene.IsGround(_path[_currentNodeId].x, _path[_currentNodeId].y - 1))
            {
                destOnGround = true;
            }

            var lastDest = _path[_currentNodeId - 1] * ConstDefineGM2D.ServerTileScale;
            lastDest.x += halfSize;
            reachedX = ReachedNodeOnXAxis(pathPos, lastDest, currentDest);
            reachedY = ReachedNodeOnYAxis(pathPos, lastDest, currentDest);
            if (destOnGround && !_grounded)
            {
                reachedY = false;
            }
        }

        private bool ReachedNodeOnXAxis(IntVec2 pathPos, IntVec2 lastDest, IntVec2 currentDest)
        {
            return (lastDest.x <= currentDest.x && pathPos.x >= currentDest.x)
                   || (lastDest.x >= currentDest.x && pathPos.x <= currentDest.x)
                   || Mathf.Abs(pathPos.x - currentDest.x) <= _curMaxSpeedX;
        }

        private bool ReachedNodeOnYAxis(IntVec2 pathPos, IntVec2 lastDest, IntVec2 currentDest)
        {
            return (lastDest.y <= currentDest.y && pathPos.y >= currentDest.y)
                   || (lastDest.y >= currentDest.y && pathPos.y <= currentDest.y)
                   || (Mathf.Abs(pathPos.y - currentDest.y) <= _curMaxSpeedX);
        }

        public virtual void DoChase(UnitBase target, out bool reachDes)
        {
            SetState(EActorState.Chase);
            reachDes = false;
            if (CheckStuck(target))
            {
                return;
            }

            if (_path.Count <= 1)
            {
                CalculateAISpeedX(CenterDownPos, target.CenterDownPos);
                return;
            }

            if (CheckTargetFarAway(target))
            {
                return;
            }

            var pathPos = _curPos + new IntVec2(GetDataSize().x / 2, 0);
            IntVec2 currentDest, nextDest;
            bool destOnGround, reachedY, reachedX;
            GetContext(ref pathPos, out currentDest, out nextDest, out destOnGround, out reachedX, out reachedY);
            if (reachedX && Mathf.Abs(pathPos.x - currentDest.x) >= 2 * ConstDefineGM2D.ServerTileScale)
            {
                FindPath(target);
                return;
            }

            if (reachedX && reachedY)
            {
                ReachCurDest(out reachDes);
                return;
            }

            if (_curBanInputTime > 0)
            {
                SetInput(EInputType.Left, false);
                SetInput(EInputType.Right, false);
            }
            else
            {
                if (!reachedX)
                {
                    CalculateAISpeedX(pathPos, currentDest);
                }
            }
        }

        //-----------------------寻路

        public virtual bool CheckAttack(UnitBase target)
        {
            if (!CanAttack)
            {
                return false;
            }

            IntVec2 rel = CenterDownPos - target.CenterDownPos;
            if (Mathf.Abs(rel.x) > _attackRange.x || Mathf.Abs(rel.y) > _attackRange.y)
            {
                return false;
            }

            return true;
        }

        public virtual bool CheckTarget(int sensitivity, int maxHeightView, out UnitBase targetUnit)
        {
            targetUnit = null;
            var players = TeamManager.Instance.Players;
            int curPlayerCount = players.Count;
            int minRelX = int.MaxValue;
            for (int i = 0; i < curPlayerCount; i++)
            {
                if (!players[i]._isAlive)
                {
                    continue;
                }

                var relPos = players[i].CenterDownPos - CenterDownPos;
                int relY = Mathf.Abs(relPos.y);
                if (relY > maxHeightView * ConstDefineGM2D.ServerTileScale)
                {
                    continue;
                }

                int relX = Mathf.Abs(relPos.x);
                if (relX < minRelX)
                {
                    minRelX = relX;
                    targetUnit = players[i];
                }
            }

            return targetUnit != null && minRelX < sensitivity * 10 * ConstDefineGM2D.ServerTileScale;
        }

        public virtual bool IsAttacted()
        {
            return _attackUnit != null;
        }

        public virtual void ChangeMoveDir()
        {
            switch (_moveDirection)
            {
                case EMoveDirection.Left:
                    _moveDirection = EMoveDirection.Right;
                    break;
                case EMoveDirection.Right:
                    _moveDirection = EMoveDirection.Left;
                    break;
            }
        }

        public void ChangeFaceDir()
        {
            switch (_moveDirection)
            {
                case EMoveDirection.Left:
                    SetFacingDir(EMoveDirection.Right);
                    break;
                case EMoveDirection.Right:
                    SetFacingDir(EMoveDirection.Left);
                    break;
            }
        }

        public virtual void DoIdle()
        {
            SetInput(EInputType.Right, false);
            SetInput(EInputType.Left, false);
            SetState(EActorState.Idle);
        }

        public virtual void DoAttack()
        {
            SetState(EActorState.Attack);
            SetInput(EInputType.Skill1, true);
        }

        public virtual void DoRun(int strength, int vigor)
        {
            SetState(EActorState.Run);
            SetInputByMoveDir(_moveDirection);
        }

        public virtual void DoEscape(int strength, int vigor)
        {
            SetState(EActorState.Escape);
            if (_attackUnit != null)
            {
                var relPos = _curPos - _attackUnit.CurPos;
                _moveDirection = relPos.x > 0 ? EMoveDirection.Right : EMoveDirection.Left;
            }

            SetInputByMoveDir(_moveDirection);
        }

        public override bool SetInputByMoveDir(EMoveDirection eMoveDirection)
        {
            if (eMoveDirection == EMoveDirection.Right)
            {
                SetInput(EInputType.Left, false);
                SetInput(EInputType.Right, true);
            }
            else
            {
                SetInput(EInputType.Left, true);
                SetInput(EInputType.Right, false);
            }

            return true;
        }

        public void ShowBangSign()
        {
            var pos = GM2DTools.TileToWorld(new IntVec2(
                _moveDirection == EMoveDirection.Right ? ColliderGrid.XMin : ColliderGrid.XMax,
                ColliderGrid.YMax));
            GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Bang, pos, ESortingOrder.EffectItem);
        }

        public void ShowDialogSign()
        {
            var pos = GM2DTools.TileToWorld(new IntVec2(
                _moveDirection == EMoveDirection.Right ? ColliderGrid.XMin : ColliderGrid.XMax,
                ColliderGrid.YMax));
            GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Dialog, pos, ESortingOrder.EffectItem);
        }

        public void ShowStupidSign()
        {
            var pos = GM2DTools.TileToWorld(new IntVec2(
                _moveDirection == EMoveDirection.Right ? ColliderGrid.XMin : ColliderGrid.XMax,
                ColliderGrid.YMax));
            GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Question, pos,
                ESortingOrder.EffectItem);
        }

        public void ShowTextBar(string content)
        {
            if (_textBar == null)
            {
                CreateTextBar();
            }

            _textBar.SetEnable(true);
            _textBar.SetContent(content);
        }

        public void StopTextBar()
        {
            if (_textBar != null)
            {
                _textBar.SetEnable(false);
            }
        }
    }
}