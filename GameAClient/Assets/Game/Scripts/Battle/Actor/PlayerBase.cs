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
        
        protected Gun _gun;

        protected long _playerId;

        [SerializeField]
        protected IntVec2 _revivePos;

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

        public override IntVec2 FirePos
        {
            get { return CenterPos; }
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
        
        protected override void Clear()
        {
            if (_input != null)
            {
                _input.Clear();
            }
            _gun = _gun ?? new Gun(this);
            SetWeapon(2);
            
            _dieTime = 0;
            _box = null;
            ClearView();

            _maxSpeedX = BattleDefine.MaxSpeedX;
            base.Clear();
        }

        public override bool SetWeapon(int id)
        {
            var tableEquipment = TableManager.Instance.GetEquipment(id);
            if (tableEquipment == null)
            {
                LogHelper.Error("GetEquipment Failed : {0}", id);
                return false;
            }
            _maxHp = tableEquipment.Hp;
            OnHpChanged(_maxHp);
            _gun.ChangeView(tableEquipment.Model);
            var skillIds = new int[3];
            skillIds[0] = 1;
            for (int i = 0; i < tableEquipment.SkillIds.Length; i++)
            {
                skillIds[i + 1] = tableEquipment.SkillIds[i];
            }
            _skillCtrl = _skillCtrl ?? new PlayerSkillCtrl(this);
            _skillCtrl.SetPoint(tableEquipment.Mp, tableEquipment.MpRecover, tableEquipment.Rp, tableEquipment.RpRecover);
            _skillCtrl.SetSkill(skillIds);
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            LogHelper.Debug("{0}, OnPlay", GetType().Name);
            _gun.Play();
            AddStates(61);
            _revivePos = _curPos;
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
                GameParticleManager.Instance.Emit("M1EffectSpawn",new Vector3(_trans.position.x, _trans.position.y - 0.5f, -100));
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
                ChangeLittleSkillState(ELittleSkillState.HoldBox);
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
                ChangeLittleSkillState(ELittleSkillState.HoldBox);
            }
            if (_box == null)
            {
                ChangeLittleSkillState(ELittleSkillState.Quicken);
            }
        }

        public override void OnBoxHoldingChanged()
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

        public override bool IsHoldingBox()
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
            _input.Clear();
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
            LogHelper.Debug("{0}, OnRevive {1}", GetType().Name, _revivePos);
            _eUnitState = EUnitState.Reviving;
            _trans.eulerAngles = new Vector3(90, 0, 0);
            _reviveEffect.Play(_trans.position + Vector3.up * 0.5f,
                                GM2DTools.TileToWorld(_revivePos), 20, () =>
                                {
                                    _eUnitState = EUnitState.Normal;
                                    _input.Clear();
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
            _input.Clear();
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
            _revivePos = pos;
        }

        public void Step(int stepY = 0)
        {
            _stepY = stepY;
            OnLand();
        }

        #endregion

        internal void OnStun(ActorBase actor)
        {
            _stunTimer = TableConvert.GetTime(BattleDefine.StunTime);
            Speed = IntVec2.zero;
            ExtraSpeed.y = 120;
            ExtraSpeed.x = actor.CenterDownPos.x > CenterDownPos.x ? -100 : 100;
            _input.ClearInput();
        }

        internal void OnKnockBack(ActorBase actor)
        {
            _stunTimer = TableConvert.GetTime(BattleDefine.StunTime);
            Speed = IntVec2.zero;
            ExtraSpeed.y = 280;
            ExtraSpeed.x = actor.CenterDownPos.x > CenterDownPos.x ? -80 : 80;
            _input.ClearInput();
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
            SetWeapon(2);
            _view.StatusBar.ShowHP();
            _view.StatusBar.ShowMP();
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
            _gun.Stop();
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            _reviveEffect.Free();
            _portalEffect.Free();
            _gun.OnObjectDestroy();
        }

        protected override void UpdateDynamicView(float deltaTime)
        {
            if (!PlayMode.Instance.SceneState.GameRunning && PlayMode.Instance.SceneState.Arrived)
            {
                return;
            }
            base.UpdateDynamicView(deltaTime);
            _gun.UpdateView();
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
            CheckBox();
            if (_isAlive)
            {
                if (!_grounded)
                {
                    if (_eClimbState > 0)
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
                        if (_climbJump)
                        {
                            Vector3 effectPos = _trans.position;
                            effectPos += _curMoveDirection == EMoveDirection.Left ? Vector3.right * 0.25f + Vector3.forward * 0.6f : Vector3.left * 0.25f + Vector3.forward * 0.6f;
                            GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.WallJump, effectPos);
                        }
                        if (_jumpState == EJumpState.Jump1 || _jumpState == EJumpState.Jump2)
                        {
                            Messenger.Broadcast(EMessengerType.OnPlayerJump);
                            _animation.PlayOnce(JumpAnimName(_jumpLevel));
                            PlayMode.Instance.CurrentShadow.RecordAnimation(JumpAnimName(_jumpLevel), false);
                        }
                        else if (_jumpState == EJumpState.Fall)
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
                    if (!IsStunning && (_input.GetKeyApplied(EInputType.Left) || _input.GetKeyApplied(EInputType.Right)))
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
        }

        protected override void OnJump()
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

//        protected override void OnLand()
//        {
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
//        }

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
                    if (!_input.GetKeyApplied(EInputType.Right) && !_input.GetKeyApplied(EInputType.Left))
                    {
                        return "Prepare";
                    }
                    if (_input.GetKeyApplied(EInputType.Right) && _box.DirectionRelativeMain == EDirectionType.Right
                        || (_input.GetKeyApplied(EInputType.Left) && _box.DirectionRelativeMain == EDirectionType.Left))
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
            if (IsStunning)
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
            if (IsStunning)
            {
                return "StunEnd";
            }
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
            if (IsStunning)
            {
                return "StunRun";
            }
            if (_jumpLevel == 2)
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
            if (IsStunning)
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
