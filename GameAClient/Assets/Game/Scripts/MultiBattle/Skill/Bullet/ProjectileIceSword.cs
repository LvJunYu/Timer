using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 10006, Type = typeof(ProjectileIceSword))]
    public class ProjectileIceSword : BlockBase
    {
        protected int _timer;
        protected UnityNativeParticleItem _effectBullet;

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
            GameParticleManager.FreeParticleItem(_effectBullet);
            _effectBullet = null;
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            GameParticleManager.FreeParticleItem(_effectBullet);
            _effectBullet = null;
        }

        internal override bool InstantiateView()
        {
            if(!base.InstantiateView())
            {
                return false;
            }
            _effectBullet = GameParticleManager.Instance.GetUnityNativeParticleItem(_tableUnit.Model, _trans);
            if (_effectBullet != null)
            {
                _effectBullet.Play();
                _effectBullet.Trans.localEulerAngles = new Vector3(0, 0, -_angle);
//                _effectBullet.Trans.RotateAround(_trans.position,Vector3.forward, -_angle);
            }
            return true;
        }

        public override void SetLifeTime(int lifeTime)
        {
            _timer = lifeTime;
        }

        public override void UpdateLogic()
        {
            if (_timer > 0)
            {
                _timer--;
                if (_timer == 0)
                {
                    OnDead();
                    PlayMode.Instance.DestroyUnit(this);
                }
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (Rotation == (int)EDirectionType.Left || Rotation == (int)EDirectionType.Right)
            {
                return base.OnUpHit(other, ref y, checkOnly);
            }
            return false;
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            return false;
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            return false;
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            return false;
        }
    }
}