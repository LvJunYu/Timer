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
        PasswordDoorOpen,
        Max
    }

    public class InputBase
    {
        [SerializeField] protected bool[] _curAppliedInputKeyAry = new bool[(int) EInputType.Max];
        protected bool[] _lastAppliedInputKeyAry = new bool[(int) EInputType.Max];
        protected bool _inputValid = true;

        public bool[] CurAppliedInputKeyAry
        {
            get { return _curAppliedInputKeyAry; }
        }

        public virtual void Clear()
        {
            ClearInput();
        }

        public virtual void ClearInput()
        {
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
            _curAppliedInputKeyAry[changeCode >> 1] = ((changeCode & 1) == 0);
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
            return _curAppliedInputKeyAry[(int) eInputType];
        }

        public bool GetKeyLastApplied(EInputType eInputType)
        {
            return _lastAppliedInputKeyAry[(int) eInputType];
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
            return GetKeyDownApplied((EInputType) ((int) EInputType.Skill1 + i));
        }

        public void SetInputValid(bool value)
        {
            _inputValid = value;
        }
    }
}