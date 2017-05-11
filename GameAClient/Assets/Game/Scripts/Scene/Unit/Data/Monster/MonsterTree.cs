/********************************************************************
** Filename : MonsterTree
** Author : Dong
** Date : 2017/4/18 星期二 下午 9:00:48
** Summary : MonsterTree
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 2001, Type = typeof(MonsterTree))]
    public class MonsterTree : MonsterBase
    {
        protected MonsterAI _mainInput;
        protected int _curMaxSpeedX;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _mainInput = new MonsterAI(this);
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _mainInput.MoveTo(PlayMode.Instance.MainUnit.CurPos);
        }

        public override void UpdateLogic()
        {
            if (_isAlive && _isStart)
            {
                if (_mainInput != null)
                {
                    _mainInput.UpdateLogic();
                }
                CheckGround();
                UpdateSpeedY();
            }
        }

        protected void CheckGround()
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
                        _mainInput._jumpState = 0;
                        _mainInput._jumpLevel = 0;
                        if (unit.Friction > friction)
                        {
                            friction = unit.Friction;
                        }
                        if (unit.Id == ConstDefineGM2D.ClayId)
                        {
                            _onClay = true;
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
            _curMaxSpeedX = _monsterSpeed;
            if (_onClay)
            {
                _curMaxSpeedX /= ClayRatio;
            }
            if (_curBanInputTime <= 0)
            {
                if (_mainInput.LeftInput == 1)
                {
                    _isMoving = true;
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
                }
                else if (_mainInput.RightInput == 1)
                {
                    _isMoving = true;
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

            //落地瞬间
            if (_grounded && !_lastGrounded)
            {
                if (_mainInput.RightInput == 0 && SpeedX < 0)
                {
                    if (_curMoveDirection != EMoveDirection.Left)
                    {
                        SetFacingDir(EMoveDirection.Left);
                    }
                }
                else if (_mainInput.LeftInput == 0 && SpeedX > 0)
                {
                    if (_curMoveDirection != EMoveDirection.Right)
                    {
                        SetFacingDir(EMoveDirection.Right);
                    }
                }
            }

            //if (!air)
            //{
            //    // 判断左右踩空
            //    if (_downUnits.Count == 1)
            //    {
            //        if (SpeedX > 0 && _downUnits[0].ColliderGrid.XMax < _colliderGrid.XMax)
            //        {
            //            OnRightStampedEmpty();
            //        }
            //        else if (SpeedX < 0 && _downUnits[0].ColliderGrid.XMin > _colliderGrid.XMin)
            //        {
            //            OnLeftStampedEmpty();
            //        }
            //    }
            //}
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
                    if (SpeedY < -160)
                    {
                        SpeedY = -160;
                    }
                }
            }
        }

        private void OnJump()
        {
        }

        protected override void UpdateMonster()
        {
            if (_isAlive && _isStart)
            {
                _animation.PlayLoop("Run");
                CheckWay();
            }
        }
    }
}
