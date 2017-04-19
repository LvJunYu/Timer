﻿/********************************************************************
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
        internal override void Enter(UnitBase ower, bool plus)
        {
            base.Enter(ower, plus);
            _eSkillType = ESkillType.Water;
        }

        protected override BulletBase CreateBullet()
        {
            return PlayMode.Instance.CreateRuntimeUnit(10001, _owner.FirePos, (byte)(_owner.FireDirection - 1), Vector2.one) as BulletBase;
        }
    }
}
