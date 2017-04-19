/********************************************************************
** Filename : MonsterBase
** Author : Dong
** Date : 2017/4/18 星期二 下午 9:05:44
** Summary : MonsterBase
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public class MonsterBase : RigidbodyUnit
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _isMonster = true;
            return true;
        }
    }
}
