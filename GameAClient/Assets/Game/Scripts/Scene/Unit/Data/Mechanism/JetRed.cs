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
    [Unit(Id = 5016, Type = typeof(JetRed))]
    public class JetRed : JetBase
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _timeScale = 3;
            _skillCtrl.CurrentSkills[0].SetValue(8, 60, AnimationLength / _timeScale);
            return true;
        }
    }
}
