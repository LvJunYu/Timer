using System;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 10006, Type = typeof(ProjectileIceSword))]
    public class ProjectileIceSword : ProjectileBase
    {
        protected int _timer;
        protected bool _stay;

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
            _stay = false;
            if (_dynamicCollider != null)
            {
                _dynamicCollider.Layer = (int)ESceneLayer.Bullet;
            }
        }

        protected override void OnRun()
        {
            base.OnRun();
            _effectBullet = GameParticleManager.Instance.GetUnityNativeParticleItem(_tableUnit.Model, _trans);
            if (_effectBullet != null)
            {
                _effectBullet.Play();
            }
        }

        public override void UpdateView(float deltaTime)
        {
            if (_timer > 0)
            {
                _timer--;
                if (_timer == 0)
                {
                    OnDestroy();
                }
            }
            else
            {
                base.UpdateView(deltaTime);
            }
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            _targetUnit = unit;
            if (unit.DynamicCollider != null || !Util.IsFloatEqual(_direction.x * _direction.y, 0))
            {
                _destroy = 1;
                return;
            }
            _stay = true;
            //静态物体
            _timer = TableConvert.GetTime(BattleDefine.IceSwordLifeTime);
            //修改下layer
            _dynamicCollider.Layer = (int)ESceneLayer.Item;
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