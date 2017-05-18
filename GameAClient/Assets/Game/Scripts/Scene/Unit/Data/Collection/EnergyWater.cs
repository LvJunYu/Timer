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
    public class EnergyWater : BlockBase
    {
        protected int _totalCount;
        protected int _currentCount;
        protected int _speedEnergy;

        protected EnergyPoolCtrl _energyPoolCtrl;

        protected ESkillType _eSkillType;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _totalCount = 300;
            _currentCount = 0;
            _speedEnergy = 1;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            UpdateEnergyEffect();
            return true;
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_energyPoolCtrl != null && _energyPoolCtrl.ParticleItem != null)
            {
                GameParticleManager.FreeParticleItem(_energyPoolCtrl.ParticleItem);
                _energyPoolCtrl = null;
            }
        }

        public override void UpdateExtraData()
        {
            _eSkillType = (ESkillType)DataScene2D.Instance.GetUnitExtra(_guid).EnergyType;
            LogHelper.Debug("{0}", _eSkillType);
            UpdateEnergyEffect();
            base.UpdateExtraData();
        }

        private void UpdateEnergyEffect()
        {
            if (_energyPoolCtrl != null && _energyPoolCtrl.ParticleItem != null)
            {
                GameParticleManager.FreeParticleItem(_energyPoolCtrl.ParticleItem);
                _energyPoolCtrl = null;
            }
            string effectName = null;
            switch (_eSkillType)
            {
                case ESkillType.Fire:
                    effectName = "M1EffectEnergyFire";
                    break;
                case ESkillType.Ice:
                    effectName = "M1EffectEnergyIce";
                    break;
                case ESkillType.Jelly:
                    effectName = "M1EffectEnergyJelly";
                    break;
                case ESkillType.Clay:
                    effectName = "M1EffectEnergyClay";
                    break;
            }
            if (!string.IsNullOrEmpty(effectName))
            {
                var particle = GameParticleManager.Instance.GetUnityNativeParticleItem(effectName, _trans);
                if (particle != null)
                {
                    particle.Play();
                    _energyPoolCtrl = particle.Trans.GetComponent<EnergyPoolCtrl>();
                    _energyPoolCtrl.ParticleItem = particle;
                }
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.SkillMgr2 != null)
            {
                if (_currentCount > 0)
                {
                    _currentCount = 0;
                    OnTrigger(other);
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        protected void OnTrigger(UnitBase other)
        {
            other.SkillMgr1.ChangeSkill<SkillWater>();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _currentCount = Util.ConstantLerp(_currentCount, _totalCount, _speedEnergy);
            if (_energyPoolCtrl != null)
            {
                _energyPoolCtrl.LiquidVolume = GetProcess();
            }
        }

        private float GetProcess()
        {
            return (float)_currentCount / _totalCount;
        }
    }
}
