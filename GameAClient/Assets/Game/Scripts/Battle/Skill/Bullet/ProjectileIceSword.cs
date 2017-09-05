using System;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 10006, Type = typeof(ProjectileIceSword))]
    public class ProjectileIceSword : BlockBase
    {
        protected int _timer;

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
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