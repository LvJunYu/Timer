/********************************************************************
** Filename : EffectClay
** Author : Dong
** Date : 2017/5/17 星期三 下午 3:00:37
** Summary : EffectClay
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Effect(Name = "EffectClay", Type = typeof(EffectClay))]
    public class EffectClay : EffectBase
    {
        public override void OnAttached(BulletBase bullet)
        {
            //黏液不能攻击主角，直接喷涂到地面上
            if (_owner.IsMain)
            {
                return;
            }
            _owner.CanMotor = false;
            if (_owner.Animation != null)
            {
                _owner.Animation.PlayOnce("OnClay");
            }
        }

        public override void OnRemoved()
        {
            _owner.CanMotor = true;
        }
    }
}
