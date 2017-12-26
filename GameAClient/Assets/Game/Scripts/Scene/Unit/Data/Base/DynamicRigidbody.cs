using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public abstract class DynamicRigidbody : RigidbodyUnit
    {
        protected int _maxSpeedX;
        protected int _curMaxSpeedX;

        protected float _speedRatio;
        protected int _motorAcc;
        protected bool _inLadder;
        protected int _dropLadderTimer;
        protected List<UnitBase> _inLadders = new List<UnitBase>(4);

        protected InputBase _input;

        // 跳跃等级
        [SerializeField] protected int _jumpLevel;

        // 跳跃状态
        [SerializeField] protected EJumpState _jumpState;

        [SerializeField] protected EClimbState _eClimbState;

        // 攀墙跳
        [SerializeField] protected bool _climbJump;

        protected int _stepY;

        /// <summary>
        /// 起跳的动画时间
        /// </summary>
        protected int _jumpTimer;

        protected const float SpeedClayRatio = 0.2f;
        protected const float SpeedFireRatio = 1.8f;
        protected const float SpeedInIceRatio = 0.6f;
        protected const float SpeedHoldingBoxRatio = 0.3f;
        protected const float SpeedChaseRatio = 2f;

        protected abstract bool IsCheckGround();
        protected abstract bool IsCheckClimb();
        protected abstract bool IsUpdateSpeedY();

        protected override bool IsClimbing
        {
            get { return ClimbState != EClimbState.None; }
        }

        protected override bool IsClimbingVertical
        {
            get
            {
                return ClimbState == EClimbState.Left || ClimbState == EClimbState.Right ||
                       ClimbState == EClimbState.Ladder;
            }
        }

        public InputBase Input
        {
            get { return _input; }
        }

        public int CurMaxSpeedX
        {
            get { return _curMaxSpeedX; }
        }

        public EClimbState ClimbState
        {
            get { return _eClimbState; }
        }

        public bool InLadder
        {
            get { return _inLadder; }
        }

        protected override void Clear()
        {
            base.Clear();
            _jumpTimer = 0;
            _jumpState = EJumpState.Land;
            _jumpLevel = -1;
            _eClimbState = EClimbState.None;
            _inLadder = false;
            _inLadders.Clear();
            _dropLadderTimer = 0;
            _climbJump = false;
            _stepY = 0;
        }

        public void Setup(InputBase inputBase)
        {
            _input = inputBase;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_isAlive && !_isFreezed)
            {
                UpdateData();
                if (IsCheckGround())
                {
                    CheckGround();
                }
                if (IsUpdateSpeedY())
                {
                    UpdateSpeedY();
                }
            }
        }

        protected virtual void UpdateData()
        {
        }

        protected virtual void CheckGround()
        {
            bool air = SpeedY != 0;
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
                        _downUnits.Add(unit);
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
            CalculateSpeedRatio();
            _curMaxSpeedX = 0;
            if (_eActiveState == EActiveState.Active)
            {
                _curMaxSpeedX = (int) (_maxSpeedX * _speedRatio * _speedStateRatio);
                AfterCheckGround();
            }
            CalculateMotor();
            if (IsCheckClimb())
            {
                CheckClimb();
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
            if (_curBanInputTime <= 0)
            {
                CaculateSpeedX(air);
            }
        }

        protected virtual void CaculateSpeedX(bool air)
        {
            int speedAcc = _motorAcc + _fanForce.x;
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
                    //落地瞬间，如果相反 立即摆正。
                    if (!_lastGrounded && _grounded && (SpeedX * speedAcc < 0))
                    {
                        speedAcc = speedAcc > 0 ? MaxFriction : -MaxFriction;
                    }
                }
                SpeedX = Util.ConstantLerp(SpeedX, speedAcc > 0 ? _curMaxSpeedX : -_curMaxSpeedX, Mathf.Abs(speedAcc));
            }
            else if (_grounded || _fanForce.y != 0 || ClimbState == EClimbState.Up || ClimbState == EClimbState.Ladder)
            {
                var friction = MaxFriction;
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

        private bool CheckLadder()
        {
            if (_inLadder && _dropLadderTimer == 0)
            {
                SpeedX = 0;
                SetClimbState(EClimbState.Ladder, _inLadders[0]);
                return true;
            }
            return false;
        }

        protected virtual void CheckClimb()
        {
            switch (ClimbState)
            {
                case EClimbState.None:
                    if (_input.GetKeyApplied(EInputType.Down) && !_grounded || _input.GetKeyApplied(EInputType.Up))
                    {
                        CheckLadder();
                    }
                    break;
                case EClimbState.Ladder:
                    if (!CheckLadderVerticalFloor())
                    {
                        SetClimbState(EClimbState.None);
                        break;
                    }
                    if (_input.GetKeyApplied(EInputType.Down) && _grounded)
                    {
                        SetClimbState(EClimbState.None);
                        break;
                    }
                    if (CheckLadderHorizontalFloor())
                    {
                        if (_input.GetKeyApplied(EInputType.Right))
                        {
                            if (!CheckLadderHorizontalFloor(_curMaxSpeedX))
                            {
                                _motorAcc = 0;
                            }
                        }
                        else if (_input.GetKeyApplied(EInputType.Left))
                        {
                            if (!CheckLadderHorizontalFloor(-_curMaxSpeedX))
                            {
                                _motorAcc = 0;
                            }
                        }
                    }
                    else
                    {
                        SetClimbState(EClimbState.None);
                    }
                    break;
                case EClimbState.Left:
                    if (_input.GetKeyApplied(EInputType.Right))
                    {
                        CheckLadder();
                    }
                    if (_input.GetKeyApplied(EInputType.Down) && _grounded)
                    {
                        SetClimbState(EClimbState.None);
                        break;
                    }
                    if (!CheckLeftClimbFloor())
                    {
                        SetClimbState(EClimbState.None);
                    }
                    break;
                case EClimbState.Right:
                    if (_input.GetKeyApplied(EInputType.Left))
                    {
                        CheckLadder();
                    }
                    if (_input.GetKeyApplied(EInputType.Down) && _grounded)
                    {
                        SetClimbState(EClimbState.None);
                        break;
                    }
                    if (!CheckRightClimbFloor())
                    {
                        SetClimbState(EClimbState.None);
                    }
                    break;
                case EClimbState.Up:
                    if (_input.GetKeyApplied(EInputType.Down))
                    {
                        CheckLadder();
                    }
                    if (CheckUpClimbFloor())
                    {
                        if (_input.GetKeyApplied(EInputType.Right))
                        {
                            if (!CheckUpClimbFloor(_curMaxSpeedX))
                            {
                                if (CheckLadder())
                                {
                                    if (!CheckLadderHorizontalFloor(_curMaxSpeedX))
                                    {
                                        _motorAcc = 0;
                                    }
                                }
                                else
                                {
                                    _motorAcc = 0;
                                }
                            }
                        }
                        else if (_input.GetKeyApplied(EInputType.Left))
                        {
                            if (!CheckUpClimbFloor(-_curMaxSpeedX))
                            {
                                if (CheckLadder())
                                {
                                    if (!CheckLadderHorizontalFloor(-_curMaxSpeedX))
                                    {
                                        _motorAcc = 0;
                                    }
                                }
                                else
                                {
                                    _motorAcc = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        SetClimbState(EClimbState.None);
                    }
                    break;
            }
            if (ClimbState != EClimbState.None)
            {
                _grounded = false;
            }
        }

        protected virtual void UpdateSpeedY()
        {
            SpeedY += _fanForce.y;
            if (!_grounded)
            {
                if (_jumpLevel == 2)
                {
                    SpeedY = Util.ConstantLerp(SpeedY, -60, 6);
                }
                else if (ClimbState > EClimbState.None)
                {
                    switch (ClimbState)
                    {
                        case EClimbState.Ladder:
                            SpeedY = 0;
                            if (_input.GetKeyApplied(EInputType.Up))
                            {
                                if (CheckLadderVerticalFloor(_curMaxSpeedX))
                                {
                                    SpeedY = _curMaxSpeedX;
                                }
                            }
                            else if (_input.GetKeyApplied(EInputType.Down))
                            {
                                if (CheckLadderVerticalFloor(-_curMaxSpeedX))
                                {
                                    SpeedY = -_curMaxSpeedX;
                                }
                                if (_grounded)
                                {
                                    _eClimbState = EClimbState.None;
                                }
                            }
                            break;
                        case EClimbState.Left:
                            SpeedY = 0;
                            if (_input.GetKeyApplied(EInputType.Up))
                            {
                                if (CheckLeftClimbFloor(_curMaxSpeedX))
                                {
                                    SpeedY = _curMaxSpeedX;
                                }
                                else
                                {
                                    if (CheckLadder() && CheckLadderVerticalFloor(_curMaxSpeedX))
                                    {
                                        SpeedY = _curMaxSpeedX;
                                    }
                                }
                            }
                            else if (_input.GetKeyApplied(EInputType.Down))
                            {
                                if (CheckLeftClimbFloor(-_curMaxSpeedX))
                                {
                                    SpeedY = -_curMaxSpeedX;
                                }
                                else
                                {
                                    if (CheckLadder() && CheckLadderVerticalFloor(-_curMaxSpeedX))
                                    {
                                        SpeedY = -_curMaxSpeedX;
                                    }
                                }
                                if (_grounded)
                                {
                                    _eClimbState = EClimbState.None;
                                }
                            }
                            break;
                        case EClimbState.Right:
                            SpeedY = 0;
                            if (_input.GetKeyApplied(EInputType.Up))
                            {
                                if (CheckRightClimbFloor(_curMaxSpeedX))
                                {
                                    SpeedY = _curMaxSpeedX;
                                }
                                else
                                {
                                    if (CheckLadder() && CheckLadderVerticalFloor(_curMaxSpeedX))
                                    {
                                        SpeedY = _curMaxSpeedX;
                                    }
                                }
                            }
                            else if (_input.GetKeyApplied(EInputType.Down))
                            {
                                if (CheckRightClimbFloor(-_curMaxSpeedX))
                                {
                                    SpeedY = -_curMaxSpeedX;
                                }
                                else
                                {
                                    if (CheckLadder() && CheckLadderVerticalFloor(-_curMaxSpeedX))
                                    {
                                        SpeedY = -_curMaxSpeedX;
                                    }
                                }
                                if (_grounded)
                                {
                                    _eClimbState = EClimbState.None;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    CaculateGravity();
                }
            }
            _fanForce.y = 0;
        }

        protected virtual void CaculateGravity()
        {
            if (SpeedY > 0 && _fanForce.y == 0)
            {
                SpeedY = Util.ConstantLerp(SpeedY, 0, 12);
            }
            else
            {
                SpeedY = Util.ConstantLerp(SpeedY, -120, 8);
            }
        }

        public override void SetStepOnClay()
        {
            if (_eClimbState == EClimbState.None)
            {
                _onClay = true;
            }
        }

        protected override void CheckClimbUnitChangeDir(EClimbState eClimbState)
        {
            //判断物体是否需要改变方向（用于该物体已经完成移动判定，不变向会出现穿透）
            if (_curClimbUnit != null && _curClimbUnit.UseMagic() && _eClimbState == eClimbState)
            {
                if (_eClimbState == EClimbState.Left && _curClimbUnit.SpeedX > 0)
                {
                    ((Magic) _curClimbUnit).ChangeMoveDirection();
                }
                else if (_eClimbState == EClimbState.Right && _curClimbUnit.SpeedX < 0)
                {
                    ((Magic) _curClimbUnit).ChangeMoveDirection();
                }
                else if (_eClimbState == EClimbState.Up && _curClimbUnit.SpeedY < 0)
                {
                    ((Magic) _curClimbUnit).ChangeMoveDirection();
                } 
            }
        }

        public override void SetClimbState(EClimbState eClimbState, UnitBase unit = null)
        {
            if (eClimbState == EClimbState.None && _eClimbState > EClimbState.None)
            {
                CheckClimbUnitChangeDir(_eClimbState);
            }
            _eClimbState = eClimbState;
            _curClimbUnit = unit;
            if (_eClimbState > EClimbState.None)
            {
                _onClay = false;
            }
            switch (ClimbState)
            {
                case EClimbState.None:
                    break;
                case EClimbState.Left:
                    SetFacingDir(EMoveDirection.Left);
                    break;
                case EClimbState.Right:
                    SetFacingDir(EMoveDirection.Right);
                    break;
            }
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("SetClimbState {0}", _eClimbState.ToString()));
            }
//            LogHelper.Debug(_eClimbState.ToString());
        }

        protected virtual void OnJump()
        {
        }

        protected virtual void OnLand()
        {
            _grounded = true;
            _jumpLevel = -1;
            _jumpTimer = 0;
            _jumpState = EJumpState.Land;
        }

        protected virtual void CalculateSpeedRatio()
        {
            _speedRatio = 1;
            if (IsHoldingBox())
            {
                _speedRatio *= SpeedHoldingBoxRatio;
            }
            if (_onClay)
            {
                _speedRatio *= SpeedClayRatio;
            }
            if (IsInState(EEnvState.Ice))
            {
                _speedRatio *= SpeedInIceRatio;
            }
        }

        protected virtual void AfterCheckGround()
        {
        }

        protected virtual void CalculateMotor()
        {
            _motorAcc = 0;
            if (CanMove && ClimbState != EClimbState.Left && ClimbState != EClimbState.Right)
            {
                if (_input.GetKeyApplied(EInputType.Right))
                {
                    _motorAcc = _onIce ? 1 : 10;
                }
                if (_input.GetKeyApplied(EInputType.Left))
                {
                    _motorAcc = _onIce ? -1 : -10;
                }
                if (_grounded && IsInState(EEnvState.Ice) && _motorAcc != 0)
                {
                    SpeedY = 120;
                }
            }
        }

        public virtual bool IsHoldingBox()
        {
            return false;
        }

        public virtual void OnBoxHoldingChanged()
        {
        }

        private IntVec2 _lastPos;

        public override void UpdateView(float deltaTime)
        {
            if (_isAlive)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                UpdateCollider(GetColliderPos(_curPos));
                _lastPos = _curPos = GetPos(_colliderPos);
                if (GM2DGame.Instance.GameMode.SaveShadowData && IsMain)
                {
                    GM2DGame.Instance.GameMode.ShadowData.RecordPos(_curPos);
                }
                UpdateTransPos();
                if (GameModeNetPlay.DebugEnable())
                {
                    GameModeNetPlay.WriteDebugData(string.Format("Guid {0} _trans.position = {1} ", Guid,
                        _trans.position));
                }
            }
            else if (GameRun.Instance.IsPlaying)
            {
                if (GM2DGame.Instance.GameMode.SaveShadowData && IsMain)
                {
                    GM2DGame.Instance.GameMode.ShadowData.RecordPos(_lastPos);
                }
            }
            UpdateDynamicView(deltaTime);
            _lastGrounded = _grounded;
        }

        protected virtual void UpdateDynamicView(float deltaTime)
        {
        }

        protected override void CheckClimbUnitZ(ref float z)
        {
            if (_curClimbUnit != null && _curClimbUnit.UseMagic())
            {
                if (_eClimbState == EClimbState.Left)
                {
                    z = Mathf.Min(z, _curClimbUnit.Trans.position.z - 1.51f);
                }
                else if (_eClimbState == EClimbState.Right)
                {
                    z = Mathf.Max(z, _curClimbUnit.Trans.position.z + 1.51f);
                }
            }
        }
    }
}