/********************************************************************
** Filename : BulletBase
** Author : Dong
** Date : 2017/3/23 星期四 下午 3:11:11
** Summary : BulletBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    /// <summary>
    /// 子弹需要做成池
    /// </summary>
    [Poolable(MinPoolSize = 10, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class BulletBase : UnitBase, IPoolableObject
    {
        protected bool _run;
        protected IntVec2 _speed;
        protected SkillBase _skill;

        public void OnGet()
        {
        }

        public void OnFree()
        {
            Clear();
        }

        public void OnDestroyObject()
        {
        }

        protected override void Clear()
        {
            base.Clear();
            _run = false;
            _speed = IntVec2.zero;
            _skill = null;
        }

        public virtual void Run(SkillBase skill)
        {
            _run = true;
            _skill = skill;
            SetPos(_skill.Owner.FirePos);
            _curMoveDirection = _skill.Owner.FireDirection;
            switch (_curMoveDirection)
            {
                case EMoveDirection.Up:
                    _speed = _skill.BulletSpeed * IntVec2.up;
                    break;
                case EMoveDirection.Right:
                    _speed = _skill.BulletSpeed * IntVec2.right;
                    break;
                case EMoveDirection.Down:
                    _speed = _skill.BulletSpeed * IntVec2.down;
                    break;
                case EMoveDirection.Left:
                    _speed = _skill.BulletSpeed * IntVec2.left;
                    break;
            }
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
                IntVec2 pointA = IntVec2.zero, pointB = IntVec2.zero;
                GM2DTools.GetBorderPoint(_colliderGrid, _curMoveDirection, ref pointA, ref pointB);
                var checkGrid = SceneQuery2D.GetGrid(pointA, pointB, (byte)(_curMoveDirection - 1), Math.Max(_speed.x, _speed.y));
                if (!DataScene2D.Instance.IsInTileMap(checkGrid))
                {
                    OnDead();
                    return;
                }
                var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.BulletHitLayer, float.MinValue, float.MaxValue, _dynamicCollider);
                for (int i = 0; i < units.Count; i++)
                {
                    if (GM2DTools.OnDirectionHit(units[i], this, _curMoveDirection))
                    {
                        DoHit(units[i]);
                        break;
                    }
                }
            }
        }

        protected virtual void DoHit(UnitBase unit)
        {
            if (_skill.Plus)
            {
                DoEdge(unit);
            }
            OnDead();
            if (unit is Switch)
            {
                //
            }
        }

        protected virtual void DoEdge(UnitBase unit)
        {
            switch (_curMoveDirection)
            {
                case EMoveDirection.Up:
                    {
                        int centerPoint = (_colliderGrid.XMax + 1 + _colliderGrid.XMin) / 2;
                        unit.DoEdge(centerPoint - _skill.Radius, centerPoint + _skill.Radius, EDirectionType.Down,
                            _skill.ESkillType);
                    }
                    break;
                case EMoveDirection.Down:
                    {
                        int centerPoint = (_colliderGrid.XMax + 1 + _colliderGrid.XMin) / 2;
                        unit.DoEdge(centerPoint - _skill.Radius, centerPoint + _skill.Radius, EDirectionType.Up,
                            _skill.ESkillType);
                    }
                    break;
                case EMoveDirection.Left:
                    {
                        int centerPoint = (_colliderGrid.YMax + 1 + _colliderGrid.YMin) / 2;
                        unit.DoEdge(centerPoint - _skill.Radius, centerPoint + _skill.Radius, EDirectionType.Right,
                            _skill.ESkillType);
                    }
                    break;
                case EMoveDirection.Right:
                    {
                        int centerPoint = (_colliderGrid.YMax + 1 + _colliderGrid.YMin) / 2;
                        unit.DoEdge(centerPoint - _skill.Radius, centerPoint + _skill.Radius, EDirectionType.Left,
                            _skill.ESkillType);
                    }
                    break;
            }
        }

        protected override void OnDead()
        {
            base.OnDead();
            PlayMode.Instance.DestroyUnit(this);
        }

        public override void UpdateView(float deltaTime)
        {
            if (!_run)
            {
                return;
            }
            if (_isAlive)
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
                ColliderScene2D.Instance.UpdateDynamicNode(_dynamicCollider);
                _lastColliderGrid = _colliderGrid;
            }
        }
    }
}
