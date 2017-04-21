/********************************************************************
** Filename : NoCacheGlassUnit
** Author : Dong
** Date : 2016/11/6 星期日 下午 2:47:39
** Summary : Ice
***********************************************************************/

using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4009, Type = typeof (Ice))]
    public class Ice : UnitWithChild
    {
        protected override bool OnInit()
        {
            if (!base.OnInit()) 
            {
                return false;
            }
            _friction = 1;
            return true;
        }

        public override void OnShootHit(UnitBase other)
        {
            if (other is BulletFire)
            {
                PlayMode.Instance.DestroyUnit(this);
                PushChild();
                OnDead();
            }
        }
    }
}