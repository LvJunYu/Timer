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
        /// <summary>
        /// 角度
        /// </summary>
        protected int _angle;
        /// <summary>
        /// 初始位置
        /// </summary>
        protected IntVec2 _originPos;

        protected int _maskRandom;
        
        protected UnityNativeParticleItem _effectBullet;

        protected IntVec2 _newSpeed;

        public int Angle
        {
            get { return _angle; }
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
            base.Clear();
        }

        public virtual void Run(SkillBase skill, int angle)
        {
            _skill = skill;
            _angle = angle;
            _maskRandom = UnityEngine.Random.Range(0, 2);
            _originPos = CenterPos;
            var rad = _angle * Mathf.Deg2Rad;
            _speed = new IntVec2((int)(_skill.ProjectileSpeed * Math.Sin(rad)), (int)(_skill.ProjectileSpeed * Math.Cos(rad)));
            _trans.eulerAngles = new Vector3(0, 0, -_angle);
            OnRun();
        }

        protected virtual void OnRun()
        {
        }

        protected virtual void UpdateAngle(int angle, EDirectionType eDirectionType)
        {
            _angle = angle;
            var rad = _angle * Mathf.Deg2Rad;
            _newSpeed = new IntVec2((int)(_skill.ProjectileSpeed * Math.Sin(rad)), (int)(_skill.ProjectileSpeed * Math.Cos(rad)));
            _trans.eulerAngles = new Vector3(0, 0, -_angle);
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
                    var rad = _angle*Mathf.Deg2Rad;
                    CenterPos = _originPos + new IntVec2((int)(_skill.CastRange * Math.Sin(rad)), (int)(_skill.CastRange * Math.Cos(rad)));
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
