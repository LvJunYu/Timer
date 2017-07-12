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
    public class SkillJelly : SkillBase
    {
        internal override void Enter(UnitBase ower)
        {
            base.Enter(ower);
            _eSkillType = ESkillType.Jelly;
            _bulletId = 10004;
        }
    }
}
