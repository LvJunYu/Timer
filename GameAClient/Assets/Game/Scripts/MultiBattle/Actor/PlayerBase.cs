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

namespace GameA.Game
{
    public class PlayerBase : ActorBase
    {
        protected Gun _gun;

        protected RoomUser _roomUser;
        protected UnitExtraDynamic _unitExtra;
        protected bool _siTouLe;
        protected float _curRopeProgress;
        protected IntVec2 _ropeOffset;

        [SerializeField] protected IntVec2 _revivePos;
        protected int _reviveScene;

        protected Box _box;
        protected ReviveEffect _reviveEffect = new ReviveEffect();
        protected ReviveEffect _portalEffect = new ReviveEffect();

        protected Table_Equipment[] _tableEquipments = new Table_Equipment[3];
        private int _lastSlot;
        private float _lastPlayTime;

        public RoomUser RoomUser
        {
            get { return _roomUser; }
        }

        public override byte TeamId
        {
            get { return _unitExtra.TeamId; }
        }

        public bool SiTouLe
        {
            get { return _siTouLe; }
        }

        public override SkillCtrl SkillCtrl
        {
            get { return _skillCtrl; }
        }

        public Box Box
        {
            get { return _box; }
        }

        public override bool CanAttack
        {
            get
            {
                return _isAlive && !IsInState(EEnvState.Stun) &&
                       !IsInState(EEnvState.Ice);
            }
        }

        public override bool CanPortal
        {
            get { return true; }
        }

        public override bool CanPassBlackHole
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
            _roomUser = roomUser;
        }

        protected override bool OnInit()
        {
            Messenger.AddListener(EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
            Messenger.AddListener(EMessengerType.OnApplicationQuit, OnApplicationQuit);
            return base.OnInit();
        }

        private void OnChangeToAppMode()
        {
            if (_view != null)
            {
                _view.SetDamageShaderValue("Value", 0);
            }
        }

        private void OnApplicationQuit()
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
            _siTouLe = false;
            _dieTime = 0;
            _box = null;
            ClearView();
//            _maxSpeedX = BattleDefine.MaxSpeedX;
            _lastSlot = -1;
        }

        public override UnitExtraDynamic UpdateExtraData()
        {
            base.UpdateExtraData();
            //玩家的UnitExtra来自复活点的Extra
            if (_unitExtra.MaxHp > 0)
            {
                _maxHp = _unitExtra.MaxHp;
            }
            else
            {
                _maxHp = _tableUnit.Hp;
            }

            _hp = _maxHp;
            if (_unitExtra.MaxSpeedX > 0)
            {
                _maxSpeedX = _unitExtra.MaxSpeedX;
            }
            else if (_unitExtra.MaxSpeedX == ushort.MaxValue)
            {
                _maxSpeedX = 0;
            }
            else
            {
                _maxSpeedX = BattleDefine.MaxSpeedX;
            }

            if (_unitExtra.JumpAbility > 0)
            {
                _jumpAbility = _unitExtra.JumpAbility;
            }
            else
            {
                _jumpAbility = _tableUnit.JumpAbility;
            }

            _injuredReduce = _unitExtra.InjuredReduce;
            _curIncrease = _unitExtra.CureIncrease;
            return _unitExtra;
        }

