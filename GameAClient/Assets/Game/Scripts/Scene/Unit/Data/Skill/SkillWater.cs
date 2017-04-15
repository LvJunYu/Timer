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
    [Skill(Name = "SkillWater", Type = typeof(SkillWater))]
    public class SkillWater : SkillBase
    {
        internal override void Enter(bool plus)
        {
            base.Enter(plus);
            _bulletSpeed = IntVec2.right * 50;
        }

        protected override BulletBase CreateBullet()
        {
            return PlayMode.Instance.CreateRuntimeUnit(10001, PlayMode.Instance.MainUnit.FirePos, 1, Vector2.one) as BulletWater;
        }
    }
}
