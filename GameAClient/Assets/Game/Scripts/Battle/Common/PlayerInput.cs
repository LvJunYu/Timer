﻿/********************************************************************
** Filename : PlayerInput
** Author : Dong
** Date : 2016/10/20 星期四 下午 10:17:57
** Summary : PlayerInput
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

namespace GameA.Game
{
    [Serializable]
    public class PlayerInput
    {
        protected PlayerBase _player;

        [SerializeField]
        protected ERunMode _runMode;

        #region state

        [SerializeField]
        protected ELittleSkillState _littleSkillState;

        [SerializeField]
        protected bool[] _curInputs = new bool[(int)EInputType.Max];
        [SerializeField]
        protected bool[] _lastInputs = new bool[(int)EInputType.Max];

        [SerializeField]
        protected float _lastHorizontal;
        [SerializeField]
        protected float _curHorizontal;
        [SerializeField]
        protected float _lastVertical;
        [SerializeField]
        protected float _curVertical;

        [SerializeField]
        protected int _leftInput;
        [SerializeField]
        protected int _rightInput;
        [SerializeField]
        protected int _upInput;
        [SerializeField]
        protected int _downInput;

        [SerializeField]
        protected bool _jumpInput;
        protected bool _lastJumpInput;

        [SerializeField]
        protected bool _assistInput;
        [SerializeField]
        protected bool _lastAssistInput;

        [SerializeField]
        protected bool[] _skillInputs = new bool[3];
        [SerializeField]
        protected bool[] _lastSkillInputs = new bool[3];

        // 跳跃等级
        [SerializeField]
        public int _jumpLevel = 0;
        // 跳跃状态
        [SerializeField] public EJumpState _jumpState;

        [SerializeField]
        public EClimbState _eClimbState;
        // 攀墙跳
        [SerializeField]
        protected bool _climbJump = false;
        protected int _stepY;
        /// <summary>
        /// 起跳的动画时间
        /// </summary>
        protected int _jumpTimer;

        #endregion

        #region Input

        [SerializeField]
        protected List<int> _inputDatas = new List<int>();

        [SerializeField]
        protected int _index;

        [SerializeField]
        protected int _curTime;
        [SerializeField]
        protected int _totalTime;

        #endregion

        public int LeftInput
        {
            get { return _leftInput; }
        }

        public int RightInput
        {
            get { return _rightInput; }
        }

        public bool JumpInput
        {
            get { return _jumpInput; }
        }

        public bool QuickenInputUp
        {
            get { return !_assistInput && _lastAssistInput; }
        }

        public bool ClimbJump
        {
            get { return _climbJump; }
        }

        public int StepY
        {
            get { return _stepY; }
            set { _stepY = value; }
        }

        public List<int> InputDatas
        {
            get { return _inputDatas; }
        }

        public EJumpState JumpState
        {
            get { return _jumpState; }
        }

        public PlayerInput(PlayerBase player)
        {
            _player = player;
            _runMode = PlayMode.Instance.ERunMode;
            if (PlayMode.Instance.InputDatas != null)
            {
                _inputDatas = PlayMode.Instance.InputDatas;
            }
        }

        public void Reset()
        {
            Clear();
            if (_runMode == ERunMode.Normal)
            {
                _inputDatas.Clear();
            }
            else
            {
                _inputDatas = PlayMode.Instance.InputDatas;
            }
            _curTime = 0;
            _totalTime = 0;
            _index = 0;
        }
        
        public bool SkillInputDown(int i)
        {
            return _skillInputs[i] && !_lastSkillInputs[i];
        }

        public void OnLand()
        {
            _jumpLevel = -1;
            _jumpTimer = 0;
            _jumpInput = false;
            _jumpState = EJumpState.Land;
        }

        public void Clear()
        {
            ClearInput();
        }

        public void ClearInput()
        {
            _jumpTimer = 0;
            _lastJumpInput = false;
            _lastHorizontal = 0;
            _curHorizontal = 0;
            _lastVertical = 0;
            _curVertical = 0;
            _leftInput = 0;
            _rightInput = 0;
            _upInput = 0;
            _downInput = 0;
            _jumpInput = false;
            _jumpState = EJumpState.Land;
            _jumpLevel = -1;
            _eClimbState = EClimbState.None;
            _climbJump = false;
            _stepY = 0;
            _assistInput = false;
            _lastAssistInput = false;
            for (int i = 0; i < _skillInputs.Length; i++)
            {
                _skillInputs[i] = false;
                _lastSkillInputs[i] = false;
            }
            for (int i = 0; i < _curInputs.Length; i++)
            {
                _curInputs[i] = false;
                _lastInputs[i] = false;
            }
        }

        public void UpdateLogic()
        {
            if (!PlayMode.Instance.SceneState.GameRunning)
            {
                return;
            }
            _curTime++;
            _totalTime = _curTime;
            CheckInput();
            if (_player.IsAlive && _player.CurBanInputTime == 0 && !_player.IsHoldingBox())
            {
                if (_leftInput > 0)
                {
                    if (_player.CurMoveDirection != EMoveDirection.Left)
                    {
                        _player.SetFacingDir(EMoveDirection.Left);
                        PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Left);
                    }
                }
                else if (_rightInput > 0)
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

        protected void CheckInput()
        {
            switch (_runMode)
            {
                case ERunMode.Normal:
                    CalcuteDatas(true, false);
                    break;
                case ERunMode.Record:
                    SyncInputDatas();
                    break;
                case ERunMode.Net:
                    if (_player.IsMain)
                    {
                        //发送网络
                        CalcuteDatas(false, true);
                    }
                    SyncInputDatas();
                    break;
            }
            for (int i = 0; i < _curInputs.Length; i++)
            {
                _lastInputs[i] = _curInputs[i];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apply">单人模式直接应用</param>
        /// <param name="sendToServer">主角且网络模式</param>
        private void CalcuteDatas(bool apply, bool sendToServer)
        {
            if (_curInputs[0] && !_lastInputs[0])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(0);
                if (apply)
                {
                    _leftInput = 1;
                }
            }
            if (!_curInputs[0] && _lastInputs[0])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(1);
                if (apply)
                {
                    _leftInput = 0;
                }
            }
            if (_curInputs[1] && !_lastInputs[1])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(2);
                if (apply)
                {
                    _rightInput = 1;
                }
            }
            if (!_curInputs[1] && _lastInputs[1])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(3);
                if (apply)
                {
                    _rightInput = 0;
                }
            }
            if (_curInputs[2] && !_lastInputs[2])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(4);
                if (apply)
                {
                    _upInput = 1;
                }
            }
            if (!_curInputs[2] && _lastInputs[2])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(5);
                if (apply)
                {
                    _upInput = 0;
                }
            }
            if (_curInputs[3] && !_lastInputs[3])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(6);
                if (apply)
                {
                    _downInput = 1;
                }
            }
            if (!_curInputs[3] && _lastInputs[3])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(7);
                if (apply)
                {
                    _downInput = 0;
                }
            }
            if (_curInputs[4] && !_lastInputs[4])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(8);
                if (apply)
                {
                    _jumpInput = true;
                }
            }
            if (!_curInputs[4] && _lastInputs[4])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(9);
                if (apply)
                {
                    _jumpInput = false;
                }
            }
            if (_curInputs[5] && !_lastInputs[5])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(10);
                if (apply)
                {
                    _assistInput = true;
                }
            }
            if (!_curInputs[5] && _lastInputs[5])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(11);
                if (apply)
                {
                    _assistInput = false;
                }
            }
            if (_curInputs[6] && !_lastInputs[6])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(12);
                if (apply)
                {
                    _skillInputs[0] = true;
                }
            }
            if (!_curInputs[6] && _lastInputs[6])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(13);
                if (apply)
                {
                    _skillInputs[0] = false;
                }
            }
            if (_curInputs[7] && !_lastInputs[7])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(14);
                if (apply)
                {
                    _skillInputs[1] = true;
                }
            }
            if (!_curInputs[7] && _lastInputs[7])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(15);
                if (apply)
                {
                    _skillInputs[1] = false;
                }
            }
            if (_curInputs[8] && !_lastInputs[8])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(16);
                if (apply)
                {
                    _skillInputs[2] = true;
                }
            }
            if (!_curInputs[8] && _lastInputs[8])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(17);
                if (apply)
                {
                    _skillInputs[2] = false;
                }
            }
        }

        private void SyncInputDatas()
        {
            while (_index < _inputDatas.Count && _inputDatas[_index] == _curTime)
            {
                _index++;
                if (_inputDatas[_index] == 0)
                {
                    _leftInput = 1;
                }
                if (_inputDatas[_index] == 1)
                {
                    _leftInput = 0;
                }
                if (_inputDatas[_index] == 2)
                {
                    _rightInput = 1;
                }
                if (_inputDatas[_index] == 3)
                {
                    _rightInput = 0;
                }
                if (_inputDatas[_index] == 4)
                {
                    _upInput = 1;
                }
                if (_inputDatas[_index] == 5)
                {
                    _upInput = 0;
                }
                if (_inputDatas[_index] == 6)
                {
                    _downInput = 1;
                }
                if (_inputDatas[_index] == 7)
                {
                    _downInput = 0;
                }
                if (_inputDatas[_index] == 8)
                {
                    _jumpInput = true;
                }
                if (_inputDatas[_index] == 9)
                {
                    _jumpInput = false;
                }
                if (_inputDatas[_index] == 10)
                {
                    _assistInput = true;
                }
                if (_inputDatas[_index] == 11)
                {
                    _assistInput = false;
                }
                if (_inputDatas[_index] == 12)
                {
                    _skillInputs[0] = true;
                }
                if (_inputDatas[_index] == 13)
                {
                    _skillInputs[0] = false;
                }
                if (_inputDatas[_index] == 14)
                {
                    _skillInputs[1] = true;
                }
                if (_inputDatas[_index] == 15)
                {
                    _skillInputs[1] = false;
                }
                if (_inputDatas[_index] == 16)
                {
                    _skillInputs[2] = true;
                }
                if (_inputDatas[_index] == 17)
                {
                    _skillInputs[2] = false;
                }
                _index++;
            }
        }

        protected virtual void CheckJump()
        {
            _climbJump = false;
            if (_jumpInput)
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
                else if (!_lastJumpInput && _jumpLevel == 0 && IsCharacterAbilityAvailable(ECharacterAbility.DoubleJump))
                {
                    _jumpLevel = 1;
                    _player.ExtraSpeed.y = 0;
                    _player.SpeedY = 150;
                    _jumpState = EJumpState.Jump2;
                    _jumpTimer = 15;
                    _jumpInput = false;
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
            if (_leftInput == 1)
            {
                eShootDir = EShootDirectionType.Left;
                if (_downInput == 1)
                {
                    eShootDir = EShootDirectionType.LeftDown;
                }
                else if (_upInput == 1)
                {
                    eShootDir = EShootDirectionType.LeftUp;
                }
            }
            else if (_rightInput == 1)
            {
                eShootDir = EShootDirectionType.Right;
                if (_downInput == 1)
                {
                    eShootDir = EShootDirectionType.RightDown;
                }
                else if (_upInput == 1)
                {
                    eShootDir = EShootDirectionType.RightUp;
                }
            }
            else if (_downInput == 1)
            {
                eShootDir = EShootDirectionType.Down;
            }
            else if (_upInput == 1)
            {
                eShootDir = EShootDirectionType.Up;
            }
            _player.ShootAngle = (int)eShootDir;
            if (IsCharacterAbilityAvailable(ECharacterAbility.Shoot))
            {
                if (_skillInputs[0])
                {
                    _player.SkillCtrl.Fire(0);
                }
                if (SkillInputDown(1))
                {
                    _player.SkillCtrl.Fire(1);
                }
                if (SkillInputDown(2))
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
            return GameProcessManager.Instance.IsCharacterAbilityAvailable(eCharacterAbility);
        }

        public void UpdateRenderer()
        {
            _lastHorizontal = _curHorizontal;
            _lastVertical = _curVertical;

            _lastJumpInput = _jumpInput;
            _lastAssistInput = _assistInput;
            for (int i = 0; i < _skillInputs.Length; i++)
            {
                _lastSkillInputs[i] = _skillInputs[i];
            }

            if (_player.IsMain)
            {
                _curHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
                _curVertical = CrossPlatformInputManager.GetAxis("Vertical");
#if IPHONE || ANDROID
#else
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    for (int i = 0; i < _curInputs.Length; i++)
                    {
                        _curInputs[i] = false;
                    }
                    return;
                }
#endif

                if (KeyDown(EInputType.Left))
                {
                    _curInputs[(int)EInputType.Left] = true;
                }
                if (KeyUp(EInputType.Left))
                {
                    _curInputs[(int)EInputType.Left] = false;
                }
                if (KeyDown(EInputType.Right))
                {
                    _curInputs[(int)EInputType.Right] = true;
                }
                if (KeyUp(EInputType.Right))
                {
                    _curInputs[(int)EInputType.Right] = false;
                }
                if (KeyDown(EInputType.Up))
                {
                    _curInputs[(int)EInputType.Up] = true;
                }
                if (KeyUp(EInputType.Up))
                {
                    _curInputs[(int)EInputType.Up] = false;
                }
                if (KeyDown(EInputType.Down))
                {
                    _curInputs[(int)EInputType.Down] = true;
                }
                if (KeyUp(EInputType.Down))
                {
                    _curInputs[(int)EInputType.Down] = false;
                }
                if (KeyDown(EInputType.Jump))
                {
                    _curInputs[(int)EInputType.Jump] = true;
                }
                if (KeyUp(EInputType.Jump))
                {
                    _curInputs[(int)EInputType.Jump] = false;
                }
                if (KeyDown(EInputType.Assist))
                {
                    _curInputs[(int)EInputType.Assist] = true;
                }
                if (KeyUp(EInputType.Assist))
                {
                    _curInputs[(int)EInputType.Assist] = false;
                }
                if (KeyDown(EInputType.Skill1))
                {
                    _curInputs[(int)EInputType.Skill1] = true;
                }
                if (KeyUp(EInputType.Skill1))
                {
                    _curInputs[(int)EInputType.Skill1] = false;
                }
                if (KeyDown(EInputType.Skill2))
                {
                    _curInputs[(int)EInputType.Skill2] = true;
                }
                if (KeyUp(EInputType.Skill2))
                {
                    _curInputs[(int)EInputType.Skill2] = false;
                }
                if (KeyDown(EInputType.Skill3))
                {
                    _curInputs[(int)EInputType.Skill3] = true;
                }
                if (KeyUp(EInputType.Skill3))
                {
                    _curInputs[(int)EInputType.Skill3] = false;
                }
            }
        }

        protected bool KeyDown(EInputType eInputType)
        {
            switch (eInputType)
            {
                case EInputType.Left:
                    return _lastHorizontal > -0.1f && _curHorizontal < -0.1f;
                case EInputType.Right:
                    return _lastHorizontal < 0.1f && _curHorizontal > 0.1f;
                case EInputType.Down:
                    return _lastVertical > -0.1f && _curVertical < -0.1f;
                case EInputType.Up:
                    return _lastVertical < 0.1f && _curVertical > 0.1f;
                case EInputType.Jump:
                    return CrossPlatformInputManager.GetButtonDown("Jump");
                case EInputType.Assist:
                    return CrossPlatformInputManager.GetButtonDown("Assist");
                case EInputType.Skill1:
                    return CrossPlatformInputManager.GetButtonDown("Fire1");
                case EInputType.Skill2:
                    return CrossPlatformInputManager.GetButtonDown("Fire2");
                case EInputType.Skill3:
                    return CrossPlatformInputManager.GetButtonDown("Fire3");
            }
            return false;
        }

        protected bool KeyUp(EInputType eInputType)
        {
            switch (eInputType)
            {
                case EInputType.Left:
                    return _lastHorizontal < -0.1f && _curHorizontal > -0.1f;
                case EInputType.Right:
                    return _lastHorizontal > 0.1f && _curHorizontal < 0.1f;
                case EInputType.Down:
                    return _lastVertical < -0.1f && _curVertical > -0.1f;
                case EInputType.Up:
                    return _lastVertical > 0.1f && _curVertical < 0.1f;
                case EInputType.Jump:
                    if (GuideManager.Instance.IsLockJumpButton)
                    {
                        return false;
                    }
                    if (GuideManager.Instance.IsUnlockedJumpButton)
                    {
                        return true;
                    }
                    return CrossPlatformInputManager.GetButtonUp("Jump");
                case EInputType.Assist:
                    return CrossPlatformInputManager.GetButtonUp("Assist");
                case EInputType.Skill1:
                    return CrossPlatformInputManager.GetButtonUp("Fire1");
                case EInputType.Skill2:
                    return CrossPlatformInputManager.GetButtonUp("Fire2");
                case EInputType.Skill3:
                    return CrossPlatformInputManager.GetButtonUp("Fire3");
            }
            return false;
        }

    }

    public enum ELittleSkillState
    {
        Quicken,
        HoldBox
    }
}
