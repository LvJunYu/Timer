/********************************************************************
** Filename : MainUnit
** Author : Dong
** Date : 2017/3/3 星期五 下午 8:28:03
** Summary : MainUnit
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 1001, Type = typeof(MainUnit))]
    public class MainUnit : ActorBase
    {
        protected const int FlashTime = 100;

        [SerializeField]
        protected MainInput _mainInput;

        protected int _currentMp;
        protected int _mpSpeed = 1;
        protected int _totalMp = 2500;
        protected SkillManager _skillMgr1;
        protected SkillManager _skillMgr2;

        [SerializeField] protected int _big;
        [SerializeField] protected int _flashTime;
        [SerializeField] protected int _invincibleTime;
        [SerializeField] protected IntVec2 _revivePos;
        protected Stack<IntVec2> _revivePosStack = new Stack<IntVec2>();
        protected Box _box;
        protected EBoxOperateType _eBoxOperateType;

        protected int _curMaxSpeedX;
        protected int _curMaxQuickenSpeedX;

        protected const int MaxSpeedX = 80;
        protected const int MaxQuickenSpeedX = 160;

        #region view

        protected ReviveEffect _reviveEffect;
        protected ReviveEffect _portalEffect;

        [SerializeField]
        protected UnityNativeParticleItem _brakeEfffect;
        [SerializeField]
        protected UnityNativeParticleItem _invincibleEfffect;
        protected UnityNativeParticleItem _inFireEfffect;

        /// <summary>
        /// 跑步声音间隔
        /// </summary>
        protected int _walkAudioInternal = 12;

        #endregion

        public override SkillManager SkillMgr1
        {
            get { return _skillMgr1; }
        }

        public override SkillManager SkillMgr2
        {
            get { return _skillMgr2; }
        }

        public bool OnClay
        {
            get { return _onClay; }
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

        public override bool IsMain
        {
            get { return true; }
        }

        public MainInput MainInput
        {
            get { return _mainInput; }
        }

        public int InvincibleTime
        {
            get { return _invincibleTime; }
            set
            {
                if (value > 0 && value > _invincibleTime)
                {
                    if (_invincibleEfffect == null)
                    {
                        _invincibleEfffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectOrbitBuff", _trans);
                        _invincibleEfffect.SetSortingOrder(ESortingOrder.EffectItem);
                    }
                    if (_invincibleEfffect != null)
                    {
                        _invincibleEfffect.Play();
                    }
                    _invincibleTime = value;
                    _flashTime = 1;
                    OutFire();
                }
            }
        }

        public IntVec2 RevivePos
        {
            get { return _revivePos; }
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
                        if (_portalEffect != null)
                        {
                            return GM2DTools.WorldToTile(_portalEffect.Position);
                        }
                        break;
                    case EUnitState.Reviving:
                        if (_reviveEffect != null)
                        {
                            return GM2DTools.WorldToTile(_reviveEffect.Position);
                        }
                        break;
                }
                return _curPos;
            }
        }

        protected Gun _gun;

        protected override bool OnInit()
        {
            LogHelper.Debug("MainUnit OnInit");
            if (!base.OnInit())
            {
                return false;
            }
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init("Idle");
			if (!_animation.AddEventHandle ("Step", OnStep)) 
            {
				return false;
			}
            _brakeEfffect = GameParticleManager.Instance.GetUnityNativeParticleItem(ParticleNameConstDefineGM2D.Brake, _trans);
            return true;
        }

        protected override void Clear()
        {
            _mainInput = _mainInput ?? new MainInput(this);
            _mainInput.Reset();

            _skillMgr1 = _skillMgr1 ?? new SkillManager(this);
            _skillMgr1.Clear();
            _skillMgr1.ChangeSkill<SkillWater>();

            _skillMgr2 = _skillMgr2 ?? new SkillManager(this);
            _skillMgr2.Clear();

            UpdateMp(0);
            _big = 0;
            _dieTime = 0;
            _flashTime = 0;
            _invincibleTime = 0;
            _box = null;
            _eBoxOperateType = EBoxOperateType.None;
            //view
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
                _view.SetRendererColor(Color.white);
            }
            if (_reviveEffect != null)
            {
                _reviveEffect.Destroy();
                _reviveEffect = null;
            }
            if (_portalEffect != null)
            {
                _portalEffect.Destroy();
                _portalEffect = null;
            }
            if (_brakeEfffect != null)
            {
                _brakeEfffect.Stop();
            }
            if (_invincibleEfffect != null)
            {
                _invincibleEfffect.Stop();
            }
            if (_inFireEfffect != null)
            {
                _inFireEfffect.Stop();
            }
            base.Clear();
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_reviveEffect != null)
            {
                _reviveEffect.Destroy();
                _reviveEffect = null;
            }
            if (_portalEffect != null)
            {
                _portalEffect.Destroy();
                _portalEffect = null;
            }
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            Debug.Log ("MainUnit.OnPlay");
            if (_gun == null)
            {
                _gun = PlayMode.Instance.CreateRuntimeUnit(10000, _curPos) as Gun;
            }
            _flashTime = 100;
            _revivePos = _curPos;
            _revivePosStack.Clear();
            if (PlayMode.Instance.IsUsingBoostItem (SoyEngine.Proto.EBoostItemType.BIT_AddLifeCount1)) {
                Life = PlayMode.Instance.SceneState.Life + 1;
            } else
            {
                Life = PlayMode.Instance.SceneState.Life;
            }
        }

        #region motor

        public override void CheckStart()
        {
            base.CheckStart();
            _isStart = true;
        }

        public override void UpdateLogic()
        {
            if (_isAlive && _isStart && !_isFreezed)
            {
                if (_attackedTimer > 0)
                {
                    _attackedTimer--;
                }
                if (_attackedTimer <= 0)
                {
                    _mainInput.UpdateLogic();
                    _skillMgr1.UpdateLogic();
                    if (_skillMgr2.UpdateLogic())
                    {
                        UpdateMp(Util.ConstantLerp(_currentMp, _totalMp, _mpSpeed));
                    }
                }
                CheckGround();
                CheckClimb();
                UpdateSpeedY();
            }
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
                    if (_dieTime > 20)
                    {
//                        UpdateRotation((_dieTime - 20)*0.3f);
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
            if (SpeedY != 0 && _mainInput._jumpState == 0)
            {
                _mainInput._jumpState = 1;
            }
            if (_invincibleTime > 0)
            {
                _invincibleTime--;
                if (_invincibleTime == 0)
                {
                    if (_view != null)
                    {
                        _view.SetRendererColor(Color.white);
                    }
                    if (_invincibleEfffect != null)
                    {
                        _invincibleEfffect.Stop();
                    }
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
                    _inFireEfffect.Trans.localPosition = Vector3.forward *(_curMoveDirection == EMoveDirection.Left ? 0.01f : -0.01f);
                    _inFireEfffect.Trans.rotation = Quaternion.identity;
                }
            }
            //if (!_isAlive)
            //{
            //    _dieTime++;
            //    if (_life <= 0)
            //    {
            //        if (_dieTime == 20)
            //        {
            //            Messenger.Broadcast(EMessengerType.GameFailedDeadMark);
            //            SpeedY = 150;
            //        }
            //        if (_dieTime > 20)
            //        {
            //            UpdateRotation((_dieTime - 20) * 0.3f);
            //        }
            //        if (_dieTime == 100)
            //        {
            //            PlayMode.Instance.SceneState.MainUnitSiTouLe();
            //            // 因生命用完而失败
            //            Messenger.Broadcast(EMessengerType.GameFinishFailed);
            //        }
            //    }
            //}
            if (!_isAlive) {
            }
            else if (!_grounded)
            {
                _mainInput._brakeTime = 0;
                if (_mainInput._eClimbState > 0)
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
                else if(_mainInput._jumpLevel == 0)
                {
                    if (_mainInput.JumpState == 101)
                    {
                        Messenger.Broadcast (EMessengerType.OnPlayerJump);
                        _animation.PlayOnce(JumpAnimName(_mainInput._jumpLevel));
						if (_mainInput.ClimbJump) {
							Vector3 effectPos = _trans.position;
							if (_curMoveDirection == EMoveDirection.Left)
							{
								effectPos += Vector3.right * 0.25f + Vector3.forward * 0.6f;
							}
							else
							{
								effectPos += Vector3.left * 0.25f + Vector3.forward * 0.6f;
							}
							GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.WallJump, effectPos);
						}
                        PlayMode.Instance.CurrentShadow.RecordAnimation(JumpAnimName(_mainInput._jumpLevel), false);
                    }
                    else if (_mainInput.JumpState == 1)
                    {
                        if (_animation.PlayLoop(FallAnimName()))
                        {
                            PlayMode.Instance.CurrentShadow.RecordAnimation(FallAnimName(), true);
                        }
                    }
                }
                else if (_mainInput._jumpLevel == 1)
                {
                    Messenger.Broadcast (EMessengerType.OnPlayerJump);
                    if (_animation.PlayLoop(JumpAnimName(_mainInput._jumpLevel)))
                    {
                        PlayMode.Instance.CurrentShadow.RecordAnimation(JumpAnimName(_mainInput._jumpLevel), true);
                    }
                }
            }
            else if (_isMoving)
            {
                bool brake = false;
                if (_mainInput._brakeTime > 0)
                {
                    brake = true;
                    if (_animation.PlayLoop(BrakeAnimName(), 1, 1))
                    {
                        PlayMode.Instance.CurrentShadow.RecordAnimation(BrakeAnimName(), false, 1, 1);
                        GameAudioManager.Instance.PlaySoundsEffects("AudioDestroySwamp");
                    }
                }
                if (!brake)
                {
                    int speed = (int)(SpeedX * 0.7f);
                    speed = Math.Abs(speed);
                    speed = Mathf.Clamp(speed, 30, 100);
                    //临时如此 应该修改动画播放速度！TODO
                    if (IsHoldingBox())
                    {
                        speed = 50;
                    }
                    if (speed <= 56)
                    {
                        if (_animation.PlayLoop(RunAnimName(speed), speed * deltaTime))
                        {
                            PlayMode.Instance.CurrentShadow.RecordAnimation(RunAnimName(speed), true, speed * deltaTime);
                        }
                        //run
                    }
                    else if (speed <= 94)
                    {
                        //run1
                        if (_animation.PlayLoop(RunAnimName(speed), speed * deltaTime))
                        {
                            PlayMode.Instance.CurrentShadow.RecordAnimation(RunAnimName(speed), true, speed * deltaTime);
                        }
                        _walkAudioInternal -= 5;
                    }
                    else
                    {
                        //run2
                        if (_animation.PlayLoop(RunAnimName(speed), speed * deltaTime))
                        {
                            PlayMode.Instance.CurrentShadow.RecordAnimation(RunAnimName(speed), true, speed * deltaTime);
                            GuideManager.Instance.OnSpecialOperate(1);
                        }
                        _walkAudioInternal -= 7;
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
                // 新手引导需要知道主角落地了
                if (!_lastGrounded)
                {
                    GuideManager.Instance.OnSpecialOperate(2);
                }
            }
            else if (!_lastGrounded)
            {
                //临时如此 应该加动画 TODO
                _animation.PlayOnce(LandAnimName());
                PlayMode.Instance.CurrentShadow.RecordAnimation(LandAnimName(), false);
                OnLand();
            }
            else if (!_animation.IsPlaying(LandAnimName()))
            {
                //stand
                if (_animation.PlayLoop(IdleAnimName()))
                {
                    PlayMode.Instance.CurrentShadow.RecordAnimation(IdleAnimName(), true);
                }
            }
            if (_brakeEfffect != null)
            {
                if (_mainInput._brakeTime > 0)
                {
                    _brakeEfffect.Play();
                }
                else
                {
                    if (_brakeEfffect.IsPlaying)
                    {
                        _brakeEfffect.Pause();
                        _animation.ClearTrack(1);
                        PlayMode.Instance.CurrentShadow.RecordClearAnimTrack(1);
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

            //UpdateShooterPoint ();

            Vector3 euler = _trans.eulerAngles;
            euler.y = _curMoveDirection == EMoveDirection.Right ? 0 : 180;
            if (_mainInput._brakeTime > 0)
            {
                euler.y += 180;
            }
            _trans.eulerAngles = euler;
            _lastGrounded = _grounded;
        }

        protected virtual void CheckGround()
        {
            bool air = false;
            int friction = 0;
            if (_mainInput._jumpState >= 100)
            {
                air = true;
            }
            if (!air && SpeedY != 0)
            {
                air = true;
            }
            if (!air)
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
                        _mainInput._jumpState = 0;
                        _mainInput._jumpLevel = 0;
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
            _curMaxSpeedX = MaxSpeedX;
            _curMaxQuickenSpeedX = MaxQuickenSpeedX;
            if (_onClay)
            {
                friction = 30;
                _curMaxSpeedX = (int) (_curMaxSpeedX * SpeedClayRatio);
                _curMaxQuickenSpeedX = (int) (_curMaxQuickenSpeedX * SpeedClayRatio);
            }
            else if (_onIce)
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
            if (IsHoldingBox())
            {
                _curMaxSpeedX = (int)(_curMaxSpeedX * SpeedHoldingBoxRatio);
            }
            if (air)
            {
                _mainInput._brakeTime = 0;
                if (_curBanInputTime <= 0)
                {
                    if (_mainInput.LeftInput == 1)
                    {
                        _isMoving = true;
                        if (_mainInput._quickenTime == 0)
                        {
                            if (SpeedX <= -_curMaxSpeedX + 4)
                            {
                                SpeedX += 4;
                            }
                            else if (SpeedX <= -_curMaxSpeedX)
                            {
                                SpeedX = -_curMaxSpeedX;
                            }
                            else if (SpeedX <= 0)
                            {
                                SpeedX -= 2;
                            }
                            else
                            {
                                SpeedX -= 10;
                            }
                        }
                        else
                        {
                            if (SpeedX <= -_curMaxQuickenSpeedX + 4)
                            {
                                SpeedX += 4;
                            }
                            else if (SpeedX <= -_curMaxQuickenSpeedX)
                            {
                                SpeedX = -_curMaxQuickenSpeedX;
                            }
                            else if (SpeedX <= 0)
                            {
                                if (SpeedX <= -_curMaxSpeedX)
                                {
                                    SpeedX--;
                                }
                                else
                                {
                                    SpeedX -= 3;
                                }
                            }
                            else
                            {
                                SpeedX -= 12;
                            }
                        }
                    }
                    else if (_mainInput.RightInput == 1)
                    {
                        _isMoving = true;
                        if (_mainInput._quickenTime == 0)
                        {
                            if (SpeedX >= _curMaxSpeedX + 4)
                            {
                                SpeedX -= 4;
                            }
                            else if (SpeedX >= _curMaxSpeedX)
                            {
                                SpeedX = _curMaxSpeedX;
                            }
                            else if (SpeedX >= 0)
                            {
                                SpeedX += 2;
                            }
                            else
                            {
                                SpeedX += 10;
                            }
                        }
                        else
                        {
                            if (SpeedX >= _curMaxQuickenSpeedX + 4)
                            {
                                SpeedX -= 4;
                            }
                            else if (SpeedX >= _curMaxQuickenSpeedX)
                            {
                                SpeedX = _curMaxQuickenSpeedX;
                            }
                            else if (SpeedX >= 0)
                            {
                                if (SpeedX > _curMaxSpeedX)
                                {
                                    SpeedX++;
                                }
                                else
                                {
                                    SpeedX += 3;
                                }
                            }
                            else
                            {
                                SpeedX += 12;
                            }
                        }
                    }
                }
            }
            else if (_lastGrounded)
            {
                if (_curBanInputTime <= 0)
                {
                    if (_mainInput.LeftInput == 1)
                    {
                        _isMoving = true;
                        if (_mainInput._quickenTime == 0)
                        {
                            if (_mainInput._brakeType == 0 && _mainInput._brakeTime > 0)
                            {
                                _mainInput._brakeTime--;
                                if (SpeedX - friction * 3 / 2 < 0)
                                {
                                    SpeedX = 0;
                                }
                                else
                                {
                                    SpeedX -= friction * 3 / 2;
                                    _mainInput._brakeTime = 10;
                                }
                            }
                            else if (SpeedX < (-_curMaxSpeedX - friction / 2))
                            {
                                SpeedX += friction / 2;
                            }
                            else if (SpeedX <= -_curMaxSpeedX)
                            {
                                SpeedX = -_curMaxSpeedX;
                            }
                            else if (SpeedX <= 0)
                            {
                                SpeedX -= 3;
                            }
                            else if (SpeedX >= 100)
                            {
                                _mainInput._brakeType = 0;
                                _mainInput._brakeTime = 5;
                            }
                            else if (SpeedX - friction * 2 < 0)
                            {
                                SpeedX = 0;
                            }
                            else
                            {
                                SpeedX -= friction * 2;
                            }
                        }
                        else
                        {
                            if (_mainInput._brakeType == 0 && _mainInput._brakeTime > 0)
                            {
                                _mainInput._brakeTime--;
                                if (SpeedX - friction * 3 / 2 < 0)
                                {
                                    SpeedX = 0;
                                }
                                else
                                {
                                    SpeedX -= friction * 3 / 2;
                                    _mainInput._brakeTime = 10;
                                }
                            }
                            else if (SpeedX < (-_curMaxQuickenSpeedX - friction / 2))
                            {
                                SpeedX += friction / 2;
                            }
                            else if (SpeedX <= -_curMaxQuickenSpeedX)
                            {
                                SpeedX = -_curMaxQuickenSpeedX;
                            }
                            else if (SpeedX <= 0)
                            {
                                if (SpeedX < -_curMaxSpeedX)
                                {
                                    SpeedX--;
                                }
                                else
                                {
                                    SpeedX -= 3;
                                }
                            }
                            else if (SpeedX >= 100)
                            {
                                _mainInput._brakeType = 0;
                                _mainInput._brakeTime = 5;
                            }
                            else if (SpeedX - friction * 3 < 0)
                            {
                                SpeedX = 0;
                            }
                            else
                            {
                                SpeedX -= friction * 3;
                            }
                        }
                    }
                    else if (_mainInput.RightInput == 1)
                    {
                        _isMoving = true;
                        if (_mainInput._quickenTime == 0)
                        {
                            if (_mainInput._brakeType == 1 && _mainInput._brakeTime > 0)
                            {
                                _mainInput._brakeTime--;
                                if (SpeedX + friction * 3 / 2 > 0)
                                {
                                    SpeedX = 0;
                                }
                                else
                                {
                                    SpeedX += friction * 3 / 2;
                                    _mainInput._brakeTime = 10;
                                }
                            }
                            else if (SpeedX > (_curMaxSpeedX + friction / 2))
                            {
                                SpeedX -= friction / 2;
                            }
                            else if (SpeedX >= _curMaxSpeedX)
                            {
                                SpeedX = _curMaxSpeedX;
                            }
                            else if (SpeedX >= 0)
                            {
                                SpeedX += 3;
                            }
                            else if (SpeedX <= -100)
                            {
                                _mainInput._brakeType = 1;
                                _mainInput._brakeTime = 5;
                            }
                            else if (SpeedX + friction * 2 > 0)
                            {
                                SpeedX = 0;
                            }
                            else
                            {
                                SpeedX += friction * 2;
                            }
                        }
                        else
                        {
                            if (_mainInput._brakeType == 1 && _mainInput._brakeTime > 0)
                            {
                                _mainInput._brakeTime--;
                                if (SpeedX + friction * 3 / 2 > 0)
                                {
                                    SpeedX = 0;
                                }
                                else
                                {
                                    SpeedX += friction * 3 / 2;
                                    _mainInput._brakeTime = 10;
                                }
                            }
                            else if (SpeedX > (_curMaxQuickenSpeedX + friction / 2))
                            {
                                SpeedX -= friction / 2;
                            }
                            else if (SpeedX >= _curMaxQuickenSpeedX)
                            {
                                SpeedX = _curMaxQuickenSpeedX;
                            }
                            else if (SpeedX >= 0)
                            {
                                if (SpeedX > _curMaxSpeedX)
                                {
                                    SpeedX++;
                                }
                                else
                                {
                                    SpeedX += 3;
                                }
                            }
                            else if (SpeedX <= -100)
                            {
                                _mainInput._brakeType = 1;
                                _mainInput._brakeTime = 5;
                            }
                            else if (SpeedX + friction * 3 < 0)
                            {
                                SpeedX = 0;
                            }
                            else
                            {
                                SpeedX += friction * 3;
                            }
                        }
                    }
                    else if (SpeedX < 0)
                    {
                        if (SpeedX >= -15)
                        {
                            SpeedX++;
                        }
                        else
                        {
                            SpeedX += friction / 2;
                        }
                    }
                    else if (SpeedX > 0)
                    {
                        if (SpeedX <= 15)
                        {
                            SpeedX--;
                        }
                        else
                        {
                            SpeedX -= friction / 2;
                        }
                    }
                }
                if (SpeedX == 0 && _mainInput._brakeTime == 0)
                {
                    _isMoving = false;
                }
            }
            //落地瞬间
            if (_grounded && !_lastGrounded)
            {
                if (_mainInput.RightInput == 0 && SpeedX < 0)
                {
                    if (_curMoveDirection != EMoveDirection.Left)
                    {
                        SetFacingDir(EMoveDirection.Left);
                        PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Left);
                    }
                }
                else if (_mainInput.LeftInput == 0 && SpeedX > 0)
                {
                    if (_curMoveDirection != EMoveDirection.Right)
                    {
                        SetFacingDir(EMoveDirection.Right);
                        PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Right);
                    }
                }
            }
        }

        private void OnFire()
        {
            _fireTimer++;
            //3秒后还是这个状态挂掉
            if (_fireTimer ==150)
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
            _mainInput._eClimbState = EClimbState.None;
            if (!_grounded && SpeedY < 0)
            {
                if (_mainInput.LeftInput > 0 && CheckLeftFloor())
                {
                    _mainInput._eClimbState = EClimbState.Left;
                }
                else if (_mainInput.RightInput > 0 && CheckRightFloor())
                {
                    _mainInput._eClimbState = EClimbState.Right;
                }
            }
        }

        protected virtual void UpdateSpeedY()
        {
            if (!_grounded)
            {
                if (_mainInput._eClimbState > EClimbState.None)
                {
                    SpeedY = Util.ConstantLerp(SpeedY, -50, 6);
                }
                else
                {
                    SpeedY -= 12;
                    if (SpeedY < -120)
                    {
                        SpeedY = -120;
                    }
                }
            }
        }

        public override void UpdateRenderer(float deltaTime)
        {
            if (_isAlive)
            {
                _mainInput.UpdateRenderer();
            }
        }

        #endregion

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
                _mainInput.ChangeFire2State(EFire2State.HoldBox);
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
                _mainInput.ChangeFire2State(EFire2State.HoldBox);
            }
            if (_box == null)
            {
                _mainInput.ChangeFire2State(EFire2State.Quicken);
            }
        }

        public void OnBoxHoldingChanged()
        {
            if (_box == null)
            {
                return;
            }
            _box.IsHoldingByMain = !_box.IsHoldingByMain;
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
            if (_big == 1)
            {
                _big = 0;
                _flashTime = FlashTime;
                //play effect
                //pause anim TODO
            }
            else
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
            LogHelper.Debug("MainPlayer OnDead");
            _invincibleTime = 0;
            _flashTime = 0;
            _big = 0;
            if (_brakeEfffect != null)
            {
                _brakeEfffect.Stop();
            }
            if (_invincibleEfffect != null)
            {
                _invincibleEfffect.Stop();
            }
            if (_inFireEfffect != null)
            {
                _inFireEfffect.Stop();
            }
            if (_gun != null)
            {
                _gun.Stop();
            }
            _mainInput.Clear();
            Messenger.Broadcast(EMessengerType.OnMainPlayerDead);
            base.OnDead();
            if (_life <= 0)
            {
                LogHelper.Debug("GameOver!");
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
            LogHelper.Debug("MainPlayer OnRevive");
            if (_view !=null && _reviveEffect == null)
            {
                var particle = GameParticleManager.Instance.GetUnityNativeParticleItem(ConstDefineGM2D.M1EffectSoul, null, ESortingOrder.LazerEffect);
                _reviveEffect = new ReviveEffect(particle);
            }
            _eUnitState = EUnitState.Reviving;
            _reviveEffect.Play(_trans.position + Vector3.up * 0.5f,
                                GM2DTools.TileToWorld(_revivePos), 6, () =>
                                {
                                    _eUnitState = EUnitState.Normal;
                                    _mainInput.Clear();
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
            if (_brakeEfffect != null)
            {
                _brakeEfffect.Stop();
            }
            if (_invincibleEfffect != null)
            {
                _invincibleEfffect.Stop();
            }
            _mainInput.Clear();
            ClearRunTime();
            _trans.eulerAngles = new Vector3(90, 0, 0);
            if (_portalEffect == null)
            {
                var particle = GameParticleManager.Instance.GetUnityNativeParticleItem(ConstDefineGM2D.PortalingEffect, null,  ESortingOrder.LazerEffect);
                _portalEffect = new ReviveEffect(particle);
            }
            _portalEffect.Play(_trans.position + Vector3.up * 0.5f,
                    GM2DTools.TileToWorld(targetPos), 3, () => PlayMode.Instance.RunNextLogic(() =>
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
            if (PlayMode.Instance.SceneState.Arrived)
            {
                _animation.PlayLoop(EnterDoorAnimName());
                PlayMode.Instance.CurrentShadow.RecordAnimation(EnterDoorAnimName(), true);
            }
            else
            {
                _animation.PlayOnce(VictoryAnimName());
                PlayMode.Instance.CurrentShadow.RecordAnimation(VictoryAnimName(), false);
            }
        }

        public override void OnRevivePos(IntVec2 pos)
        {
            if (_revivePos == pos) return;
            _revivePosStack.Push(_revivePos);
            _revivePos = pos;
        }

        protected void RollbackRevivePos()
        {
            _revivePos = _revivePosStack.Pop();
        }

        public void Step(int stepY = 0)
        {
            _mainInput.Step = true;
            _mainInput.StepY = stepY;
            _grounded = true;
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
            GameRun.Instance.Pause();
//            Debug.Log ("MainUnit.OnDeadAll");
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
		        GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.RunOnMud, _trans.position + Vector3.up*0.2f, scale, 1);
		    }
		}

        #endregion

        #region view

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
                if ((_box.DirectionRelativeMain == EDirectionType.Right &&  _mainInput.RightInput > 0)
                    || _box.DirectionRelativeMain == EDirectionType.Left && _mainInput.LeftInput > 0)
                {
                    return "Push";
                }
                else
                {
                    return "Pull";
                }
            }
            if (speed <= 56)
            {
                return "Run";
            }
            else if (speed <= 94)
            {
                return "Run2";
            }
            else
            {
                return "Run";
            }
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
        protected virtual string EnterDoorAnimName()
        {
            return "Jump2";
        }
        protected virtual string BrakeAnimName()
        {
            return "Brake";
        }

        #endregion

        internal void OnStun(ActorBase actor)
        {
            //晕2秒
            _attackedTimer = 100;
            Speed = IntVec2.zero;
            ExtraSpeed.y = 120;
            ExtraSpeed.x = actor.CenterPos.x > CenterPos.x ? -100 : 100;
            _mainInput.ClearInput();
        }

        internal void OnKnockBack(ActorBase actor)
        {
            _attackedTimer = 100;
            Speed = IntVec2.zero;
            ExtraSpeed.y = 280;
            ExtraSpeed.x = actor.CenterPos.x > CenterPos.x ? -80 : 80;
            _mainInput.ClearInput();
        }

        internal override void InFire()
        {
            if (!_isAlive)
            {
                return;
            }
            if (IsInvincible)
            {
                return;
            }
            if (_eDieType == EDieType.Fire)
            {
                return;
            }
            _eDieType = EDieType.Fire;
            if (_inFireEfffect == null)
            {
                _inFireEfffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectDeathFire", _trans);
            }
            if (_inFireEfffect != null)
            {
                _inFireEfffect.Play();
            }
            if (_view != null)
            {
                _view.SetRendererColor(new Color(0.2f, 0.2f, 0.2f, 1f));
            }
        }

        internal override void OutFire()
        {
            base.OutFire();
            if (_eDieType == EDieType.Fire)
            {
                if (_inFireEfffect != null)
                {
                    _inFireEfffect.Stop();
                }
                if (_view != null)
                {
                    _view.SetRendererColor(Color.white);
                }
            }
        }

        #region skill

        public override void ChangeSkill<T>()
        {
            if (_skillMgr2 != null)
            {
                if (_skillMgr2.ChangeSkill<T>())
                {
                    UpdateMp(0);
                }
            }
        }

        private void UpdateMp(int mp)
        {
            if (_currentMp == mp)
            {
                return;
            }
            _currentMp = Math.Min(_totalMp, mp);
            Messenger<int, int>.Broadcast(EMessengerType.OnMPChanged, _currentMp, _totalMp);
        }

        internal override int AddMp(int mp)
        {
            var oldMp = _currentMp;
            UpdateMp(_currentMp + mp);
            return _currentMp - oldMp;
        }

        internal bool Skill()
        {
            if (_skillMgr2 == null)
            {
                return false;
            }
            if (_currentMp < _skillMgr2.UseMp)
            {
                //TODO UI提示
                LogHelper.Warning("MP is not enough!");
                return false;
            }
            if (!_skillMgr2.Fire())
            {
                return false;
            }
            UpdateMp(_currentMp - _skillMgr2.UseMp);
            return true;
        }

        internal void SkillWater()
        {
            if (_skillMgr1 != null)
            {
                _skillMgr1.Fire();
            }
        }

        #endregion
    }
}
