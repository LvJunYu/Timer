/********************************************************************
** Filename : JetBase
** Author : Dong
** Date : 2017/5/16 星期二 上午 10:52:35
** Summary : JetBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    public class JetBase : Magic
    {
        protected SkillCtrl _skillCtrl;
        protected int _timeScale;
        protected const int AnimationLength = 15;
        protected UnityNativeParticleItem _effect;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _shootAngle = (_unitDesc.Rotation) * 90;
            _skillCtrl = new SkillCtrl(this, 1);
            _skillCtrl.ChangeSkill(1, 0);
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

            _effect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectJetFireRun", _trans);
            if (_effect != null)
            {
                _effect.Play();
            }
            return true;
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_effect);
            _effect = null;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (!_ctrlBySwitch)
            {
                if (_skillCtrl != null)
                {
                    _skillCtrl.UpdateLogic();
                    if (_skillCtrl.Fire(0))
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
}
