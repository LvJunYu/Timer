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
//        public override void OnAttached()
//        {
//            _owner.InFire();
//        }
//
//        public override void OnRemoved()
//        {
//            if (_owner.IsAlive)
//            {
//                _owner.OutFire();
//            }
//        }
    }
}
