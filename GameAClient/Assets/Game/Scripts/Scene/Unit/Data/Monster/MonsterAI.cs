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
        None,
        Think,
        Seek,
        Attack
    }

    public class MonsterAI : MonsterBase
    {
        public static IntVec2 SeekRange = new IntVec2(13, 4) * ConstDefineGM2D.ServerTileScale;
        public static IntVec2 AttackRange = new IntVec2(1, 1) * ConstDefineGM2D.ServerTileScale;
        public static IntVec2 PathRange = new IntVec2(2, 2);
        private const int MaxStuckFrames = 30;
        private const int MaxReSeekFrames = 5;

        protected List<IntVec2> _path = new List<IntVec2>();
        protected EMonsterState _eState;

        protected int _currentNodeId;
        protected int _jumpSpeed;
        protected int _thinkTimer;
        protected int _stuckTimer;
        protected int _reSeekTimer;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _monsterSpeed = 40;
            return true;
        }

        protected override void Clear()
        {
            _path.Clear();
            _eState = EMonsterState.Think;

            _currentNodeId = -1;
            _jumpSpeed = 0;
            _thinkTimer = 0;
            _stuckTimer = 0;
            _reSeekTimer = 0;
            _curFriction = _friction;
            base.Clear();
        }

        protected override void OnDead()
        {
            base.OnDead();
            _eState = EMonsterState.None;
        }

        protected override void UpdateMonsterAI()
        {
            if (!_canMotor)
            {
                SpeedX = 0;
                ChangeState(EMonsterState.None);
                _path.Clear();
                return;
            }
            if (_path != null)
            {
                for (int i = 0; i < _path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(_path[i].x + 0.5f, _path[i].y + 0.5f), new Vector3(_path[i + 1].x + 0.5f, _path[i + 1].y + 0.5f));
                }
            }
            _thinkTimer++;
            switch (_eState)
            {
                case EMonsterState.Think:
                    SpeedX = Util.ConstantLerp(SpeedX, 0, _curFriction);
                    if (_thinkTimer > 50)
                    {
                        OnThink();
                        _thinkTimer = 0;
                    }
                    break;
                case EMonsterState.Seek:
                    OnSeek();
                    break;
                case EMonsterState.Attack:
                    OnAttack();
                    break;
            }
        }

        private void ChangeState(EMonsterState state)
        {
            _eState = state;
            //LogHelper.Debug("ChangeState : {0}", _eState);
        }

        protected void MoveTo()
        {
            _path.Clear();
            _stuckTimer = 0;
            _reSeekTimer = 0;
            var path = ColliderScene2D.Instance.FindPath(this, PlayMode.Instance.MainUnit, 3);
            if (path != null && path.Count > 1)
            {
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    _path.Add(path[i]);
                }
                _currentNodeId = 1;
                SpeedY = GetJumpSpeedForNode(0);
                ChangeState(EMonsterState.Seek);
            }
            else
            {
                _currentNodeId = -1;
                ChangeState(EMonsterState.Think);
            }
        }

        protected void OnThink()
        {
            IntVec2 rel = CenterPos - PlayMode.Instance.MainUnit.CenterPos;
            if (Mathf.Abs(rel.x) <= SeekRange.x && Mathf.Abs(rel.y) <= SeekRange.y)
            {
                MoveTo();
            }
        }

        protected void OnAttack()
        {
            IntVec2 rel = CenterPos - PlayMode.Instance.MainUnit.CenterPos;
            if (Mathf.Abs(rel.x) > AttackRange.x || Mathf.Abs(rel.y) > AttackRange.y)
            {
                ChangeState(EMonsterState.Seek);
            }
        }

        protected void OnSeek()
        {
            IntVec2 rel = CenterPos - PlayMode.Instance.MainUnit.CenterPos;
            if (Mathf.Abs(rel.x) <= AttackRange.x && Mathf.Abs(rel.y) <= AttackRange.y)
            {
                ChangeState(EMonsterState.Attack);
                return;
            }
            //如果此次寻路的终点举例目标点差距太远的话，就重新寻路。
            IntVec2 distance = _path[_path.Count - 1] - PlayMode.Instance.MainUnit.CurPos / ConstDefineGM2D.ServerTileScale;
            if (Mathf.Abs(distance.x) > PathRange.x || Mathf.Abs(distance.y) > PathRange.y)
            {
                ++_reSeekTimer;
                if (_reSeekTimer > MaxReSeekFrames)
                {
                    MoveTo();
                    return;
                }
            }
            else
            {
                _reSeekTimer = 0;
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
                    _currentNodeId = -1;
                    ChangeState(EMonsterState.Think);
                    return;
                }
                if (_grounded)
                {
                    SpeedY = GetJumpSpeedForNode(lastNodeId);
                }
                return;
            }
            var pathPos = GetColliderPos(_curPos);
            if (_curBanInputTime <= 0)
            {
                //着火了 迅速跑
                if (_eDieType == EDieType.Fire)
                {

                }
                if (!reachedX)
                {
                    if (currentDest.x - pathPos.x > ConstDefineGM2D.AIMaxPositionError)
                    {
                        //向右
                        SetFacingDir(EMoveDirection.Right);
                        SpeedX = Util.ConstantLerp(SpeedX, _curMaxSpeedX, _curFriction);
                    }
                    else if (pathPos.x - currentDest.x > ConstDefineGM2D.AIMaxPositionError)
                    {
                        //向左
                        SetFacingDir(EMoveDirection.Left);
                        SpeedX = Util.ConstantLerp(SpeedX, -_curMaxSpeedX, _curFriction);
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
                        !ColliderScene2D.Instance.HasBlockInLine(checkedX/ConstDefineGM2D.ServerTileScale,
                            pathPos.y/ConstDefineGM2D.ServerTileScale, _path[_currentNodeId + 1].y))
                    {
                        if (nextDest.x - pathPos.x > ConstDefineGM2D.AIMaxPositionError)
                        {
                            //向右
                            SetFacingDir(EMoveDirection.Right);
                            SpeedX = Util.ConstantLerp(SpeedX, _curMaxSpeedX, _curFriction);
                        }
                        else if (pathPos.x - nextDest.x > ConstDefineGM2D.AIMaxPositionError)
                        {
                            //向左
                            SetFacingDir(EMoveDirection.Left);
                            SpeedX = Util.ConstantLerp(SpeedX, -_curMaxSpeedX, _curFriction);
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
                    return 190;
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
            //if (reachedX && Mathf.Abs(pathPos.x - currentDest.x) > ConstDefineGM2D.AIMaxPositionError && Mathf.Abs(pathPos.x - currentDest.x) < ConstDefineGM2D.AIMaxPositionError * 3.0f)
            //{
            //    _curPos.x = currentDest.x;
            //}
            if (destOnGround && !_grounded)
            {
                reachedY = false;
            }
        }

        private bool ReachedNodeOnXAxis(IntVec2 pathPos, IntVec2 lastDest, IntVec2 currentDest)
        {
            return (lastDest.x <= currentDest.x && pathPos.x >= currentDest.x)
                   || (lastDest.x >= currentDest.x && pathPos.x <= currentDest.x)
                   || Mathf.Abs(pathPos.x - currentDest.x) <= ConstDefineGM2D.AIMaxPositionError;
        }

        private bool ReachedNodeOnYAxis(IntVec2 pathPos, IntVec2 lastDest, IntVec2 currentDest)
        {
            return (lastDest.y <= currentDest.y && pathPos.y >= currentDest.y)
                   || (lastDest.y >= currentDest.y && pathPos.y <= currentDest.y)
                   || (Mathf.Abs(pathPos.y - currentDest.y) <= ConstDefineGM2D.AIMaxPositionError);
        }

        protected override void UpdateMonsterView()
        {
            base.UpdateMonsterView();
            if (_eState == EMonsterState.Seek)
            {
                if (_curPos == _lastPos)
                {
                    ++_stuckTimer;
                    if (_stuckTimer > MaxStuckFrames)
                    {
                        MoveTo();
                    }
                }
                else
                {
                    _stuckTimer = 0;
                }
            }
        }
    }
}