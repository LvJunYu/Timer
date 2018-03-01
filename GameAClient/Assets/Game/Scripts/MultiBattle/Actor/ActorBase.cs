/********************************************************************
** Filename : ActorBase
** Author : Dong
** Date : 2017/5/20 星期六 上午 10:51:33
** Summary : ActorBase
***********************************************************************/

using System;
using System.Collections.Generic;
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
        public string Run = "Run";
        protected const string Attack = "Attack";
        protected const int _breakFlagDuration = 5 * ConstDefineGM2D.FixedFrameCount; //使坏标记持续时间，用于击杀者判断
        protected PlayerBase _curBreaker; //使坏的人，用于击杀者判断
        protected int _breakFrame; //使坏的帧数，用于击杀判断
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
        protected int _jumpAbility;
        protected IntVec2 _attackRange;
        protected int _injuredReduce;
        protected int _curIncrease;

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

        internal override void OnObjectDestroy()
        {
            if (_statusBar != null)
            {
                Object.Destroy(_statusBar.gameObject);
                _statusBar = null;
            }

            RemoveAllStates();
            base.OnObjectDestroy();
        }

        public override void CheckStart()
        {
            base.CheckStart();
            _hasWaterCheckedInFrame = false;
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

            if (_dropLadderTimer > 0)
            {
                _dropLadderTimer--;
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
                switch (ClimbState)
                {
                    case EClimbState.None:
                    case EClimbState.Ladder:
//                    case EClimbState.Rope:
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
                    if (ClimbState == EClimbState.Left)
                    {
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
                    }
                    else if (ClimbState == EClimbState.Right)
                    {
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
                    }
                    else if (ClimbState == EClimbState.Up)
                    {
                        _climbJump = true;
                        SpeedY = -10;
                    }
                    else if (ClimbState == EClimbState.Ladder)
                    {
                        //按着下的时候 直接下来
                        if (_input.GetKeyApplied(EInputType.Down))
                        {
                            SpeedX = 0;
                            SpeedY = 0;
                            _dropLadderTimer = 15;
                        }
                        else
                        {
                            SpeedY = 120;
                            if (_input.GetKeyApplied(EInputType.Up))
                            {
                                _dropLadderTimer = 15;
                            }
                        }
                    }
                    else if (ClimbState == EClimbState.Rope)
                    {
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
            if (_dropLadderTimer > 0 && _input.GetKeyUpApplied(EInputType.Down) ||
                _input.GetKeyUpApplied(EInputType.Up))
            {
                _dropLadderTimer = 0;
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
                    Messenger<UnitBase>.Broadcast(EMessengerType.OnPlayerDead, _curBreaker);
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

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
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
                hpChanged = (int) (hpChanged * TableConvert.GetInjuredReduce(_injuredReduce));
            }
            else
            {
                hpChanged = (int) (hpChanged * TableConvert.GetCurIncrease(_curIncrease));
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
                        Messenger<UnitBase>.Broadcast(EMessengerType.OnPlayerDead, killer);
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

            if (_statusBar != null) _statusBar.SetOwner(this);
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
                if (!projectile.Skill.Owner.CanHarm(this))
                {
                    return false;
                }
            }

            return true;
        }
    }
}