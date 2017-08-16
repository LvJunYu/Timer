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
        /// <summary>
        /// 是否相撞
        /// </summary>
        private static HashSet<IntVec3> _cacheHitUnits = new HashSet<IntVec3>();
        /// <summary>
        /// 是否相交
        /// </summary>
        private static HashSet<IntVec3> _cacheIntersectUnits = new HashSet<IntVec3>();
        
        protected bool _onClay;
        protected bool _onIce;
        [SerializeField] protected IntVec2 _fanForce;
        protected Dictionary<IntVec3, IntVec2> _fanForces = new Dictionary<IntVec3, IntVec2>();

        protected override void Clear()
        {
            base.Clear();
            for (int i = 0; i < 4; i++)
            {
                _hitUnits[i] = null;
            }
            _cacheCheckedDownUnits.Clear();
            _cacheHitUnits.Clear();
            _cacheIntersectUnits.Clear();
            _onClay = false;
            _onIce = false;
            _fanForce = IntVec2.zero;
            _fanForces.Clear();
        }
        
        public override void SetStepOnClay()
        {
            _onClay = true;
        }

        public override void SetStepOnIce()
        {
            _onIce = true;
        }
        
        public override void CheckStart()
        {
            _downUnit = null;
            _downUnits.Clear();
            _isCalculated = false;
            Speed += ExtraSpeed;
            ExtraSpeed = IntVec2.zero;
            if (_curBanInputTime > 0)
            {
                _curBanInputTime--;
            }
        }

        protected override void UpdateCollider(IntVec2 min)
        {
            for (int i = 0; i < 4; i++)
            {
                _hitUnits[i] = null;
            }
            _cacheIntersectUnits.Clear();
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
                ColliderScene2D.Instance.UpdateDynamicNode(_dynamicCollider, _lastColliderGrid);
                _lastColliderGrid = _colliderGrid;
            }
            else if(!_isFreezed)  //静止的时候检测是否交叉
            {
                var units = ColliderScene2D.GridCastAllReturnUnits(_colliderGrid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue, _dynamicCollider);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive)
                    {
                        CheckIntersect(unit);
                    }
                }
            }
        }

        private bool Intersect(UnitBase unit)
        {
//            //俩移动物体不相交
//            if (unit.DynamicCollider != null)
//            {
//                return false;
//            }
            return _colliderGrid.Intersects(unit.ColliderGrid);
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
                    if (unit.IsAlive)
                    {
                        CheckIntersect(unit);
                        int ymin = 0;
                        if (unit.OnDownHit(this, ref ymin, true))
                        {
                            CheckHit(unit, EDirectionType.Up);
                        }
                        if (!Intersect(unit) && unit.OnDownHit(this, ref ymin))
                        {
                            flag = true;
                            if (ymin < y)
                            {
                                y = ymin;
                                hit = unit;
                            }
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
                    _cacheCheckedDownUnits.Add(unit.Guid);
                    if (unit.IsAlive)
                    {
                        CheckIntersect(unit);
                        int ymin = 0;
                        if (unit.OnUpHit(this, ref ymin, true))
                        {
                            CheckHit(unit, EDirectionType.Down);
                        }
                        if (!Intersect(unit) && unit.OnUpHit(this, ref ymin))
                        {
                            flag = true;
                            if (ymin > y)
                            {
                                y = ymin;
                                var delta = Mathf.Abs(CenterDownPos.x - unit.CenterDownPos.x);
                                if (deltaX > delta)
                                {
                                    deltaX = delta;
                                    hit = unit;
                                }
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
                var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue, _dynamicCollider);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive)
                    {
                        CheckIntersect(unit);
                        int xmin = 0;
                        if (unit.OnRightHit(this, ref xmin, true))
                        {
                            CheckHit(unit, EDirectionType.Left);
                        }
                        if (!Intersect(unit) && unit.OnRightHit(this, ref xmin))
                        {
                            flag = true;
                            if (xmin > x)
                            {
                                x = xmin;
                                hit = unit;
                            }
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
                var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue, _dynamicCollider);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive)
                    {
                        CheckIntersect(unit);
                        int xmin = 0;
                        if (unit.OnLeftHit(this, ref xmin, true))
                        {
                            CheckHit(unit, EDirectionType.Right);
                        }
                        if (!Intersect(unit) && unit.OnLeftHit(this, ref xmin))
                        {
                            flag = true;
                            if (xmin < x)
                            {
                                x = xmin;
                                hit = unit;
                            }
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

        private void CheckIntersect(UnitBase unit)
        {
            if (!_cacheIntersectUnits.Contains(unit.Guid))
            {
                _cacheIntersectUnits.Add(unit.Guid);
                unit.OnIntersect(this);
            }
        }

        private void CheckHit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (!_cacheHitUnits.Contains(unit.Guid))
            {
                _cacheHitUnits.Add(unit.Guid);
                Hit(unit, eDirectionType);
            }
        }

        protected virtual void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
        }
        
        internal override void InFan(UnitBase fanUnit, IntVec2 force)
        {
            if (_fanForces.ContainsKey(fanUnit.Guid))
            {
                _fanForces[fanUnit.Guid] = force;
            }
            else
            {
                _fanForces.Add(fanUnit.Guid, force);
            }
            _fanForce= IntVec2.zero;
            var iter = _fanForces.GetEnumerator();
            while (iter.MoveNext())
            {
                _fanForce += iter.Current.Value;
            }
            ExtraSpeed.x = _fanForce.x;
        }

        internal override void OutFan(UnitBase fanUnit)
        {
            _fanForces.Remove(fanUnit.Guid);
            _fanForce = IntVec2.zero;
            if (_fanForces.Count > 0)
            {
                var iter = _fanForces.GetEnumerator();
                while (iter.MoveNext())
                {
                    _fanForce += iter.Current.Value;
                }
            }
        }
        
        public override IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            IntVec2 deltaImpactPos = IntVec2.zero;
            if (!_isCalculated)
            {
                if (_downUnits.Count > 0)
                {
                    int right = 0;
                    int left = 0;
                    int deltaY = int.MinValue;
                    for (int i = 0; i < _downUnits.Count; i++)
                    {
                        var deltaPos = _downUnits[i].GetDeltaImpactPos(this);
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
                    deltaImpactPos = new IntVec2(deltaX, deltaY);
                }
                _isCalculated = true;
            }
            return deltaImpactPos;
        }
    }
}
