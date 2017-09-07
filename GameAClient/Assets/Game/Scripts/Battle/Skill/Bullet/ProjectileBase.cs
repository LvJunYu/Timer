/********************************************************************
** Filename : ProjectileBase
** Author : Dong
** Date : 2017/3/23 星期四 下午 3:11:11
** Summary : ProjectileBase
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
    public class ProjectileBase : RigidbodyUnit, IPoolableObject
    {
        protected SkillBase _skill;
        /// <summary>
        /// 是否被阻挡
        /// </summary>
        protected byte _destroy;

        protected Vector2 _direction;
        /// <summary>
        /// 初始位置
        /// </summary>
        protected IntVec2 _originPos;

        protected int _maskRandom;
        
        protected UnityNativeParticleItem _effectBullet;

        protected IntVec2 _newSpeed;
        
        protected UnitBase _targetUnit;

        protected bool _run;

        public Vector2 Direction
        {
            get { return _direction; }
        }

        public UnitBase TargetUnit
        {
            get { return _targetUnit; }
        }

        public int MaskRandom
        {
            get { return _maskRandom; }
        }

        public void OnGet()
        {
        }

        public void OnFree()
        {
            //赋值为空 TODO
            _dynamicCollider = null;
            Clear();
        }

        public void OnDestroyObject()
        {
        }

        protected override void Clear()
        {
            _run = false;
            _skill = null;
            _speed = IntVec2.zero;
            _destroy = 0;
            _angle = 0;
            _originPos = IntVec2.zero;
            _newSpeed = IntVec2.zero;
            FreeEffect(_effectBullet);
            _effectBullet = null;
            _targetUnit = null;
            base.Clear();
        }

        public virtual void Run(SkillBase skill, float angle)
        {
            _run = true;
            _skill = skill;
            _maskRandom = UnityEngine.Random.Range(0, 2);
            _originPos = CenterPos;
            SetAngle(angle);
            _effectBullet = GameParticleManager.Instance.GetUnityNativeParticleItem(_tableUnit.Model, _trans);
            if (_effectBullet != null)
            {
                _effectBullet.Play();
            }
            OnRun();
        }

        protected void SetAngle(float angle)
        {
            _angle = angle;
            _direction = GM2DTools.GetDirection(_angle);
            _speed = new IntVec2((int) (_skill.ProjectileSpeed * _direction.x),
                (int) (_skill.ProjectileSpeed * _direction.y));
            _trans.eulerAngles = new Vector3(0, 0, -_angle);
        }

        protected virtual void OnRun()
        {
        }

        protected virtual void UpdateAngle(float angle, EDirectionType eDirectionType)
        {
            SetAngle(angle);
        }
        
        public override void UpdateLogic()
        {
            Speed += _fanForce;
            _fanForce = IntVec2.zero;
        }
        
        public override void UpdateView(float deltaTime)
        {
            if (_isAlive)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                //超出最大射击距离
                if ((CenterPos - _originPos).SqrMagnitude() >= _skill.CastRange * _skill.CastRange)
                {
                    CenterPos = _originPos + new IntVec2((int)(_skill.CastRange * _direction.x), (int)(_skill.CastRange * _direction.y));
                    _destroy = 1;
                }
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
                if (_destroy > 0)
                {
                    OnDestroy();
                }
            }
        }
        
        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            _targetUnit = unit;
            _destroy = 1;
        }

        protected virtual void OnDestroy()
        {
            _skill.OnProjectileHit(this);
            _isAlive = false;
            --Life;
            if (_view != null)
            {
                GameAudioManager.Instance.PlaySoundsEffects(_tableUnit.DestroyAudioName);
                GameParticleManager.Instance.Emit(_tableUnit.DestroyEffectName, _trans.position, new Vector3(0, 0, _angle), Vector3.one, 1f);
            }
            PlayMode.Instance.DestroyUnit(this);
        }
    }
}
