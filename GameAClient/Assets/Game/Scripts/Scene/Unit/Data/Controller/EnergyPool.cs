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
        protected int _timer;

        protected EnergyPoolCtrl _energyPoolCtrl;
        protected UnityNativeParticleItem _efffect;
        protected int _weaponId;

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
            _timer = 0;
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
            _weaponId = DataScene2D.Instance.GetUnitExtra(_guid).UnitValue;
            UpdateEnergyEffect();
            base.UpdateExtraData();
        }

        private void UpdateEnergyEffect()
        {
            FreeEffect(_efffect);
            _efffect = null;
            _energyPoolCtrl = null;
            string effectName = "M1EffectEnergyFire";
            if (!string.IsNullOrEmpty(effectName))
            {
                _efffect = GameParticleManager.Instance.GetUnityNativeParticleItem(effectName, _trans);
                if (_efffect != null)
                {
                    _efffect.Play();
                    _energyPoolCtrl = _efffect.Trans.GetComponent<EnergyPoolCtrl>();
//                    _energyPoolCtrl.LiquidVolume = GetProcess();
                }
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsAlive)
            {
                if (_timer == 0)
                {
                    _timer = 200;
                    LogHelper.Debug(_weaponId+"~~~~~~~~~~~~~~~");
                    other.ChangeWeapon(_weaponId);
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_timer > 0)
            {
                _timer--;
            }
            if (_energyPoolCtrl != null)
            {
                _energyPoolCtrl.LiquidVolume = (float)(200-_timer)/200;
            }
        }
    }
}
