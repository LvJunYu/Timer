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
    public class MonsterBase : ActorBase
    {
        protected IntVec2 _lastPos;
        protected int _monsterSpeed;
        protected int _curMaxSpeedX;
        protected int _curFriction;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _isMonster = true;
            _monsterSpeed = 30;
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _lastPos = _curPos;
            _curMaxSpeedX = _monsterSpeed;
        }

        public override void UpdateLogic()
        {
            if (_isAlive && _isStart && !_isFreezed)
            {
                bool air = false;
                _curFriction = _friction;
                if (SpeedY != 0)
                {
                    air = true;
                }
                if (!air)
                {
                    _onClay = false;
                    bool downExist = false;
                    int deltaX = int.MaxValue;
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
                            if (unit.Friction > _curFriction)
                            {
                                _curFriction = unit.Friction;
                            }
                            var edge = unit.GetUpEdge(this);
                            if (unit.OnClay() || edge.ESkillType == ESkillType.Clay)
                            {
                                _onClay = true;
                            }
                            else if (unit.OnIce() || edge.ESkillType == ESkillType.Ice)
                            {
                                _onIce = true;
                            }
                            var delta = Math.Abs(CenterPos.x - unit.CenterPos.x);
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
                if (air && _grounded)
                {
                    Speed += _lastExtraDeltaPos;
                    _grounded = false;
                }
                _curMaxSpeedX = _monsterSpeed;
                if (_onClay)
                {
                    _curFriction = 30;
                    _curMaxSpeedX /= ClayRatio;
                }
                else if (_onIce)
                {
                    _curFriction = 1;
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
                UpdateMonsterAI();
                if (_canMotor && _curBanInputTime <= 0)
                {
                    //着火了 迅速跑
                    if (_eDieType == EDieType.Fire)
                    {
                        
                    }
                    if (_curMoveDirection == EMoveDirection.Right)
                    {
                        SpeedX = Util.ConstantLerp(SpeedX, _monsterSpeed, _curFriction);
                    }
                    else
                    {
                        SpeedX = Util.ConstantLerp(SpeedX, -_monsterSpeed, _curFriction);
                    }
                }
                if (!_grounded)
                {
                    SpeedY -= 12;
                    if (SpeedY < -120)
                    {
                        SpeedY = -120;
                    }
                }
            }
        }

        protected virtual void UpdateMonsterAI()
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
                if (OutOfMap())
                {
                    return;
                }
                UpdateMonsterView();
                _lastGrounded = _grounded;
                _lastPos = _curPos;
            }
            if (!_isAlive)
            {
                _dieTime ++;
                if (_dieTime == 100)
                {
                    PlayMode.Instance.DestroyUnit(this);
                }
            }
        }

        protected virtual void UpdateMonsterView()
        {
        }

        protected virtual void OnRightStampedEmpty()
        {
        }

        protected virtual void OnLeftStampedEmpty()
        {
        }

        protected void CheckWay()
        {
            if (_hitUnits != null)
            {
                if (_curMoveDirection == EMoveDirection.Left && _hitUnits[(int)EDirectionType.Left] != null)
                {
                    ChangeWay(EMoveDirection.Right);
                }
                else if (_curMoveDirection == EMoveDirection.Right && _hitUnits[(int)EDirectionType.Right] != null)
                {
                    ChangeWay(EMoveDirection.Left);
                }
            }
        }

        public override void ChangeWay(EMoveDirection eMoveDirection)
        {
            if (!_isMonster)
            {
                return;
            }
            if (_curMaxSpeedX != 0)
            {
                SpeedX = eMoveDirection == EMoveDirection.Right ? _curMaxSpeedX : -_curMaxSpeedX;
            }
            SetFacingDir(eMoveDirection);
        }
    }
}
