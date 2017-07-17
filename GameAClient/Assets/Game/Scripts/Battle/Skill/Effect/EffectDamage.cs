/********************************************************************
** Filename : EffectDamage
** Author : Dong
** Date : 2017/3/22 星期三 上午 11:22:42
** Summary : EffectDamage
***********************************************************************/

namespace GameA.Game
{
    public class EffectDamage : EffectBase
    {
        private int _damage;

        public override void Init(params object[] values)
        {
            _damage = (int)values[0];
        }

        public override bool OnAttached(ActorBase target)
        {
            target.Hp += _damage;
            return base.OnAttached(target);
        }
    }
}