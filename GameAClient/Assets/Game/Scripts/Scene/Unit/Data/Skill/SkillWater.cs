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
        protected override BulletBase CreateBullet()
        {
            return PlayMode.Instance.CreateRuntimeUnit(10001, _owner.FirePos, (byte)_owner.FireDirection, Vector2.one) as BulletWater;
        }
    }
}
