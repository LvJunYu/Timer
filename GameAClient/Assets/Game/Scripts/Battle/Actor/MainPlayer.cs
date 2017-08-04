/********************************************************************
** Filename : MainPlayer
** Author : Dong
** Date : 2017/3/3 星期五 下午 8:28:03
** Summary : MainPlayer
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    [Unit(Id = 1002, Type = typeof(MainPlayer))]
    public class MainPlayer : PlayerBase
    {
        public override bool IsMain
        {
            get { return true; }
        }
    }
}
