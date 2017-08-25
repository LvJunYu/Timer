/********************************************************************
** Filename : MonsterAI
** Author : Dong
** Date : 2017/5/11 星期四 下午 5:30:25
** Summary : MonsterAI
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public enum EMonsterState
    {
        Idle,
        Think,
        Seek,
        Attack,
    }

    public class MonsterAI : MonsterBase
    {
        protected int _currentNodeId;
        protected int _jumpSpeed;

        protected override void Clear()
        {
            _currentNodeId = 0;
            _jumpSpeed = 0;
            base.Clear();
        }

        protected void FindPath()
        {
            SetInput(EInputType.Left, false);
            SetInput(EInputType.Right, false);
            _path.Clear();
            _stuckTimer = 0;
            _reSeekTimer = 0;
            _currentNodeId = 1;
            var mainUnit = PlayMode.Instance.MainPlayer;
            var path = ColliderScene2D.Instance.FindPath(this, mainUnit, 3);
            if (path != null && path.Count > 1)
            {
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    _path.Add(path[i]);
                }
                SpeedY = GetJumpSpeedForNode(0);
                ChangeState(EMonsterState.Seek);
            }
            LogHelper.Debug("FindPath {0}", _path.Count);
        }

        protected override void OnThink()
        {
            FindPath();
        }

        protected override void OnSeek()
        {
            if (_curPos != _lastPos)
            {
                _stuckTimer = 0;
            }
            else
            {
                ++_stuckTimer;
                if (_stuckTimer > MaxStuckFrames)
                {
                    FindPath();
                    return;
                }
            }
            _lastPos = _curPos;
            
            //如果此次寻路的终点举例目标点差距太远的话，就重新寻路。
            IntVec2 distance = _path[_path.Count - 1] - PlayMode.Instance.MainPlayer.CurPos / ConstDefineGM2D.ServerTileScale;
            if (Mathf.Abs(distance.x) <= PathRange.x && Mathf.Abs(distance.y) <= PathRange.y)
            {
                _reSeekTimer = 0;
            }
            else
            {
                ++_reSeekTimer;
                if (_reSeekTimer > MaxReSeekFrames)
                {
                    FindPath();
                    return;
                }
            }
            IntVec2 currentDest, nextDest;
            bool destOnGround, reachedY, reachedX;
            GetContext(out currentDest, out nextDest, out destOnGround, out reachedX, out reachedY);
            //到达路径点
            if (reachedX && reachedY)
            {
                int lastNodeId = _currentNodeId;
                _currentNodeId++;
                if (_currentNodeId >= _path.Count)
                {
                    LogHelper.Debug("Reach Final!");
                    _path.Clear();
                    return;
                }
                if (_grounded)
                {
                    SpeedY = GetJumpSpeedForNode(lastNodeId);
                }
                return;
            }
            if (_curBanInputTime > 0)
            {
                SetInput(EInputType.Left, false);
                SetInput(EInputType.Right, false);
            }
            else
            {
                var pathPos = GetColliderPos(_curPos);
                if (!reachedX)
                {
                    if (currentDest.x - pathPos.x >= _curMaxSpeedX)
                    {
                        //向右
                        SetInput(EInputType.Right, true);
                    }
                    else if (pathPos.x - currentDest.x >= _curMaxSpeedX)
                    {
                        //向左
                        SetInput(EInputType.Left, true);
                    }
                }
                else if (_path.Count > _currentNodeId + 1 && !destOnGround)
                {
                    int checkedX = 0;

                    var size = GetColliderSize();
                    //下落时解决提前落地的问题
                    if (_path[_currentNodeId + 1].x != _path[_currentNodeId].x)
                    {
                        if (_path[_currentNodeId + 1].x > _path[_currentNodeId].x)
                        {
                            checkedX = pathPos.x + size.x;
                        }
                        else
                        {
                            checkedX = pathPos.x - 1;
                        }
                    }

                    if (checkedX != 0 &&
                        !ColliderScene2D.Instance.HasBlockInLine(checkedX / ConstDefineGM2D.ServerTileScale,
                            pathPos.y / ConstDefineGM2D.ServerTileScale, _path[_currentNodeId + 1].y))
                    {
                        if (nextDest.x - pathPos.x >= _curMaxSpeedX)
                        {
                            //向右
                            SetInput(EInputType.Right, true);
                        }
                        else if (pathPos.x - nextDest.x >= _curMaxSpeedX)
                        {
                            //向左
                            SetInput(EInputType.Left, true);
                        }

                        if (ReachedNodeOnXAxis(pathPos, currentDest, nextDest) &&
                            ReachedNodeOnYAxis(pathPos, currentDest, nextDest))
                        {
                            _currentNodeId += 1;
                        }
                    }
                }
            }
        }

        private int GetJumpSpeedForNode(int lastNodeId)
        {
            int currentNodeId = lastNodeId + 1;
            if (_path[currentNodeId].y - _path[lastNodeId].y > 0 && _grounded)
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
                        return GetJumpSpeed(jumpHeight);
                    }
                }
            }
            return 0;
        }

        private int GetJumpSpeed(int jumpHeight)
        {
            if (jumpHeight <= 0)
            {
                return 0;
            }
            switch (jumpHeight)
            {
                case 1:
                    return 140;
                case 2:
                    return 180;
                case 3:
                    return 230;
            }
            return 0;
        }

        private void GetContext(out IntVec2 currentDest, out IntVec2 nextDest,
       out bool destOnGround, out bool reachedX, out bool reachedY)
        {
            currentDest = _path[_currentNodeId] * ConstDefineGM2D.ServerTileScale;
            nextDest = currentDest;
            if (_path.Count > _currentNodeId + 1)
            {
                nextDest = _path[_currentNodeId + 1] * ConstDefineGM2D.ServerTileScale;
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
            var lastDest = _path[_currentNodeId - 1] * ConstDefineGM2D.ServerTileScale;
            var pathPos = GetColliderPos(_curPos);
            reachedX = ReachedNodeOnXAxis(pathPos, lastDest, currentDest);
            reachedY = ReachedNodeOnYAxis(pathPos, lastDest, currentDest);
//            if (reachedX && Mathf.Abs(pathPos.x - currentDest.x) >_curMaxSpeedX && Mathf.Abs(pathPos.x - currentDest.x) < _curMaxSpeedX * 3.0f)
//            {
//                _curPos.x = currentDest.x;
//            }
            if (destOnGround && !_grounded)
            {
                reachedY = false;
            }
        }

        private bool ReachedNodeOnXAxis(IntVec2 pathPos, IntVec2 lastDest, IntVec2 currentDest)
        {
            return (lastDest.x <= currentDest.x && pathPos.x >= currentDest.x)
                   || (lastDest.x >= currentDest.x && pathPos.x <= currentDest.x)
                   || Mathf.Abs(pathPos.x - currentDest.x) <= _curMaxSpeedX;
        }

        private bool ReachedNodeOnYAxis(IntVec2 pathPos, IntVec2 lastDest, IntVec2 currentDest)
        {
            return (lastDest.y <= currentDest.y && pathPos.y >= currentDest.y)
                   || (lastDest.y >= currentDest.y && pathPos.y <= currentDest.y)
                   || (Mathf.Abs(pathPos.y - currentDest.y) <= _curMaxSpeedX);
        }
    }
}