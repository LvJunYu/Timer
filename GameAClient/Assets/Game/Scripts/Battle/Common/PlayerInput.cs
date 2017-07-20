/********************************************************************
** Filename : PlayerInput
** Author : Dong
** Date : 2016/10/20 星期四 下午 10:17:57
** Summary : PlayerInput
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

namespace GameA.Game
{
    public enum EInputType
    {
        Left,
        Right,
        Up,
        Down,
        Jump,
        Quicken,
        Skill1,
        Skill2,
        Max
    }

    [Serializable]
    public class PlayerInput
    {
        protected PlayerBase _player;

        [SerializeField]
        protected ERunMode _runMode;
        private const int JumpFirstMaxTime = 105;
        private const int JumpSecondMaxTime = 205;
        private const int QuickenMaxTime = 3*ConstDefineGM2D.FixedFrameCount;
        private const int QuickenCDTime = 20*ConstDefineGM2D.FixedFrameCount;

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
        protected bool _quickenInput;
        [SerializeField]
        protected bool _lastQuickenInput;
        public int _quickenTime;
        public int _quickenCDTime;

        [SerializeField]
        protected bool _skill1Input;
        [SerializeField]
        protected bool _lastSkill1Input;
        [SerializeField]
        protected bool _skill2Input;
        [SerializeField]
        protected bool _lastSkill2Input;

        // 跳跃等级
        [SerializeField]
        public int _jumpLevel = 0;
        // 跳跃状态
        [SerializeField]
        public int _jumpState = 0;
        // 停止跳跃标志
        [SerializeField]
        protected bool _stopJump = false;

        [SerializeField]
        public EClimbState _eClimbState;
        // 攀墙跳
        [SerializeField]
        protected bool _climbJump = false;
        protected bool _step;
        protected int _stepY;

        #endregion

        #region Input

        protected const int WallJumpBanInputTime = 20;
        protected const int QuickenTime = 3*ConstDefineGM2D.FixedFrameCount;

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

        public bool Skill1Input
        {
            get { return _skill1Input; }
        }

        public bool Skill2InputDown
        {
            get { return _skill2Input && !_lastSkill2Input; }
        }

        public bool QuickenInputUp
        {
            get { return !_quickenInput && _lastQuickenInput; }
        }

        public bool ClimbJump
        {
            get { return _climbJump; }
        }

        public bool Step
        {
            get { return _step; }
            set
            {
                if (value)
                {
                    ResetJumpState();
                }
                _step = value;
            }
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

        public int JumpState
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

        public void ResetJumpState()
        {
            _jumpLevel = 0;
            _jumpState = 0;
        }

        public void Clear()
        {
            ClearInput();
            UpdateQuickenCDTime(0);
        }

        public void ClearInput()
        {
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
            _jumpState = 0;
            _jumpLevel = 0;
            _stopJump = false;
            _eClimbState = EClimbState.None;
            _climbJump = false;
            _step = false;
            _stepY = 0;
            _quickenInput = false;
            _lastQuickenInput = false;
            _skill1Input = false;
            _lastSkill1Input = false;
            _skill2Input = false;
            _lastSkill2Input = false;
            for (int i = 0; i < _curInputs.Length; i++)
            {
                _curInputs[i] = false;
                _lastInputs[i] = false;
            }
        }

        private void UpdateQuickenCDTime(int value)
        {
            if (_quickenCDTime == value)
            {
                return;
            }
            _quickenCDTime = value;
            Messenger<int, int>.Broadcast(EMessengerType.OnSpeedUpCDChanged, _quickenCDTime, QuickenCDTime);
        }

        public void UpdateLogic()
        {
            if (!PlayMode.Instance.SceneState.GameRunning)
            {
                return;
            }
            _curTime++;
            _totalTime = _curTime;
            if (_quickenCDTime > 0)
            {
                UpdateQuickenCDTime(_quickenCDTime - 1);
            }
            if (_quickenTime > 0)
            {
                _quickenTime--;
            }
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
            CheckQuicken();
            CheckSkill();
            _step = false;
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
                    _quickenInput = true;
                }
            }
            if (!_curInputs[5] && _lastInputs[5])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(11);
                if (apply)
                {
                    _quickenInput = false;
                }
            }
            if (_curInputs[6] && !_lastInputs[6])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(12);
                if (apply)
                {
                    _skill1Input = true;
                }
            }
            if (!_curInputs[6] && _lastInputs[6])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(13);
                if (apply)
                {
                    _skill1Input = false;
                }
            }
            if (_curInputs[7] && !_lastInputs[7])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(14);
                if (apply)
                {
                    _skill2Input = true;
                }
            }
            if (!_curInputs[7] && _lastInputs[7])
            {
                _inputDatas.Add(_curTime);
                _inputDatas.Add(15);
                if (apply)
                {
                    _skill2Input = false;
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
                    _quickenInput = true;
                }
                if (_inputDatas[_index] == 11)
                {
                    _quickenInput = false;
                }
                if (_inputDatas[_index] == 12)
                {
                    _skill1Input = true;
                }
                if (_inputDatas[_index] == 13)
                {
                    _skill1Input = false;
                }
                if (_inputDatas[_index] == 14)
                {
                    _skill2Input = true;
                }
                if (_inputDatas[_index] == 15)
                {
                    _skill2Input = false;
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
                    _player.CurBanInputTime = 20;
                    _climbJump = true;
                    _player.SpeedY = 0;
                    _player.ExtraSpeed.y = 0;
                    _jumpState = 100;
                    _jumpLevel = 0;
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
                else if ((_step || _jumpState == 0))
                {
                    if (_step)
                    {
                        _player.ExtraSpeed.y = _stepY;
                    }
                    _player.SpeedY = 0;
                    _jumpLevel = 0;
                    _jumpState = 100;
                }
                else if ((_jumpState > 0 && _jumpState < 200) && !_lastJumpInput && _jumpLevel == 0 && IsCharacterAbilityAvailable(ECharacterAbility.DoubleJump))
                {
                    _player.SpeedY = 0;
                    _player.ExtraSpeed.y = 0;
                    _jumpState = 200;
                    _jumpLevel = 1;
                }
                _stopJump = false;
            }

            if (_player.ExtraSpeed.y < 0)
            {
                _jumpState = 1;
                if (_jumpState > 200)
                {
                    _jumpInput = false;
                }
            }
            if (_jumpState > 200 && _player.SpeedY <= 0)
            {
                _jumpInput = false;
                _stopJump = true;
            }
            if (_jumpState >= 200 && !_jumpInput)
            {
                _stopJump = true;
            }
            if (_jumpState >= 202)
            {
                _jumpState++;
                _player.SpeedY += 10;
            }
            else if (_jumpState >= 200)
            {
                _jumpState++;
                _player.SpeedY += 70;
            }
            else if (_jumpState >= 102)
            {
                _jumpState++;
                _player.SpeedY += 10;
            }
            else if (_jumpState >= 100)
            {
                _jumpState++;
                _player.SpeedY += _player.OnClay ? 50 : 70;
            }
            if ((_jumpState > JumpFirstMaxTime && _jumpState < 200) || _jumpState > JumpSecondMaxTime)
            {
                _jumpState = 1;
                _jumpInput = false;
            }
            if ((_jumpState >= 102 || _jumpState >= 202) && _stopJump)
            {
                _jumpState = 1;
            }
        }

        protected void CheckQuicken()
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
                        if (_quickenCDTime > 0)
                        {
                            return;
                        }
                        if (_quickenTime > 0)
                        {
                            return;
                        }
                        if (QuickenInputUp && IsCharacterAbilityAvailable(ECharacterAbility.SpeedUp))
                        {
                            _quickenTime = QuickenMaxTime;
                            UpdateQuickenCDTime(QuickenCDTime);
                        }
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

            if (_skill1Input && IsCharacterAbilityAvailable(ECharacterAbility.Shoot))
            {
                _player.SkillCtrl.Fire(0);
            }
            if (Skill2InputDown && IsCharacterAbilityAvailable(ECharacterAbility.Shoot))
            {
                _player.SkillCtrl.Fire(1);
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
            _lastQuickenInput = _quickenInput;
            _lastSkill1Input = _skill1Input;
            _lastSkill2Input = _skill2Input;

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
                if (KeyDown(EInputType.Quicken))
                {
                    _curInputs[(int)EInputType.Quicken] = true;
                }
                if (KeyUp(EInputType.Quicken))
                {
                    _curInputs[(int)EInputType.Quicken] = false;
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
                case EInputType.Quicken:
                    return CrossPlatformInputManager.GetButtonDown("Fire3");
                case EInputType.Skill1:
                    return CrossPlatformInputManager.GetButtonDown("Fire1");
                case EInputType.Skill2:
                    return CrossPlatformInputManager.GetButtonDown("Fire2");
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
                case EInputType.Quicken:
                    return CrossPlatformInputManager.GetButtonUp("Fire3");
                case EInputType.Skill1:
                    return CrossPlatformInputManager.GetButtonUp("Fire1");
                case EInputType.Skill2:
                    return CrossPlatformInputManager.GetButtonUp("Fire2");
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
