/********************************************************************
** Filename : JetBase
** Author : Dong
** Date : 2017/5/16 星期二 上午 10:52:35
** Summary : JetBase
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public class JetBase : Magic
    {
        protected SkillCtrl _skillCtrl;
        protected int _timeScale;
        protected const int AnimationLength = 15;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _shootRot = (_unitDesc.Rotation) * 90;
            _skillCtrl = new SkillCtrl(this);
            _skillCtrl.ChangeSkill<SkillWater>();
            _timeScale = 1;
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
                if (_skillCtrl.Fire())
                {
                    if (_animation != null)
                    {
                        _animation.PlayOnce(((EDirectionType)_unitDesc.Rotation).ToString(), _timeScale);
                    }
                }
            }
        }
    }
}
