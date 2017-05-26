/********************************************************************
** Filename : EffectIce
** Author : Dong
** Date : 2017/5/17 星期三 下午 3:00:14
** Summary : EffectIce
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Effect(Name = "EffectIce", Type = typeof(EffectIce))]
    public class EffectIce : EffectBase
    {
        public override void OnAttached(BulletBase bullet)
        {
            //黏液不能攻击主角，直接喷涂到地面上
            if (_owner.IsMain)
            {
                return;
            }
            _owner.CanMotor = false;
            _owner.CanAttack = false;
            if (_owner.Animation != null)
            {
                _owner.Animation.PlayOnce("OnIce");
            }
        }

        public override void OnRemoved()
        {
            _owner.CanMotor = true;
            _owner.CanAttack = true;
        }
    }
}
