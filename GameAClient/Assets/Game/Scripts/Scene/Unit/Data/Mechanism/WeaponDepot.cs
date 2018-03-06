using DG.Tweening;
using NewResourceSolution;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5007, Type = typeof(WeaponDepot))]
    public class WeaponDepot : CollectionBase
    {
       
        private const string IconFormat = "{0}Icon";
        protected int _timer;
        protected int _weaponId;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            _tweener = _trans.DOMoveY(_trans.position.y + 0.1f, 0.6f);
            _tweener.Play();
            _tweener.SetLoops(-1, LoopType.Yoyo);

            return true;
        }

        protected override void InitAssetPath()
        {
            _assetPath = GetSpriteName(_weaponId);
        }

        protected override void Clear()
        {
            _timer = 0;
            base.Clear();
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _weaponId = unitExtra.ChildId;
            return unitExtra;
        }

        protected override void OnTrigger(UnitBase other)
        {
            if (_timer == 0)
            {
                _timer = UnitDefine.EnergyTimer;
                other.SetWeapon(_weaponId, GetUnitExtra());
                base.OnTrigger(other);
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_timer > 0)
            {
                _timer--;
            }
        }

        private static string GetSpriteName(int weaponId)
        {
            var tableEquipment = TableManager.Instance.GetEquipment(weaponId);
            if (tableEquipment != null)
            {
                return tableEquipment.Model;
            }

            return "M1WeaponWater";
        }

        public static Sprite GetSpriteIcon(int weaponId)
        {
            return JoyResManager.Instance.GetSprite(string.Format(IconFormat, GetSpriteName(weaponId)));
        }
    }
}