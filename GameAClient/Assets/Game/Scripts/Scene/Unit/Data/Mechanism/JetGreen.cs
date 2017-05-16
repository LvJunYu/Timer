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
    public class JetGreen : Magic
    {
        protected SkillCtrl _skillCtrl;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _shootRot = (_unitDesc.Rotation) * 90;
            _skillCtrl = new SkillCtrl(this);
            _skillCtrl.ChangeSkill<SkillWater>(true);
            _skillCtrl.CurrentSkill.SetValue(50, 60, 15);
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            InitAssetRotation();
            return true;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_skillCtrl != null)
            {
                _skillCtrl.UpdateLogic();
                if (_skillCtrl.JetFire())
                {
                    if (_animation != null)
                    {
                        _animation.PlayOnce(((EDirectionType)_unitDesc.Rotation).ToString());
                    }
                }
            }
        }
    }
}
 