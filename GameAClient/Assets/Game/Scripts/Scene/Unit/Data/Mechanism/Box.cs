﻿/********************************************************************
** Filename : BoxUnit
** Author : Dong
** Date : 2017/1/9 星期一 下午 11:30:13
** Summary : Box
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 5004, Type = typeof(Box))]
    public class Box : RigidbodyUnit
    {
        protected bool _isHoldingByMain;

        protected EDirectionType _directionRelativeMain;

        public bool IsHoldingByMain
        {
            get { return _isHoldingByMain; }
            set { _isHoldingByMain = value; }
        }

        public EDirectionType DirectionRelativeMain
        {
            get { return _directionRelativeMain; }
            set { _directionRelativeMain = value; }
        }

        protected override void Clear()
        {
            _isHoldingByMain = false;
            base.Clear();
        }

        protected override void OnDead()
        {
            if (!_isAlive)
            {
                return;
            }
            PlayMode.Instance.DestroyUnit(this);
            base.OnDead();
        }

        public override void UpdateLogic()
        {
            if (_isAlive && _isStart)
            {
                bool air = false;
                int friction = 0;
                if (SpeedY != 0)
                {
                    air = true;
                }
                if (!air)
                {
                    bool downExist = false;
                    var units = EnvManager.RetriveDownUnits(this);
                    for (int i = 0; i < units.Count; i++)
                    {
                        var unit = units[i];
                        if (unit.IsMain || unit.IsMonster)
                        {
                            continue;
                        }
                        int ymin = 0;
                        if (unit != null && unit.IsAlive && CheckOnFloor(unit) && unit.OnUpHit(this, ref ymin, true))
                        {
                            downExist = true;
                            _grounded = true;
                            _downUnits.Add(unit);
                            unit.UpUnits.Add(this);
                            if (unit.Friction > friction)
                            {
                                friction = unit.Friction;
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
                }
                SpeedX = Util.ConstantLerp(SpeedX, 0, friction);
                if (!_grounded)
                {
                    SpeedY -= 15;
                    if (SpeedY < -160)
                    {
                        SpeedY = -160;
                    }
                }
            }
        }

        public override void UpdateView(float deltaTime)
        {
            if (_isStart && _isAlive && _dynamicCollider != null)
            {
                _deltaPos = _speed + _extraDeltaPos;
                if (_isHoldingByMain)
                {
                    _deltaPos.x = 0;
                    var mainUnit = PlayMode.Instance.MainUnit;
                    if (_deltaPos.y != 0)
                    {
                        mainUnit.OnBoxHoldingChanged();
                    }
                    else
                    {
                        var deltaMainPos = mainUnit.Speed + mainUnit.ExtraDeltaPos;
                        _deltaPos += IntVec2.right*deltaMainPos.x;
                    }
                }
                _curPos += _deltaPos;
                LimitPos();
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                if (_view != null)
                {
                    _trans.position = GetTransPos();
                }
                if (OutOfMap())
                {
                    return;
                }
            }
        }

        protected override void CheckDown()
        {
            if (_deltaPos.y < 0)
            {
                bool flag = false;
                int y = 0;
                UnitBase hit = null;
                var min = new IntVec2(_colliderGrid.XMin, _colliderGrid.YMin + _deltaPos.y);
                var grid = new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin, min.y + _colliderGrid.YMax - _colliderGrid.YMin);
                var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue, _dynamicCollider);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    int ymin = 0;
                    if (unit.IsAlive && unit.OnUpHit(this, ref ymin))
                    {
                        flag = true;
                        if (ymin > y)
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
                    _hitUnits[(int)EDirectionType.Down] = hit;
                }
            }
        }
    }
}
