﻿/********************************************************************
** Filename : BulletBase
** Author : Dong
** Date : 2017/3/23 星期四 下午 3:11:11
** Summary : BulletBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    /// <summary>
    /// 子弹需要做成池
    /// </summary>
    [Poolable(MinPoolSize = 10, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class BulletBase : UnitBase, IPoolableObject
    {
        protected bool _run;
        protected IntVec2 _startPos;
        protected IntVec2 _speed;
        protected IntVec2 _pointA;
        protected IntVec2 _pointB;
        protected int _velocity;

        public void OnGet()
        {
        }

        public void OnFree()
        {
        }

        public void OnDestroyObject()
        {
        }

        public virtual void Run(IntVec2 startPos, IntVec2 speed)
        {
            _run = true;
            _startPos = startPos;
            SetPos(_startPos);
            _speed = speed;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (!_run)
            {
                return;
            }
            if (_isAlive)
            {
                GM2DTools.GetBorderPoint(ColliderGrid, _curMoveDirection, ref _pointA, ref _pointB);
                var checkGrid = SceneQuery2D.GetGrid(_pointA, _pointB, (byte)(_curMoveDirection - 1), _velocity);
                if (!DataScene2D.Instance.IsInTileMap(checkGrid))
                {
                    Speed = IntVec2.zero;
                    return;
                }
                var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.BulletHitLayer, float.MinValue, float.MaxValue, _dynamicCollider);
                for (int i = 0; i < units.Count; i++)
                {
                    if (GM2DTools.OnDirectionHit(units[i], this, _curMoveDirection))
                    {
                        Speed = IntVec2.zero;
                        break;
                    }
                }
            }
        }

        public override void UpdateView(float deltaTime)
        {
            if (!_run)
            {
                return;
            }
            if (_isStart && _isAlive)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                UpdateCollider(GetColliderPos(_curPos));
                if (_view != null)
                {
                    _trans.position = GetTransPos();
                }
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
                ColliderScene2D.Instance.UpdateDynamicNode(_dynamicCollider, new IntVec2(_lastColliderGrid.XMin, _lastColliderGrid.YMin));
                _lastColliderGrid = _colliderGrid;
            }
        }
    }
}
