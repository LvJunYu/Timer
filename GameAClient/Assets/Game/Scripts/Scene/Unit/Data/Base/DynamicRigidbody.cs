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
        
        protected InputBase _input;
        
        // 跳跃等级
        [SerializeField]
        protected int _jumpLevel;
        // 跳跃状态
        [SerializeField] protected EJumpState _jumpState;

        [SerializeField]
        protected EClimbState _eClimbState;
        // 攀墙跳
        [SerializeField]
        protected bool _climbJump;
        protected int _stepY;
        /// <summary>
        /// 起跳的动画时间
        /// </summary>
        protected int _jumpTimer;

        protected abstract bool IsCheckGround();
        protected abstract bool IsCheckClimb();
        protected abstract bool IsUpdateSpeedY();

        public InputBase Input
        {
            get { return _input; }
        }

        protected override void Clear()
        {
            base.Clear();
            _jumpTimer = 0;
            _jumpState = EJumpState.Land;
            _jumpLevel = -1;
            _eClimbState = EClimbState.None;
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

            if (_isAlive && _isStart && !_isFreezed)
            {
                UpdateData();
                if (IsCheckGround())
                {
                    CheckGround();
                }
                if (IsCheckClimb())
                {
                    CheckClimb();
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
            bool air = false;
            int friction = 0;
            if (SpeedY != 0)
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
            CalculateMotor();
            _curMaxSpeedX = (int)(_maxSpeedX * _speedRatio);
            if (_curBanInputTime <= 0)
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
                    }
                    SpeedX = Util.ConstantLerp(SpeedX, speedAcc > 0 ? _curMaxSpeedX : -_curMaxSpeedX, Mathf.Abs(speedAcc));
                }
                else if (_grounded || _fanForce.y != 0)
                {
                    friction = MaxFriction;
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
        
        protected virtual void CheckClimb()
        {
            _eClimbState = EClimbState.None;
            if (!_grounded && SpeedY < 0)
            {
                if (_input.GetKeyApplied(EInputType.Left) && CheckLeftFloor())
                {
                    _eClimbState = EClimbState.Left;
                }
                else if (_input.GetKeyApplied(EInputType.Right) && CheckRightFloor())
                {
                    _eClimbState = EClimbState.Right;
                }
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
                else if (_eClimbState > EClimbState.None)
                {
                    SpeedY = Util.ConstantLerp(SpeedY, -50, 6);
                }
                else
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
            }
            _fanForce.y = 0;
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

        protected virtual void CalculateMotor()
        {
            _motorAcc = 0;
            if (_input.GetKeyApplied(EInputType.Right) && CanMove)
            {
                _motorAcc = _onIce ? 1 : 10;
            }
            if (_input.GetKeyApplied(EInputType.Left) && CanMove)
            {
                _motorAcc = _onIce ? -1 : -10;
            }
            _speedRatio = 1;
            if (IsHoldingBox())
            {
                _speedRatio *= SpeedHoldingBoxRatio;
            }
            if (_onClay)
            {
                _speedRatio *= SpeedClayRatio;
            }
        }

        public virtual bool IsHoldingBox()
        {
            return false;
        }

        public virtual void OnBoxHoldingChanged()
        {
        }
        
        public override void UpdateView(float deltaTime)
        {
            if (_isStart && _isAlive)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                LimitPos();
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
                CheckOutOfMap();
            }
            UpdateDynamicView(deltaTime);
            _lastGrounded = _grounded;
        }

        protected virtual void UpdateDynamicView(float deltaTime)
        {
        }
    }
}