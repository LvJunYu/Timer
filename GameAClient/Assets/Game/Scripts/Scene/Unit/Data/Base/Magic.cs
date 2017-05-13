/********************************************************************
** Filename : Magic
** Author : Dong
** Date : 2017/3/16 星期四 下午 1:37:01
** Summary : Magic
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class Magic : UnitBase
    {
        protected int _timerMagic;
        /// <summary>
        /// 必须可以被640整除
        /// </summary>
        protected int _velocity;
        protected IntVec2 _pointA;
        protected IntVec2 _pointB;
        protected UnitBase _magicRotate;
        protected bool _run = true;

        public override bool CanControlledBySwitch
        {
            get { return UseMagic(); }
        }

        protected override void Clear()
        {
            base.Clear();
            _magicRotate = null;
            if (!IsHero && _curMoveDirection != EMoveDirection.None)
            {
                _timerMagic = 0;
                _velocity = 20;
                switch (_curMoveDirection)
                {
                    case EMoveDirection.Up:
                        SpeedY = _velocity;
                        break;
                    case EMoveDirection.Down:
                        SpeedY = -_velocity;
                        break;
                    case EMoveDirection.Left:
                        SpeedX = -_velocity;
                        break;
                    case EMoveDirection.Right:
                        SpeedX = _velocity;
                        break;
                }
            }
        }

        internal override void OnOtherSwitch()
        {
            _run = !_run;
        }

        public bool UseMagic()
        {
            return _curMoveDirection != EMoveDirection.None;
        }

        public override void UpdateLogic()
        {
            if (!_run || !UseMagic())
            {
                return;
            }
            if (_isStart && _isAlive)
            {
                if (Speed != IntVec2.zero)
                {
                    GM2DTools.GetBorderPoint(ColliderGrid, _curMoveDirection, ref _pointA, ref _pointB);
                    var checkGrid = SceneQuery2D.GetGrid(_pointA, _pointB, (byte)(_curMoveDirection - 1), _velocity);
                    if (!DataScene2D.Instance.IsInTileMap(checkGrid))
                    {
                        _timerMagic = 0;
                        Speed = IntVec2.zero;
                        ChangeMoveDirection();
                        return;
                    }
                    var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, _curMoveDirection == EMoveDirection.Up
                        ? EnvManager.MovingEarthBlockUpLayer
                        : EnvManager.MovingEarthBlockLayer, float.MinValue, float.MaxValue, _dynamicCollider);
                    for (int i = 0; i < units.Count; i++)
                    {
                        if (GM2DTools.OnDirectionHit(units[i], this, _curMoveDirection))
                        {
                            _timerMagic = 0;
                            Speed = IntVec2.zero;
                            _magicRotate = null;
                            ChangeMoveDirection();
                            break;
                        }
                        if (IsValidBlueStoneRotation(units[i]))
                        {
                            _magicRotate = units[i];
                            break;
                        }
                    }
                    if (_magicRotate != null)
                    {
                        if (_magicRotate.CurPos == _curPos)
                        {
                            _timerMagic = 0;
                            Speed = IntVec2.zero;
                            _curMoveDirection = (EMoveDirection) (_magicRotate.Rotation + 1);
                            _magicRotate = null;
                        }
                    }
                }
                else
                {
                    _timerMagic++;
                    if (_timerMagic == 25)
                    {
                        switch (_curMoveDirection)
                        {
                            case EMoveDirection.Up:
                                SpeedY = _velocity;
                                break;
                            case EMoveDirection.Down:
                                SpeedY = - _velocity;
                                break;
                            case EMoveDirection.Left:
                                SpeedX = -_velocity;
                                break;
                            case EMoveDirection.Right:
                                SpeedX = _velocity;
                                break;
                        }
                    }
                }
            }
        }

        private bool IsValidBlueStoneRotation(UnitBase unit)
        {
            if (unit == null || unit.Id != ConstDefineGM2D.BlueStoneRotateId)
            {
                return false;
            }
            //var delta = Mathf.Abs(unit.Rotation + 1 - (int) _curMoveDirection);
            //if (delta == 0 || delta == 2)
            //{
            //    return false;
            //}
            return true;
        }

        private void ChangeMoveDirection()
        {
            switch (_curMoveDirection)
            {
                case EMoveDirection.Up:
                    _curMoveDirection = EMoveDirection.Down;
                    break;
                case EMoveDirection.Down:
                    _curMoveDirection = EMoveDirection.Up;
                    break;
                case EMoveDirection.Left:
                    _curMoveDirection = EMoveDirection.Right;
                    break;
                case EMoveDirection.Right:
                    _curMoveDirection = EMoveDirection.Left;
                    break;
            }
        }

        public override void UpdateView(float deltaTime)
        {
            if (!_run || !UseMagic())
            {
                return;
            }
            if (_isStart && _isAlive)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
            }
        }

        protected virtual void UpdateCollider(IntVec2 min)
        {
            if (_colliderPos.Equals(min))
            {
                return;
            }
            _colliderPos = min;
            _colliderGrid = GetColliderGrid(_colliderPos);
            if (!_lastColliderGrid.Equals(_colliderGrid))
            {
                _dynamicCollider.Grid = _colliderGrid;
                ColliderScene2D.Instance.UpdateDynamicNode(_dynamicCollider);
                _lastColliderGrid = _colliderGrid;
            }
        }

        public override IntVec2 GetDeltaImpactPos()
        {
            if (!_run || !UseMagic())
            {
                return _deltaImpactPos;
            }
            if (!_isCalculated && _dynamicCollider != null)
            {
                if (_downUnits.Count > 0)
                {
                    int right = 0;
                    int left = 0;
                    int deltaY = int.MinValue;
                    for (int i = 0; i < _downUnits.Count; i++)
                    {
                        var deltaPos = _downUnits[i].GetDeltaImpactPos();
                        if (deltaPos.x > 0 && deltaPos.x > right)
                        {
                            right = deltaPos.x;
                        }
                        if (deltaPos.x < 0 && deltaPos.x < left)
                        {
                            left = deltaPos.x;
                        }
                        if (deltaPos.y > deltaY)
                        {
                            deltaY = deltaPos.y;
                        }
                    }
                    int deltaX = right + left;
                    _deltaImpactPos = new IntVec2(SpeedX + deltaX, SpeedY + deltaY);
                }
                else
                {
                    _deltaImpactPos = Speed;
                }
                _isCalculated = true;
            }
            return _deltaImpactPos;
        }
    }
}
