using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class RigidbodyComponent : UnitComponent
    {
        protected UnitBase[] _hitUnits = new UnitBase[4];
        private static HashSet<IntVec3> _cacheCheckedDownUnits = new HashSet<IntVec3>();
        protected static HashSet<IntVec3> _cacheHitUnits = new HashSet<IntVec3>();
        private static HashSet<IntVec3> _cacheIntersectUnits = new HashSet<IntVec3>();
        protected bool _onClay;
        protected bool _onIce;
        protected IntVec2 _fanForce;
        protected Dictionary<IntVec3, IntVec2> _fanForces = new Dictionary<IntVec3, IntVec2>();
        protected UnitBase _excludeUnit;
        protected UnitBase _downUnit;
        protected List<UnitBase> _downUnits = new List<UnitBase>();
        protected bool _isCalculated;
        public IntVec2 ExtraSpeed;

        public override void Clear()
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
            _downUnit = null;
            _downUnits.Clear();
            _isCalculated = false;
            ExtraSpeed = IntVec2.zero;
        }

        public void SetStepOnIce()
        {
            _onIce = true;
        }

        public override void BeforeLogic()
        {
            base.BeforeLogic();
            _downUnit = null;
            _downUnits.Clear();
            _isCalculated = false;
            _transCom.Speed += ExtraSpeed;
            ExtraSpeed = IntVec2.zero;
        }

        protected void UpdateCollider(IntVec2 min)
        {
            for (int i = 0; i < 4; i++)
            {
                _hitUnits[i] = null;
            }

            _cacheIntersectUnits.Clear();

            if (Unit.IsFreezed)
            {
                _colliderCom.ColliderPos = min;
                _colliderCom.ColliderGrid = _colliderCom.GetColliderGrid(_colliderCom.ColliderPos);
            }
            else
            {
                _cacheHitUnits.Clear();
                _colliderCom.ColliderPosY = min.y;
                if (Unit.IsAlive)
                {
                    CheckUp();
                    CheckDown();
                }

                _colliderCom.ColliderGrid = _colliderCom.GetColliderGrid(_colliderCom.ColliderPos);

                _colliderCom.ColliderPosX = min.x;
                if (_transCom.DeltaPos.x != 0 && Unit.IsAlive)
                {
                    CheckLeft();
                    CheckRight();
                }

                _colliderCom.ColliderGrid = _colliderCom.GetColliderGrid(_colliderCom.ColliderPos);
            }

            if (_colliderCom.UpdateDynamicUnit())
            {
            }
            else if (!Unit.IsFreezed) //静止的时候检测是否交叉
            {
                using (var units = ColliderScene2D.GridCastAllReturnUnits(_colliderCom.ColliderGrid,
                    JoyPhysics2D.GetColliderLayerMask(Unit.DynamicCollider.Layer), float.MinValue, float.MaxValue,
                    Unit.DynamicCollider))
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
            return _colliderCom.ColliderGrid.Intersects(unit.ColliderGrid);
        }

        protected virtual void CheckClimbUnitChangeDir(EClimbState eClimbState)
        {
        }

        protected void CheckUp()
        {
            if (_transCom.DeltaPosY > 0)
            {
                bool flag = false;
                int y = int.MaxValue;
                UnitBase hit = null;
                var min = new IntVec2(Unit.ColliderGrid.XMin, Unit.ColliderGrid.YMin + _transCom.DeltaPos.y);
                var grid = new Grid2D(min.x, min.y, min.x + Unit.ColliderGrid.XMax - Unit.ColliderGrid.XMin,
                    min.y + Unit.ColliderGrid.YMax - Unit.ColliderGrid.YMin);
                using (var units = ColliderScene2D.GridCastAllReturnUnits(grid,
                    JoyPhysics2D.GetColliderLayerMask(Unit.DynamicCollider.Layer), float.MinValue, float.MaxValue,
                    Unit.DynamicCollider))
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        if (unit.IsAlive && unit != _excludeUnit)
                        {
                            CheckIntersect(unit);
                            var checkHit = unit.GetComponent<BlockComponent>();
                            if (checkHit != null)
                            {
                                int ymin = 0;
                                if (checkHit.OnDownHit(Unit, ref ymin, true))
                                {
                                    CheckHit(unit, EDirectionType.Up);
                                }

                                if (!Intersect(unit) && checkHit.OnDownHit(Unit, ref ymin))
                                {
                                    int speedY = _transCom.ExtraDeltaPos.y;
                                    if ((unit.IsActor || unit is Box) && speedY > 0 &&
                                        unit.ColliderGrid.YMin == Unit.ColliderGrid.YMax + 1 &&
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
                }

                if (flag)
                {
                    _colliderCom.ColliderPosY = y;
                    _transCom.DeltaPosY = y - _colliderCom.ColliderPosY;
                    _hitUnits[(int) EDirectionType.Up] = hit;
                }
            }
        }

        protected void CheckDown()
        {
            _cacheCheckedDownUnits.Clear();
            if (_transCom.DeltaPosY < 0)
            {
                bool flag = false;
                int y = 0;
                int deltaX = int.MaxValue;
                UnitBase hit = null;
                var min = new IntVec2(_colliderCom.ColliderGrid.XMin,
                    _colliderCom.ColliderGrid.YMin + _transCom.DeltaPosY);
                var grid = new Grid2D(min.x, min.y,
                    min.x + _colliderCom.ColliderGrid.XMax - _colliderCom.ColliderGrid.XMin,
                    min.y + _colliderCom.ColliderGrid.YMax - _colliderCom.ColliderGrid.YMin);
                using (var units = ColliderScene2D.GridCastAllReturnUnits(grid,
                    JoyPhysics2D.GetColliderLayerMask(Unit.DynamicCollider.Layer), float.MinValue, float.MaxValue,
                    Unit.DynamicCollider))
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        _cacheCheckedDownUnits.Add(unit.Guid);
                        if (unit.IsAlive && unit != _excludeUnit)
                        {
                            CheckIntersect(unit);
                            int ymin = 0;
                            if (unit.OnUpHit(Unit, ref ymin, true))
                            {
                                CheckHit(unit, EDirectionType.Down);
                            }

                            if (!Intersect(unit) && unit.OnUpHit(Unit, ref ymin))
                            {
                                flag = true;
                                if (ymin > y)
                                {
                                    y = ymin;
                                    var delta = Mathf.Abs(_transCom.CenterDownPos.x - unit.CenterDownPos.x);
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
                    _colliderCom.ColliderPosY = y;
                    _transCom.DeltaPosY = y - _colliderCom.ColliderPosY;
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
                    if (unit.OnUpHit(Unit, ref ymin))
                    {
                        CheckHit(unit, EDirectionType.Down);
                    }
                }
            }
        }

        protected void CheckLeft()
        {
            if (_transCom.DeltaPosX < 0)
            {
                bool flag = false;
                int x = 0;
                UnitBase hit = null;
                var min = new IntVec2(_colliderCom.ColliderGrid.XMin + _transCom.DeltaPosX,
                    _colliderCom.ColliderGrid.YMin);
                var grid = new Grid2D(min.x, min.y,
                    min.x + _colliderCom.ColliderGrid.XMax - _colliderCom.ColliderGrid.XMin,
                    min.y + _colliderCom.ColliderGrid.YMax - _colliderCom.ColliderGrid.YMin);
                using (var units = ColliderScene2D.GridCastAllReturnUnits(grid,
                    JoyPhysics2D.GetColliderLayerMask(Unit.DynamicCollider.Layer), float.MinValue, float.MaxValue,
                    Unit.DynamicCollider))
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        if (unit.IsAlive && unit != _excludeUnit)
                        {
                            CheckIntersect(unit);
                            int xmin = 0;
                            if (unit.OnRightHit(Unit, ref xmin, true))
                            {
                                CheckHit(unit, EDirectionType.Left);
                            }

                            if (!Intersect(unit) && unit.OnRightHit(Unit, ref xmin))
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
                    _colliderCom.ColliderPosX = x;
                    _hitUnits[(int) EDirectionType.Left] = hit;
                    CheckClimbUnitChangeDir(EClimbState.Right);
                }
            }
        }

        protected void CheckRight()
        {
            if (_transCom.DeltaPosX > 0)
            {
                bool flag = false;
                int x = int.MaxValue;
                UnitBase hit = null;
                var min = new IntVec2(_colliderCom.ColliderGrid.XMin + _transCom.DeltaPosX,
                    _colliderCom.ColliderGrid.YMin);
                var grid = new Grid2D(min.x, min.y,
                    min.x + _colliderCom.ColliderGrid.XMax - _colliderCom.ColliderGrid.XMin,
                    min.y + _colliderCom.ColliderGrid.YMax - _colliderCom.ColliderGrid.YMin);
                using (var units = ColliderScene2D.GridCastAllReturnUnits(grid,
                    JoyPhysics2D.GetColliderLayerMask(Unit.DynamicCollider.Layer), float.MinValue, float.MaxValue,
                    Unit.DynamicCollider))
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        if (unit.IsAlive && unit != _excludeUnit)
                        {
                            CheckIntersect(unit);
                            int xmin = 0;
                            if (unit.OnLeftHit(Unit, ref xmin, true))
                            {
                                CheckHit(unit, EDirectionType.Right);
                            }

                            if (!Intersect(unit) && unit.OnLeftHit(Unit, ref xmin))
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
                    _colliderCom.ColliderPosX = x;
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
                unit.OnIntersect(Unit);
            }
        }

        protected void CheckHit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (!_cacheHitUnits.Contains(unit.Guid))
            {
                _cacheHitUnits.Add(unit.Guid);
                Unit.Hit(unit, eDirectionType);
            }
        }

        public void InFan(UnitBase fanUnit, IntVec2 force)
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

        public void OutFan(UnitBase fanUnit)
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

        public IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            if (!Unit.IsInterest)
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
                        var deltaPos = _downUnits[i].GetDeltaImpactPos(Unit);
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
        
        protected TransComponent _transCom;
        protected ColliderComponent _colliderCom;

        public override void Awake()
        {
            base.Awake();
            _transCom = Unit.GetComponent<TransComponent>();
            _colliderCom = Unit.GetComponent<ColliderComponent>();
        }

        public override void Dispose()
        {
            _transCom = null;
            _colliderCom = null;
            base.Dispose();
        }
    }
}