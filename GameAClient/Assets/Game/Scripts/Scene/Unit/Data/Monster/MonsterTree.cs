/********************************************************************
** Filename : MonsterTree
** Author : Dong
** Date : 2017/5/11 星期四 下午 5:30:25
** Summary : MonsterTree
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public enum EMonsterState
    {
        Idle,
        Seek,
        Attack
    }

    [Unit(Id = 2001, Type = typeof(MonsterTree))]
    public class MonsterTree : MonsterBase
    {
        public static IntVec2 SeekRange = new IntVec2(13, 4) * ConstDefineGM2D.ServerTileScale;
        public static IntVec2 AttackRange = new IntVec2(1, 1) * ConstDefineGM2D.ServerTileScale;
        protected List<IntVec2> _path = new List<IntVec2>();
        protected EMonsterState _eMonsterState;

        protected int _stuckTime;
        protected int _currentNodeId;

        protected int _friction;
        protected int _jumpSpeed;

        protected override void Clear()
        {
            base.Clear();
            _path.Clear();
            _eMonsterState = EMonsterState.Idle;
        }

        protected virtual void CalculateMonsterState()
        {
            IntVec2 rel = CenterPos - PlayMode.Instance.MainUnit.CenterPos;
            switch (_eMonsterState)
            {
                case EMonsterState.Idle:
                    if (Mathf.Abs(rel.x) <= SeekRange.x && Mathf.Abs(rel.y) <= SeekRange.y)
                    {
                        _eMonsterState = EMonsterState.Seek;
                        MoveTo();
                    }
                    break;
                case EMonsterState.Seek:
                    if (Mathf.Abs(rel.x) <= AttackRange.x && Mathf.Abs(rel.y) <= AttackRange.y)
                    {
                        _eMonsterState = EMonsterState.Attack;
                        goto case EMonsterState.Attack;
                    }
                    OnSeek();
                    break;
                case EMonsterState.Attack:
                    if (Mathf.Abs(rel.x) > AttackRange.x && Mathf.Abs(rel.y) > AttackRange.y)
                    {
                        _eMonsterState = EMonsterState.Seek;
                    }
                    break;
            }
        }

        protected void MoveTo()
        {
            _path.Clear();
            _stuckTime = 0;
            var path = ColliderScene2D.Instance.FindPath(this, PlayMode.Instance.MainUnit, 3);
            if (path != null && path.Count > 1)
            {
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    _path.Add(path[i]);
                }
                _currentNodeId = 1;
            }
            else
            {
                _currentNodeId = -1;
            }
        }

        public override void UpdateLogic()
        {
            if (_path != null)
            {
                for (int i = 0; i < _path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(_path[i].x + 0.5f, _path[i].y + 0.5f), new Vector3(_path[i + 1].x + 0.5f, _path[i + 1].y + 0.5f));
                }
            }
            if (_isAlive && _isStart)
            {
                bool air = false;
                _friction = 6;
                if (SpeedY != 0)
                {
                    air = true;
                }
                if (!air)
                {
                    _onClay = false;
                    bool downExist = false;
                    int deltaX = int.MaxValue;
                    var units = EnvManager.RetriveDownUnits(this);
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        int ymin = 0;
                        if (unit != null && unit.IsAlive && CheckOnFloor(unit) && unit.OnUpHit(this, ref ymin, true))
                        {
                            downExist = true;
                            _grounded = true;
                            _downUnits.Add(unit);
                            if (unit.Friction > _friction)
                            {
                                _friction = unit.Friction;
                            }
                            if (unit.Id == ConstDefineGM2D.ClayId)
                            {
                                _onClay = true;
                            }
                            var delta = Math.Abs(CenterPos.x - unit.CenterPos.x);
                            if (deltaX > delta)
                            {
                                deltaX = delta;
                                _downUnit = unit;
                            }
                        }
                    }
                    if (!downExist)
                    {
                        air = true;
                    }
                }
                if (air && _grounded)
                {
                    Speed += _lastExtraDeltaPos;
                    _grounded = false;
                    if (SpeedY > 0)
                    {
                        OnJump();
                    }
                }
                _curMaxSpeedX = _monsterSpeed;
                if (_onClay)
                {
                    _curMaxSpeedX /= ClayRatio;
                }
                CalculateMonsterState();
                if (!_grounded)
                {
                    SpeedY -= 12;
                    if (SpeedY < -120)
                    {
                        SpeedY = -120;
                    }
                }
            }
        }

        private void OnJump()
        {
        }

        protected void OnSeek()
        {
            IntVec2 lastDest, currentDest, nextDest;
            bool destOnGround, reachedY, reachedX;
            GetContext(out lastDest, out currentDest, out nextDest, out destOnGround, out reachedX, out reachedY);
            IntVec2 pathPosition = _curPos / ConstDefineGM2D.ServerTileScale;
            //到达路径点
            if (reachedX && reachedY)
            {
                int lastNodeId = _currentNodeId;
                _currentNodeId++;
                if (_currentNodeId >= _path.Count)
                {
                    _currentNodeId = -1;
                    _eMonsterState = EMonsterState.Idle;
                    return;
                }
                if (_grounded)
                {
                    SpeedY = GetJumpSpeedForNode(lastNodeId);
                }
                return;
            }
            if (!reachedX)
            {
                if (currentDest.x - pathPosition.x > ConstDefineGM2D.AIMaxPositionError)
                {
                    //向右
                    SetFacingDir(EMoveDirection.Right);
                    SpeedX = Util.ConstantLerp(SpeedX, _curMaxSpeedX, _friction);
                }
                else if (pathPosition.x - currentDest.x > ConstDefineGM2D.AIMaxPositionError)
                {
                    //向左
                    SetFacingDir(EMoveDirection.Left);
                    SpeedX = Util.ConstantLerp(SpeedX, -_curMaxSpeedX, _friction);
                }
            }
            else if (_path.Count > _currentNodeId + 1 && !destOnGround)
            {
                int checkedX = 0;

                var size = GetColliderSize() / ConstDefineGM2D.ServerTileScale;
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
                        //向右
                        SetFacingDir(EMoveDirection.Right);
                        SpeedX = Util.ConstantLerp(SpeedX, _curMaxSpeedX, _friction);
                    }
                    else if (pathPosition.x - nextDest.x > ConstDefineGM2D.AIMaxPositionError)
                    {
                        //向左
                        SetFacingDir(EMoveDirection.Left);
                        SpeedX = Util.ConstantLerp(SpeedX, -_curMaxSpeedX, _friction);
                    }

                    if (ReachedNodeOnXAxis(pathPosition, currentDest, nextDest) &&
                        ReachedNodeOnYAxis(pathPosition, currentDest, nextDest))
                    {
                        _currentNodeId += 1;
                        return;
                    }
                }
            }
            //if (_unit.CurPos == _oldPosition)
            //{
            //    ++_stuckFrames;
            //    if (_stuckFrames > MaxStuckFrames)
            //    {
            //        MoveTo(_path[_path.Count - 1]);
            //    }
            //}
            //else
            //{
            //    _stuckFrames = 0;
            //}
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
            IntVec2 pathPosition = _curPos / ConstDefineGM2D.ServerTileScale;

            reachedX = ReachedNodeOnXAxis(pathPosition, lastDest, currentDest);
            reachedY = ReachedNodeOnYAxis(pathPosition, lastDest, currentDest);

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
    }
}