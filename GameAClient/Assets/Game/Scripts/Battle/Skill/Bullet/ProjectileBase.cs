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
        protected bool _run;
        protected SkillBase _skill;
        /// <summary>
        /// 速度
        /// </summary>
        protected IntVec2 _speed;
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

        protected int _delayRunTime;

        protected int _maskRandom;
        
        protected UnityNativeParticleItem _effectBullet;

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
            _destroy = 0;
            _angle = 0;
            _originPos = IntVec2.zero;
            FreeEffect(_effectBullet);
            _effectBullet = null;
            base.Clear();
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _effectBullet = GameParticleManager.Instance.GetUnityNativeParticleItem(_tableUnit.Model, _trans);
            if (_effectBullet == null)
            {
                return false;
            }
            _effectBullet.Play();
            return true;
        }

        public virtual void Run(SkillBase skill, int angle, int delayRunTime)
        {
            _skill = skill;
            _angle = angle;
            _delayRunTime = delayRunTime;
            if (_delayRunTime == 0)
            {
                _run = true;
            }
            _maskRandom = UnityEngine.Random.Range(0, 2);
            _originPos = CenterPos;
            var rad = _angle * Mathf.Deg2Rad;
            _speed = new IntVec2((int)(_skill.ProjectileSpeed * Math.Sin(rad)), (int)(_skill.ProjectileSpeed * Math.Cos(rad)));
            _trans.eulerAngles = new Vector3(0, 0, -_angle);
            if (_animation != null)
            {
                _animation.Init("Run", false);
            }
        }

        protected void UpdateAngle(int angle)
        {
            _angle = angle;
            var rad = _angle * Mathf.Deg2Rad;
            _speed = new IntVec2((int)(_skill.ProjectileSpeed * Math.Sin(rad)), (int)(_skill.ProjectileSpeed * Math.Cos(rad)));
            _trans.eulerAngles = new Vector3(0, 0, -_angle);
        }

        public override void UpdateLogic()
        {
            if (_delayRunTime > 0)
            {
                _delayRunTime--;
                if (_delayRunTime == 0 && _skill.Owner.IsAlive)
                {
                    _run = true;
                }
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
                string effectName = _tableUnit.DestroyEffectName;
                if (_destroy == 1)
                {
                    effectName = "M1EffectBlisterStart";
                }
                GameParticleManager.Instance.Emit(effectName, _trans.position, new Vector3(0, 0, _angle), Vector3.one);
            }
            Clear();
            PlayMode.Instance.DestroyUnit(this);
        }
    }
}
