/********************************************************************
** Filename : EnergyPool
** Author : Dong
** Date : 2017/3/22 星期三 下午 2:35:55
** Summary : EnergyPool
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 8001, Type = typeof(EnergyPool))]
    public class EnergyPool : BlockBase
    {
        protected int _totalMp;
        protected int _currentMp;
        protected int _mpSpeed;

        protected EnergyPoolCtrl _energyPoolCtrl;
        protected UnityNativeParticleItem _efffect;

        protected EEnergyType _eSkillType;

        protected override bool OnInit()
        {
            _totalMp = 4000;
            _mpSpeed = 2;
            if (!base.OnInit())
            {
                return false;
            }
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
            _currentMp = _totalMp;
            if (_energyPoolCtrl != null)
            {
                _energyPoolCtrl.LiquidVolume = 1;
            }
            base.Clear();
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_efffect);
            _efffect = null;
        }

        public override void UpdateExtraData()
        {
            _eSkillType = (EEnergyType)DataScene2D.Instance.GetUnitExtra(_guid).UnitValue;
            UpdateEnergyEffect();
            base.UpdateExtraData();
        }

        private void UpdateEnergyEffect()
        {
            FreeEffect(_efffect);
            _efffect = null;
            _energyPoolCtrl = null;
            string effectName = null;
            switch (_eSkillType)
            {
                case EEnergyType.Fire:
                    effectName = "M1EffectEnergyFire";
                    break;
                case EEnergyType.Ice:
                    effectName = "M1EffectEnergyIce";
                    break;
                case EEnergyType.Jelly:
                    effectName = "M1EffectEnergyJelly";
                    break;
                case EEnergyType.Clay:
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
            if (other.SkillCtrl != null)
            {
                OnTrigger(other);
                ////如果技能不一样
                //var addedMp = other.AddMp(_currentMp);
                //_currentMp -= addedMp;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        protected void OnTrigger(UnitBase other)
        {
//            switch (_eSkillType)
//            {
//                case ESkillType.Fire:
//                    other.SkillCtrl.ChangeSkill<SkillFire>(0);
//                    break;
//                case ESkillType.Ice:
//                    other.SkillCtrl.ChangeSkill<SkillIce>(0);
//                    break;
//                case ESkillType.Jelly:
//                    other.SkillCtrl.ChangeSkill<SkillJelly>(0);
//                    break;
//                case ESkillType.Clay:
//                    other.SkillCtrl.ChangeSkill<SkillClay>(0);
//                    break;
//            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _currentMp = Util.ConstantLerp(_currentMp, _totalMp, _mpSpeed);
            if (_energyPoolCtrl != null)
            {
                _energyPoolCtrl.LiquidVolume = GetProcess();
            }
        }

        private float GetProcess()
        {
            if (_totalMp <= 0)
            {
                return 0;
            }
            return (float)_currentMp / _totalMp;
        }
    }
}
