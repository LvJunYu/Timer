/********************************************************************
** Filename : UnitAttribute
** Author : Dong
** Date : 2016/10/2 星期日 下午 6:28:10
** Summary : UnitAttribute
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UnitAttribute : Attribute
    {
        public ushort Id;
        public Type Type;
    }
}
