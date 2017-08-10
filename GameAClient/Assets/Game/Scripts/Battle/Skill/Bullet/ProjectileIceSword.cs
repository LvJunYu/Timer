using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 10006, Type = typeof(ProjectileIceSword))]
    public class ProjectileIceSword : ProjectileBase
    {
        protected int _timer;

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
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
            //静态物体
            if (unit.DynamicCollider == null)
            {
                _timer = TableConvert.GetTime(BattleDefine.IceSwordLifeTime);
                //修改下layer
                _dynamicCollider.Layer = (int)ESceneLayer.Item;
            }
            else
            {
                _destroy = 1;
            }
        }
    }
}