        public override bool SetWeapon(int weaponId, UnitExtraDynamic unitExtra = null, int slot = -1)
        {
            var tableEquipment = TableManager.Instance.GetEquipment(weaponId);
            if (tableEquipment == null)
            {
                LogHelper.Error("SetWeapon Failed, WeaponId: {0}", weaponId);
                return false;
            }

            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("Player {0} SetWeapon {1}",
                    _roomUser == null ? -1 : _roomUser.Guid, weaponId));
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
            if (slot == -1 && !_skillCtrl.HasEmptySlot(out slot))
            {
                slot = _lastSlot + 1;
                _lastSlot = slot;
                if (_lastSlot == 2)
                {
                    _lastSlot = -1;
                }
            }
            if (!_skillCtrl.SetSkill(tableSkill.Id, (EWeaponInputType) tableEquipment.InputType, slot, unitExtra))
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
            if (IsMain)
            {
                Messenger<Table_Equipment, int>.Broadcast(EMessengerType.OnSkillSlotChanged, tableEquipment, slot);
            }

//            CalculateMaxHp();
//            OnHpChanged(_maxHp);
            ChangeGunView(slot, null);
            return true;
        }

        public override void ChangeGunView(int slot, EShootDirectionType? eShootDir)
        {
            var tableEquip = _tableEquipments[slot];
            if (_gun != null && tableEquip != null)
            {
                _gun.ChangeView(tableEquip.Model);
                _gun.ChangeGun(tableEquip, eShootDir);
            }
        }

        private void CalculateMaxHp()
        {
//            _maxHp = _tableUnit.Hp;
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
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("Player {0} OnPlay",
                    _roomUser == null ? -1 : _roomUser.Guid));
            }

            LogHelper.Debug("{0}, OnPlay", GetType().Name);
            _gun.Play();
            _revivePos = _curPos;
            _reviveScene = Scene2DManager.Instance.CurSceneIndex;
            _siTouLe = false;
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

            var weapons = _unitExtra.InternalUnitExtras.ToList<UnitExtraDynamic>();
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i] != null)
                {
                    SetWeapon(weapons[i].ChildId, weapons[i], i);
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

            if (IsValidBox(_hitUnits[(int) EDirectionType.Right]))
            {
                if (IsMain)
                {
                    var uiControl = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                    if (uiControl != null)
                    {
                        uiControl.SetAssistBtnVisible(true);
                    }

                    //弹出UI给提示
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "按 辅助 键可以推拉木箱");
                }

                _box = _hitUnits[(int) EDirectionType.Right] as Box;
                if (_box != null)
                {
                    _box.DirectionRelativeMain = EDirectionType.Right;
                }
            }
            else if (IsValidBox(_hitUnits[(int) EDirectionType.Left]))
            {
                if (IsMain)
                {
                    var uiControl = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                    if (uiControl != null)
                    {
                        uiControl.SetAssistBtnVisible(true);
                    }

                    //弹出UI给提示
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "按 辅助 键可以推拉木箱");
                }

                _box = _hitUnits[(int) EDirectionType.Left] as Box;
                if (_box != null)
                {
                    _box.DirectionRelativeMain = EDirectionType.Left;
                }
            }
        }

        protected override void CheckAssist()
        {
            if (_box != null && _box.IsHoldingByPlayer && _box.Holder != this)
            {
                return;
            }

            base.CheckAssist();
        }

        public override void OnBoxHoldingChanged()
        {
            if (_box == null)
            {
                return;
            }

            _box.IsHoldingByPlayer = !_box.IsHoldingByPlayer;
            if (_box.IsHoldingByPlayer)
            {
                _box.SetHoder(this);
                SetFacingDir((EMoveDirection) (_box.DirectionRelativeMain + 1));
            }

            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("Player {0} OnBoxHoldingChanged {1}",
                    _roomUser == null ? -1 : _roomUser.Guid, _box.IsHoldingByPlayer));
            }

            LogHelper.Debug("OnBoxHoldingChanged: " + _box.IsHoldingByPlayer);
        }

        private bool IsValidBox(UnitBase unit)
        {
            return unit != null && unit.Id == UnitDefine.BoxId && unit.ColliderGrid.YMin == _colliderGrid.YMin;
        }

        public override bool IsHoldingBox()
        {
            return _box != null && _box.IsHoldingByPlayer;
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
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("Player {0} OnDead",
                    _roomUser == null ? -1 : _roomUser.Guid));
            }
            LogHelper.Debug("{0}, OnDead", GetType().Name);
            if (_gun != null)
            {
                _gun.Stop();
            }
            if (_box != null)
            {
                _box.IsHoldingByPlayer = false;
                _box = null;
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
            if (IsMain)
            {
                Messenger.Broadcast(EMessengerType.OnMainPlayerDead);
            }
        }

        protected void OnRevive()
        {
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("Player {0} OnRevive {1} In Scene {2}",
                    _roomUser == null ? -1 : _roomUser.Guid, _revivePos, _reviveScene));
            }
            LogHelper.Debug("{0}, OnRevive {1}", GetType().Name, _revivePos);
            if (_view != null)
            {
                GameParticleManager.Instance.Emit("M1EffectAirDeath", _trans.position + Vector3.up * 0.5f);
            }
            _eUnitState = EUnitState.Reviving;
            Scene2DManager.Instance.ChangeScene(_reviveScene, EChangeSceneType.ChangeScene);
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
                    _moveDirection = EMoveDirection.Right;
                    _trans.eulerAngles = new Vector3(0, 0, 0);
                    SetPos(_revivePos);
                    if (IsMain)
                    {
                        PlayMode.Instance.UpdateWorldRegion(_curPos);
                    }
                    _animation.Reset();
                    _animation.PlayLoop(IdleAnimName());
                    if (_gun != null)
                    {
                        _gun.Play();
                        _gun.Revive();
                    }
                    if (IsMain)
                    {
                        GM2DGame.Instance.GameMode.RecordAnimation(IdleAnimName(), true);
                        GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Reborn);
                        Messenger.Broadcast(EMessengerType.OnMainPlayerRevive);
                    }
                    if (_statusBar != null)
                    {
                        _statusBar.SetHPActive(true);
                    }
                    if (PlayMode.Instance.SceneState.Statistics.NetBattleReviveInvincibleTime > 0)
                    {
                        AddStates(null, 61);
                    }
                });
        }

        public override void OnPortal(IntVec2 targetPos, IntVec2 speed)
        {
            if (_eUnitState == EUnitState.Portaling)
            {
                return;
            }

            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("Player {0} OnPortal",
                    _roomUser == null ? -1 : _roomUser.Guid));
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
                    _moveDirection = EMoveDirection.Right;
                    _trans.eulerAngles = new Vector3(0, 0, 0);
                    Speed = speed;
                    SetPos(targetPos);
                    if (IsMain)
                    {
                        PlayMode.Instance.UpdateWorldRegion(_curPos);
                        GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Reborn);
                    }

                    _animation.Reset();
                    _animation.PlayLoop(IdleAnimName());
                    if (GM2DGame.Instance.GameMode.SaveShadowData && IsMain)
                    {
                        GM2DGame.Instance.GameMode.ShadowData.RecordOutPortal();
                        GM2DGame.Instance.GameMode.RecordAnimation(IdleAnimName(), true);
                    }

                    if (_statusBar != null)
                    {
                        _statusBar.SetHPActive(true);
                    }
                }));
        }

        public virtual void OnSucceed()
        {
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("Player {0} OnSucceed",
                    _roomUser == null ? -1 : _roomUser.Guid));
            }

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
            _reviveScene = Scene2DManager.Instance.CurSceneIndex;
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

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            _animation.Init("Idle1");
            if (!_animation.AddEventHandle("Yan", OnStep))
            {
                return false;
            }

            _reviveEffect.Set(GameParticleManager.Instance.GetUnityNativeParticleItem(ConstDefineGM2D.M1EffectSoul,
                null, ESortingOrder.LazerEffect));
            _portalEffect.Set(GameParticleManager.Instance.GetUnityNativeParticleItem(ConstDefineGM2D.PortalingEffect,
                null, ESortingOrder.LazerEffect));
            CreateStatusBar();
            if (_statusBar != null)
            {
                _statusBar.SetHPActive(true);
            }
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

        internal override void OnDispose()
        {
            Messenger.RemoveListener(EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
            Messenger.RemoveListener(EMessengerType.OnApplicationQuit, OnApplicationQuit);
            base.OnDispose();
        }

        internal override void OnObjectDestroy()
        {
            _gun.OnObjectDestroy();
            base.OnObjectDestroy();
            _reviveEffect.Free();
            _portalEffect.Free();
        }

        protected override void UpdateDynamicView(float deltaTime)
        {
            if (!PlayMode.Instance.SceneState.GameRunning && PlayMode.Instance.SceneState.Arrived)
            {
                return;
            }

            base.UpdateDynamicView(deltaTime);
            _gun.UpdateView(deltaTime);
            if (!_isAlive)
            {
                _dieTime++;
                if (_life > 0)
                {
                    if (_dieTime % ConstDefineGM2D.FixedFrameCount == 0)
                    {
                        var reviveTime = PlayMode.Instance.SceneState.Statistics.NetBattleReviveTime;
                        int dieSecond = _dieTime / ConstDefineGM2D.FixedFrameCount;
                        if (dieSecond == Mathf.Max(1, reviveTime))
                        {
                            if (IsMain)
                            {
                                CameraManager.Instance.CameraCtrlPlay.ResetCameraPlayer();
                            }
                            OnRevive();
                        }

                        if (IsMain)
                        {
                            Messenger<int>.Broadcast(EMessengerType.OnMainPlayerReviveTime, reviveTime - dieSecond);
                        }
                    }
                }
                else
                {
                    if (_dieTime == 50)
                    {
                        Messenger.Broadcast(EMessengerType.GameFailedDeadMark);
                    }

                    if (_dieTime == 100)
                    {
                        _siTouLe = true;
                        if (GM2DGame.Instance.GameMode.IsMulti)
                        {
                            if (PlayerManager.Instance.CheckAllPlayerSiTouLe())
                            {
                                PlayMode.Instance.SceneState.AllPlayerSiTouLe();
                            }
                        }
                        else
                        {
                            if (IsMain)
                            {
                                PlayMode.Instance.SceneState.MainUnitSiTouLe();
                                Messenger.Broadcast(EMessengerType.GameFinishFailed); // 因生命用完而失败
                            }

                            return;
                        }
                    }
                }

                if (_dieTime > ConstDefineGM2D.FixedFrameCount && IsMain &&
                    UnityEngine.Input.GetKeyDown(KeyCode.Space) && _eUnitState != EUnitState.Reviving)
                {
                    CameraManager.Instance.CameraCtrlPlay.SetNextCameraPlayer();
                }
            }

            CheckBox();
            if (_isAlive)
            {
                if (!_grounded && ClimbState == EClimbState.None)
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

                        PlayClimbEffect();
                        if (_onClay)
                        {
                            speed = 30;
                        }

                        if (IsHoldingBox())
                        {
                            speed = 50;
                        }

                        if (speed == 0 && (_eClimbState == EClimbState.Ladder || _eClimbState == EClimbState.Rope))
                        {
                            _animation.PlayLoop(IdleAnimName());
                            if (IsMain)
                            {
                                GM2DGame.Instance.GameMode.RecordAnimation(IdleAnimName(), true);
                            }
                        }
                        else
                        {
                            if (_eClimbState == EClimbState.Rope)
                            {
                                speed = 50;
                            }

                            _animation.PlayLoop(MoveAnimName(speed), speed * deltaTime);
                            if (IsMain)
                            {
                                GM2DGame.Instance.GameMode.RecordAnimation(MoveAnimName(speed), true,
                                    speed * deltaTime);
                            }
                        }
                    }
                    else
                    {
                        _animation.PlayLoop(IdleAnimName());
                        if (IsMain)
                        {
                            GM2DGame.Instance.GameMode.RecordAnimation(IdleAnimName(), true);
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

        private void PlayClimbEffect()
        {
            if (Time.time - _lastPlayTime < 0.3f) return;
            if (ClimbState == EClimbState.Left && _input.GetKeyApplied(EInputType.Up))
            {
                Vector3 rotate = new Vector3(0, 0, 90);
                GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.ClimbOnClay,
                    _trans.position + Vector3.left * 0.2f + Vector3.up * 0.7f, rotate, Vector3.one);
                _lastPlayTime = Time.time;
            }
            else if (ClimbState == EClimbState.Up)
            {
                if (_input.GetKeyApplied(EInputType.Left))
                {
                    Vector3 rotate = new Vector3(0, 180, 0);
                    GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.ClimbOnClay,
                        _trans.position + Vector3.up * 0.7f, rotate, Vector3.one);
                    _lastPlayTime = Time.time;
                }
                else if (_input.GetKeyApplied(EInputType.Right))
                {
                    Vector3 rotate = Vector3.zero;
                    GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.ClimbOnClay,
                        _trans.position + Vector3.up * 0.7f, rotate, Vector3.one);
                    _lastPlayTime = Time.time;
                }
            }
        }

        private bool IsClimbingSide()
        {
            return (ClimbState == EClimbState.Left || ClimbState == EClimbState.Right ||
                    ClimbState == EClimbState.Ladder || ClimbState == EClimbState.Rope) &&
                   (_input.GetKeyApplied(EInputType.Up) || _input.GetKeyApplied(EInputType.Down));
        }

        private bool IsInputSideValid()
        {
            return (ClimbState == EClimbState.None || ClimbState == EClimbState.Up ||
                    ClimbState == EClimbState.Ladder) &&
                   (_input.GetKeyApplied(EInputType.Left) || _input.GetKeyApplied(EInputType.Right));
        }

        protected override void OnJump()
        {
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("Player {0} OnJump ",
                    _roomUser == null ? -1 : _roomUser.Guid));
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
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("Player {0} OnLand ",
                    _roomUser == null ? -1 : _roomUser.Guid));
            }

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
        protected virtual void OnStep()
        {
            if (_downUnit == null || _view == null)
            {
                return;
            }

            Vector3 rotate = _moveDirection == EMoveDirection.Right ? Vector3.zero : new Vector3(0, 180, 0);
            if (_downUnit.Id == UnitDefine.ClayId || _onClay)
            {
                GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.RunOnClay,
                    _trans.position, rotate, Vector3.one);
            }
        }

        protected virtual string MoveAnimName(float speed)
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

            switch (ClimbState)
            {
                case EClimbState.Left:
                case EClimbState.Right:
                case EClimbState.Ladder:
                case EClimbState.Rope:
                    return "ClimbRunRight";
                case EClimbState.Up:
                    return "ClimbRunUp";
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

            switch (ClimbState)
            {
                case EClimbState.Left:
                case EClimbState.Right:
                case EClimbState.Ladder:
                case EClimbState.Rope:
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

        public void SetUnitExtra(UnitExtraDynamic unitExtra)
        {
            _unitExtra = unitExtra;
            UpdateExtraData();
            //根据队伍刷新血条颜色
            if (_statusBar != null)
            {
                _statusBar.RefreshBar();
            }
        }

        public override UnitExtraDynamic GetUnitExtra()
        {
            if (_unitExtra == null)
            {
                _unitExtra = new UnitExtraDynamic();
            }
            return _unitExtra;
        }

        public override bool CanHarm(UnitBase unit)
        {
            if (unit.IsPlayer)
            {
                return PlayMode.Instance.SceneState.CanHarmType(EHarmType.SelfPlayer) && IsSameTeam(unit.TeamId) ||
                       PlayMode.Instance.SceneState.CanHarmType(EHarmType.EnemyPlayer) && !IsSameTeam(unit.TeamId);
            }

            if (unit.IsMonster)
            {
                return PlayMode.Instance.SceneState.CanHarmType(EHarmType.SelfMonster) && IsSameTeam(unit.TeamId) ||
                       PlayMode.Instance.SceneState.CanHarmType(EHarmType.EnemyMonster) && !IsSameTeam(unit.TeamId);
            }

            return true;
        }

        public void OnIntersectLadder(Ladder ladder, bool value)
        {
            if (value)
            {
                if (!_inLadders.Contains(ladder))
                {
                    _inLadders.Add(ladder);
                }

                _inLadder = true;
            }
            else
            {
                if (_inLadders.Contains(ladder))
                {
                    _inLadders.Remove(ladder);
                }

                if (_inLadders.Count == 0)
                {
                    _inLadder = false;
                }
            }
        }

        protected override void GetCarryUnits()
        {
            if (_eClimbState > EClimbState.None && _curClimbUnit != null)
            {
                _carryUnits.Add(_curClimbUnit);
            }
            else
            {
                base.GetCarryUnits();
            }
        }

        public void CheckRope(RopeJoint ropeJoint)
        {
            if (_eClimbState != EClimbState.Rope && CanClimb && ropeJoint.GetTimer(this) == 0 && Speed != IntVec2.zero)
            {
                SetClimbState(EClimbState.Rope, ropeJoint);
                ropeJoint.JumpOnRope(_moveDirection);
                _curRopeProgress = 0;
                Messenger<int, bool, PlayerBase>.Broadcast(EMessengerType.OnPlayerClimbRope, ropeJoint.RopeIndex, true, this);
            }
        }

        protected override bool CheckRopeVerticalFloor(int deltaPosY = 0)
        {
            if (!CanClimb) return false;
            Grid2D grid = new Grid2D(_colliderGrid.XMin - _ropeOffset.x, _colliderGrid.YMin - _ropeOffset.y,
                _colliderGrid.XMax - _ropeOffset.x, _colliderGrid.YMax - _ropeOffset.y);
            return _curClimbUnit.ColliderGrid.Intersects(grid);
        }

        protected override void OnLeaveRope()
        {
            var ropeJoint = _curClimbUnit as RopeJoint;
            if (ropeJoint != null)
            {
                Messenger<int, bool, PlayerBase>.Broadcast(EMessengerType.OnPlayerClimbRope, ropeJoint.RopeIndex, false, this);
            }
            _ropeOffset = IntVec2.zero;
        }

        public override void UpdateView(float deltaTime)
        {
            if (_eClimbState == EClimbState.Rope)
            {
                RopeJoint joint = _curClimbUnit as RopeJoint;
                if (joint != null)
                {
                    if (_input.GetKeyApplied(EInputType.Up))
                    {
                        _curRopeProgress += 1f;
                    }
                    else if (_input.GetKeyApplied(EInputType.Down))
                    {
                        _curRopeProgress -= 1f;
                        if (joint.NextJoint == null && _curRopeProgress < 0)
                        {
                            _curRopeProgress = 0;
                        }
                    }

                    IntVec2 delta = IntVec2.zero;
                    if (_curRopeProgress >= 1)
                    {
                        if (joint.JointIndex != 0)
                        {
                            _curClimbUnit = joint.PreJoint;
                        }

                        _curRopeProgress = 0;
                    }
                    else if (_curRopeProgress > 0)
                    {
                        delta = joint.GetNeighborRelativePos(true);
                    }
                    else if (_curRopeProgress <= -1)
                    {
                        if (joint.NextJoint != null)
                        {
                            _curClimbUnit = joint.NextJoint;
                        }

                        _curRopeProgress = 0;
                    }
                    else if (_curRopeProgress < 0)
                    {
                        delta = -1 * joint.GetNeighborRelativePos(false);
                    }

                    joint = _curClimbUnit as RopeJoint;
                    if (joint != null)
                    {
                        joint.CarryPlayer();
                    }

                    if (_moveDirection == EMoveDirection.Left)
                    {
                        _ropeOffset = IntVec2.right * 150;
                    }
                    else
                    {
                        _ropeOffset = IntVec2.left * 150;
                    }

                    Speed = _curClimbUnit.CenterPos + _ropeOffset - CenterPos + delta * _curRopeProgress;
                }
            }

            base.UpdateView(deltaTime);
        }

        protected override void ClearRunTime()
        {
            base.ClearRunTime();
            if (_eClimbState > EClimbState.None)
            {
                SetClimbState(EClimbState.None);
            }
        }
    }
}