/********************************************************************
** Filename : Enums
** Author : Dong
** Date : 2016/10/19 星期三 上午 10:29:21
** Summary : Enums
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public enum EClimbState
    {
        None,
        Left,
        Right,
        Up,
    }

    public enum EUnitState
    {
        Normal,
        Portaling,
        Reviving,
    }
}
