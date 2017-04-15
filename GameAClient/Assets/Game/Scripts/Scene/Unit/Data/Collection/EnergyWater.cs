/********************************************************************
** Filename : EnergyWater
** Author : Dong
** Date : 2017/3/22 星期三 下午 2:35:55
** Summary : EnergyWater
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 6101, Type = typeof(EnergyWater))]
    public class EnergyWater : Energy
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _totalCount = 300;
            _currentCount = 0;
            _speed = 1;
            return true;
        }

        protected override void OnTrigger()
        {
            SkillManager.Instance.ChangeSkill<SkillWater>(_plus);
            //播放特效 声音
            LogHelper.Debug("EnergyWater");
        }
    }
}
