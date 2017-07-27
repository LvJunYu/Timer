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
    public class PlayerInputBase : ActorInputBase
    {
        protected PlayerBase _player;

        public PlayerInputBase(PlayerBase player) : base(player)
        {
            _player = player;
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
            if (_player.IsAlive && _player.CurBanInputTime == 0 && !_player.IsHoldingBox())
            {
                if (LeftInput)
                {
                    if (_player.CurMoveDirection != EMoveDirection.Left)
                    {
                        _player.SetFacingDir(EMoveDirection.Left);
                        PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Left);
                    }
                }
                else if (RightInput)
                {
                    if (_player.CurMoveDirection != EMoveDirection.Right)
                    {
                        _player.SetFacingDir(EMoveDirection.Right);
                        PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Right);
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
                    _player.CurBanInputTime = BattleDefine.WallJumpBanInputTime;
                    _player.ExtraSpeed.y = 0;
                    _jumpLevel = 0;
                    _jumpState = EJumpState.Jump1;
                    if (_eClimbState == EClimbState.Left)
                    {
                        _player.SpeedX = 120;
                        _player.SetFacingDir(EMoveDirection.Right);
                    }
                    else if (_eClimbState == EClimbState.Right)
                    {
                        _player.SpeedX = -120;
                        _player.SetFacingDir(EMoveDirection.Left);
                    }
                }
                else if (_jumpLevel == -1)
                {
                    if (_stepY > 0)
                    {
                        _player.ExtraSpeed.y = _stepY;
                        _stepY = 0;
                    }
                    _jumpLevel = 0;
                    _player.SpeedY = _player.OnClay ? 100 : 150;
                    _jumpState = EJumpState.Jump1;
                    _jumpTimer = 10;
                }
                else if (!GetKeyLastApplied(EInputType.Jump) && _jumpLevel == 0 && IsCharacterAbilityAvailable(ECharacterAbility.DoubleJump))
                {
                    _jumpLevel = 1;
                    _player.ExtraSpeed.y = 0;
                    _player.SpeedY = 150;
                    _jumpState = EJumpState.Jump2;
                    _jumpTimer = 15;
                    _curAppliedInputKeyAry[(int) EInputType.Jump] = false;
//                    if (_player.WingCount > 0)
//                    {
//                        _player.WingCount--;
//                    }
                }
            }
            if (_jumpTimer > 0)
            {
                _jumpTimer--;
            }
            if ((_jumpTimer == 0 && _player.SpeedY > 0) || _player.SpeedY < 0)
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
                            _player.OnBoxHoldingChanged();
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
            var eShootDir = _player.CurMoveDirection == EMoveDirection.Left ? EShootDirectionType.Left : EShootDirectionType.Right;
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
            _player.ShootAngle = (int)eShootDir;
            if (IsCharacterAbilityAvailable(ECharacterAbility.Shoot))
            {
                if (GetKeyApplied(EInputType.Skill1))
                {
                    _player.SkillCtrl.Fire(0);
                }
                if (GetKeyDownApplied(EInputType.Skill2))
                {
                    _player.SkillCtrl.Fire(1);
                }
                if (GetKeyDownApplied(EInputType.Skill3))
                {
                    _player.SkillCtrl.Fire(2);
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
            return GM2DGame.Instance.GameMode.IsPlayerCharacterAbilityAvailable(_player, eCharacterAbility);
        }
    }

    public enum ELittleSkillState
    {
        Quicken,
        HoldBox
    }
}
