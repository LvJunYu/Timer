/********************************************************************
** Filename : EffectJelly
** Author : Dong
** Date : 2017/5/17 星期三 下午 3:00:56
** Summary : EffectJelly
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Effect(Name = "EffectJelly", Type = typeof(EffectJelly))]
    public class EffectJelly : EffectBase
    {
        public override void OnAttached(BulletBase bullet)
        {
            var angle = bullet.Angle;
            _owner.ExtraSpeed.y = angle <= 90 || angle >= 270 ? 180 : -180;
            if (angle <= 90)
            {
                _owner.ExtraSpeed.x = 180;
            }
            else if (angle >= 270)
            {
                _owner.ExtraSpeed.x = -180;
            }
        }
    }
}
