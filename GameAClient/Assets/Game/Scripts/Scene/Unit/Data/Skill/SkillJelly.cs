/********************************************************************
** Filename : SkillJelly
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:40:08
** Summary : SkillJelly
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace GameA.Game
{
    [Skill(Name = "SkillJelly", Type = typeof(SkillJelly))]
    public class SkillJelly : SkillBase
    {
        internal override void Enter(UnitBase ower, bool plus)
        {
            base.Enter(ower, plus);
            _eSkillType = ESkillType.Jelly;
        }

        protected override BulletBase CreateBullet()
        {
            return PlayMode.Instance.CreateRuntimeUnit(10004, _owner.FirePos, (byte)(_owner.FireDirection - 1), Vector2.one) as BulletBase;
        }
    }
}
