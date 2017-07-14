///********************************************************************
//** Filename : SkillIce
//** Author : Dong
//** Date : 2017/4/18 星期二 下午 6:55:06
//** Summary : SkillIce
//***********************************************************************/
//
//using System;
//using System.Collections;
//using UnityEngine;
//
//namespace GameA.Game
//{
//    public class SkillIce : PaintSkill
//    {
//        internal override void Enter(UnitBase ower)
//        {
//            base.Enter(ower);
//            _eSkillType = ESkillType.Ice;
//            _bulletId = 10003;
//            _bulletSpeed = 100;
//        }
//
//        protected override void OnSkillCast()
//        {
//            CreateProjectile(_bulletId, GetProjectilePos(_bulletId), _owner.ShootAngle);
//            CreateProjectile(_bulletId, GetProjectilePos(_bulletId), _owner.ShootAngle, 10);
//        }
//
//        protected override void OnHeroHit(ProjectileBase projectile, UnitBase target)
//        {
//            if (target.HasBuff(EBuffType.Fire))
//            {
//                target.RemoveBuff(EBuffType.Fire);
//                return;
//            }
//            if (target.HasBuff(EBuffType.FirePlus))
//            {
//                target.RemoveBuff(EBuffType.FirePlus);
//                return;
//            }
//            target.AddBuff(EBuffType.Ice, 150, 
//                EffectMgr.GetEffect<EffectIce>(),
//                EffectMgr.GetEffect<EffectSlowSpeed>(0.2f), 
//                EffectMgr.GetEffect<EffectPersistent>(50, EffectMgr.GetEffect<EffectDamage>(10)));
//        }
//    }
//    
//    public class SkillIcePlus : PaintSkill
//    {
//        internal override void Enter(UnitBase ower)
//        {
//            base.Enter(ower);
//            _eSkillType = ESkillType.Ice;
//            _bulletId = 10003;
//            _bulletSpeed = 100;
//        }
//
//        protected override void OnSkillCast()
//        {
//            CreateProjectile(_bulletId, GetProjectilePos(_bulletId), _owner.ShootAngle);
//        }
//
//        protected override void OnHeroHit(ProjectileBase projectile, UnitBase target)
//        {
//            if (target.HasBuff(EBuffType.Fire))
//            {
//                target.RemoveBuff(EBuffType.Fire);
//                return;
//            }
//            if (target.HasBuff(EBuffType.FirePlus))
//            {
//                target.RemoveBuff(EBuffType.FirePlus);
//                return;
//            }
//            target.AddBuff(EBuffType.IcePlus, 250, 
//                EffectMgr.GetEffect<EffectIce>(),
//                EffectMgr.GetEffect<EffectSlowSpeed>(0.5f), 
//                EffectMgr.GetEffect<EffectPersistent>(50,EffectMgr.GetEffect<EffectDamage>(50)));
//        }
//    }
//}
