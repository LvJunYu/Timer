/********************************************************************
** Filename : SkillClay
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:38:23
** Summary : SkillClay
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace GameA.Game
{
    [Skill(Name = "SkillClay", Type = typeof(SkillClay))]
    public class SkillClay : SkillBase
    {
        internal override void Enter(UnitBase ower, bool plus)
        {
            base.Enter(ower, plus);
            _eSkillType = ESkillType.Clay;
        }

        protected override BulletBase CreateBullet()
        {
            return PlayMode.Instance.CreateRuntimeUnit(10005, _owner.FirePos, (byte)(_owner.FireDirection - 1), Vector2.one) as BulletBase;
        }
    }
}
