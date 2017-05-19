/********************************************************************
** Filename : EnergyPool
** Author : Dong
** Date : 2017/3/22 星期三 下午 2:35:55
** Summary : EnergyPool
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 6101, Type = typeof(EnergyPool))]
    public class EnergyPool : BlockBase
    {
        protected int _totalCount;
        protected int _currentCount;
        protected int _speedEnergy;

        protected EnergyPoolCtrl _energyPoolCtrl;
        protected UnityNativeParticleItem _efffect;

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

        protected override void Clear()
        {
            _currentCount = 0;
            if (_energyPoolCtrl != null)
            {
                _energyPoolCtrl.LiquidVolume = GetProcess();
            }
            base.Clear();
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_efffect != null)
            {
                GameParticleManager.FreeParticleItem(_efffect);
                _efffect = null;
            }
        }

        public override void UpdateExtraData()
        {
            _eSkillType = (ESkillType)DataScene2D.Instance.GetUnitExtra(_guid).EnergyType;
            UpdateEnergyEffect();
            base.UpdateExtraData();
        }

        private void UpdateEnergyEffect()
        {
            if (_efffect != null)
            {
                GameParticleManager.FreeParticleItem(_efffect);
                _efffect = null;
            }
            _energyPoolCtrl = null;
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
                _efffect = GameParticleManager.Instance.GetUnityNativeParticleItem(effectName, _trans);
                if (_efffect != null)
                {
                    _efffect.Play();
                    _energyPoolCtrl = _efffect.Trans.GetComponent<EnergyPoolCtrl>();
                    _energyPoolCtrl.LiquidVolume = GetProcess();
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
            switch (_eSkillType)
            {
                case ESkillType.Fire:
                    other.SkillMgr2.ChangeSkill<SkillFire>();
                    break;
                case ESkillType.Ice:
                    other.SkillMgr2.ChangeSkill<SkillIce>();
                    break;
                case ESkillType.Jelly:
                    other.SkillMgr2.ChangeSkill<SkillJelly>();
                    break;
                case ESkillType.Clay:
                    other.SkillMgr2.ChangeSkill<SkillClay>();
                    break;
            }
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
