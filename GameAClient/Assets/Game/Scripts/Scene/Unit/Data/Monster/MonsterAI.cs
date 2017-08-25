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
        protected static IntVec2 SeekRange = new IntVec2(13, 4) * ConstDefineGM2D.ServerTileScale;
        protected static IntVec2 AttackRange = new IntVec2(1, 1) * ConstDefineGM2D.ServerTileScale;
        public static IntVec2 PathRange = new IntVec2(3, 2);
        protected const int MaxStuckFrames = 30;
        protected const int MaxReSeekFrames = 5;
        protected List<IntVec2> _path = new List<IntVec2>();

        protected IntVec2 _lastPos;
        protected EMonsterState _eState;
        
        protected int _thinkTimer;
        protected int _stuckTimer;
        protected int _reSeekTimer;
        
        protected int _currentNodeId;
        protected int _jumpSpeed;
        
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            //AI 最大速度>=50. 碰撞高度<=520
            _maxSpeedX = 50;
            return true;
        }

        protected override void Clear()
        {
            _lastPos = _curPos;
            _eState = EMonsterState.Think;
            _path.Clear();
            _thinkTimer = 0;
            _stuckTimer = 0;
            _reSeekTimer = 0;
            _currentNodeId = 0;
            _jumpSpeed = 0;
            base.Clear();
        }
        
        protected virtual void ChangeState(EMonsterState state)
        {
            _eState = state;
            //LogHelper.Debug("ChangeState : {0}", _eState);
        }
        
        protected override void UpdateMonsterAI()
        {
            _curMaxSpeedX = (int)(_maxSpeedX * _speedRatio * _speedStateRatio);
            if (_thinkTimer > 0)
            {
                _thinkTimer--;
            }
            ChangeState(EMonsterState.Idle);
            IntVec2 rel = CenterDownPos - PlayMode.Instance.MainPlayer.CenterDownPos;
            if (PlayMode.Instance.MainPlayer.CanMove)
            {
                if (ConditionAttack(rel))
                {
                    ChangeState(EMonsterState.Attack);
                }
                else if(ConditionSeek(rel))
                {
                    ChangeState(EMonsterState.Seek);
                }
                else if(ConditionThink(rel))
                {
                    ChangeState(EMonsterState.Think);
                }
            }
            if (_eState != EMonsterState.Seek)
            {
                SetInput(EInputType.Right, false);
                SetInput(EInputType.Left, false);
            }
            SetInput(EInputType.Skill1, false);
            
            switch (_eState)
            {
                case EMonsterState.Think:
                    OnThink();
                    break;
                case EMonsterState.Seek:
                    OnSeek();
                    break;
                case EMonsterState.Attack:
                    OnAttack();
                    break;
            }
#if UNITY_EDITOR
            if (_path != null)
            {
                for (int i = 0; i < _path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(_path[i].x + 0.5f, _path[i].y + 0.5f), new Vector3(_path[i + 1].x + 0.5f, _path[i + 1].y + 0.5f));
                }
            }
#endif
        }

        protected virtual void OnAttack()
        {
            SetInput(EInputType.Skill1, true);
        }

        protected virtual void OnThink()
        {
            FindPath();
        }

        protected virtual void OnSeek()
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

            var pathPos = GetColliderPos(CenterDownPos);
            IntVec2 currentDest, nextDest;
            bool destOnGround, reachedY, reachedX;
            GetContext(ref pathPos, out currentDest, out nextDest, out destOnGround, out reachedX, out reachedY);
            if (reachedX && Mathf.Abs(pathPos.x - currentDest.x) >= 2 * ConstDefineGM2D.ServerTileScale)
            {
                FindPath();
                return;
            }
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
                if (!reachedX)
                {
                    if (currentDest.x - pathPos.x >= _curMaxSpeedX)
                    {
                        //向右
                        SetInput(EInputType.Right, true);
                        SetInput(EInputType.Left, false);
                    }
                    else if (pathPos.x - currentDest.x >= _curMaxSpeedX)
                    {
                        //向左
                        SetInput(EInputType.Left, true);
                        SetInput(EInputType.Right, false);
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
                            SetInput(EInputType.Left, false);
                        }
                        else if (pathPos.x - nextDest.x >= _curMaxSpeedX)
                        {
                            //向左
                            SetInput(EInputType.Left, true);
                            SetInput(EInputType.Right, false);
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
        
        protected virtual bool ConditionAttack(IntVec2 rel)
        {
            if (!CanAttack)
            {
                return false;
            }
            if (Mathf.Abs(rel.x) > AttackRange.x || Mathf.Abs(rel.y) > AttackRange.y)
            {
                return false;
            }
            return true;
        }

        protected virtual bool ConditionSeek(IntVec2 rel)
        {
            if (!CanMove)
            {
                return false;
            }
            if (_path.Count <= 1)
            {
                return false;
            }
            return true;
        }
        
        protected virtual bool ConditionThink(IntVec2 rel)
        {
            if (!CanMove)
            {
                return false;
            }
            if (_thinkTimer > 0)
            {
                return false;
            }
            _thinkTimer = 50;
            if (Mathf.Abs(rel.x) > SeekRange.x || Mathf.Abs(rel.y) > SeekRange.y)
            {
                return false;
            }
            return true;
        }
        
        protected void FindPath()
        {
            _lastPos = _curPos;
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
                    return 220;
            }
            return 0;
        }

        private void GetContext(ref IntVec2 pathPos, out IntVec2 currentDest, out IntVec2 nextDest,
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
            reachedX = ReachedNodeOnXAxis(pathPos, lastDest, currentDest);
            reachedY = ReachedNodeOnYAxis(pathPos, lastDest, currentDest);
            if (destOnGround && !_grounded)
            {
                reachedY = false;
            }

            if (!reachedX)
            {
                if (_curMoveDirection == EMoveDirection.Left)
                {
                    if (ColliderScene2D.Instance.IsGround(_curPos.x / ConstDefineGM2D.ServerTileScale,
                        _curPos.y / ConstDefineGM2D.ServerTileScale))
                    {
                        reachedX = true;
                    }
                }
                else if (_curMoveDirection == EMoveDirection.Right)
                {
                    if (ColliderScene2D.Instance.IsGround(_curPos.x / ConstDefineGM2D.ServerTileScale + 1,
                        _curPos.y / ConstDefineGM2D.ServerTileScale))
                    {
                        reachedX = true;
                    }
                }
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