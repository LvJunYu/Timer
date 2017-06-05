/********************************************************************
** Filename : StoneUnit
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:06:41
** Summary : StoneUnit
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 4007, Type = typeof(Stone))]
    public class Stone : PaintBlock
    {
        public override void DoPaint(int start, int end, EDirectionType direction, ESkillType eSkillType, bool draw = true)
        {
            if (!_isAlive)
            {
                return;
            }
            //如果是火的话干掉自己生成焦土
            if (eSkillType == ESkillType.Fire)
            {
                PlayMode.Instance.CreateRuntimeUnit(4013, _curPos);
                PlayMode.Instance.DestroyUnit(this);
            }
        }
    }
}
