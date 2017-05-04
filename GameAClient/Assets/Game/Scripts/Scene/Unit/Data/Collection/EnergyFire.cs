/********************************************************************
** Filename : EnergyFire
** Author : Dong
** Date : 2017/4/20 星期四 上午 10:10:05
** Summary : EnergyFire
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 6102, Type = typeof(EnergyFire))]
    public class EnergyFire : Energy
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

        protected override void OnTrigger(UnitBase other)
        {
            other.SkillCtrl.ChangeSkill<SkillFire>(_plus);
        }
    }
}
