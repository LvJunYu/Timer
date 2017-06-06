/********************************************************************
** Filename : Gun
** Author : Dong
** Date : 2017/6/6 星期二 下午 3:29:13
** Summary : Gun
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 10000, Type = typeof(Gun))]
    public class Gun : RigidbodyUnit
    {
        [SerializeField]
        /// <summary>
        /// 射击光点和主角脚下中心点的偏移
        /// </summary>
        protected IntVec2 _shooterEffectOffset = new IntVec2(240, 320);
        protected UnityNativeParticleItem _shooterEffect;
        protected MainUnit _mainUnit;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _mainUnit = PlayMode.Instance.MainUnit;
            _curPos = _mainUnit.CurMoveDirection == EMoveDirection.Right
                ? _mainUnit.CenterPos + new IntVec2(-_shooterEffectOffset.x, _shooterEffectOffset.y)
                : _mainUnit.CenterPos + new IntVec2(_shooterEffectOffset.x - GetColliderSize().x, _shooterEffectOffset.y);
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            //初始化ShooterEffect
            _shooterEffect = GameParticleManager.Instance.GetUnityNativeParticleItem(ParticleNameConstDefineGM2D.Shooter, _trans);
            _shooterEffect.Play();
            return true;
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_shooterEffect);
            _shooterEffect = null;
        }

        public override void UpdateLogic()
        {
            if (_isStart && _isAlive && _mainUnit != null)
            {
                var destPos = _mainUnit.CurMoveDirection == EMoveDirection.Right
                    ? _mainUnit.CenterPos + new IntVec2(-_shooterEffectOffset.x, _shooterEffectOffset.y)
                    : _mainUnit.CenterPos + new IntVec2(_shooterEffectOffset.x - GetColliderSize().x, _shooterEffectOffset.y);
                _speed = (destPos - _curPos) / 6;
                if (_speed.x == 0)
                {
                    _speed.x = destPos.x - _curPos.x;
                }
                if (_speed.y == 0)
                {
                    _speed.y = destPos.y - _curPos.y;
                }
            }
        }

        public override void UpdateView(float deltaTime)
        {
            if (_isStart && _isAlive)
            {
                _deltaPos = _speed;
                _curPos += _deltaPos;
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
            }
        }
    }
}
