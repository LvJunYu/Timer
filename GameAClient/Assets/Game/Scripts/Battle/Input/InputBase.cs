using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameA.Game
{
    //输入类型 下标为v 按下为2v 抬起为2v+1
    public enum EInputType
    {
        Left,
        Right,
        Up,
        Down,
        Jump,
        Assist,
        Skill1,
        Skill2,
        Skill3,
        Max
    }
    
    public class InputBase
    {
        [SerializeField]
        protected ELittleSkillState _littleSkillState;

        // 跳跃等级
        [SerializeField]
        protected int _jumpLevel = 0;
        // 跳跃状态
        [SerializeField] protected EJumpState _jumpState;

        [SerializeField]
        protected EClimbState _eClimbState;
        // 攀墙跳
        [SerializeField]
        protected bool _climbJump = false;
        protected int _stepY;
        /// <summary>
        /// 起跳的动画时间
        /// </summary>
        protected int _jumpTimer;

        [SerializeField]
        protected bool[] _curAppliedInputKeyAry = new bool[(int)EInputType.Max];
        protected bool[] _lastAppliedInputKeyAry = new bool[(int)EInputType.Max];

        public bool LeftInput
        {
            get { return GetKeyApplied(EInputType.Left); }
        }

        public bool RightInput
        {
            get { return GetKeyApplied(EInputType.Right); }
        }

        public bool JumpInput
        {
            get { return GetKeyDownApplied(EInputType.Jump); }
        }

        public bool QuickenInputUp
        {
            get { return GetKeyUpApplied(EInputType.Assist); }
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

        public EJumpState JumpState
        {
            get { return _jumpState; }
        }

        public int JumpLevel
        {
            get { return _jumpLevel; }
        }

        public EClimbState EClimbState
        {
            get { return _eClimbState; }
            set { _eClimbState = value; }
        }

        public void OnLand()
        {
            _jumpLevel = -1;
            _jumpTimer = 0;
            _jumpState = EJumpState.Land;
        }

        public virtual void Clear()
        {
            ClearInput();
        }

        public virtual void ClearInput()
        {
            _jumpTimer = 0;
            _jumpState = EJumpState.Land;
            _jumpLevel = -1;
            _eClimbState = EClimbState.None;
            _climbJump = false;
            _stepY = 0;
            for (int i = 0; i < _curAppliedInputKeyAry.Length; i++)
            {
                _curAppliedInputKeyAry[i] = false;
                _lastAppliedInputKeyAry[i] = false;
            }
        }

        /// <summary>
        /// 应用按键改变码 按下=EInputType.Value*2 抬起=EInputType.Value*2+1
        /// </summary>
        /// <param name="changeCode"></param>
        public void ApplyKeyChangeCode(int changeCode)
        {
            _curAppliedInputKeyAry[changeCode>>1] = ((changeCode & 1) == 0);
        }

        /// <summary>
        /// 当前帧抬起按键
        /// </summary>
        /// <param name="eInputType"></param>
        public void ApplyKeyUp(EInputType eInputType)
        {
            _curAppliedInputKeyAry[(int) eInputType] = false;
        }

        /// <summary>
        /// 当前帧按下按键
        /// </summary>
        /// <param name="eInputType"></param>
        public void ApplyKeyDown(EInputType eInputType)
        {
            _curAppliedInputKeyAry[(int) eInputType] = true;
        }
        
        /// <summary>
        /// 应用这一帧所有指令
        /// </summary>
        /// <param name="inputDataList"></param>
        public void ApplyInputData(List<int> inputDataList)
        {
            PrepareForApplyInput();
            if (inputDataList == null)
            {
                return;
            }
            for (int i = 0; i < inputDataList.Count; i++)
            {
                ApplyKeyChangeCode(inputDataList[i]);
            }
        }

        public void PrepareForApplyInput()
        {
            Array.Copy(_curAppliedInputKeyAry, _lastAppliedInputKeyAry, _curAppliedInputKeyAry.Length);
        }

        public bool GetKeyApplied(EInputType eInputType)
        {
            return _curAppliedInputKeyAry[(int)eInputType];
        }

        public bool GetKeyLastApplied(EInputType eInputType)
        {
            return _lastAppliedInputKeyAry[(int)eInputType];
        }

        public bool GetKeyDownApplied(EInputType eInputType)
        {
            int inx = (int) eInputType;
            return _curAppliedInputKeyAry[inx] && !_lastAppliedInputKeyAry[inx];
        }
        
        public bool GetKeyUpApplied(EInputType eInputType)
        {
            int inx = (int) eInputType;
            return !_curAppliedInputKeyAry[inx] && _lastAppliedInputKeyAry[inx];
        }
        
        public bool SkillInputDown(int i)
        {
            return GetKeyDownApplied((EInputType) ((int)EInputType.Skill1 + i));
        }
    }
}