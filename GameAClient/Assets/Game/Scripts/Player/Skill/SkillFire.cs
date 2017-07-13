/********************************************************************
** Filename : SkillFire
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:37:29
** Summary : SkillFire
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace GameA.Game
{
    public class SkillFire : PaintSkill
    {
        internal override void Enter(UnitBase ower)
        {
            base.Enter(ower);
            _eSkillType = ESkillType.Fire;
            _bulletId = 10002;
        }

        protected override void OnSkillCast()
        {
            CreateBullet(_bulletId, GetBulletPos(_bulletId), _owner.ShootAngle);
        }

        protected override void OnHeroHit(BulletBase bullet, UnitBase target)
        {
            if (target.HasBuff(EBuffType.Ice))
            {
                target.RemoveBuff(EBuffType.Ice);
                return;
            }
            if (target.HasBuff(EBuffType.IcePlus))
            {
                target.RemoveBuff(EBuffType.IcePlus);
                return;
            }
            target.AddBuff(EBuffType.Fire, 150, 
                EffectMgr.GetEffect<EffectFire>(),
                EffectMgr.GetEffect<EffectPersistent>(50, EffectMgr.GetEffect<EffectDamage>(10)));
        }
    }
    
    public class SkillFirePlus : PaintSkill
    {
        internal override void Enter(UnitBase ower)
        {
            base.Enter(ower);
            _eSkillType = ESkillType.Fire;
            _bulletId = 10002;
        }
        
        protected override void OnSkillCast()
        {
            CreateBullet(_bulletId, GetBulletPos(_bulletId), _owner.ShootAngle);
        }

        protected override void OnHeroHit(BulletBase bullet, UnitBase target)
        {
            if (target.HasBuff(EBuffType.Ice))
            {
                target.RemoveBuff(EBuffType.Ice);
                return;
            }
            if (target.HasBuff(EBuffType.IcePlus))
            {
                target.RemoveBuff(EBuffType.IcePlus);
                return;
            }
            target.AddBuff(EBuffType.FirePlus, 200, 
                EffectMgr.GetEffect<EffectFire>(),
                EffectMgr.GetEffect<EffectPersistent>(50, EffectMgr.GetEffect<EffectDamage>(100)));
        }
    }
}
