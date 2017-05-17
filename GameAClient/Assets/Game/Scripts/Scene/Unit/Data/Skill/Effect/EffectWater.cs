/********************************************************************
** Filename : EffectWater
** Author : Dong
** Date : 2017/5/17 星期三 下午 2:59:13
** Summary : EffectWater
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Effect(Name = "EffectWater", Type = typeof(EffectWater))]
    public class EffectWater : EffectBase
    {
        public override void OnAttached(BulletBase bullet)
        {
            if (_owner.IsMain)
            {
                return;
            }
        }

        public override void OnRemoved()
        {
        }
    }
}
