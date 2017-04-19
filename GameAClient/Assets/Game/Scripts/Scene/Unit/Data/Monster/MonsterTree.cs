/********************************************************************
** Filename : MonsterTree
** Author : Dong
** Date : 2017/4/18 星期二 下午 9:00:48
** Summary : MonsterTree
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 2001, Type = typeof(MonsterTree))]
    public class MonsterTree : MonsterBase
    {
        protected override void UpdateMonster()
        {
            if (_isAlive && _isStart)
            {

            }
        }
    }
}
