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
        protected int _rotation;
        /// <summary>
        /// 初始位置
        /// </summary>
        protected IntVec2 _originPos;

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
            _skill = null;
            _speed = IntVec2.zero;
            _blocked = false;
            _rotation = 0;
            _originPos = IntVec2.zero;
        }

        public virtual void Run(SkillBase skill)
        {
            _originPos = CenterPos;
            _run = true;
            _skill = skill;
            _rotation = _skill.Owner.ShootRot;
            var rad = _rotation * Mathf.Deg2Rad;
            _speed = new IntVec2((int)(_skill.BulletSpeed * Math.Sin(rad)), (int)(_skill.BulletSpeed * Math.Cos(rad)));
            _trans.eulerAngles = new Vector3(0, 0, -_rotation);
            if (_animation != null)
            {
                _animation.Init("Run");
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
                    var rad = _rotation*Mathf.Deg2Rad;
                    CenterPos = _originPos + new IntVec2((int)(_skill.Range * Math.Sin(rad)), (int)(_skill.Range * Math.Cos(rad)));
                    _blocked = true;
                }
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                if (_view != null)
                {
                    _trans.position = GM2DTools.TileToWorld(CenterPos, _skill.Owner.Trans.position.z);
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
                GameParticleManager.Instance.Emit(_tableUnit.DestroyEffectName, _trans.position, new Vector3(0, 0, _rotation), Vector3.one);
            }
            Clear();
            PlayMode.Instance.DestroyUnit(this);
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            _blocked = true;
            if (_skill.Plus)
            {
                DoEdge(unit, eDirectionType);
            }
            if (unit is Switch)
            {
                //
            }
        }

        protected virtual void DoEdge(UnitBase unit, EDirectionType eDirectionType)
        {
            switch (eDirectionType)
            {
                case EDirectionType.Up:
                    {
                        int centerPoint = (_colliderGrid.XMax + 1 + _colliderGrid.XMin) / 2;
                        unit.DoEdge(centerPoint - _skill.Radius, centerPoint + _skill.Radius, EDirectionType.Down,
                            _skill.ESkillType);
                    }
                    break;
                case EDirectionType.Down:
                    {
                        int centerPoint = (_colliderGrid.XMax + 1 + _colliderGrid.XMin) / 2;
                        unit.DoEdge(centerPoint - _skill.Radius, centerPoint + _skill.Radius, EDirectionType.Up,
                            _skill.ESkillType);
                    }
                    break;
                case EDirectionType.Left:
                    {
                        int centerPoint = (_colliderGrid.YMax + 1 + _colliderGrid.YMin) / 2;
                        unit.DoEdge(centerPoint - _skill.Radius, centerPoint + _skill.Radius, EDirectionType.Right,
                            _skill.ESkillType);
                    }
                    break;
                case EDirectionType.Right:
                    {
                        int centerPoint = (_colliderGrid.YMax + 1 + _colliderGrid.YMin) / 2;
                        unit.DoEdge(centerPoint - _skill.Radius, centerPoint + _skill.Radius, EDirectionType.Left,
                            _skill.ESkillType);
                    }
                    break;
            }
        }
    }
}
