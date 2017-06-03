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
        protected bool _blocked;
        /// <summary>
        /// 角度
        /// </summary>
        protected int _angle;
        /// <summary>
        /// 初始位置
        /// </summary>
        protected IntVec2 _originPos;

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
            _blocked = false;
            _angle = 0;
            _originPos = IntVec2.zero;
            base.Clear();
        }

        public virtual void Run(SkillBase skill)
        {
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
                    _blocked = true;
                }
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                if (_view != null)
                {
                    _trans.position = GetTransPos();
                }
                if (_blocked)
                {
                    OnBlocked();
                }
            }
        }

        protected virtual void OnBlocked()
        {
            _isAlive = false;
            --Life;
            if (_view != null)
            {
                GameAudioManager.Instance.PlaySoundsEffects(_tableUnit.DestroyAudioName);
                GameParticleManager.Instance.Emit(_tableUnit.DestroyEffectName, _trans.position, new Vector3(0, 0, _angle), Vector3.one);
            }
            Clear();
            PlayMode.Instance.DestroyUnit(this);
        }

        protected override void HitForPaint(UnitBase unit, EDirectionType eDirectionType)
        {
            _blocked = true;
            if (unit.CanPainted)
            {
                DoPaint(unit, eDirectionType);
            }
        }

        private static int PaintDepth = (int) (ConstDefineGM2D.ServerTileScale * 0.15f);

        protected virtual void DoPaint(UnitBase unit, EDirectionType eDirectionType)
        {
            switch (eDirectionType)
            {
                case EDirectionType.Up:
                    {
                        int centerPoint = (_colliderGrid.XMax + 1 + _colliderGrid.XMin) / 2;
                        var start = centerPoint - _skill.Radius;
                        var end = centerPoint + _skill.Radius;
                        unit.DoPaint(start, end, EDirectionType.Down, _skill.ESkillType);
                        if (start <= unit.ColliderGrid.XMin)
                        {
                            start = unit.ColliderGrid.YMin;
                            end = unit.ColliderGrid.YMin + PaintDepth;
                            unit.DoPaint(start, end, EDirectionType.Left, _skill.ESkillType);
                        }
                        if (end >= unit.ColliderGrid.XMax)
                        {
                            start = unit.ColliderGrid.YMin;
                            end = unit.ColliderGrid.YMin + PaintDepth;
                            unit.DoPaint(start, end, EDirectionType.Right, _skill.ESkillType);
                        }
                    }
                    break;
                case EDirectionType.Down:
                    {
                        int centerPoint = (_colliderGrid.XMax + 1 + _colliderGrid.XMin) / 2;
                        var start = centerPoint - _skill.Radius;
                        var end = centerPoint + _skill.Radius;
                        unit.DoPaint(start, end, EDirectionType.Up,_skill.ESkillType);
                        if (start <= unit.ColliderGrid.XMin)
                        {
                            start = unit.ColliderGrid.YMax - PaintDepth;
                            end = unit.ColliderGrid.YMax;
                            unit.DoPaint(start, end, EDirectionType.Left, _skill.ESkillType);
                        }
                        if (end >= unit.ColliderGrid.XMax)
                        {
                            start = unit.ColliderGrid.YMax - PaintDepth;
                            end = unit.ColliderGrid.YMax;
                            unit.DoPaint(start, end, EDirectionType.Right, _skill.ESkillType);
                        }
                    }
                    break;
                case EDirectionType.Left:
                    {
                        int centerPoint = (_colliderGrid.YMax + 1 + _colliderGrid.YMin) / 2;
                        var start = centerPoint - _skill.Radius;
                        var end = centerPoint + _skill.Radius;
                        unit.DoPaint(start, end, EDirectionType.Right, _skill.ESkillType);
                        if (start <= unit.ColliderGrid.YMin)
                        {
                            start = unit.ColliderGrid.XMax - PaintDepth;
                            end = unit.ColliderGrid.XMax;
                            unit.DoPaint(start, end, EDirectionType.Down, _skill.ESkillType);
                        }
                        if (end >= unit.ColliderGrid.YMax)
                        {
                            start = unit.ColliderGrid.XMax - PaintDepth;
                            end = unit.ColliderGrid.XMax;
                            unit.DoPaint(start, end, EDirectionType.Up, _skill.ESkillType);
                        }
                    }
                    break;
                case EDirectionType.Right:
                    {
                        int centerPoint = (_colliderGrid.YMax + 1 + _colliderGrid.YMin) / 2;
                        var start = centerPoint - _skill.Radius;
                        var end = centerPoint + _skill.Radius;
                        unit.DoPaint(start, end, EDirectionType.Left,_skill.ESkillType);
                        if (start <= unit.ColliderGrid.YMin)
                        {
                            start = unit.ColliderGrid.XMin;
                            end = unit.ColliderGrid.XMin + PaintDepth;
                            unit.DoPaint(start, end, EDirectionType.Down, _skill.ESkillType);
                        }
                        if (end >= unit.ColliderGrid.YMax)
                        {
                            start = unit.ColliderGrid.XMin;
                            end = unit.ColliderGrid.XMin + PaintDepth;
                            unit.DoPaint(start, end, EDirectionType.Up, _skill.ESkillType);
                        }
                    }
                    break;
            }
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            _blocked = true;
            if (unit.IsHero && unit.EffectMgr != null)
            {
                LogHelper.Debug("OnSkilled:{0}", _skill);
                var effectMgr = unit.EffectMgr;
                switch (_skill.ESkillType)
                {
                    case ESkillType.Water:
                        //解除火、黏液；速度变慢
                        effectMgr.RemoveEffect<EffectFire>();
                        effectMgr.RemoveEffect<EffectClay>();
                        effectMgr.AddEffect<EffectWater>(this);
                        break;
                    case ESkillType.Fire:
                        //解除冰；着火后四处逃窜 2s 后 死亡，如果此时碰到水，火消灭。水块的话1秒内朝着泥土跳跃，跳不上被淹死。
                        effectMgr.RemoveEffect<EffectIce>();
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
    }
}
