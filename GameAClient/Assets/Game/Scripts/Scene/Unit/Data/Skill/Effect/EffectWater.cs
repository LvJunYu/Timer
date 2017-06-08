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
        public override void Init(UnitBase target)
        {
            base.Init(target);
            _eSkillType = ESkillType.Water;
        }

        /// <summary>
        /// 解除火、黏液；速度变慢
        /// </summary>
        /// <param name="bullet"></param>
        public override void OnAttached(BulletBase bullet)
        {
            if (_owner.IsMain)
            {
                return;
            }
            _owner.EffectMgr.RemoveEffect<EffectFire>();
            _owner.EffectMgr.RemoveEffect<EffectClay>();
        }

        public override void OnRemoved()
        {
        }
    }
}
