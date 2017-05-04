/********************************************************************
** Filename : EnergyClay
** Author : Dong
** Date : 2017/4/20 星期四 上午 10:10:52
** Summary : EnergyClay
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 6105, Type = typeof(EnergyClay))]
    public class EnergyClay : Energy
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
            other.SkillCtrl.ChangeSkill<SkillClay>(_plus);
        }
    }
}
