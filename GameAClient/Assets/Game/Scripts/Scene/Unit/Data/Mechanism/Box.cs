/********************************************************************
** Filename : BoxUnit
** Author : Dong
** Date : 2017/1/9 星期一 下午 11:30:13
** Summary : Box
***********************************************************************/

using System;
using SoyEngine;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 5004, Type = typeof(Box))]
    public class Box : RigidbodyUnit
    {
        protected bool _isHoldingByPlayer;
        protected PlayerBase _holder;
        protected int _fallDistance;

        protected EDirectionType _directionRelativeMain;

        public bool IsHoldingByPlayer
        {
            get { return _isHoldingByPlayer; }
            set { _isHoldingByPlayer = value; }
        }

        public PlayerBase Holder
        {
            get { return _holder; }
        }

        public override bool CanPortal
        {
            get { return true; }
        }

        public EDirectionType DirectionRelativeMain
        {
            get { return _directionRelativeMain; }
            set { _directionRelativeMain = value; }
        }

        public int FallDistance
        {
            get { return _fallDistance; }
        }

        protected override void Clear()
        {
            _isHoldingByPlayer = false;
            _fallDistance = 0;
            base.Clear();
        }

        protected override void OnDead()
        {
            if (!_isAlive)
            {
                return;
            }

            PlayMode.Instance.DestroyUnit(this);
            base.OnDead();
        }

        public override void UpdateLogic()
        {
            if (_isAlive)
            {
                bool air = SpeedY != 0;
                if (!air)
                {
                    _fallDistance = 0;
                    _onClay = false;
                    _onIce = false;
                    bool downExist = false;
                    using (var units = EnvManager.RetriveDownUnits(this))
                    {
                        for (int i = 0; i < units.Count; i++)
                        {
                            var unit = units[i];
                            int ymin = 0;
                            if (unit.IsAlive && CheckOnFloor(unit) && unit.OnUpHit(this, ref ymin, true))
                            {
                                downExist = true;
                                _grounded = true;
                                _downUnits.Add(unit);
                            }
                        }
                    }

                    if (!downExist)
                    {
                        air = true;
                    }
                }

                if (air && _grounded)
                {
                    Speed += _lastExtraDeltaPos;
                    _grounded = false;
                }

                if (_grounded)
                {
                    var friction = MaxFriction;
                    if (_onIce)
                    {
                        friction = 1;
                    }

                    SpeedX = Util.ConstantLerp(SpeedX, 0, friction);
                }
                else
                {
                    SpeedY -= 15;
                    if (SpeedY < -160)
                    {
                        SpeedY = -160;
                    }
                }
            }
        }

        public override void UpdateView(float deltaTime)
        {
            if (_isAlive && _dynamicCollider != null)
            {
                _deltaPos = _speed + _extraDeltaPos;
                if (_isHoldingByPlayer)
                {
                    _deltaPos.x = 0;
                    if (_deltaPos.y != 0)
                    {
                        _holder.OnBoxHoldingChanged();
                    }
                    else
                    {
                        var deltaMainPos = _holder.Speed + _holder.ExtraDeltaPos;
                        _deltaPos += IntVec2.right * deltaMainPos.x;
                    }
                }

                var lasPos = _curPos;
                _curPos += _deltaPos;
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                if (_curPos.y < lasPos.y)
                {
                    _fallDistance += lasPos.y - _curPos.y;
                }

                UpdateTransPos();
            }
        }

        public void SetHoder(PlayerBase playerBase)
        {
            _holder = playerBase;
        }
    }
}