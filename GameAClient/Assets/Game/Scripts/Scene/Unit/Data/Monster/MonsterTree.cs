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
        Think,
        Seek,
        Attack
    }

    [Unit(Id = 2001, Type = typeof(MonsterTree))]
    public class MonsterTree : MonsterBase
    {
        public static IntVec2 SeekRange = new IntVec2(13, 4) * ConstDefineGM2D.ServerTileScale;
        public static IntVec2 AttackRange = new IntVec2(2, 1) * ConstDefineGM2D.ServerTileScale;
        private const int MaxStuckFrames = 30;

        protected List<IntVec2> _path = new List<IntVec2>();
        protected EMonsterState _eState;

        protected int _stuckTime;
        protected int _currentNodeId;

        protected int _jumpSpeed;

        protected int _thinkTimer;
        protected IntVec2 _lastPos;
        protected int _stuckTimer;

        protected override void Clear()
        {
            base.Clear();
            _path.Clear();
            _eState = EMonsterState.Think;

            _stuckTime = 0;
            _currentNodeId = -1;

            _jumpSpeed = 0;
            _thinkTimer = 0;
            _stuckTimer = 0;
            _lastPos = _curPos;
        }

        protected virtual void CalculateMonsterState()
        {
            _thinkTimer++;
            switch (_eState)
            {
                case EMonsterState.Think:
                    SpeedX = Util.ConstantLerp(SpeedX, 0, 6);
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
                    SpeedX = Util.ConstantLerp(SpeedX, 0, 6);
                    OnAttack();
                    break;
            }
        }

        private void ChangeState(EMonsterState state)
        {
            _eState = state;
            LogHelper.Debug("ChangeState : {0}", _eState);
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
                SpeedY = GetJumpSpeedForNode(0);
                ChangeState(EMonsterState.Seek);
            }
            else
            {
                _currentNodeId = -1;
                ChangeState(EMonsterState.Think);
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
                            if (unit.Id == UnitDefine.ClayId)
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
                return;
            }
            if (_canAttack)
            {

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
            if (!reachedX)
            {
                if (_canMotor)
                {
                    if (currentDest.x - pathPos.x > ConstDefineGM2D.AIMaxPositionError)
                    {
                        //向右
                        SetFacingDir(EMoveDirection.Right);
                        //SpeedX = _curMaxSpeedX;
                        SpeedX = Util.ConstantLerp(SpeedX, _curMaxSpeedX, 6);
                    }
                    else if (pathPos.x - currentDest.x > ConstDefineGM2D.AIMaxPositionError)
                    {
                        //向左
                        SetFacingDir(EMoveDirection.Left);
                        //SpeedX = -_curMaxSpeedX;
                        SpeedX = Util.ConstantLerp(SpeedX, -_curMaxSpeedX, 6);
                    }
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

                if (checkedX != 0 && !ColliderScene2D.Instance.AnySolidBlockInStripe(checkedX / ConstDefineGM2D.ServerTileScale, pathPos.y / ConstDefineGM2D.ServerTileScale, _path[_currentNodeId + 1].y))
                {
                    if (_canMotor)
                    {
                        if (nextDest.x - pathPos.x > ConstDefineGM2D.AIMaxPositionError)
                        {
                            //向右
                            SetFacingDir(EMoveDirection.Right);
                            //SpeedX = _curMaxSpeedX;
                            SpeedX = Util.ConstantLerp(SpeedX, _curMaxSpeedX, 6);
                        }
                        else if (pathPos.x - nextDest.x > ConstDefineGM2D.AIMaxPositionError)
                        {
                            //向左
                            SetFacingDir(EMoveDirection.Left);
                            //SpeedX = -_curMaxSpeedX;
                            SpeedX = Util.ConstantLerp(SpeedX, -_curMaxSpeedX, 6);
                        }
                    }

                    if (ReachedNodeOnXAxis(pathPos, currentDest, nextDest) &&
                        ReachedNodeOnYAxis(pathPos, currentDest, nextDest))
                    {
                        _currentNodeId += 1;
                        return;
                    }
                }
            }
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

        private int GetJumpSpeedForNode(int lastNodeId)
        {
            if (_canMotor)
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
            LogHelper.Debug("UpdateMonsterView : {0}", _eState);
            switch (_eState)
            {
                    case EMonsterState.Think:
                    if (_animation != null)
                    {
                        _animation.PlayLoop("Idle");
                    }
                    break;
                    case EMonsterState.Seek:
                    if (_animation != null)
                    {
                        _animation.PlayLoop("Run");
                    }
                    break;
                    case EMonsterState.Attack:
                    if (_animation != null && !_animation.IsPlaying("Attack", 1))
                    {
                        _animation.PlayOnce("Attack", 1, 1);
                    }
                    break;
            }

            _lastPos = _curPos;
        }
    }
}