/********************************************************************
** Filename : SkillIce
** Author : Dong
** Date : 2017/4/18 星期二 下午 6:55:06
** Summary : SkillIce
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace GameA.Game
{
    [Skill(Name = "SkillIce", Type = typeof(SkillIce))]
    public class SkillIce : SkillBase
    {
        internal override void Enter(UnitBase ower, bool plus)
        {
            base.Enter(ower, plus);
            _eSkillType = ESkillType.Ice;
        }

        protected override BulletBase CreateBullet()
        {
            return PlayMode.Instance.CreateRuntimeUnit(10003, _owner.FirePos, (byte)(_owner.FireDirection - 1), Vector2.one) as BulletBase;
        }
    }
}
