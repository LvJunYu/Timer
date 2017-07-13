/********************************************************************
** Filename : SkillWater
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:37:08
** Summary : SkillWater
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class SkillWater : PaintSkill
    {
        internal override void Enter(UnitBase ower)
        {
            base.Enter(ower);
            _eSkillType = ESkillType.Water;
            _bulletId = 10001;
        }

        protected override void OnHeroHit(BulletBase bullet, UnitBase target)
        {
            target.ExcuteEffect(EffectMgr.GetEffect<EffectDamage>(10));
        }
    }
    
    public class SkillWaterPlus : PaintSkill
    {
        internal override void Enter(UnitBase ower)
        {
            base.Enter(ower);
            _eSkillType = ESkillType.Water;
            _bulletId = 10001;
        }
        
        protected override void OnSkillCast()
        {
            CreateBullet(_bulletId, GetBulletPos(_bulletId), _owner.ShootAngle);
        }

        public override void OnBulletHit(BulletBase bullet)
        {
            base.OnBulletHit(bullet);
            //播放特效
        }

        protected override void OnHeroHit(BulletBase bullet, UnitBase target)
        {
            //对范围的友军解除所有负面和控制效果，对范围内敌人造成100点伤害
            target.RemoveAllDebuffs();
            target.ExcuteEffect(EffectMgr.GetEffect<EffectDamage>(100));
        }
    }
}
