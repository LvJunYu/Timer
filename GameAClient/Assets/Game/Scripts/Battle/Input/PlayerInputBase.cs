/********************************************************************
** Filename : PlayerInputBase
** Author : Dong
** Date : 2016/10/20 星期四 下午 10:17:57
** Summary : PlayerInputBase
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{

    [Serializable]
    public class PlayerInputBase : InputBase
    {
        protected DynamicRigidbody _unit;

        public PlayerInputBase(DynamicRigidbody unit) 
        {
            _unit = unit;
        }

        public void UpdateLogic()
        {
            if (!PlayMode.Instance.SceneState.GameRunning)
            {
                return;
            }
            if (_unit.IsAlive && _unit.CurBanInputTime == 0 && !_unit.IsHoldingBox())
            {
                if (LeftInput)
                {
                    if (_unit.CurMoveDirection != EMoveDirection.Left)
                    {
                        _unit.SetFacingDir(EMoveDirection.Left);
                        if (_unit.IsMain)
                        {
                            PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Left);
                        }
                    }
                }
                else if (RightInput)
                {
                    if (_unit.CurMoveDirection != EMoveDirection.Right)
                    {
                        _unit.SetFacingDir(EMoveDirection.Right);
                        if (_unit.IsMain)
                        {
                            PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Right);
                        }
                    }
                }
            }
            CheckJump();
            CheckAssist();
            CheckSkill();
        }

        protected virtual void CheckJump()
        {
            _climbJump = false;
            if (GetKeyApplied(EInputType.Jump))
            {
                //攀墙跳
                if (_eClimbState > EClimbState.None)
                {
                    _climbJump = true;
                    _unit.CurBanInputTime = BattleDefine.WallJumpBanInputTime;
                    _unit.ExtraSpeed.y = 0;
                    _jumpLevel = 0;
                    _jumpState = EJumpState.Jump1;
                    if (_eClimbState == EClimbState.Left)
                    {
                        _unit.SpeedX = 120;
                        _unit.SetFacingDir(EMoveDirection.Right);
                    }
                    else if (_eClimbState == EClimbState.Right)
                    {
                        _unit.SpeedX = -120;
                        _unit.SetFacingDir(EMoveDirection.Left);
                    }
                }
                else if (_jumpLevel == -1)
                {
                    if (_stepY > 0)
                    {
                        _unit.ExtraSpeed.y = _stepY;
                        _stepY = 0;
                    }
                    _jumpLevel = 0;
                    _unit.SpeedY = _unit.OnClay ? 100 : 150;
                    _jumpState = EJumpState.Jump1;
                    _jumpTimer = 10;
                }
                else if (!GetKeyLastApplied(EInputType.Jump) && IsCharacterAbilityAvailable(ECharacterAbility.DoubleJump))
                {
                    if (_jumpLevel == 0 || _jumpLevel == 2)
                    {
                        if (_unit.WingCount > 0)
                        {
                            _unit.WingCount--;
                            _jumpLevel = 2;
                            _unit.SpeedY = 120;
                        }
                        else
                        {
                            _jumpLevel = 1;
                            _unit.SpeedY = 150;
                            _jumpState = EJumpState.Jump2;
                        }
                        _unit.ExtraSpeed.y = 0;
                        _jumpTimer = 15;
                        _curAppliedInputKeyAry[(int) EInputType.Jump] = false;
                    }
                }
            }
            if (_jumpTimer > 0)
            {
                _jumpTimer--;
            }
            if ((_jumpTimer == 0 && _unit.SpeedY > 0) || _unit.SpeedY < 0)
            {
                _jumpState = EJumpState.Fall;
            }
        }
        
        protected void CheckAssist()
        {
            switch (_littleSkillState)
            {
                case ELittleSkillState.HoldBox:
                    {
                        if (QuickenInputUp)
                        {
                            _unit.OnBoxHoldingChanged();
                        }
                    }
                    break;
                case ELittleSkillState.Quicken:
                    {
        
                    }
                    break;
            }
        }

        protected void CheckSkill()
        {
            var eShootDir = _unit.CurMoveDirection == EMoveDirection.Left ? EShootDirectionType.Left : EShootDirectionType.Right;
            if (GetKeyApplied(EInputType.Left))
            {
                eShootDir = EShootDirectionType.Left;
                if (GetKeyApplied(EInputType.Down))
                {
                    eShootDir = EShootDirectionType.LeftDown;
                }
                else if (GetKeyApplied(EInputType.Up))
                {
                    eShootDir = EShootDirectionType.LeftUp;
                }
            }
            else if (GetKeyApplied(EInputType.Right))
            {
                eShootDir = EShootDirectionType.Right;
                if (GetKeyApplied(EInputType.Down))
                {
                    eShootDir = EShootDirectionType.RightDown;
                }
                else if (GetKeyApplied(EInputType.Up))
                {
                    eShootDir = EShootDirectionType.RightUp;
                }
            }
            else if (GetKeyApplied(EInputType.Down))
            {
                eShootDir = EShootDirectionType.Down;
            }
            else if (GetKeyApplied(EInputType.Up))
            {
                eShootDir = EShootDirectionType.Up;
            }
            _unit.ShootAngle = (int)eShootDir;
            if (IsCharacterAbilityAvailable(ECharacterAbility.Shoot))
            {
                if (GetKeyApplied(EInputType.Skill1))
                {
                    _unit.SkillCtrl.Fire(0);
                }
                if (GetKeyDownApplied(EInputType.Skill2))
                {
                    _unit.SkillCtrl.Fire(1);
                }
                if (GetKeyDownApplied(EInputType.Skill3))
                {
                    _unit.SkillCtrl.Fire(2);
                }
            }
        }

        public void ChangeLittleSkillState(ELittleSkillState eLittleSkillState)
        {
            if (_littleSkillState == eLittleSkillState)
            {
                return;
            }
            _littleSkillState = eLittleSkillState;
            Messenger<ELittleSkillState>.Broadcast(EMessengerType.OnLittleSkillChanged, _littleSkillState);
        }

        private bool IsCharacterAbilityAvailable(ECharacterAbility eCharacterAbility)
        {
            if (!_unit.IsMain)
            {
                return true;
            }
            return GM2DGame.Instance.GameMode.IsPlayerCharacterAbilityAvailable(_unit, eCharacterAbility);
        }
    }

    public enum ELittleSkillState
    {
        Quicken,
        HoldBox
    }
}
