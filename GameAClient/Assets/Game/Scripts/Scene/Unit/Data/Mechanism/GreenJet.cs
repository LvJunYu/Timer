/********************************************************************
** Filename : GreenJet
** Author : Dong
** Date : 2017/4/7 星期五 下午 4:50:59
** Summary : GreenJet
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5015, Type = typeof(GreenJet))]
    public class GreenJet : Magic
    {
        protected SkillCtrl _skillCtrl;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _shootRot = _unitDesc.Rotation;
            _skillCtrl = new SkillCtrl(this);
            _skillCtrl.ChangeSkill<SkillWater>(true);
            return true;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

        }
    }
}
 