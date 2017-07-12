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
    public class SkillClay : SkillBase
    {
        internal override void Enter(UnitBase ower)
        {
            base.Enter(ower);
            _eSkillType = ESkillType.Clay;
            _bulletId = 10005;
        }
    }
}
