/********************************************************************
** Filename : MainInput
** Author : Dong
** Date : 2016/10/20 星期四 下午 10:17:57
** Summary : MainInput
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace GameA.Game
{
    public enum InputType
    {
        Left,
        Right,
        Jump,
        Shoot,
        Quicken,
        Max
    }

    [Serializable]
    public class MainInput
    {
        protected MainUnit _unit;

        [SerializeField]
        protected ERunMode _runMode;
        private const int JumpFirstMaxTime = 105;
        private const int JumpSecondMaxTime = 205;
        private const int QuickenMaxTime = 5*ConstDefineGM2D.FixedFrameCount;

        #region state

        [SerializeField]
        protected EFire2State _fire2State;

        [SerializeField]
        protected bool[] _curInputs = new bool[(int)InputType.Max];
        [SerializeField]
        protected bool[] _lastInputs = new bool[(int)InputType.Max];

        [SerializeField]
        protected float _lastHorizontal;
        [SerializeField]
        protected float _curHorizontal;

        [SerializeField]
        protected int _leftInput;
        [SerializeField]
        protected int _rightInput;

        [SerializeField]
        public int _brakeTime;
        public int _brakeType;

        [SerializeField]
        protected bool _jumpInput;
        protected bool _lastJumpInput;

        [SerializeField]
        protected bool _shootInput;
        [SerializeField]
        protected bool _lastShootInput;

        [SerializeField]
        protected bool _quickenInput;
        [SerializeField]
        protected bool _lastQuickenInput;
        public int _quickenTime;

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

        public bool ShootInput
        {
            get { return _shootInput; }
        }

        public bool ShootInputDown
        {
            get { return _shootInput && !_lastShootInput; }
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
            set { _step = value; }
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

        public MainInput(MainUnit mainUnit)
        {
            _unit = mainUnit;
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

        public void Clear()
        {
            _lastJumpInput = false;
            _lastHorizontal = 0;
            _curHorizontal = 0;
            _leftInput = 0;
            _rightInput = 0;
            _brakeTime = 0;
            _brakeType = 0;
            _jumpInput = false;
            _jumpState = 1;
            _stopJump = false;
            _eClimbState = EClimbState.None;
            _climbJump = false;
            _step = false;
            _stepY = 0;
            _shootInput = false;
            _lastShootInput = false;
            _quickenInput = false;
            _lastQuickenInput = false;
            _quickenTime = 0;
            for (int i = 0; i < _curInputs.Length; i++)
            {
                _curInputs[i] = false;
                _lastInputs[i] = false;
            }
        }

        public void UpdateRenderer()
        {
            _lastHorizontal = _curHorizontal;
            _curHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            if (KeyDown(InputType.Left))
            {
                _curInputs[(int)InputType.Left] = true;
            }
            if (KeyUp(InputType.Left))
            {
                _curInputs[(int)InputType.Left] = false;
            }
            if (KeyDown(InputType.Right))
            {
                _curInputs[(int)InputType.Right] = true;
            }
            if (KeyUp(InputType.Right))
            {
                _curInputs[(int)InputType.Right] = false;
            }
            if (KeyDown(InputType.Jump))
            {
                _curInputs[(int)InputType.Jump] = true;
            }
            if (KeyUp(InputType.Jump))
            {
                _curInputs[(int)InputType.Jump] = false;
            }
            if (KeyDown(InputType.Shoot))
            {
                _curInputs[(int)InputType.Shoot] = true;
            }
            if (KeyUp(InputType.Shoot))
            {
                _curInputs[(int)InputType.Shoot] = false;
            }
            if (KeyDown(InputType.Quicken))
            {
                _curInputs[(int)InputType.Quicken] = true;
            }
            if (KeyUp(InputType.Quicken))
            {
                _curInputs[(int)InputType.Quicken] = false;
            }
        }

        protected bool KeyDown(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Left:
                    return _lastHorizontal > -0.1f && _curHorizontal < -0.1f;
                case InputType.Right:
                    return _lastHorizontal < 0.1f && _curHorizontal > 0.1f;
                case InputType.Jump:
                    return CrossPlatformInputManager.GetButtonDown("Jump");
                case InputType.Shoot:
                    return CrossPlatformInputManager.GetButtonDown("Fire1");
                case InputType.Quicken:
                    return CrossPlatformInputManager.GetButtonDown("Fire2");
            }
            return false;
        }

        protected bool KeyUp(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Left:
                    return _lastHorizontal < -0.1f && _curHorizontal > -0.1f;
                case InputType.Right:
                    return _lastHorizontal > 0.1f && _curHorizontal < 0.1f;
                case InputType.Jump:
                    if (GuideManager.Instance.IsLockJumpButton)
                    {
                        return false;
                    }
                    if (GuideManager.Instance.IsUnlockedJumpButton)
                    {
                        return true;
                    }
                    return CrossPlatformInputManager.GetButtonUp("Jump");
                case InputType.Shoot:
                    return CrossPlatformInputManager.GetButtonUp("Fire1");
                case InputType.Quicken:
                    return CrossPlatformInputManager.GetButtonUp("Fire2");
            }
            return false;
        }

        public void UpdateLogic()
        {
            if (!PlayMode.Instance.SceneState.GameRunning)
            {
                return;
            }
            _curTime++;
            _totalTime = _curTime;
            if (_quickenTime > 0)
            {
                _quickenTime--;
            }
            CheckInput();
            if (_unit.IsAlive && _unit.CurBanInputTime == 0)
            {
                if (_leftInput > 0)
                {
                    if (_unit.CurMoveDirection != EMoveDirection.Left)
                    {
                        _unit.SetFacingDir(EMoveDirection.Left);
                        PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Left);
                    }
                }
                else if (_rightInput > 0)
                {
                    if (_unit.CurMoveDirection != EMoveDirection.Right)
                    {
                        _unit.SetFacingDir(EMoveDirection.Right);
                        PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Right);
                    }
                }
            }
            CheckJump();
            CheckFire();
            CheckFire2();
            _step = false;
        }

        protected void CheckInput()
        {
            _lastJumpInput = _jumpInput;
            _lastShootInput = _shootInput;
            _lastQuickenInput = _quickenInput;
            if (_runMode == ERunMode.Normal)
            {
                if (_curInputs[0] && !_lastInputs[0])
                {
                    _inputDatas.Add(_curTime);
                    _inputDatas.Add(0);
                    _leftInput = 1;
                }
                if (!_curInputs[0] && _lastInputs[0])
                {
                    _inputDatas.Add(_curTime);
                    _inputDatas.Add(1);
                    _leftInput = 0;
                    _brakeTime = 0;
                }
                if (_curInputs[1] && !_lastInputs[1])
                {
                    _inputDatas.Add(_curTime);
                    _inputDatas.Add(2);
                    _rightInput = 1;
                }
                if (!_curInputs[1] && _lastInputs[1])
                {
                    _inputDatas.Add(_curTime);
                    _inputDatas.Add(3);
                    _rightInput = 0;
                    _brakeTime = 0;
                }
                if (_curInputs[2] && !_lastInputs[2])
                {
                    _inputDatas.Add(_curTime);
                    _inputDatas.Add(4);
                    _jumpInput = true;
                }
                if (!_curInputs[2] && _lastInputs[2])
                {
                    _inputDatas.Add(_curTime);
                    _inputDatas.Add(5);
                    _jumpInput = false;
                }
                if (_curInputs[3] && !_lastInputs[3])
                {
                    _inputDatas.Add(_curTime);
                    _inputDatas.Add(6);
                    _shootInput = true;
                }
                if (!_curInputs[3] && _lastInputs[3])
                {
                    _inputDatas.Add(_curTime);
                    _inputDatas.Add(7);
                    _shootInput = false;
                }
                if (_curInputs[4] && !_lastInputs[4])
                {
                    _inputDatas.Add(_curTime);
                    _inputDatas.Add(8);
                    _quickenInput = true;
                }
                if (!_curInputs[4] && _lastInputs[4])
                {
                    _inputDatas.Add(_curTime);
                    _inputDatas.Add(9);
                    _quickenInput = false;
                }
            }
            else if (_runMode == ERunMode.Record)
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
                        _brakeTime = 0;
                    }
                    if (_inputDatas[_index] == 2)
                    {
                        _rightInput = 1;
                    }
                    if (_inputDatas[_index] == 3)
                    {
                        _rightInput = 0;
                        _brakeTime = 0;
                    }
                    if (_inputDatas[_index] == 4)
                    {
                        _jumpInput = true;
                    }
                    if (_inputDatas[_index] == 5)
                    {
                        _jumpInput = false;
                    }
                    if (_inputDatas[_index] == 6)
                    {
                        _shootInput = true;
                    }
                    if (_inputDatas[_index] == 7)
                    {
                        _shootInput = false;
                    }
                    if (_inputDatas[_index] == 8)
                    {
                        _quickenInput = true;
                    }
                    if (_inputDatas[_index] == 9)
                    {
                        _quickenInput = false;
                    }
                    _index++;
                }
            }
            for (int i = 0; i < _curInputs.Length; i++)
            {
                _lastInputs[i] = _curInputs[i];
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
                    _unit.SpeedY = 0;
                    _unit.ExtraSpeed.y = 0;
                    _jumpState = 100;
                    _jumpLevel = 0;
                    if (_eClimbState == EClimbState.Left)
                    {
                        _unit.SpeedX = 80;
                        if (_unit.CurMoveDirection != EMoveDirection.Right)
                        {
                            _unit.SetFacingDir(EMoveDirection.Right);
                            PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Right);
                        }
                    }
                    else if (_eClimbState == EClimbState.Right)
                    {
                        _unit.SpeedX = -80;
                        if (_unit.CurMoveDirection != EMoveDirection.Left)
                        {
                            _unit.SetFacingDir(EMoveDirection.Left);
                            PlayMode.Instance.CurrentShadow.RecordDirChange(EMoveDirection.Left);
                        }
                    }
                    //_unit.CurBanInputTime = 20;
                }
                else if ((_step || _jumpState == 0))
                {
                    if (_step)
                    {
                        _unit.SpeedY = 0;
                        _unit.ExtraSpeed.y = _stepY;
                        _jumpLevel = 0;
                    }
                    _jumpState = 100;
                }
                else if ((_jumpState > 0 && _jumpState < 200) && !_lastJumpInput && _jumpLevel == 0)
                {
                    _unit.SpeedY = 0;
                    _unit.ExtraSpeed.y = 0;
                    _jumpState = 200;
                    _jumpLevel = 1;
                }
                _stopJump = false;
            }
            if (_unit.ExtraSpeed.y < 0)
            {
                _jumpState = 1;
                if (_jumpState > 200)
                {
                    _jumpInput = false;
                }
            }
            if (_jumpState > 200 && _unit.SpeedY <= 0)
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
                _unit.SpeedY += 10;
            }
            else if (_jumpState >= 200)
            {
                _jumpState++;
                _unit.SpeedY += 70;
            }
            else if (_jumpState >= 102)
            {
                _jumpState++;
                _unit.SpeedY += 10;
            }
            else if (_jumpState >= 100)
            {
                _jumpState++;
                _unit.SpeedY += 70;
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

        protected void CheckFire()
        {
            if (ShootInputDown)
            {
                SkillManager.Instance.Fire();
            }
        }

        protected void CheckFire2()
        {
            switch (_fire2State)
            {
                    case EFire2State.HoldBox:
                {
                    if (QuickenInputUp)
                    {
                        _unit.OnBoxHoldingChanged();
                    }
                }
                    break;
                    case EFire2State.Quicken:
                {
                    if (_quickenTime > 0)
                    {
                        return;
                    }
                    if (QuickenInputUp)
                    {
                        _quickenTime = QuickenMaxTime;
                    }
                }
                    break;
            }
        }

        public void ChangeFire2State(EFire2State eFire2State)
        {
            _fire2State = eFire2State;
        }
    }

    public enum EFire2State
    {
        Quicken,
        HoldBox
    }
}
