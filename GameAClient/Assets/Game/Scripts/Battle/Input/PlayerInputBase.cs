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
        protected DynamicRigidbody _rigidUnit;

        public PlayerInputBase(DynamicRigidbody rigidUnit) 
        {
            _rigidUnit = rigidUnit;
        }

        public void Reset()
        {
            Clear();
        }

        public void UpdateLogic()
        {
            if (!PlayMode.Instance.SceneState.GameRunning)
            {
                return;
            }
            if (_rigidUnit.IsAlive && _rigidUnit.CurBanInputTime == 0 && !_rigidUnit.IsHoldingBox())
            {
                if (LeftInput)
                {
                    if (_rigidUnit.CurMoveDirection != EMoveDirection.Left)
                    {
                        _rigidUnit.SetFacingDir(EMoveDirection.Left);
                        if (_rigidUnit.IsMain)
                        {
                            PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Left);
                        }
                    }
                }
                else if (RightInput)
                {
                    if (_rigidUnit.CurMoveDirection != EMoveDirection.Right)
                    {
                        _rigidUnit.SetFacingDir(EMoveDirection.Right);
                        if (_rigidUnit.IsMain)
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
                    _rigidUnit.CurBanInputTime = BattleDefine.WallJumpBanInputTime;
                    _rigidUnit.ExtraSpeed.y = 0;
                    _jumpLevel = 0;
                    _jumpState = EJumpState.Jump1;
                    if (_eClimbState == EClimbState.Left)
                    {
                        _rigidUnit.SpeedX = 120;
                        _rigidUnit.SetFacingDir(EMoveDirection.Right);
                    }
                    else if (_eClimbState == EClimbState.Right)
                    {
                        _rigidUnit.SpeedX = -120;
                        _rigidUnit.SetFacingDir(EMoveDirection.Left);
                    }
                }
                else if (_jumpLevel == -1)
                {
                    if (_stepY > 0)
                    {
                        _rigidUnit.ExtraSpeed.y = _stepY;
                        _stepY = 0;
                    }
                    _jumpLevel = 0;
                    _rigidUnit.SpeedY = _rigidUnit.OnClay ? 100 : 150;
                    _jumpState = EJumpState.Jump1;
                    _jumpTimer = 10;
                }
                else if (!GetKeyLastApplied(EInputType.Jump) && IsCharacterAbilityAvailable(ECharacterAbility.DoubleJump))
                {
                    if (_jumpLevel == 0 || _jumpLevel == 2)
                    {
                        if (_rigidUnit.WingCount > 0)
                        {
                            _rigidUnit.WingCount--;
                            _jumpLevel = 2;
                            _rigidUnit.SpeedY = 120;
                        }
                        else
                        {
                            _jumpLevel = 1;
                            _rigidUnit.SpeedY = 150;
                            _jumpState = EJumpState.Jump2;
                        }
                        _rigidUnit.ExtraSpeed.y = 0;
                        _jumpTimer = 15;
                        _curAppliedInputKeyAry[(int) EInputType.Jump] = false;
                    }
                }
            }
            if (_jumpTimer > 0)
            {
                _jumpTimer--;
            }
            if ((_jumpTimer == 0 && _rigidUnit.SpeedY > 0) || _rigidUnit.SpeedY < 0)
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
                            _rigidUnit.OnBoxHoldingChanged();
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
            var eShootDir = _rigidUnit.CurMoveDirection == EMoveDirection.Left ? EShootDirectionType.Left : EShootDirectionType.Right;
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
            _rigidUnit.ShootAngle = (int)eShootDir;
            if (IsCharacterAbilityAvailable(ECharacterAbility.Shoot))
            {
                if (GetKeyApplied(EInputType.Skill1))
                {
                    _rigidUnit.SkillCtrl.Fire(0);
                }
                if (GetKeyDownApplied(EInputType.Skill2))
                {
                    _rigidUnit.SkillCtrl.Fire(1);
                }
                if (GetKeyDownApplied(EInputType.Skill3))
                {
                    _rigidUnit.SkillCtrl.Fire(2);
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
            if (!_rigidUnit.IsMain)
            {
                return true;
            }
            return GM2DGame.Instance.GameMode.IsPlayerCharacterAbilityAvailable(_rigidUnit, eCharacterAbility);
        }
    }

    public enum ELittleSkillState
    {
        Quicken,
        HoldBox
    }
}
