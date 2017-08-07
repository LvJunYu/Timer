﻿/********************************************************************
** Filename : PlayerBase
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:34:10
** Summary : PlayerBase
***********************************************************************/

using System;
using System.Collections;
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

        protected Table_Equipment[] _tableEquipments = new Table_Equipment[3];

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

        public override bool IsPlayer
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
            for (int i = 0; i < _tableEquipments.Length; i++)
            {
                _tableEquipments[i] = null;
            }
            _gun = _gun ?? new Gun(this);
            SetWeapon(101,0);
            SetWeapon(102,1);
            SetWeapon(103,2);
            SetWeapon(201,2);
            
            _dieTime = 0;
            _box = null;
            ClearView();

            _maxSpeedX = BattleDefine.MaxSpeedX;
            base.Clear();
        }
        
        public override bool SetWeapon(int id, int slot = 0)
        {
            var tableEquipment = TableManager.Instance.GetEquipment(id);
            if (tableEquipment == null)
            {
                LogHelper.Error("GetEquipment Failed : {0}", id);
                return false;
            }
            _tableEquipments[slot] = tableEquipment;
            CalculateMaxHp();
            OnHpChanged(_maxHp);
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this, 3);
            if (_skillCtrl.SetSkill(tableEquipment.SkillId, slot))
            {
                if (_skillCtrl.CurrentSkills[slot].TableSkill.CostType == (int)ECostType.Magic)
                {
                    _gun.ChangeView(tableEquipment.Model);
                }
            }
            return true;
        }

        private void CalculateMaxHp()
        {
            _maxHp = _tableUnit.Hp;
            for (int i = 0; i < _tableEquipments.Length; i++)
            {
                var tableEquip = _tableEquipments[i];
                if (tableEquip != null)
                {
                    _maxHp += tableEquip.HpAdd;
                }
            }
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
                Messenger<string>.Broadcast(EMessengerType.GameLog, "按 E 键可以推拉木箱");
                _box = _hitUnits[(int)EDirectionType.Right] as Box;
                if (_box != null)
                {
                    _box.DirectionRelativeMain = EDirectionType.Right;
                }
            }
            else if (IsValidBox(_hitUnits[(int)EDirectionType.Left]))
            {
                //弹出UI给提示
                Messenger<string>.Broadcast(EMessengerType.GameLog, "按 E 键可以推拉木箱");
                _box = _hitUnits[(int)EDirectionType.Left] as Box;
                if (_box != null)
                {
                    _box.DirectionRelativeMain = EDirectionType.Left;
                }
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
            base.OnDead();
            if (_life <= 0)
            {
                LogHelper.Debug("GameOver!");
                GameRun.Instance.Pause();
                OnDeadAll();
            }
            Messenger.Broadcast(EMessengerType.OnMainPlayerDead);
        }

        protected void OnRevive()
        {
            LogHelper.Debug("{0}, OnRevive {1}", GetType().Name, _revivePos);
            if (_view != null)
            {
                GameParticleManager.Instance.Emit("M1EffectAirDeath", _trans.position + Vector3.up * 0.5f);
            }
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
                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.M2Reborn);
                }));
        }

        public virtual void OnSucceed()
        {
            _animation.ClearTrack(0);
            _animation.ClearTrack(1);
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
            _animation.PlayLoop(VictoryAnimName(), 1, 1);
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
            SetWeapon(101,0);
            SetWeapon(102,1);
            SetWeapon(103,2);
            SetWeapon(201,2);
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
                if (_dieTime == 50)
                {
                    if (_life > 0)
                    {
                        OnRevive();
                    }
                    Messenger.Broadcast(EMessengerType.GameFailedDeadMark);
                }
                if (_life <= 0 && _dieTime == 100)
                {
                    PlayMode.Instance.SceneState.MainUnitSiTouLe();
                    // 因生命用完而失败
                    Messenger.Broadcast(EMessengerType.GameFinishFailed);
                }
            }
            CheckBox();
            if (_isAlive)
            {
                if (!_grounded && _eClimbState == EClimbState.None)
                {
//                    if (_eClimbState > 0)
//                    {
//                        _animation.PlayLoop(ClimbAnimName());
//                        if (GameRun.Instance.LogicFrameCnt % 5 == 0)
//                        {
//                            Vector3 effectPos = _trans.position;
//                            if (_curMoveDirection == EMoveDirection.Left)
//                            {
//                                effectPos += Vector3.left * 0.25f + Vector3.forward * 0.6f;
//                            }
//                            else
//                            {
//                                effectPos += Vector3.right * 0.25f + Vector3.forward * 0.6f;
//                            }
//                            GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.WallClimb, effectPos);
//                        }
//                    }
//                    else
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
                        }
                        else if (_jumpState == EJumpState.Fall)
                        {
                            _animation.PlayLoop(FallAnimName());
                        }
                    }
                }
                else
                {
                    if (CanMove && (_input.GetKeyApplied(EInputType.Left) || _input.GetKeyApplied(EInputType.Right)))
                    {
                        var speed = Math.Abs(SpeedX);
                        speed = Mathf.Clamp(speed, 20, 100);
                        if (IsHoldingBox())
                        {
                            speed = 50;
                        }
                        _animation.PlayLoop(RunAnimName(speed), speed * deltaTime);
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
                        _animation.PlayLoop(IdleAnimName());
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
            if(_eClimbState != EClimbState.None)
            {
                return "ClimbRun";
            }
            if (speed <= 60)
            {
                return "Run";
            }
            return "Run";
        }

        protected virtual string JumpAnimName(int jumpLevel)
        {
            if (IsInState(EEnvState.Stun))
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
            if (IsInState(EEnvState.Stun))
            {
                return "StunLand";
            }
            if (IsHoldingBox())
            {
                return "Prepare";
            }
            if(_eClimbState != EClimbState.None)
            {
                return "ClimbIdle";
            }
            return "Idle";
        }

        protected virtual string ClimbAnimName()
        {
            return "Climb";
        }

        protected virtual string FallAnimName()
        {
            if (IsInState(EEnvState.Stun))
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
            if (IsInState(EEnvState.Stun))
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
