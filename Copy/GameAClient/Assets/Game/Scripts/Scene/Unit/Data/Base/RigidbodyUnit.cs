﻿/********************************************************************
** Filename : RigidbodyUnit
** Author : Dong
** Date : 2017/3/11 星期六 下午 1:34:31
** Summary : RigidbodyUnit
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    public class RigidbodyUnit : BlockBase
    {
        [SerializeField] protected UnitBase[] _hitUnits = new UnitBase[4];
        private static HashSet<IntVec3> _cacheCheckedDownUnits = new HashSet<IntVec3>();

        /// <summary>
        /// 是否相撞
        /// </summary>
        protected static HashSet<IntVec3> _cacheHitUnits = new HashSet<IntVec3>();

        /// <summary>
        /// 是否相交
        /// </summary>
        private static HashSet<IntVec3> _cacheIntersectUnits = new HashSet<IntVec3>();

        protected bool _onClay;
        protected bool _onIce;
        [SerializeField] protected IntVec2 _fanForce;
        protected Dictionary<IntVec3, IntVec2> _fanForces = new Dictionary<IntVec3, IntVec2>();

        protected UnitBase _excludeUnit;

        public override bool IsRigidbody
        {
            get { return true; }
        }

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
            _excludeUnit = null;
        }

        public override void SetStepOnIce()
        {
            _onIce = true;
        }

        public override void BeforeLogic()
        {
            _downUnit = null;
            _downUnits.Clear();
            _isCalculated = false;
            Speed += ExtraSpeed;
            ExtraSpeed = IntVec2.zero;
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
                ColliderPos = min;
                ColliderGrid = GetColliderGrid(ColliderPos);
            }
            else
            {
                _cacheHitUnits.Clear();
                ColliderPos.y = min.y;
                if (_isAlive)
                {
                    CheckUp();
                    CheckDown();
                }

                ColliderGrid = GetColliderGrid(ColliderPos);

                ColliderPos.x = min.x;
                if (_deltaPos.x != 0 && _isAlive)
                {
                    CheckLeft();
                    CheckRight();
                }

                ColliderGrid = GetColliderGrid(ColliderPos);
            }

            if (!LastColliderGrid.Equals(ColliderGrid))
            {
                _dynamicCollider.Grid = ColliderGrid;
                ColliderScene2D.CurScene.UpdateDynamicUnit(this, LastColliderGrid);
                LastColliderGrid = ColliderGrid;
            }
            else if (!_isFreezed) //静止的时候检测是否交叉
            {
                using (var units = ColliderScene2D.GridCastAllReturnUnits(ColliderGrid,
                    JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue,
                    _dynamicCollider))
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        if (unit.IsAlive && unit != _excludeUnit)
                        {
                            CheckIntersect(unit);
                        }
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
            return ColliderGrid.Intersects(unit.ColliderGrid);
        }

        protected virtual void CheckClimbUnitChangeDir(EClimbState eClimbState)
        {
        }

        protected virtual void CheckUp()
        {
            if (_deltaPos.y > 0)
            {
                bool flag = false;
                int y = int.MaxValue;
                UnitBase hit = null;
                var min = new IntVec2(ColliderGrid.XMin, ColliderGrid.YMin + _deltaPos.y);
                var grid = new Grid2D(min.x, min.y, min.x + ColliderGrid.XMax - ColliderGrid.XMin,
                    min.y + ColliderGrid.YMax - ColliderGrid.YMin);
                using (var units = ColliderScene2D.GridCastAllReturnUnits(grid,
                    JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue,
                    _dynamicCollider))
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        if (unit.IsAlive && unit != _excludeUnit)
                        {
                            CheckIntersect(unit);
                            int ymin = 0;
                            if (unit.OnDownHit(this, ref ymin, true))
                            {
                                CheckHit(unit, EDirectionType.Up);
                            }

                            if (!Intersect(unit) && unit.OnDownHit(this, ref ymin))
                            {
                                int speedY = _extraDeltaPos.y;
                                if ((unit.IsActor || unit is Box) && _extraDeltaPos.y > 0 &&
                                    unit.ColliderGrid.YMin == ColliderGrid.YMax + 1 &&
                                    unit.CheckUpValid(ref speedY, ref unit, true))
                                {
                                }
                                else
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
                    }
                }

                if (flag)
                {
                    ColliderPos.y = y;
                    _deltaPos.y = y - ColliderPos.y;
                    _hitUnits[(int) EDirectionType.Up] = hit;
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
                var min = new IntVec2(ColliderGrid.XMin, ColliderGrid.YMin + _deltaPos.y);
                var grid = new Grid2D(min.x, min.y, min.x + ColliderGrid.XMax - ColliderGrid.XMin,
                    min.y + ColliderGrid.YMax - ColliderGrid.YMin);
                using (var units = ColliderScene2D.GridCastAllReturnUnits(grid,
                    JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue,
                    _dynamicCollider))
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        _cacheCheckedDownUnits.Add(unit.Guid);
                        if (unit.IsAlive && unit != _excludeUnit)
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
                }

                if (flag)
                {
                    ColliderPos.y = y;
                    _deltaPos.y = y - ColliderPos.y;
                    _hitUnits[(int) EDirectionType.Down] = hit;
                    CheckClimbUnitChangeDir(EClimbState.Up);
                }
            }

            for (int i = 0; i < _downUnits.Count; i++)
            {
                var unit = _downUnits[i];
                if (_cacheCheckedDownUnits.Contains(unit.Guid))
                {
                    continue;
                }

                if (unit.IsAlive && unit != _excludeUnit)
                {
                    CheckIntersect(unit);
                    int ymin = 0;
                    if (unit.OnUpHit(this, ref ymin))
                    {
                        CheckHit(unit, EDirectionType.Down);
                    }
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
                var min = new IntVec2(ColliderGrid.XMin + _deltaPos.x, ColliderGrid.YMin);
                var grid = new Grid2D(min.x, min.y, min.x + ColliderGrid.XMax - ColliderGrid.XMin,
                    min.y + ColliderGrid.YMax - ColliderGrid.YMin);
                using (var units = ColliderScene2D.GridCastAllReturnUnits(grid,
                    JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue,
                    _dynamicCollider))
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        if (unit.IsAlive && unit != _excludeUnit)
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
                }

                if (flag)
                {
                    ColliderPos.x = x;
                    _hitUnits[(int) EDirectionType.Left] = hit;
                    CheckClimbUnitChangeDir(EClimbState.Right);
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
                var min = new IntVec2(ColliderGrid.XMin + _deltaPos.x, ColliderGrid.YMin);
                var grid = new Grid2D(min.x, min.y, min.x + ColliderGrid.XMax - ColliderGrid.XMin,
                    min.y + ColliderGrid.YMax - ColliderGrid.YMin);
                using (var units = ColliderScene2D.GridCastAllReturnUnits(grid,
                    JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue,
                    _dynamicCollider))
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        if (unit.IsAlive && unit != _excludeUnit)
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
                }

                if (flag)
                {
                    ColliderPos.x = x;
                    _hitUnits[(int) EDirectionType.Right] = hit;
                    CheckClimbUnitChangeDir(EClimbState.Left);
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

        protected virtual void CheckHit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (!_cacheHitUnits.Contains(unit.Guid))
            {
                _cacheHitUnits.Add(unit.Guid);
                Hit(unit, eDirectionType);
            }
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

            _fanForce = IntVec2.zero;

            using (var iter = _fanForces.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    _fanForce += iter.Current.Value;
                }
            }

            ExtraSpeed.x = _fanForce.x;
        }

        internal override void OutFan(UnitBase fanUnit)
        {
            _fanForces.Remove(fanUnit.Guid);
            _fanForce = IntVec2.zero;
            if (_fanForces.Count > 0)
            {
                using (var iter = _fanForces.GetEnumerator())
                {
                    while (iter.MoveNext())
                    {
                        _fanForce += iter.Current.Value;
                    }
                }
            }
        }

        public override IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            if (!_isInterest)
            {
                return IntVec2.zero;
            }

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