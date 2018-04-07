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
    public class Ice : SkillBlock
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation.Init("Run");
            return true;
        }

        protected override void CheckSkillHit(UnitBase other, Grid2D grid, EDirectionType eDirectionType)
        {
            OnEffect(other, eDirectionType);
        }

        public static void OnEffect(UnitBase other, EDirectionType eDirectionType)
        {
            switch (eDirectionType)
            {
                    case EDirectionType.Up:
                        other.SetStepOnIce();
                        break;
            }
        }

//        public override void OnShootHit(UnitBase other)
//        {
//            if (other is ProjectileFire)
//            {
//                PlayMode.Instance.DestroyUnit(this);
//                PushChild();
//                OnDead();
//            }
//        }
    }
}