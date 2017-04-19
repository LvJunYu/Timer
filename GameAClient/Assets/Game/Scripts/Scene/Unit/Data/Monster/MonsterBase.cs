/********************************************************************
** Filename : MonsterBase
** Author : Dong
** Date : 2017/4/18 星期二 下午 9:05:44
** Summary : MonsterBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    public class MonsterBase : RigidbodyUnit
    {
        protected int _monsterSpeed;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _isMonster = true;
            _monsterSpeed = 20;
            return true;
        }

        public override void UpdateLogic()
        {
            if (_isAlive && _isStart)
            {
                bool air = false;
                int friction = 0;
                if (SpeedY != 0)
                {
                    air = true;
                }
                if (!air)
                {
                    bool downExist = false;
                    var units = EnvManager.RetriveDownUnits(this);
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        int ymin = 0;
                        if (unit != null && unit.IsAlive && CheckOnFloor(unit) && unit.OnUpHit(this, ref ymin, true))
                        {
                            downExist = true;
                            _grounded = true;
                            _downUnits.Add(unit);
                            unit.UpUnits.Add(this);
                            if (unit.Friction > friction)
                            {
                                friction = unit.Friction;
                            }
                        }
                    }
                    if (!downExist)
                    {
                        air = true;
                    }
                }
                if (!air)
                {
                    // 判断左右踩空
                    if (_downUnits.Count == 1)
                    {
                        if (SpeedX > 0 && _downUnits[0].ColliderGrid.XMax < _colliderGrid.XMax)
                        {
                            OnRightStampedEmpty();
                        }
                        else if (SpeedX < 0 && _downUnits[0].ColliderGrid.XMin > _colliderGrid.XMin)
                        {
                            OnLeftStampedEmpty();
                        }
                    }
                }
                if (air && _grounded)
                {
                    Speed += _lastExtraDeltaPos;
                    _grounded = false;
                }
                if (_curBanInputTime <= 0)
                {
                    if (_curMoveDirection == EMoveDirection.Right)
                    {
                        SpeedX = Util.ConstantLerp(SpeedX, _monsterSpeed, friction);
                    }
                    else
                    {
                        SpeedX = Util.ConstantLerp(SpeedX, -_monsterSpeed, friction);
                    }
                }
                if (!_grounded)
                {
                    SpeedY -= 12;
                    if (SpeedY < -160)
                    {
                        SpeedY = -160;
                    }
                }
            }
        }

        public override void UpdateView(float deltaTime)
        {
            if (_isStart && _isAlive && _dynamicCollider != null)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                LimitPos();
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                if (_view != null)
                {
                    _trans.position = GetTransPos();
                }
                if (OutOfMap())
                {
                    return;
                }
                _lastGrounded = _grounded;
            }
        }

        protected virtual void OnRightStampedEmpty()
        {
        }

        protected virtual void OnLeftStampedEmpty()
        {
        }
    }
}
