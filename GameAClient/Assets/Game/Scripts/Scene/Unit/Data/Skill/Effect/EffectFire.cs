/********************************************************************
** Filename : EffectFire
** Author : Dong
** Date : 2017/5/17 星期三 下午 2:59:52
** Summary : EffectFire
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Effect(Name = "EffectFire", Type = typeof(EffectFire))]
    public class EffectFire : EffectBase
    {
        public override void Init(UnitBase target)
        {
            base.Init(target);
            _eSkillType = ESkillType.Fire;
        }

        public override void OnAttached(BulletBase bullet)
        {
            //解除冰；着火后四处逃窜 2s 后 死亡，如果此时碰到水，火消灭。水块的话1秒内朝着泥土跳跃，跳不上被淹死。
            _owner.EffectMgr.RemoveEffect<EffectIce>();
        }
    }
}
