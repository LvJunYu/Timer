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
            //修改下layer
            _dynamicCollider.Layer = (int)ESceneLayer.Item;
            if (unit.IsActor)
            {
                _destroy = 1;
            }
            else
            {
                _timer = TableConvert.GetTime(5000);
            }
        }
    }
}