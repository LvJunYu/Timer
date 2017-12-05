using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA.Game
{
    public class LocalPlayerInput : InputBase
    {
        #region 检查输入
        
        [SerializeField]
        protected bool[] _curCheckInputKeyAry = new bool[(int)EInputType.Max];
        [SerializeField]
        protected bool[] _lastCheckInputKeyAry = new bool[(int)EInputType.Max];
        
        [SerializeField]
        protected float _lastCheckHorizontal;
        [SerializeField]
        protected float _curCheckHorizontal;
        [SerializeField]
        protected float _lastCheckVertical;
        [SerializeField]
        protected float _curCheckVertical;
        #endregion
        
        //检查输入最终的结果
        protected List<int> _curCheckInputChangeList = new List<int>((int)EInputType.Max);

        public List<int> CurCheckInputChangeList
        {
            get { return _curCheckInputChangeList; }
        }

        public void ProcessCheckInput()
        {
            PrepareForCheckInput();
            CheckInput();
            GenerateCheckInputChangeList();
        }

        protected void GenerateCheckInputChangeList()
        {
            _curCheckInputChangeList.Clear();
            for (int i = 0, len = (int)EInputType.Max; i < len; i++)
            {
                if (_curCheckInputKeyAry[i]) 
                {
                    if (!_lastCheckInputKeyAry[i])
                    {
                        _curCheckInputChangeList.Add(i<<1); //i*2
                    }
                }
                else
                {
                    if (_lastCheckInputKeyAry[i])
                    {
                        _curCheckInputChangeList.Add(i<<1 | 1); //i*2+1
                    }
                }
            }
        }

        protected void PrepareForCheckInput()
        {
            _lastCheckHorizontal = _curCheckHorizontal;
            _lastCheckVertical = _curCheckVertical;
            Array.Copy(_curCheckInputKeyAry, _lastCheckInputKeyAry, _curCheckInputKeyAry.Length);
        }
        
        public override void ClearInput()
        {
            base.ClearInput();
            _lastCheckHorizontal = 0;
            _curCheckHorizontal = 0;
            _lastCheckVertical = 0;
            _curCheckVertical = 0;
            _curCheckInputChangeList.Clear();
            for (int i = 0; i < _curCheckInputKeyAry.Length; i++)
            {
                _curCheckInputKeyAry[i] = false;
                _lastCheckInputKeyAry[i] = false;
            }
        }
        
        public void CheckInput()
        {
            _curCheckHorizontal = CrossPlatformInputManager.GetAxis(InputManager.TagHorizontal);
            _curCheckVertical = CrossPlatformInputManager.GetAxis(InputManager.TagVertical);
#if IPHONE || ANDROID
#else
#endif

            if (GetKeyDownCheck(EInputType.Left))
            {
                _curCheckInputKeyAry[(int)EInputType.Left] = true;
            }
            if (GetKeyUpCheck(EInputType.Left))
            {
                _curCheckInputKeyAry[(int)EInputType.Left] = false;
            }
            if (GetKeyDownCheck(EInputType.Right))
            {
                _curCheckInputKeyAry[(int)EInputType.Right] = true;
            }
            if (GetKeyUpCheck(EInputType.Right))
            {
                _curCheckInputKeyAry[(int)EInputType.Right] = false;
            }
            if (GetKeyDownCheck(EInputType.Up))
            {
                _curCheckInputKeyAry[(int)EInputType.Up] = true;
            }
            if (GetKeyUpCheck(EInputType.Up))
            {
                _curCheckInputKeyAry[(int)EInputType.Up] = false;
            }
            if (GetKeyDownCheck(EInputType.Down))
            {
                _curCheckInputKeyAry[(int)EInputType.Down] = true;
            }
            if (GetKeyUpCheck(EInputType.Down))
            {
                _curCheckInputKeyAry[(int)EInputType.Down] = false;
            }
            if (GetKeyDownCheck(EInputType.Jump))
            {
                _curCheckInputKeyAry[(int)EInputType.Jump] = true;
            }
            if (GetKeyUpCheck(EInputType.Jump))
            {
                _curCheckInputKeyAry[(int)EInputType.Jump] = false;
            }
            if (GetKeyDownCheck(EInputType.Assist))
            {
                _curCheckInputKeyAry[(int)EInputType.Assist] = true;
            }
            if (GetKeyUpCheck(EInputType.Assist))
            {
                _curCheckInputKeyAry[(int)EInputType.Assist] = false;
            }
            if (GetKeyDownCheck(EInputType.Skill1))
            {
                _curCheckInputKeyAry[(int)EInputType.Skill1] = true;
            }
            if (GetKeyUpCheck(EInputType.Skill1))
            {
                _curCheckInputKeyAry[(int)EInputType.Skill1] = false;
            }
            if (GetKeyDownCheck(EInputType.Skill2))
            {
                _curCheckInputKeyAry[(int)EInputType.Skill2] = true;
            }
            if (GetKeyUpCheck(EInputType.Skill2))
            {
                _curCheckInputKeyAry[(int)EInputType.Skill2] = false;
            }
            if (GetKeyDownCheck(EInputType.Skill3))
            {
                _curCheckInputKeyAry[(int)EInputType.Skill3] = true;
            }
            if (GetKeyUpCheck(EInputType.Skill3))
            {
                _curCheckInputKeyAry[(int)EInputType.Skill3] = false;
            }
        }

        protected bool GetKeyDownCheck(EInputType eInputType)
        {
            switch (eInputType)
            {
                case EInputType.Left:
                    return _lastCheckHorizontal > -0.1f && _curCheckHorizontal < -0.1f;
                case EInputType.Right:
                    return _lastCheckHorizontal < 0.1f && _curCheckHorizontal > 0.1f;
                case EInputType.Down:
                    return _lastCheckVertical > -0.1f && _curCheckVertical < -0.1f;
                case EInputType.Up:
                    return _lastCheckVertical < 0.1f && _curCheckVertical > 0.1f;
                case EInputType.Jump:
                    return CrossPlatformInputManager.GetButton(InputManager.TagJump);
                case EInputType.Assist:
                    return CrossPlatformInputManager.GetButton(InputManager.TagAssist);
                case EInputType.Skill1:
                    return CrossPlatformInputManager.GetButton(InputManager.TagSkill1);
                case EInputType.Skill2:
                    return CrossPlatformInputManager.GetButton(InputManager.TagSkill2);
                case EInputType.Skill3:
                    return CrossPlatformInputManager.GetButton(InputManager.TagSkill3);
            }
            return false;
        }

        protected bool GetKeyUpCheck(EInputType eInputType)
        {
            switch (eInputType)
            {
                case EInputType.Left:
                    return _lastCheckHorizontal < -0.1f && _curCheckHorizontal > -0.1f;
                case EInputType.Right:
                    return _lastCheckHorizontal > 0.1f && _curCheckHorizontal < 0.1f;
                case EInputType.Down:
                    return _lastCheckVertical < -0.1f && _curCheckVertical > -0.1f;
                case EInputType.Up:
                    return _lastCheckVertical > 0.1f && _curCheckVertical < 0.1f;
                case EInputType.Jump:
                    if (GuideManager.Instance.IsLockJumpButton)
                    {
                        return false;
                    }
                    if (GuideManager.Instance.IsUnlockedJumpButton)
                    {
                        return true;
                    }
                    return !CrossPlatformInputManager.GetButton(InputManager.TagJump);
                case EInputType.Assist:
                    return !CrossPlatformInputManager.GetButton(InputManager.TagAssist);
                case EInputType.Skill1:
                    return !CrossPlatformInputManager.GetButton(InputManager.TagSkill1);
                case EInputType.Skill2:
                    return !CrossPlatformInputManager.GetButton(InputManager.TagSkill2);
                case EInputType.Skill3:
                    return !CrossPlatformInputManager.GetButton(InputManager.TagSkill3);
            }
            return false;
        }
    }
}