/********************************************************************
** Filename : EnergyIce
** Author : Dong
** Date : 2017/4/20 星期四 上午 10:10:52
** Summary : EnergyIce
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 6103, Type = typeof(EnergyIce))]
    public class EnergyIce : Energy
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
            _plus = true;
            return true;
        }

        protected override void OnTrigger()
        {
            SkillManager.Instance.ChangeSkill<SkillIce>(_plus);
        }
    }
}
