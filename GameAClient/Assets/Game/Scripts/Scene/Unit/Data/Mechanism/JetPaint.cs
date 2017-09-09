/********************************************************************
** Filename : JetRed
** Author : Dong
** Date : 2017/4/7 星期五 下午 4:51:57
** Summary : JetRed
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 5016, Type = typeof(JetPaint))]
    public class JetPaint : JetBase
    {
        protected override void SetValue()
        {
            _timeScale = 3;
            _skillCtrl.CurrentSkills[0].SetValue(TableConvert.GetTime(200),  TableConvert.GetRange(600));
        }
    }
}
