/********************************************************************
** Filename : SkillAttribute
** Author : Dong
** Date : 2017/3/22 星期三 上午 11:22:42
** Summary : SkillAttribute
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SkillAttribute : Attribute
    {
        public string Name;
        public Type Type;
    }
}
