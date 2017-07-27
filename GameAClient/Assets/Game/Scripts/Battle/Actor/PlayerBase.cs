/********************************************************************
** Filename : PlayerBase
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:34:10
** Summary : PlayerBase
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameA.Game
{
    public class PlayerBase : ActorBase
    {
        #region Data

        protected long _playerId;

        [SerializeField]
        protected PlayerInputBase _playerInput;

        protected Gun _gun;

        [SerializeField]
        protected IntVec2 _revivePos;
        /// <summary>
        /// 复活点被吃了
        /// </summary>
        protected Stack<IntVec2> _revivePosStack = new Stack<IntVec2>();

        protected Box _box;

        protected ReviveEffect _reviveEffect = new ReviveEffect();
        protected ReviveEffect _portalEffect = new ReviveEffect();

        public long PlayerId
        {
            get { return _playerId; }
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
            get
            {
                return HasStateType(EStateType.Invincible);
            }
        }

        public PlayerInputBase PlayerInput
        {
            get { return _playerInput; }
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

        public void Set(RoomUser roomUser)
        {
            _playerId = roomUser.Guid;
        }

        public void Setup(PlayerInputBase inputBase)
        {
            _playerInput = inputBase;
        }
        
        
        protected override void Clear()
        {
            if (_playerInput != null)
            {
                _playerInput.Reset();
            }

            _skillCtrl = _skillCtrl ?? new SkillCtrl(this, 3);
            _skillCtrl.Clear();
            ChangeWeapon(1);
            _dieTime = 0;
            _box = null;
            ClearView();
            base.Clear();
        }

        public override bool ChangeWeapon(int id)
        {
            var tableEquipment = TableManager.Instance.GetEquipment(id);
            if (tableEquipment == null)
            {
                LogHelper.Error("GetEquipment Failed : {0}", id);
                return false;
            }
            _skillCtrl.SetPoint(tableEquipment.Mp,tableEquipment.MpRecover,tableEquipment.Rp,tableEquipment.RpRecover);
            int[] skillIds = new int[3];
            skillIds[0] = 1;
            for (int i = 0; i < tableEquipment.SkillIds.Length; i++)
            {
                skillIds[i + 1] = tableEquipment.SkillIds[i];
            }
            return _skillCtrl.ChangeSkill(skillIds);
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            LogHelper.Debug("{0}, OnPlay", GetType().Name);
            if (_gun == null)
            {
                _gun = PlayMode.Instance.CreateRuntimeUnit(10000, _curPos) as Gun;
            }
            AddStates(61);
            _revivePos = _curPos;
            _revivePosStack.Clear();
            if (PlayMode.Instance.IsUsingBoostItem(EBoostItemType.BIT_AddLifeCount1))
            {
                Life = PlayMode.Instance.SceneState.Life + 1;
            }
            else
            {
                Life = PlayMode.Instance.SceneState.Life;
            }
            if (_trans != null)
            {
                GameParticleManager.Instance.Emit("M1EffectSpawn", _trans.position);
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
//            if (_playerInput._jumpState == EJumpState.Fall)
//            {
//                air = true;
//            }
            if (SpeedY != 0)
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
                        var delta = Mathf.Abs(CenterDownPos.x - unit.CenterDownPos.x);
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
            if (!_lastGrounded && _grounded)
            {
                OnLand();
            }
            _curMaxSpeedX = BattleDefine.MaxSpeedX;
            float ratio = 1;
            if (IsHoldingBox())
            {
                ratio *= SpeedHoldingBoxRatio;
            }
            if (_onClay)
            {
                ratio *= SpeedClayRatio;
            }
            _curMaxSpeedX = (int)(_curMaxSpeedX * ratio);
            if (air || _grounded)
            {
                if (_curBanInputTime <= 0)
                {
                    int motorAcc = 0;
                    int speedAcc;
                    if (_playerInput.RightInput)
                    {
                        motorAcc = _onIce ? 1 : 10;
                    }
                    if (_playerInput.LeftInput)
                    {
                        motorAcc = _onIce ? -1 : -10;
                    }
                    speedAcc = motorAcc + _fanForce.x;
                    if (speedAcc != 0)
                    {
                        //在空中和冰上 同方向的时候
                        if (_onIce || air)
                        {
                            if ((speedAcc > 0 && _fanForce.x > 0) || (speedAcc < 0 && _fanForce.x < 0))
                            {
                                speedAcc = 1;
                            }
                        }
                        else
                        {
                            SpeedX -= _fanForce.x;
                        }
                        SpeedX = Util.ConstantLerp(SpeedX, speedAcc > 0 ? _curMaxSpeedX : -_curMaxSpeedX, Mathf.Abs(speedAcc));
                    }
                    else if (_grounded || _fanForce.y != 0)
                    {
                        if (_fanForce.x == 0)
                        {
                            if (_onIce)
                            {
                                friction = 1;
                            }
                        }
                        SpeedX = Util.ConstantLerp(SpeedX, 0, friction);
                    }
                }
            }
        }

        protected virtual void CheckClimb()
        {
            _playerInput.EClimbState = EClimbState.None;
            if (!_grounded && SpeedY < 0)
            {
                if (_playerInput.LeftInput && CheckLeftFloor())
                {
                    _playerInput.EClimbState = EClimbState.Left;
                }
                else if (_playerInput.RightInput && CheckRightFloor())
                {
                    _playerInput.EClimbState = EClimbState.Right;
                }
            }
        }

        protected virtual void UpdateSpeedY()
        {
            SpeedY += _fanForce.y;
            _fanForce.y = 0;
            if (!_grounded)
            {
                if (_playerInput.JumpLevel == 2)
                {
                    SpeedY = Util.ConstantLerp(SpeedY, -60, 6);
                }
                else if (_playerInput.EClimbState > EClimbState.None)
                {
                    SpeedY = Util.ConstantLerp(SpeedY, -50, 6);
                }
                else
                {
                    if (SpeedY > 0)
                    {
                        SpeedY = Util.ConstantLerp(SpeedY, 0, 12);
                    }
                    else
                    {
                        SpeedY = Util.ConstantLerp(SpeedY, -120, 8);
                    }
                }
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
                SetFacingDir((EMoveDirection)(_box.DirectionRelativeMain + 1));
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

        public override void OnCrushHit(UnitBase other)
        {
            if (_grounded)
            {
                OnDead();
            }
        }

        protected override void OnDead()
        {
            if (!_isAlive)
            {
                return;
            }
            LogHelper.Debug("{0}, OnDead", GetType().Name);
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
            _trans.eulerAngles = new Vector3(90, 0, 0);
            _reviveEffect.Play(_trans.position + Vector3.up * 0.5f,
                                GM2DTools.TileToWorld(_revivePos), 20, () =>
                                {
                                    _eUnitState = EUnitState.Normal;
                                    _playerInput.Clear();
                                    ClearRunTime();
                                    _isAlive = true;
                                    OnHpChanged(_maxHp);
                                    AddStates(61);
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
            _playerInput.StepY = stepY;
            OnLand();
        }

        #endregion

        internal void OnStun(ActorBase actor)
        {
            //晕2秒
            _attackedTimer = TableConvert.GetTime(BattleDefine.StunTime);
            Speed = IntVec2.zero;
            ExtraSpeed.y = 120;
            ExtraSpeed.x = actor.CenterDownPos.x > CenterDownPos.x ? -100 : 100;
            _playerInput.ClearInput();
        }

        internal void OnKnockBack(ActorBase actor)
        {
            _attackedTimer = TableConvert.GetTime(BattleDefine.StunTime);
            Speed = IntVec2.zero;
            ExtraSpeed.y = 280;
            ExtraSpeed.x = actor.CenterDownPos.x > CenterDownPos.x ? -80 : 80;
            _playerInput.ClearInput();
        }

        #endregion

        #region View

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
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            _reviveEffect.Free();
            _portalEffect.Free();
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
            if (_isAlive)
            {
                if (!_grounded)
                {
                    if (_playerInput.EClimbState > 0)
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
                    else
                    {
                        if (_playerInput.ClimbJump)
                        {
                            Vector3 effectPos = _trans.position;
                            effectPos += _curMoveDirection == EMoveDirection.Left ? Vector3.right * 0.25f + Vector3.forward * 0.6f : Vector3.left * 0.25f + Vector3.forward * 0.6f;
                            GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.WallJump, effectPos);
                        }
                        if (_playerInput.JumpState == EJumpState.Jump1 || _playerInput.JumpState == EJumpState.Jump2)
                        {
                            Messenger.Broadcast(EMessengerType.OnPlayerJump);
                            _animation.PlayOnce(JumpAnimName(_playerInput.JumpLevel));
                            PlayMode.Instance.CurrentShadow.RecordAnimation(JumpAnimName(_playerInput.JumpLevel), false);
                        }
                        else if (_playerInput.JumpState == EJumpState.Fall)
                        {
                            if (_animation.PlayLoop(FallAnimName()))
                            {
                                PlayMode.Instance.CurrentShadow.RecordAnimation(FallAnimName(), true);
                            }
                        }
                    }
                }
                else
                {
                    if (_playerInput.LeftInput || _playerInput.RightInput)
                    {
                        var speed = Math.Abs(SpeedX);
                        speed = Mathf.Clamp(speed, 20, 100);
                        if (IsHoldingBox())
                        {
                            speed = 50;
                        }
                        if (_animation.PlayLoop(RunAnimName(speed), speed * deltaTime))
                        {
                            PlayMode.Instance.CurrentShadow.RecordAnimation(RunAnimName(speed), true, speed * deltaTime);
                        }
                        if (speed <= BattleDefine.MaxSpeedX)
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
                            int randomValue = Random.Range(0, 3);
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
            _grounded = true;
            _playerInput.OnLand();
//            // 新手引导需要知道主角落地了
//            GuideManager.Instance.OnSpecialOperate(2);
//            if (_downUnit == null || _view == null)
//            {
//                return;
//            }
//            if (_downUnit.Id == UnitDefine.ClayId)
//            {
//                GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Land, _trans.position);
//            }
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

        protected virtual string RunAnimName(float speed)
        {
            if (IsHoldingBox())
            {
                if (_speed.x == 0)
                {
                    if (!_playerInput.RightInput && !_playerInput.LeftInput)
                    {
                        return "Prepare";
                    }
                    if (_playerInput.RightInput && _box.DirectionRelativeMain == EDirectionType.Right
                        || (_playerInput.LeftInput && _box.DirectionRelativeMain == EDirectionType.Left))
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
            if (_playerInput.JumpLevel == 2)
            {
                return "Fly";
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
