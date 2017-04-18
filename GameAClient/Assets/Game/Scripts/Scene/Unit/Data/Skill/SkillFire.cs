﻿/********************************************************************
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
    [Skill(Name = "SkillFire", Type = typeof(SkillFire))]
    public class SkillFire : SkillBase
    {
        internal override void Enter(UnitBase ower, bool plus)
        {
            base.Enter(ower, plus);
            _eSkillType = ESkillType.Fire;
        }

        protected override BulletBase CreateBullet()
        {
            return PlayMode.Instance.CreateRuntimeUnit(10002, _owner.FirePos, (byte)(_owner.FireDirection - 1), Vector2.one) as BulletBase;
        }
    }
}
