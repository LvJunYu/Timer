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
        protected SkillManager _skillManager;
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
            _skillManager = new SkillManager(this);
            _skillManager.ChangeSkill<SkillFire>();
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
                if (_skillManager != null)
                {
                    _skillManager.UpdateLogic();
                    if (_skillManager.Fire())
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
