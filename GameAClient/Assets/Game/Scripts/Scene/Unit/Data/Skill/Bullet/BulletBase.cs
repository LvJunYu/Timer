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
    public class BulletBase : RigidbodyUnit, IPoolableObject
    {
        protected bool _run;
        protected SkillBase _skill;
        /// <summary>
        /// 速度
        /// </summary>
        protected IntVec2 _speed;
        /// <summary>
        /// 是否被阻挡
        /// </summary>
        protected byte _blocked;
        /// <summary>
        /// 角度
        /// </summary>
        protected int _angle;
        /// <summary>
        /// 初始位置
        /// </summary>
        protected IntVec2 _originPos;

        protected int _maskRandom;

        public int Angle
        {
            get { return _angle; }
        }

        public void OnGet()
        {
        }

        public void OnFree()
        {
            //赋值为空
            _dynamicCollider = null;
            Clear();
        }

        public void OnDestroyObject()
        {
        }

        internal override void Reset()
        {
            base.Reset();
            PlayMode.Instance.DestroyUnit(this);
        }

        protected override void Clear()
        {
            _run = false;
            _skill = null;
            _speed = IntVec2.zero;
            _blocked = 0;
            _angle = 0;
            _originPos = IntVec2.zero;
            base.Clear();
        }

        public virtual void Run(SkillBase skill)
        {
            _maskRandom = UnityEngine.Random.Range(0, 2);
            _originPos = CenterPos;
            _run = true;
            _skill = skill;
            _angle = _skill.Owner.ShootAngle;
            var rad = _angle * Mathf.Deg2Rad;
            _speed = new IntVec2((int)(_skill.BulletSpeed * Math.Sin(rad)), (int)(_skill.BulletSpeed * Math.Cos(rad)));
            _trans.eulerAngles = new Vector3(0, 0, -_angle);
            if (_animation != null)
            {
                _animation.Init("Run", false);
            }
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
                //超出最大射击距离
                if ((CenterPos - _originPos).SqrMagnitude() >= _skill.Range * _skill.Range)
                {
                    var rad = _angle*Mathf.Deg2Rad;
                    CenterPos = _originPos + new IntVec2((int)(_skill.Range * Math.Sin(rad)), (int)(_skill.Range * Math.Cos(rad)));
                    _blocked = 1;
                }
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
                if (_blocked > 0)
                {
                    OnBlocked();
                }
            }
        }

        protected virtual void OnBlocked()
        {
            //检查地块范围
            var checkGrid = _colliderGrid.Enlarge(_skill.Radius);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer));
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (unit.IsAlive)
                {
                    Effect(unit, checkGrid);
                }
            }
            _isAlive = false;
            --Life;
            if (_view != null)
            {
                GameAudioManager.Instance.PlaySoundsEffects(_tableUnit.DestroyAudioName);
                string effectName = _tableUnit.DestroyEffectName;
                if (_blocked == 1)
                {
                    effectName = "M1EffectBlisterStart";
                }
                GameParticleManager.Instance.Emit(effectName, _trans.position, new Vector3(0, 0, _angle), Vector3.one);
            }
            Clear();
            PlayMode.Instance.DestroyUnit(this);
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            _blocked = 1;
        }

        protected void Effect(UnitBase unit, Grid2D checkGrid)
        {
            if (unit.CanPainted)
            {
                _blocked = 2;
                int length = ConstDefineGM2D.ServerTileScale;
                var guid = unit.Guid;
                UnitBase neighborUnit;
                if (_curPos.y < unit.ColliderGrid.YMin)
                {
                    if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x, guid.y - length, guid.z), out neighborUnit))
                    {
                        DoPaint(unit, EDirectionType.Down);
                    }
                }
                else if (_curPos.y > unit.ColliderGrid.YMax)
                {
                    if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x, guid.y + length, guid.z), out neighborUnit))
                    {
                        DoPaint(unit, EDirectionType.Up);
                    }
                }
                if (_curPos.x < unit.ColliderGrid.XMin)
                {
                    if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x - length, guid.y, guid.z), out neighborUnit))
                    {
                        DoPaint(unit, EDirectionType.Left);
                    }
                }
                else if (_curPos.x > unit.ColliderGrid.XMax)
                {
                    if (!ColliderScene2D.Instance.TryGetUnit(new IntVec3(guid.x + length, guid.y, guid.z), out neighborUnit))
                    {
                        DoPaint(unit, EDirectionType.Right);
                    }
                }
            }
            else if (unit.IsHero && unit.EffectMgr != null)
            {
                _blocked = 2;
                var effectMgr = unit.EffectMgr;
                switch (_skill.ESkillType)
                {
                    case ESkillType.Water:
                        //解除火、黏液；速度变慢
                        effectMgr.AddEffect<EffectWater>(this);
                        break;
                    case ESkillType.Fire:
                        effectMgr.AddEffect<EffectFire>(this);
                        break;
                    case ESkillType.Ice:
                        //被冻成冰块
                        effectMgr.AddEffect<EffectIce>(this);
                        break;
                    case ESkillType.Jelly:
                        //被弹飞
                        effectMgr.AddEffect<EffectJelly>(this);
                        effectMgr.RemoveEffect<EffectJelly>();
                        break;
                    case ESkillType.Clay:
                        //被黏住 但可以攻击
                        effectMgr.AddEffect<EffectClay>(this);
                        break;
                }
            }
        }

        protected virtual void DoPaint(UnitBase unit, EDirectionType eDirectionType)
        {
            var paintDepth = PaintBlock.TileOffsetHeight;
            switch (eDirectionType)
            {
                case EDirectionType.Down:
                    {
                        int centerPoint = (_colliderGrid.XMax + 1 + _colliderGrid.XMin) / 2;
                        var start = centerPoint - _skill.Radius;
                        var end = centerPoint + _skill.Radius;
                        unit.DoPaint(start, end, EDirectionType.Down, _skill.ESkillType, _maskRandom);

                        if (start <= unit.ColliderGrid.XMin)
                        {
                            start = unit.ColliderGrid.YMin;
                            end = unit.ColliderGrid.YMin + paintDepth;
                            unit.DoPaint(start, end, EDirectionType.Left, _skill.ESkillType, _maskRandom, false);
                        }
                        if (end >= unit.ColliderGrid.XMax)
                        {
                            start = unit.ColliderGrid.YMin;
                            end = unit.ColliderGrid.YMin + paintDepth;
                            unit.DoPaint(start, end, EDirectionType.Right, _skill.ESkillType, _maskRandom, false);
                        }
                    }
                    break;
                case EDirectionType.Up:
                    {
                        int centerPoint = (_colliderGrid.XMax + 1 + _colliderGrid.XMin) / 2;
                        var start = centerPoint - _skill.Radius;
                        var end = centerPoint + _skill.Radius;
                        unit.DoPaint(start, end, EDirectionType.Up, _skill.ESkillType, _maskRandom);
                        if (start <= unit.ColliderGrid.XMin)
                        {
                            start = unit.ColliderGrid.YMax - paintDepth;
                            end = unit.ColliderGrid.YMax;
                            unit.DoPaint(start, end, EDirectionType.Left, _skill.ESkillType, _maskRandom, false);
                        }
                        if (end >= unit.ColliderGrid.XMax)
                        {
                            start = unit.ColliderGrid.YMax - paintDepth;
                            end = unit.ColliderGrid.YMax;
                            unit.DoPaint(start, end, EDirectionType.Right, _skill.ESkillType, _maskRandom, false);
                        }
                    }
                    break;
                case EDirectionType.Right:
                    {
                        int centerPoint = (_colliderGrid.YMax + 1 + _colliderGrid.YMin) / 2;
                        var start = centerPoint - _skill.Radius;
                        var end = centerPoint + _skill.Radius;
                        unit.DoPaint(start, end, EDirectionType.Right, _skill.ESkillType, _maskRandom);
                        if (start <= unit.ColliderGrid.YMin)
                        {
                            start = unit.ColliderGrid.XMax - paintDepth;
                            end = unit.ColliderGrid.XMax;
                            unit.DoPaint(start, end, EDirectionType.Down, _skill.ESkillType, _maskRandom, false);
                        }
                        if (end >= unit.ColliderGrid.YMax)
                        {
                            start = unit.ColliderGrid.XMax - paintDepth;
                            end = unit.ColliderGrid.XMax;
                            unit.DoPaint(start, end, EDirectionType.Up, _skill.ESkillType, _maskRandom, false);
                        }
                    }
                    break;
                case EDirectionType.Left:
                    {
                        int centerPoint = (_colliderGrid.YMax + 1 + _colliderGrid.YMin) / 2;
                        var start = centerPoint - _skill.Radius;
                        var end = centerPoint + _skill.Radius;
                        unit.DoPaint(start, end, EDirectionType.Left, _skill.ESkillType, _maskRandom);
                        if (start <= unit.ColliderGrid.YMin)
                        {
                            start = unit.ColliderGrid.XMin;
                            end = unit.ColliderGrid.XMin + paintDepth;
                            unit.DoPaint(start, end, EDirectionType.Down, _skill.ESkillType, _maskRandom, false);
                        }
                        if (end >= unit.ColliderGrid.YMax)
                        {
                            start = unit.ColliderGrid.XMin;
                            end = unit.ColliderGrid.XMin + paintDepth;
                            unit.DoPaint(start, end, EDirectionType.Up, _skill.ESkillType, _maskRandom, false);
                        }
                    }
                    break;
            }
        }

        private bool IsShootUp()
        {
            return _angle < 90 || _angle > 270;
        }

        private bool IsShootDown()
        {
            return _angle > 90 && _angle < 270;
        }

        private bool IsShootRight()
        {
            return _angle > 0 && _angle < 180;
        }

        private bool IsShootLeft()
        {
            return _angle > 180;
        }
    }
}
