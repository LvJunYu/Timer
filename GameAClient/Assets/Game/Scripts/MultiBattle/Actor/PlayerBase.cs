/********************************************************************
** Filename : PlayerBase
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:34:10
** Summary : PlayerBase
***********************************************************************/

using System;
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

        [SerializeField] protected IntVec2 _revivePos;

        protected Box _box;

        protected ReviveEffect _reviveEffect = new ReviveEffect();
        protected ReviveEffect _portalEffect = new ReviveEffect();

        protected Table_Equipment[] _tableEquipments = new Table_Equipment[3];
        private int _lastSlot;

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
            get { return HasStateType(EStateType.Invincible); }
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

        protected override bool OnInit()
        {
            Messenger.AddListener(EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
            return base.OnInit();
        }
        
        private void OnChangeToAppMode()
        {
            if (_view != null)
            {
                _view.SetDamageShaderValue("Value", 0);
            }
        }

        protected override void Clear()
        {
            base.Clear();
            if (_input != null)
            {
                _input.Clear();
            }
            for (int i = 0; i < _tableEquipments.Length; i++)
            {
                _tableEquipments[i] = null;
            }
            _gun = _gun ?? new Gun(this);
            _dieTime = 0;
            _box = null;
            ClearView();
            _maxSpeedX = BattleDefine.MaxSpeedX;
            _lastSlot = -1;
        }

        public override bool SetWeapon(int weaponId)
        {
            var tableEquipment = TableManager.Instance.GetEquipment(weaponId);
            if (tableEquipment == null)
            {
                LogHelper.Error("SetWeapon Failed, WeaponId: {0}", weaponId);
                return false;
            }
            int skillId = tableEquipment.SkillId;
            var tableSkill = TableManager.Instance.GetSkill(skillId);
            if (tableSkill == null)
            {
                LogHelper.Error("SetWeapon Failed, SkillId : {0}", skillId);
                return false;
            }
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this, 3);
            _skillCtrl.RemoveSkill(skillId);
            int slot;
            if (!_skillCtrl.HasEmptySlot(out slot))
            {
                slot = _lastSlot + 1;
                _lastSlot = slot;
                if (_lastSlot == 2)
                {
                    _lastSlot = -1;
                }
            }
            if (!_skillCtrl.SetSkill(tableSkill.Id, (EWeaponInputType) tableEquipment.InputType, slot))
            {
                return false;
            }
            if (Application.isMobilePlatform)
            {
                var inputControl = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                if (inputControl != null)
                {
                    inputControl.SetSkillBtnVisible(slot, true);
                }
            }
            _tableEquipments[slot] = tableEquipment;
            //发送事件
            Messenger<Table_Equipment, int>.Broadcast(EMessengerType.OnSkillSlotChanged, tableEquipment, slot);
            CalculateMaxHp();
            OnHpChanged(_maxHp);
            ChangeGunView(slot);
            return true;
        }

        public override void ChangeGunView(int slot)
        {
            var tableEquip = _tableEquipments[slot];
            if (_gun != null && tableEquip != null)
            {
                _gun.ChangeView(tableEquip.Model);
            }
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
                GameParticleManager.Instance.Emit("M1EffectSpawn",
                    new Vector3(_trans.position.x, _trans.position.y - 0.5f, -100));
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
            if (IsValidBox(_hitUnits[(int) EDirectionType.Right]))
            {
                var uiControl = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                if (uiControl != null)
                {
                    uiControl.SetAssistBtnVisible(true);
                }
                //弹出UI给提示
                Messenger<string>.Broadcast(EMessengerType.GameLog, "按 辅助 键可以推拉木箱");
                _box = _hitUnits[(int) EDirectionType.Right] as Box;
                if (_box != null)
                {
                    _box.DirectionRelativeMain = EDirectionType.Right;
                }
            }
            else if (IsValidBox(_hitUnits[(int) EDirectionType.Left]))
            {
                var uiControl = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                if (uiControl != null)
                {
                    uiControl.SetAssistBtnVisible(true);
                }
                //弹出UI给提示
                Messenger<string>.Broadcast(EMessengerType.GameLog, "按 辅助 键可以推拉木箱");
                _box = _hitUnits[(int) EDirectionType.Left] as Box;
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
                SetFacingDir((EMoveDirection) (_box.DirectionRelativeMain + 1));
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
            }
            if (GM2DGame.Instance.GameMode.SaveShadowData && IsMain)
            {
                GM2DGame.Instance.GameMode.ShadowData.RecordDeath();
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
                GM2DTools.TileToWorld(_revivePos), 8, () =>
                {
                    _eDieType = EDieType.None;
                    _eUnitState = EUnitState.Normal;
                    _input.Clear();
                    ClearRunTime();
                    _isAlive = true;
                    if (GM2DGame.Instance.GameMode.SaveShadowData && IsMain)
                    {
                        GM2DGame.Instance.GameMode.ShadowData.RecordRevive();
                    }
                    OnHpChanged(_maxHp);
                    _dieTime = 0;
                    if (_box != null)
                    {
                        _box.IsHoldingByMain = false;
                        _box = null;
                    }
                    _trans.eulerAngles = new Vector3(0, 0, 0);
                    SetPos(_revivePos);
                    PlayMode.Instance.UpdateWorldRegion(_curPos);
                    if (_gun != null)
                    {
                        _gun.Play();
                    }
                    _animation.Reset();
                    _animation.PlayLoop(IdleAnimName());
                    GM2DGame.Instance.GameMode.RecordAnimation(IdleAnimName(), true);
                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Reborn);
                    Messenger.Broadcast(EMessengerType.OnMainPlayerRevive);
                    if (_statusBar != null)
                    {
                        _statusBar.SetHPActive(true);
                    }
                });
        }

        public override void OnPortal(IntVec2 targetPos, IntVec2 speed)
        {
            if (_eUnitState == EUnitState.Portaling)
            {
                return;
            }
            if (_statusBar != null)
            {
                _statusBar.SetHPActive(false);
            }
            _eUnitState = EUnitState.Portaling;
            PlayMode.Instance.Freeze(this);
            _input.Clear();
            ClearRunTime();
            if (GM2DGame.Instance.GameMode.SaveShadowData && IsMain)
            {
                GM2DGame.Instance.GameMode.ShadowData.RecordEnterPortal();
            }
            _trans.eulerAngles = new Vector3(90, 0, 0);
            _portalEffect.Play(_trans.position + Vector3.up * 0.5f,
                GM2DTools.TileToWorld(targetPos), 8, () => PlayMode.Instance.RunNextLogic(() =>
                {
                    _eUnitState = EUnitState.Normal;
                    PlayMode.Instance.UnFreeze(this);
                    _trans.eulerAngles = new Vector3(0, 0, 0);
                    Speed = speed;
                    SetPos(targetPos);
                    PlayMode.Instance.UpdateWorldRegion(_curPos);
                    _animation.Reset();
                    _animation.PlayLoop(IdleAnimName());
                    if (GM2DGame.Instance.GameMode.SaveShadowData && IsMain)
                    {
                        GM2DGame.Instance.GameMode.ShadowData.RecordOutPortal();
                        GM2DGame.Instance.GameMode.RecordAnimation(IdleAnimName(), true);
                    }
                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Reborn);
                    if (_statusBar != null)
                    {
                        _statusBar.SetHPActive(true);
                    }
                }));
        }

        public virtual void OnSucceed()
        {
            _animation.ClearTrack(0);
            _animation.ClearTrack(1);
            if (GM2DGame.Instance.GameMode.SaveShadowData && IsMain)
            {
                GM2DGame.Instance.GameMode.ShadowData.RecordClearAnimTrack(0);
                GM2DGame.Instance.GameMode.ShadowData.RecordClearAnimTrack(1);
            }
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
            _animation.PlayLoop(VictoryAnimName(), 1, 1);
            GM2DGame.Instance.GameMode.RecordAnimation(VictoryAnimName(), true, 1, 1);
        }

        public override void OnRevivePos(IntVec2 pos)
        {
            _revivePos = pos;
        }

        public void Step(int stepY = 0)
        {
            _stepY = stepY;
            _grounded = true;
//            _jumpLevel = 0;
//            _jumpTimer = 10;
//            _jumpState = EJumpState.Jump1;
            _jumpLevel = -1;
            _jumpTimer = 0;
            _jumpState = EJumpState.Land;
        }

        #endregion

        #endregion

        #region View

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init("Idle1");
            if (!_animation.AddEventHandle("Step", OnStep))
            {
                return false;
            }
            _reviveEffect.Set(GameParticleManager.Instance.GetUnityNativeParticleItem(ConstDefineGM2D.M1EffectSoul,
                null, ESortingOrder.LazerEffect));
            _portalEffect.Set(GameParticleManager.Instance.GetUnityNativeParticleItem(ConstDefineGM2D.PortalingEffect,
                null, ESortingOrder.LazerEffect));
            CreateStatusBar();
            _statusBar.SetHPActive(true);
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
            Messenger.RemoveListener(EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
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
                    else
                    {
                        Messenger.Broadcast(EMessengerType.GameFailedDeadMark);
                    }
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
                    if (_climbJump)
                    {
                        Vector3 effectPos = _trans.position;
                        effectPos += _moveDirection == EMoveDirection.Left
                            ? Vector3.right * 0.25f + Vector3.forward * 0.6f
                            : Vector3.left * 0.25f + Vector3.forward * 0.6f;
                        GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.WallJump, effectPos);
                    }
                    if (_jumpState == EJumpState.Jump1 || _jumpState == EJumpState.Jump2)
                    {
                        Messenger.Broadcast(EMessengerType.OnPlayerJump);
                        _animation.PlayOnce(JumpAnimName(_jumpLevel));
                        GM2DGame.Instance.GameMode.RecordAnimation(JumpAnimName(_jumpLevel), false);
                    }
                    else if (_jumpState == EJumpState.Fall &&
                             (!_animation.IsPlaying("JumpStart") && !_animation.IsPlaying("Jump2") &&
                              !_animation.IsPlaying("Fly") && !_animation.IsPlaying("StunStart")))
                    {
                        _animation.PlayLoop(FallAnimName());
                        GM2DGame.Instance.GameMode.RecordAnimation(FallAnimName(), true);
                    }
                }
                else
                {
                    if (CanMove && (IsInputSideValid() || IsClimbingSide()))
                    {
                        var speed = Math.Abs(_curMaxSpeedX);
                        if (IsClimbingSide())
                        {
                            speed = Math.Abs(SpeedY);
                        }
                        if (_onClay)
                        {
                            speed = 30;
                        }
                        if (IsHoldingBox())
                        {
                            speed = 50;
                        }
                        _animation.PlayLoop(RunAnimName(speed), speed * deltaTime);
                        GM2DGame.Instance.GameMode.RecordAnimation(RunAnimName(speed), true, speed * deltaTime);
                    }
                    else
                    {
                        _animation.PlayLoop(IdleAnimName());
                        GM2DGame.Instance.GameMode.RecordAnimation(IdleAnimName(), true);
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

        private bool IsClimbingSide()
        {
            return (_eClimbState == EClimbState.Left || _eClimbState == EClimbState.Right) &&
                   (_input.GetKeyApplied(EInputType.Up) || _input.GetKeyApplied(EInputType.Down));
        }

        private bool IsInputSideValid()
        {
            return (_eClimbState == EClimbState.None || _eClimbState == EClimbState.Up) &&
                   (_input.GetKeyApplied(EInputType.Left) || _input.GetKeyApplied(EInputType.Right));
        }

        protected override void OnJump()
        {
            if (!GameAudioManager.Instance.IsPlaying(AudioNameConstDefineGM2D.Sping))
            {
                GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Jump);
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

        protected override void OnLand()
        {
            base.OnLand();
            if (_downUnit == null || _view == null)
            {
                return;
            }
            if (_downUnit.Id == UnitDefine.ClayId)
            {
                GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Land, _trans.position);
            }
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
            Vector3 scale = _moveDirection == EMoveDirection.Right ? Vector3.one : new Vector3(-1, 1, 1);
            if (_downUnit.Id == UnitDefine.ClayId)
            {
                GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.RunOnMud,
                    _trans.position + Vector3.up * 0.2f, scale);
            }
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
            switch (_eClimbState)
            {
                case EClimbState.Left:
                case EClimbState.Right:
                    return "ClimbRunRight";
                case EClimbState.Up:
                    return "ClimbRunUp";
            }
            if (speed <= 60)
            {
                return Run;
            }
            return Run;
        }

        protected virtual string JumpAnimName(int jumpLevel)
        {
            if (IsInState(EEnvState.Stun))
            {
                return "StunStart";
            }
            if (jumpLevel == 0)
            {
                return "JumpStart";
            }
            if (jumpLevel == 1)
            {
                return "Jump2";
            }
            if (jumpLevel == 2)
            {
                return "Fly";
            }
            return "JumpStart";
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
            switch (_eClimbState)
            {
                case EClimbState.Left:
                case EClimbState.Right:
                    return "ClimbIdleRight";
                case EClimbState.Up:
                    return "ClimbIdleUp";
            }
            return "Idle1";
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
            if (SpeedY > 0)
            {
                return "Jump";
            }
            return "Fall";
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