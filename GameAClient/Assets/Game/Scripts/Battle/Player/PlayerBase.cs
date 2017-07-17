/********************************************************************
** Filename : PlayerBase
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:34:10
** Summary : PlayerBase
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class PlayerBase : ActorBase
    {
        #region Data

        protected long _playerGuid;

        [SerializeField]
        protected PlayerInput _playerInput;

        protected int _curMaxSpeedX;
        [SerializeField]
        protected int _flashTime;
        [SerializeField]
        protected int _invincibleTime;

        protected SkillCtrl _skillCtrl;
        protected Gun _gun;

        [SerializeField]
        protected IntVec2 _revivePos;
        /// <summary>
        /// 复活点被吃了
        /// </summary>
        protected Stack<IntVec2> _revivePosStack = new Stack<IntVec2>();

        protected Box _box;
        protected EBoxOperateType _eBoxOperateType;

        protected ReviveEffect _reviveEffect = new ReviveEffect();
        protected ReviveEffect _portalEffect = new ReviveEffect();

        public long PlayerGuid
        {
            get { return _playerGuid; }
        }

        public override SkillCtrl SkillCtrl
        {
            get { return _skillCtrl; }
        }

        public Box Box
        {
            get { return _box; }
        }

        public override bool CanPortal
        {
            get { return true; }
        }

        public override bool IsInvincible
        {
            get { return _invincibleTime > 0 || _flashTime > 0; }
        }

        public PlayerInput PlayerInput
        {
            get { return _playerInput; }
        }

        public int InvincibleTime
        {
            get { return _invincibleTime; }
            set
            {
                if (value > 0 && value > _invincibleTime)
                {
                    PlayEffect(_invincibleEfffect);
                    _invincibleTime = value;
//                    OutFire();
                }
            }
        }

        public override IntVec2 FirePos
        {
            get { return _gun.CurPos; }
        }

        public IntVec2 CameraFollowPos
        {
            get
            {
                switch (_eUnitState)
                {
                    case EUnitState.Portaling:
                        return GM2DTools.WorldToTile(_portalEffect.Position);
                    case EUnitState.Reviving:
                        return GM2DTools.WorldToTile(_reviveEffect.Position);
                }
                return _curPos;
            }
        }
        
        
        protected override void Clear()
        {
            _playerInput = _playerInput ?? new PlayerInput(this);
            _playerInput.Reset();

            _skillCtrl = _skillCtrl ?? new SkillCtrl(this, 2);
            _skillCtrl.Clear();
            ChangeWeapon(3);
            
            _dieTime = 0;
            _flashTime = 0;
            _invincibleTime = 0;
            _box = null;
            _eBoxOperateType = EBoxOperateType.None;
            ClearView();
            base.Clear();
        }

        public bool ChangeWeapon(int id)
        {
            var tableEquipment = TableManager.Instance.GetEquipment(id);
            if (tableEquipment == null)
            {
                LogHelper.Error("GetEquipment Failed : {0}", id);
                return false;
            }
            return _skillCtrl.ChangeSkill(tableEquipment.SkillIds);
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            LogHelper.Debug("{0}, OnPlay", GetType().Name);
            if (_gun == null)
            {
                _gun = PlayMode.Instance.CreateRuntimeUnit(10000, _curPos) as Gun;
            }
            _flashTime = ActorDefine.FlashTime;
            _revivePos = _curPos;
            _revivePosStack.Clear();
            if (PlayMode.Instance.IsUsingBoostItem(SoyEngine.Proto.EBoostItemType.BIT_AddLifeCount1))
            {
                Life = PlayMode.Instance.SceneState.Life + 1;
            }
            else
            {
                Life = PlayMode.Instance.SceneState.Life;
            }
        }

        public override void CheckStart()
        {
            base.CheckStart();
            _isStart = true;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_isAlive && _isStart && !_isFreezed)
            {
                if (_attackedTimer > 0)
                {
                    _attackedTimer--;
                }
                if (_attackedTimer <= 0)
                {
                    _playerInput.UpdateLogic();
                    _skillCtrl.UpdateLogic();
                }
                CheckGround();
                CheckClimb();
                UpdateSpeedY();
            }
        }

        protected virtual void CheckGround()
        {
            bool air = false;
            int friction = 0;
            if (_playerInput._jumpState >= 100)
            {
                air = true;
            }
            if (!air && SpeedY != 0)
            {
                air = true;
            }
            if (air)
            {
                friction = MaxFriction;
            }
            else
            {
                _onClay = false;
                _onIce = false;
                bool downExist = false;
                int deltaX = int.MaxValue;
                List<UnitBase> units = EnvManager.RetriveDownUnits(this);
                for (int i = 0; i < units.Count; i++)
                {
                    UnitBase unit = units[i];
                    int ymin = 0;
                    if (unit != null && unit.IsAlive && CheckOnFloor(unit) &&
                        unit.OnUpHit(this, ref ymin, true))
                    {
                        downExist = true;
                        _grounded = true;
                        _attackedTimer = 0;
                        _downUnits.Add(unit);
                        _playerInput._jumpState = 0;
                        _playerInput._jumpLevel = 0;
                        if (unit.Friction > friction)
                        {
                            friction = unit.Friction;
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
                        var delta = Mathf.Abs(CenterPos.x - unit.CenterPos.x);
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
            //起跳瞬间！
            if (air && _grounded)
            {
                Speed += _lastExtraDeltaPos;
                _grounded = false;
                if (SpeedY > 0)
                {
                    OnJump();
                }
            }
             if (_onIce)
            {
                friction = 1;
            }
            if (_eDieType == EDieType.Fire)
            {
                OnFire();
            }
            else
            {
                _fireTimer = 0;
            }
            if (!_lastOnClay && _onClay)
            {
                _speedRatio += SpeedClayRatio;
            }
            else if (_lastOnClay && !_onClay)
            {
                _speedRatio -= SpeedClayRatio;
            }
            _curMaxSpeedX = _playerInput._quickenTime == 0 ? ActorDefine.MaxSpeedX : ActorDefine.MaxQuickenSpeedX;
            _curMaxSpeedX = (int)(_curMaxSpeedX * _speedRatio);

            if (air || _grounded)
            {
                if (_curBanInputTime <= 0)
                {
                    if (_playerInput.LeftInput == 1)
                    {
                        //不在冰上的时候反向走速度直接设为0
                        if (SpeedX > 0 && !_onIce)
                        {
                            SpeedX = 0;
                        }
                        SpeedX = Util.ConstantLerp(SpeedX, -_curMaxSpeedX, Math.Min(friction, 10));
                    }
                    else if (_playerInput.RightInput == 1)
                    {
                        //不在冰上的时候反向走速度直接设为0
                        if (SpeedX < 0 && !_onIce)
                        {
                            SpeedX = 0;
                        }
                        SpeedX = Util.ConstantLerp(SpeedX, _curMaxSpeedX, Math.Min(friction, 10));
                    }
                    else if (_grounded)
                    {
                        SpeedX = Util.ConstantLerp(SpeedX, 0, friction);
                    }
                }
            }
        }

        private void OnFire()
        {
            _fireTimer++;
            //3秒后还是这个状态挂掉
            if (_fireTimer == 150)
            {
                OnDead();
                if (_animation != null)
                {
                    _animation.Reset();
                    _animation.PlayOnce("Death");
                }
            }
        }

        protected virtual void CheckClimb()
        {
            _playerInput._eClimbState = EClimbState.None;
            if (!_grounded && SpeedY < 0)
            {
                if (_playerInput.LeftInput > 0 && CheckLeftFloor())
                {
                    _playerInput._eClimbState = EClimbState.Left;
                }
                else if (_playerInput.RightInput > 0 && CheckRightFloor())
                {
                    _playerInput._eClimbState = EClimbState.Right;
                }
            }
        }

        protected virtual void UpdateSpeedY()
        {
            if (!_grounded)
            {
                if (_playerInput._eClimbState > EClimbState.None)
                {
                    SpeedY = Util.ConstantLerp(SpeedY, -50, 6);
                }
                else
                {
                    SpeedY = Util.ConstantLerp(SpeedY, -120, 12);
                }
            }
        }

        public override void UpdateRenderer(float deltaTime)
        {
            if (_isAlive)
            {
                _playerInput.UpdateRenderer();
            }
        }

        #region box

        protected virtual void CheckBox()
        {
            if (IsHoldingBox())
            {
                if (_colliderGrid.YMin != _box.ColliderGrid.YMin)
                {
                    OnBoxHoldingChanged();
                }
                return;
            }
            if (_box != null)
            {
                if (_deltaPos.y != 0 ||
                    (_deltaPos.x < 0 && _box.DirectionRelativeMain == EDirectionType.Right) ||
                    (_deltaPos.x > 0 && _box.DirectionRelativeMain == EDirectionType.Left))
                {
                    _box = null;
                }
            }
            if (IsValidBox(_hitUnits[(int)EDirectionType.Right]))
            {
                //弹出UI给提示
                Messenger<string>.Broadcast(EMessengerType.GameLog, "按 L 键可以推拉木箱");
                _box = _hitUnits[(int)EDirectionType.Right] as Box;
                if (_box != null)
                {
                    _box.DirectionRelativeMain = EDirectionType.Right;
                }
                _playerInput.ChangeLittleSkillState(ELittleSkillState.HoldBox);
            }
            else if (IsValidBox(_hitUnits[(int)EDirectionType.Left]))
            {
                //弹出UI给提示
                Messenger<string>.Broadcast(EMessengerType.GameLog, "按 L 键可以推拉木箱");
                _box = _hitUnits[(int)EDirectionType.Left] as Box;
                if (_box != null)
                {
                    _box.DirectionRelativeMain = EDirectionType.Left;
                }
                _playerInput.ChangeLittleSkillState(ELittleSkillState.HoldBox);
            }
            if (_box == null)
            {
                _playerInput.ChangeLittleSkillState(ELittleSkillState.Quicken);
            }
        }

        public void OnBoxHoldingChanged()
        {
            if (_box == null)
            {
                return;
            }
            _box.IsHoldingByMain = !_box.IsHoldingByMain;
            if (_box.IsHoldingByMain)
            {
                _speedRatio += SpeedHoldingBoxRatio;
            }
            else
            {
                _speedRatio -= SpeedHoldingBoxRatio;
            }
            LogHelper.Debug("OnBoxHoldingChanged: " + _box.IsHoldingByMain);
        }

        private bool IsValidBox(UnitBase unit)
        {
            return unit != null && unit.Id == UnitDefine.BoxId && unit.ColliderGrid.YMin == _colliderGrid.YMin;
        }

        public bool IsHoldingBox()
        {
            return _box != null && _box.IsHoldingByMain;
        }

        public EBoxOperateType GetBoxOperateType()
        {
            if (!IsHoldingBox())
            {
                return EBoxOperateType.None;
            }
            var deltaPos = _speed + _extraDeltaPos;
            if (deltaPos.x == 0)
            {
                return EBoxOperateType.None;
            }
            if ((deltaPos.x > 0 && _box.DirectionRelativeMain == EDirectionType.Right)
                || (deltaPos.x < 0 && _box.DirectionRelativeMain == EDirectionType.Left))
            {
                return EBoxOperateType.Push;
            }
            return EBoxOperateType.Pull;
        }

        #endregion

        #region event

        public override void OnShootHit(UnitBase other)
        {
            OnDamage();
        }

        public override void OnHeroTouch(UnitBase other)
        {
            if (_invincibleTime > 0)
            {
                other.OnInvincibleHit(this);
                return;
            }
            OnDamage();
        }

        public override void OnCrushHit(UnitBase other)
        {
            if (_grounded)
            {
                OnDead();
            }
        }

        public override void OnDamage()
        {
            if (!_isAlive)
            {
                return;
            }
            if (!PlayMode.Instance.SceneState.GameRunning)
            {
                return;
            }
            if (_invincibleTime > 0)
            {
                return;
            }
            if (_flashTime > 0)
            {
                return;
            }
            OnDead();
        }

        protected override void OnDead()
        {
            if (!_isAlive)
            {
                return;
            }
            LogHelper.Debug("{0}, OnDead", GetType().Name);
            _invincibleTime = 0;
            _flashTime = 0;
            StopEffect(_invincibleEfffect);
            StopEffect(_inFireEfffect);
            if (_gun != null)
            {
                _gun.Stop();
            }
            _playerInput.Clear();
            Messenger.Broadcast(EMessengerType.OnMainPlayerDead);
            base.OnDead();
            if (_life <= 0)
            {
                LogHelper.Debug("GameOver!");
                GameRun.Instance.Pause();
                OnDeadAll();
            }
            else
            {
                PlayMode.Instance.CurrentShadow.RecordNormalDeath();
                if (_view != null)
                {
                    GameParticleManager.Instance.Emit("M1EffectAirDeath", _trans.position + Vector3.up * 0.5f);
                }
                OnRevive();
            }
        }

        protected void OnRevive()
        {
            LogHelper.Debug("{0}, OnRevive", GetType().Name);
            _eUnitState = EUnitState.Reviving;
            _reviveEffect.Play(_trans.position + Vector3.up * 0.5f,
                                GM2DTools.TileToWorld(_revivePos), 20, () =>
                                {
                                    _eUnitState = EUnitState.Normal;
                                    _playerInput.Clear();
                                    ClearRunTime();
                                    _isAlive = true;
                                    _flashTime = 100;
                                    _dieTime = 0;
                                    _box = null;
                                    _trans.eulerAngles = new Vector3(0, 0, 0);
                                    SetPos(_revivePos);
                                    PlayMode.Instance.UpdateWorldRegion(_curPos);
                                    if (_gun != null)
                                    {
                                        _gun.Play();
                                    }
                                    _animation.Reset();
                                    _animation.PlayLoop(IdleAnimName());
                                    PlayMode.Instance.CurrentShadow.RecordAnimation(IdleAnimName(), true);
                                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.M2Reborn);
                                    Messenger.Broadcast(EMessengerType.OnMainPlayerRevive);
                                });
        }

        public override void OnPortal(IntVec2 targetPos, IntVec2 speed)
        {
            if (_eUnitState == EUnitState.Portaling)
            {
                return;
            }
            _eUnitState = EUnitState.Portaling;
            PlayMode.Instance.Freeze(this);
            StopEffect(_invincibleEfffect);
            _playerInput.Clear();
            ClearRunTime();
            _trans.eulerAngles = new Vector3(90, 0, 0);
            _portalEffect.Play(_trans.position + Vector3.up * 0.5f,
                GM2DTools.TileToWorld(targetPos), 30, () => PlayMode.Instance.RunNextLogic(() =>
                {
                    _eUnitState = EUnitState.Normal;
                    PlayMode.Instance.UnFreeze(this);
                    _trans.eulerAngles = new Vector3(0, 0, 0);
                    Speed = speed;
                    SetPos(targetPos);
                    PlayMode.Instance.UpdateWorldRegion(_curPos);
                    _animation.Reset();
                    _animation.PlayLoop(IdleAnimName());
                    PlayMode.Instance.CurrentShadow.RecordAnimation(IdleAnimName(), true);
                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.M2Reborn);
                }));
        }

        public virtual void OnSucceed()
        {
            _animation.ClearTrack(0);
            PlayMode.Instance.CurrentShadow.RecordClearAnimTrack(0);
            _animation.ClearTrack(1);
            PlayMode.Instance.CurrentShadow.RecordClearAnimTrack(1);
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
            _animation.PlayLoop(VictoryAnimName(), 1, 1);
            PlayMode.Instance.CurrentShadow.RecordAnimation(VictoryAnimName(), true);
        }

        public override void OnRevivePos(IntVec2 pos)
        {
            if (_revivePos == pos)
            {
                return;
            }
            _revivePosStack.Push(_revivePos);
            _revivePos = pos;
        }

        protected void RollbackRevivePos()
        {
            _revivePos = _revivePosStack.Pop();
        }

        public void Step(int stepY = 0)
        {
            _playerInput.Step = true;
            _playerInput.StepY = stepY;
            _grounded = true;
        }

        #endregion

        internal void OnStun(ActorBase actor)
        {
            //晕2秒
            _attackedTimer = ActorDefine.StunTime;
            Speed = IntVec2.zero;
            ExtraSpeed.y = 120;
            ExtraSpeed.x = actor.CenterPos.x > CenterPos.x ? -100 : 100;
            _playerInput.ClearInput();
        }

        internal void OnKnockBack(ActorBase actor)
        {
            _attackedTimer = ActorDefine.StunTime;
            Speed = IntVec2.zero;
            ExtraSpeed.y = 280;
            ExtraSpeed.x = actor.CenterPos.x > CenterPos.x ? -80 : 80;
            _playerInput.ClearInput();
        }

//        internal override void InFire()
//        {
//            if (!_isAlive)
//            {
//                return;
//            }
//            if (IsInvincible)
//            {
//                return;
//            }
//            if (_eDieType == EDieType.Fire)
//            {
//                return;
//            }
//            _eDieType = EDieType.Fire;
//            if (_inFireEfffect == null)
//            {
//                _inFireEfffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectDeathFire", _trans);
//            }
//            if (_inFireEfffect != null)
//            {
//                _inFireEfffect.Play();
//            }
//            if (_view != null)
//            {
//                _view.SetRendererColor(new Color(0.2f, 0.2f, 0.2f, 1f));
//            }
//        }
//
//        internal override void OutFire()
//        {
//            base.OutFire();
//            if (_eDieType == EDieType.Fire)
//            {
//                if (_inFireEfffect != null)
//                {
//                    _inFireEfffect.Stop();
//                }
//                if (_view != null)
//                {
//                    _view.SetRendererColor(Color.white);
//                }
//            }
//        }

        #endregion

        #region View

        [SerializeField]
        protected UnityNativeParticleItem _invincibleEfffect;
        protected UnityNativeParticleItem _inFireEfffect;

        /// <summary>
        /// 跑步声音间隔
        /// </summary>
        protected int _walkAudioInternal = 12;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init("Idle");
            if (!_animation.AddEventHandle("Step", OnStep))
            {
                return false;
            }
            _invincibleEfffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectOrbitBuff", _trans);
            _inFireEfffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectDeathFire", _trans);
            _reviveEffect.Set(GameParticleManager.Instance.GetUnityNativeParticleItem(ConstDefineGM2D.M1EffectSoul, null, ESortingOrder.LazerEffect));
            _portalEffect.Set(GameParticleManager.Instance.GetUnityNativeParticleItem(ConstDefineGM2D.PortalingEffect, null, ESortingOrder.LazerEffect));
            return true;
        }

        private void ClearView()
        {
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
                _view.SetRendererColor(Color.white);
            }
            _reviveEffect.Stop();
            _portalEffect.Stop();
            StopEffect(_invincibleEfffect);
            StopEffect(_inFireEfffect);
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            _reviveEffect.Free();
            _portalEffect.Free();
            FreeEffect(_invincibleEfffect);
            FreeEffect(_inFireEfffect);
            _invincibleEfffect = null;
            _inFireEfffect = null;
        }

        public override void UpdateView(float deltaTime)
        {
            if (!PlayMode.Instance.SceneState.GameRunning && PlayMode.Instance.SceneState.Arrived)
            {
                return;
            }
            if (_isAlive && _isStart)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                LimitPos();
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
            }
            if (!_isAlive)
            {
                _dieTime++;
                if (_life <= 0)
                {
                    if (_dieTime == 20)
                    {
                        Messenger.Broadcast(EMessengerType.GameFailedDeadMark);
                        SpeedY = 150;
                    }
                    if (_dieTime == 100)
                    {
                        PlayMode.Instance.SceneState.MainUnitSiTouLe();
                        // 因生命用完而失败
                        Messenger.Broadcast(EMessengerType.GameFinishFailed);
                    }
                }
            }
            CheckOutOfMap();
            CheckBox();
            if (_invincibleTime > 0)
            {
                _invincibleTime--;
                if (_invincibleTime == 0)
                {
                    if (_view != null)
                    {
                        _view.SetRendererColor(Color.white);
                    }
                    StopEffect(_invincibleEfffect);
                }
                else
                {
                    Chameleon();
                }
            }
            if (_flashTime > 0)
            {
                _flashTime--;
                FlashRenderer(_flashTime);
            }
            if (_eDieType == EDieType.Fire)
            {
                if (_inFireEfffect != null)
                {
                    _inFireEfffect.Trans.localPosition = Vector3.forward * (_curMoveDirection == EMoveDirection.Left ? 0.01f : -0.01f);
                    _inFireEfffect.Trans.rotation = Quaternion.identity;
                }
            }
            if (_isAlive)
            {
                if (!_grounded)
                {
                    if (_playerInput._eClimbState > 0)
                    {
                        if (_animation.PlayLoop(ClimbAnimName()))
                        {
                            PlayMode.Instance.CurrentShadow.RecordAnimation(ClimbAnimName(), true);
                        }
                        if (GameRun.Instance.LogicFrameCnt % 5 == 0)
                        {
                            Vector3 effectPos = _trans.position;
                            if (_curMoveDirection == EMoveDirection.Left)
                            {
                                effectPos += Vector3.left * 0.25f + Vector3.forward * 0.6f;
                            }
                            else
                            {
                                effectPos += Vector3.right * 0.25f + Vector3.forward * 0.6f;
                            }
                            GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.WallClimb, effectPos);
                        }
                    }
                    else if (_playerInput._jumpLevel >= 0)
                    {
                        if (_playerInput.ClimbJump)
                        {
                            Vector3 effectPos = _trans.position;
                            effectPos += _curMoveDirection == EMoveDirection.Left ? Vector3.right * 0.25f + Vector3.forward * 0.6f : Vector3.left * 0.25f + Vector3.forward * 0.6f;
                            GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.WallJump, effectPos);
                        }
                        if (_playerInput.JumpState == 101 || _playerInput.JumpState == 201)
                        {
                            Messenger.Broadcast(EMessengerType.OnPlayerJump);
                            _animation.PlayOnce(JumpAnimName(_playerInput._jumpLevel));
                            PlayMode.Instance.CurrentShadow.RecordAnimation(JumpAnimName(_playerInput._jumpLevel), false);
                        }
                        else if ((SpeedY != 0 || _playerInput.JumpState == 1) && !_animation.IsPlaying(JumpAnimName(_playerInput._jumpLevel)))
                        {
                            if (_animation.PlayLoop(FallAnimName()))
                            {
                                PlayMode.Instance.CurrentShadow.RecordAnimation(FallAnimName(), true);
                            }
                        }
                    }
                }
                else if (SpeedX != 0)
                {
                    if (_playerInput.LeftInput != 0 || _playerInput.RightInput != 0)
                    {
                        var speed = Math.Abs(SpeedX);
                        speed = Mathf.Clamp(speed, 30, 100);
                        if (IsHoldingBox())
                        {
                            speed = 50;
                        }
                        if (_animation.PlayLoop(RunAnimName(speed), speed * deltaTime))
                        {
                            PlayMode.Instance.CurrentShadow.RecordAnimation(RunAnimName(speed), true, speed * deltaTime);
                        }
                        if (speed <= ActorDefine.MaxSpeedX)
                        {
                            _walkAudioInternal -= 7;
                        }
                        else
                        {
                            _walkAudioInternal -= 5;
                            //GuideManager.Instance.OnSpecialOperate(1);
                        }
                        if (_walkAudioInternal <= 0)
                        {
                            int randomValue = UnityEngine.Random.Range(0, 3);
                            switch (randomValue)
                            {
                                case 0:
                                    GameAudioManager.Instance.PlaySoundsEffects("AudioWalkWood01");
                                    break;
                                case 1:
                                    GameAudioManager.Instance.PlaySoundsEffects("AudioWalkWood02");
                                    break;
                                case 2:
                                    GameAudioManager.Instance.PlaySoundsEffects("AudioWalkWood03");
                                    break;
                            }
                            _walkAudioInternal = 35;
                        }
                    }
                    else
                    {
                        if (_animation.PlayLoop(IdleAnimName()))
                        {
                            PlayMode.Instance.CurrentShadow.RecordAnimation(IdleAnimName(), true);
                        }
                    }
                }
                else if (!_lastGrounded)
                {
                    //_animation.PlayOnce(LandAnimName());
                    //PlayMode.Instance.CurrentShadow.RecordAnimation(LandAnimName(), false);
                    OnLand();
                }
                else if (!_animation.IsPlaying(LandAnimName()))
                {
                    if (_animation.PlayLoop(IdleAnimName()))
                    {
                        PlayMode.Instance.CurrentShadow.RecordAnimation(IdleAnimName(), true);
                    }
                }
            }
            if (_reviveEffect != null)
            {
                _reviveEffect.Update();
            }
            if (_portalEffect != null)
            {
                _portalEffect.Update();
            }
            _lastGrounded = _grounded;
            _lastOnClay = _onClay;
        }

        protected void OnJump()
        {
            if (!GameAudioManager.Instance.IsPlaying(AudioNameConstDefineGM2D.GameAudioSpingEffect))
            {
                GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioLightingJump);
            }
            if (_downUnit == null || _view == null)
            {
                return;
            }
            if (_downUnit.Id == UnitDefine.ClayId)
            {
                GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Jump, _trans.position);
            }
        }

        protected void OnLand()
        {
            // 新手引导需要知道主角落地了
            GuideManager.Instance.OnSpecialOperate(2);
            if (_downUnit == null || _view == null)
            {
                return;
            }
            if (_downUnit.Id == UnitDefine.ClayId)
            {
                GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Land, _trans.position);
            }
        }

        protected void OnDeadAll()
        {
            _animation.PlayOnce(DeathAnimName());
            PlayMode.Instance.CurrentShadow.RecordAnimation(DeathAnimName(), false);
        }

        /// <summary>
        /// 播放跑步烟尘
        /// </summary>
        protected void OnStep()
        {
            if (_downUnit == null || _view == null)
            {
                return;
            }
            Vector3 scale = _curMoveDirection == EMoveDirection.Right ? Vector3.one : new Vector3(-1, 1, 1);
            if (_downUnit.Id == UnitDefine.ClayId)
            {
                GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.RunOnMud, _trans.position + Vector3.up * 0.2f, scale, 1);
            }
        }

        protected void FlashRenderer(int time)
        {
            if (_view == null)
            {
                return;
            }
            int t = time % 20;
            var a = t > 9 ? Mathf.Clamp01((t - 10) * 0.1f + 0.3f) : Mathf.Clamp01(1f - t * 0.1f + 0.3f);
            _view.SetRendererColor(new Color(1f, 1f, 1f, a));
        }

        protected void Chameleon()
        {
            if (_view == null)
            {
                return;
            }
            var a = new Color(1f, 0.8235f, 0.804f, 0.804f);
            var b = new Color(0.9f, 0.41f, 0.804f, 0.804f);
            var c = new Color(1f, 0.745f, 0.63f, 0.804f);
            const int interval = 5;
            int t = GameRun.Instance.LogicFrameCnt % (3 * interval);
            if (t < interval)
            {
                _view.SetRendererColor(Color.Lerp(a, b, t * (1f / interval)));
            }
            else if (t < 2 * interval)
            {
                _view.SetRendererColor(Color.Lerp(c, b, (2f * interval - t) * (1f / interval)));
            }
            else
            {
                _view.SetRendererColor(Color.Lerp(a, c, (3f * interval - t) * (1f / interval)));
            }
        }

        protected virtual string RunAnimName(float speed)
        {
            if (IsHoldingBox())
            {
                if (_speed.x == 0)
                {
                    if (_playerInput.RightInput == 0 && _playerInput.LeftInput == 0)
                    {
                        return "Prepare";
                    }
                    if (_playerInput.RightInput > 0 && _box.DirectionRelativeMain == EDirectionType.Right
                        || (_playerInput.LeftInput > 0 && _box.DirectionRelativeMain == EDirectionType.Left))
                    {
                        return "Push";
                    }
                    return "Pull";
                }
                if ((_speed.x > 0 && _box.DirectionRelativeMain == EDirectionType.Right)
                    || (_speed.x < 0 && _box.DirectionRelativeMain == EDirectionType.Left))
                {
                    return "Push";
                }
                return "Pull";
            }
            if (speed <= 60)
            {
                return "Run";
            }
            return "Run2";
        }

        protected virtual string JumpAnimName(int jumpLevel)
        {
            if (_attackedTimer > 0)
            {
                return "StunStart";
            }
            if (jumpLevel == 0)
            {
                return "Jump";
            }
            if (jumpLevel == 1)
            {
                return "Jump2";
            }
            return "Jump";
        }

        protected virtual string IdleAnimName()
        {
            if (IsHoldingBox())
            {
                return "Prepare";
            }
            return "Idle";
        }

        protected virtual string ClimbAnimName()
        {
            return "Climb";
        }

        protected virtual string FallAnimName()
        {
            if (_attackedTimer > 0)
            {
                return "StunRun";
            }
            return "Fall";
        }

        protected virtual string DeathAnimName()
        {
            return "Death";
        }

        protected virtual string LandAnimName()
        {
            if (_attackedTimer > 0)
            {
                return "StunEnd";
            }
            return "Land";
        }

        protected virtual string VictoryAnimName()
        {
            return "Victory";
        }
 
        #endregion
    }
}
