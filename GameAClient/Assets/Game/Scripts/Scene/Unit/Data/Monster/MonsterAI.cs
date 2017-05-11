/********************************************************************
** Filename : MonsterAI
** Author : Dong
** Date : 2017/4/27 星期四 下午 1:51:20
** Summary : MonsterAI
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class MonsterAI
    {
        public enum EActionState
        {
            None = 0,
            MoveTo,
        }

        public const int MaxStuckFrames = 20;
        protected MonsterBase _unit;
        public EActionState _currentActionState;
        public int _currentNodeId = -1;
        public IntVec2 _destination;

        public int _framesOfJumping = 0;

        public IntVec2 _oldPosition;

        public bool _onGround = false;

        protected int _stuckFrames = 0;

        public List<IntVec2> _path = new List<IntVec2>();

        [SerializeField]
        protected int _leftInput;
        protected int _lastLeftInput;
        [SerializeField]
        protected int _rightInput;
        protected int _lastRightInput;

        protected bool _jumpInput;
        protected bool _lastJumpInput;

        public int _jumpState;
        public int _jumpLevel;
        protected bool _stopJump;

        [SerializeField]
        public int _brakeTime;
        public int _brakeType;

        [SerializeField]
        public EClimbState _eClimbState;

        private const int JumpFirstMaxTime = 105;
        private const int JumpSecondMaxTime = 205;

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

        public MonsterAI(MonsterBase monster)
        {
            _unit = monster;
        }

        public void UpdateLogic()
        {
            _oldPosition = _unit.CurPos;
            _lastJumpInput = _jumpInput;
            _lastRightInput = _rightInput;
            _lastLeftInput = _leftInput;
            _onGround = _unit.Grounded;
            CalculateInput();
            UpdateMonster();
        }

        private void CalculateInput()
        {
            switch (_currentActionState)
            {
                case EActionState.None:
                    if (_framesOfJumping > 0)
                    {
                        _framesOfJumping--;
                        _jumpInput = true;
                    }
                    break;
                case EActionState.MoveTo:
                    IntVec2 lastDest, currentDest, nextDest;
                    bool destOnGround, reachedY, reachedX;
                    GetContext(out lastDest, out currentDest, out nextDest, out destOnGround, out reachedX, out reachedY);
                    IntVec2 pathPosition = _unit.CurPos / ConstDefineGM2D.ServerTileScale;

                    _leftInput = 0;
                    _rightInput = 0;
                    _jumpInput = false;
                    //到达路径点
                    if (reachedX && reachedY)
                    {
                        int lastNodeId = _currentNodeId;
                        _currentNodeId++;
                        if (_currentNodeId >= _path.Count)
                        {
                            _currentNodeId = -1;
                            _currentActionState = EActionState.None;
                            break;
                        }
                        if (_onGround)
                        {
                            _framesOfJumping = GetJumpFramesForNode(lastNodeId);
                        }
                        goto case EActionState.MoveTo;
                    }
                    else if (!reachedX)
                    {
                        if (currentDest.x - pathPosition.x > ConstDefineGM2D.AIMaxPositionError)
                        {
                            _rightInput = 1;
                        }
                        else if (pathPosition.x - currentDest.x > ConstDefineGM2D.AIMaxPositionError)
                        {
                            _leftInput = 1;
                        }
                    }
                    else if (!reachedY && _path.Count > _currentNodeId + 1 && !destOnGround)
                    {
                        int checkedX = 0;

                        var size = _unit.GetColliderSize() / ConstDefineGM2D.ServerTileScale;
                        //下落时解决提前落地的问题
                        if (_path[_currentNodeId + 1].x != _path[_currentNodeId].x)
                        {
                            if (_path[_currentNodeId + 1].x > _path[_currentNodeId].x)
                            {
                                checkedX = pathPosition.x + Math.Max(1, size.x);
                            }
                            else
                            {
                                checkedX = pathPosition.x - 1;
                            }
                        }

                        if (checkedX != 0 && !ColliderScene2D.Instance.AnySolidBlockInStripe(checkedX, pathPosition.y, _path[_currentNodeId + 1].y))
                        {
                            if (nextDest.x - pathPosition.x > ConstDefineGM2D.AIMaxPositionError)
                            {
                                _rightInput = 1;
                            }
                            else if (pathPosition.x - nextDest.x > ConstDefineGM2D.AIMaxPositionError)
                            {
                                _leftInput = 1;
                            }

                            if (ReachedNodeOnXAxis(pathPosition, currentDest, nextDest) &&
                                ReachedNodeOnYAxis(pathPosition, currentDest, nextDest))
                            {
                                _currentNodeId += 1;
                                goto case EActionState.MoveTo;
                            }
                        }
                    }

                    if (_framesOfJumping > 0 && (!_onGround || (reachedX && !destOnGround) || (_onGround && destOnGround)))
                    {
                        _jumpInput = true;
                        if (!_onGround)
                        {
                            --_framesOfJumping;
                        }
                    }

                    if (_unit.CurPos == _oldPosition)
                    {
                        ++_stuckFrames;
                        if (_stuckFrames > MaxStuckFrames)
                        {
                            MoveTo(_path[_path.Count - 1]);
                        }
                    }
                    else
                    {
                        _stuckFrames = 0;
                    }
                    break;
            }
        }

        private int GetJumpFrameCount(int deltaY)
        {
            if (deltaY <= 0)
            {
                return 0;
            }
            switch (deltaY)
            {
                case 1:
                    return 1;
                case 2:
                    return 5;
                case 3:
                    return 6;
                case 4:
                    return 9;
                case 5:
                    return 15;
                case 6:
                    return 21;
                default:
                    return 30;
            }
        }

        private int GetJumpFramesForNode(int lastNodeId)
        {
            int currentNodeId = lastNodeId + 1;

            if (_path[currentNodeId].y - _path[lastNodeId].y > 0 && _unit.Grounded)
            {
                int jumpHeight = 1;
                for (int i = currentNodeId; i < _path.Count; ++i)
                {
                    if (_path[i].y - _path[lastNodeId].y >= jumpHeight)
                    {
                        jumpHeight = _path[i].y - _path[lastNodeId].y;
                    }
                    if (_path[i].y - _path[lastNodeId].y < jumpHeight || ColliderScene2D.Instance.IsGround(_path[i].x, _path[i].y - 1))
                    {
                        return GetJumpFrameCount(jumpHeight);
                    }
                }
            }
            return 0;
        }

        private bool ReachedNodeOnXAxis(IntVec2 pathPosition, IntVec2 prevDest, IntVec2 currentDest)
        {
            return (prevDest.x <= currentDest.x && pathPosition.x >= currentDest.x)
                   || (prevDest.x >= currentDest.x && pathPosition.x <= currentDest.x)
                   || Mathf.Abs(pathPosition.x - currentDest.x) <= ConstDefineGM2D.AIMaxPositionError;
        }

        private bool ReachedNodeOnYAxis(IntVec2 pathPosition, IntVec2 prevDest, IntVec2 currentDest)
        {
            return (prevDest.y <= currentDest.y && pathPosition.y >= currentDest.y)
                   || (prevDest.y >= currentDest.y && pathPosition.y <= currentDest.y)
                   || (Mathf.Abs(pathPosition.y - currentDest.y) <= ConstDefineGM2D.AIMaxPositionError);
        }

        private void GetContext(out IntVec2 lastDest, out IntVec2 currentDest, out IntVec2 nextDest,
            out bool destOnGround, out bool reachedX, out bool reachedY)
        {
            lastDest = _path[_currentNodeId - 1];
            currentDest = _path[_currentNodeId];
            nextDest = currentDest;

            if (_path.Count > _currentNodeId + 1)
            {
                nextDest = _path[_currentNodeId + 1];
            }
            destOnGround = false;
            for (int i = _path[_currentNodeId].x; i < _path[_currentNodeId].x + 1; ++i)
            {
                if (ColliderScene2D.Instance.IsGround(i, _path[_currentNodeId].y - 1))
                {
                    destOnGround = true;
                    break;
                }
            }

            IntVec2 pathPosition = _unit.CurPos / ConstDefineGM2D.ServerTileScale;

            reachedX = ReachedNodeOnXAxis(pathPosition, lastDest, currentDest);
            reachedY = ReachedNodeOnYAxis(pathPosition, lastDest, currentDest);

            //这里需要 TODO
            if (reachedX && Mathf.Abs(pathPosition.x - currentDest.x) > ConstDefineGM2D.AIMaxPositionError &&
                Mathf.Abs(pathPosition.x - currentDest.x) < ConstDefineGM2D.AIMaxPositionError * 3.0f &&
                _lastLeftInput == 0 && _lastRightInput == 0)
            {
                pathPosition.x = currentDest.x;
                _unit.SetPos(pathPosition * ConstDefineGM2D.ServerTileScale);
            }
            if (destOnGround && !_onGround)
            {
                reachedY = false;
            }
        }

        public void MoveTo(IntVec2 destination)
        {
            _stuckFrames = 0;
            _path.Clear();
            List<IntVec2> path = ColliderScene2D.Instance.FindPath(_unit, PlayMode.Instance.MainUnit, 3);
            if (path != null && path.Count > 1)
            {
                for (int i = path.Count - 1; i >= 0; --i)
                {
                    _path.Add(path[i]);
                }
                _currentNodeId = 1;
                _currentActionState = EActionState.MoveTo;
                _framesOfJumping = GetJumpFramesForNode(0);
            }
            else
            {
                _currentNodeId = -1;
                if (_currentActionState == EActionState.MoveTo)
                {
                    _currentActionState = EActionState.None;
                }
            }
        }

        private void UpdateMonster()
        {
            if (_jumpInput)
            {
                _unit.SpeedY = 140;
                return;
            }
            return;
            if (_jumpInput)
            {
                if (_jumpState == 0)
                {
                    _jumpState = 100;
                }
                else if ((_jumpState > 0 && _jumpState < 200) && !_lastJumpInput && _jumpLevel == 0)
                {
                    //二段跳
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
            LogHelper.Debug("Monster:"+_jumpState+"~"+_unit.SpeedY+"~"+_unit.Grounded);
        }
    }
}