/********************************************************************
** Filename : RigidbodyUnit
** Author : Dong
** Date : 2017/3/11 星期六 下午 1:34:31
** Summary : RigidbodyUnit
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    public class RigidbodyUnit : BlockBase
    {
        [SerializeField]
        protected UnitBase[] _hitUnits = new UnitBase[4];
        private static HashSet<IntVec3> _cacheCheckedDownUnits = new HashSet<IntVec3>();
        private static HashSet<IntVec3> _cacheHitUnits = new HashSet<IntVec3>();
        protected bool _onClay;
        protected const int ClayRatio = 5;

        protected override void Clear()
        {
            base.Clear();
            for (int i = 0; i < 4; i++)
            {
                _hitUnits[i] = null;
            }
            _cacheCheckedDownUnits.Clear();
            _cacheHitUnits.Clear();
        }

        protected override void UpdateCollider(IntVec2 min)
        {
            for (int i = 0; i < 4; i++)
            {
                _hitUnits[i] = null;
            }
            if (_isFreezed)
            {
                _colliderPos = min;
                _colliderGrid = GetColliderGrid(_colliderPos);
            }
            else
            {
                _cacheHitUnits.Clear();
                _colliderPos.y = min.y;
                if (_isAlive)
                {
                    CheckUp();
                    CheckDown();
                }
                _colliderGrid = GetColliderGrid(_colliderPos);

                _colliderPos.x = min.x;
                if (_deltaPos.x != 0 && _isAlive)
                {
                    CheckLeft();
                    CheckRight();
                }
                _colliderGrid = GetColliderGrid(_colliderPos);
            }

            if (!_lastColliderGrid.Equals(_colliderGrid))
            {
                _dynamicCollider.Grid = _colliderGrid;
                ColliderScene2D.Instance.UpdateDynamicNode(_dynamicCollider);
                _lastColliderGrid = _colliderGrid;
            }
        }

        protected virtual void CheckUp()
        {
            if (_deltaPos.y > 0)
            {
                bool flag = false;
                int y = int.MaxValue;
                UnitBase hit = null;
                var min = new IntVec2(_colliderGrid.XMin, _colliderGrid.YMin + _deltaPos.y);
                var grid = new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin, min.y + _colliderGrid.YMax - _colliderGrid.YMin);
                var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue, _dynamicCollider);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    int ymin = 0;
                    if (unit.IsAlive && unit.OnDownHit(this, ref ymin))
                    {
                        if (!_cacheHitUnits.Contains(unit.Guid))
                        {
                            _cacheHitUnits.Add(unit.Guid);
                            unit.OnHit(this);
                            Hit(unit);
                        }
                        flag = true;
                        if (ymin < y)
                        {
                            y = ymin;
                            hit = unit;
                        }
                    }
                }
                if (flag)
                {
                    _colliderPos.y = y;
                    _deltaPos.y = y - _colliderPos.y;
                    _hitUnits[(int)EDirectionType.Up] = hit;
                }
            }
        }

        protected virtual void CheckDown()
        {
            _cacheCheckedDownUnits.Clear();
            if (_deltaPos.y < 0)
            {
                bool flag = false;
                int y = 0;
                int deltaX = int.MaxValue;
                UnitBase hit = null;
                var min = new IntVec2(_colliderGrid.XMin, _colliderGrid.YMin + _deltaPos.y);
                var grid = new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin, min.y + _colliderGrid.YMax - _colliderGrid.YMin);
                var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue, _dynamicCollider);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    int ymin = 0;
                    _cacheCheckedDownUnits.Add(unit.Guid);
                    if (unit.IsAlive && unit.OnUpHit(this, ref ymin))
                    {
                        if (!_cacheHitUnits.Contains(unit.Guid))
                        {
                            _cacheHitUnits.Add(unit.Guid);
                            unit.OnHit(this);
                            Hit(unit);
                        }
                        flag = true;
                        if (ymin > y)
                        {
                            y = ymin;
                            var delta = Mathf.Abs(CenterPos.x - unit.CenterPos.x);
                            if (deltaX > delta)
                            {
                                deltaX = delta;
                                hit = unit;
                            }
                        }
                    }
                }
                if (flag)
                {
                    _colliderPos.y = y;
                    _deltaPos.y = y - _colliderPos.y;
                    _hitUnits[(int)EDirectionType.Down] = hit;
                }
            }
            for (int i = 0; i < _downUnits.Count; i++)
            {
                var unit = _downUnits[i];
                if (_cacheCheckedDownUnits.Contains(unit.Guid))
                {
                    continue;
                }
                if (unit.IsAlive)
                {
                    int ymin = 0;
                    unit.OnUpHit(this, ref ymin);
                }
            }
        }

        protected virtual void CheckLeft()
        {
            if (_deltaPos.x < 0)
            {
                bool flag = false;
                int x = 0;
                UnitBase hit = null;
                var min = new IntVec2(_colliderGrid.XMin + _deltaPos.x, _colliderGrid.YMin);
                var grid = new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin, min.y + _colliderGrid.YMax - _colliderGrid.YMin);
                //var grid = _deltaPos.y >= 0 ? new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin, min.y + _colliderGrid.YMax - _colliderGrid.YMin + _deltaPos.y) : new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin + _deltaPos.y, min.y + _colliderGrid.YMax - _colliderGrid.YMin);
                var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue, _dynamicCollider);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    int xmin = 0;
                    if (unit.IsAlive && unit.OnRightHit(this, ref xmin))
                    {
                        if (!_cacheHitUnits.Contains(unit.Guid))
                        {
                            _cacheHitUnits.Add(unit.Guid);
                            unit.OnHit(this);
                            Hit(unit);
                        }
                        flag = true;
                        if (xmin > x)
                        {
                            x = xmin;
                            hit = unit;
                        }
                    }
                }
                if (flag)
                {
                    _colliderPos.x = x;
                    _hitUnits[(int)EDirectionType.Left] = hit;
                }
            }
        }

        protected virtual void CheckRight()
        {
            if (_deltaPos.x > 0)
            {
                bool flag = false;
                int x = int.MaxValue;
                UnitBase hit = null;
                var min = new IntVec2(_colliderGrid.XMin + _deltaPos.x, _colliderGrid.YMin);
                var grid = new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin, min.y + _colliderGrid.YMax - _colliderGrid.YMin);
                //var grid = _deltaPos.y >= 0 ? new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin, min.y + _colliderGrid.YMax - _colliderGrid.YMin + _deltaPos.y) : new Grid2D(min.x, min.y + _deltaPos.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin, min.y + _colliderGrid.YMax - _colliderGrid.YMin);
                var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue, _dynamicCollider);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    int xmin = 0;
                    if (unit.IsAlive && unit.OnLeftHit(this, ref xmin))
                    {
                        if (!_cacheHitUnits.Contains(unit.Guid))
                        {
                            _cacheHitUnits.Add(unit.Guid);
                            unit.OnHit(this);
                            Hit(unit);
                        }
                        flag = true;
                        if (xmin < x)
                        {
                            x = xmin;
                            hit = unit;
                        }
                    }
                }
                if (flag)
                {
                    _colliderPos.x = x;
                    _hitUnits[(int)EDirectionType.Right] = hit;
                }
            }
        }

        protected virtual void Hit(UnitBase unit)
        {
        }
    }
}
