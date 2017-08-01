/********************************************************************
** Filename : JetGreen
** Author : Dong
** Date : 2017/4/7 星期五 下午 4:50:59
** Summary : JetGreen
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5015, Type = typeof(JetGreen))]
    public class JetGreen : JetBase
    {
        protected override void SetValue()
        {
            _skillCtrl.CurrentSkills[0].SetValue(2000, 600);
        }
    }
}
 