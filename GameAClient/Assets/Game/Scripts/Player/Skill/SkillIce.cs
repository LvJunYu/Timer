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
        internal override void Enter(UnitBase ower)
        {
            base.Enter(ower);
            _eSkillType = ESkillType.Ice;
            _bulletId = 10003;
        }
    }
}
