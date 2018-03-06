/********************************************************************
** Filename : EnergyPool
** Author : Dong
** Date : 2017/3/22 星期三 下午 2:35:55
** Summary : EnergyPool
***********************************************************************/

using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8001, Type = typeof(EnergyPool))]
    public class EnergyPool : BlockBase
    {
        protected int _timer;
        protected int _weaponId;
        protected EnergyPoolCtrl _energyPoolCtrl;

        protected UnityNativeParticleItem _efffectWeapon;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            if (_withEffect == null)
            {
                return false;
            }
            _energyPoolCtrl = _withEffect.Trans.GetComponent<EnergyPoolCtrl>();
            _energyPoolCtrl.LiquidVolume = 0;
            UpdateEnergyEffect();
            return true;
        }

        protected override void Clear()
        {
            _timer = 0;
            if (_energyPoolCtrl != null)
            {
                _energyPoolCtrl.LiquidVolume = 0;
            }
            base.Clear();
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_efffectWeapon);
            _efffectWeapon = null;
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _weaponId = unitExtra.ChildId;
            return unitExtra;
        }

        private void UpdateEnergyEffect()
        {
            if (_weaponId == 0)
            {
                return;
            }
            var tableEquipment = TableManager.Instance.GetEquipment(_weaponId);
            if (tableEquipment == null)
            {
                LogHelper.Error("GetEquipment Failed : {0}", _weaponId);
                return;
            }
            FreeEffect(_efffectWeapon);
            _efffectWeapon = null;
            if (!string.IsNullOrEmpty(tableEquipment.Model) && _energyPoolCtrl != null)
            {
                _efffectWeapon =
                    GameParticleManager.Instance.GetUnityNativeParticleItem(tableEquipment.Model,
                        _energyPoolCtrl.Weapon);
                if (_efffectWeapon != null)
                {
                    _efffectWeapon.Play();
                }
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsAlive && other.IsPlayer)
            {
                if (_timer == 0)
                {
                    _timer = UnitDefine.EnergyTimer;
                    other.SetWeapon(_weaponId, GetUnitExtra());
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
                _energyPoolCtrl.LiquidVolume = (float) (_timer) / UnitDefine.EnergyTimer;
            }
        }
    }
}