using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 2007, Type = typeof(MonsterDragon))]
    public class MonsterDragon : MonsterAI_2
    {
        private int _weaponId;

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            //兼容老版本
            if (unitExtra.ChildId < 1000)
            {
                _weaponId = unitExtra.ChildId / 100 * 1000 + unitExtra.ChildId % 100;
            }
            else
            {
                _weaponId = unitExtra.ChildId;
            }

            return unitExtra;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            SetWeapon(_weaponId);
        }

        public override void StartSkill()
        {
            if (_animation != null && !_animation.IsPlaying(Attack, 1))
            {
                _animation.PlayOnce(Attack, 1, 1);
            }
        }

        public override bool SetWeapon(int id, UnitExtraDynamic unitExtra = null, int slot = -1,
            ESkillType skillType = ESkillType.Normal)
        {
            if (id == 0)
            {
                return false;
            }

            var tableEquipment = TableManager.Instance.GetEquipment(id);
            if (tableEquipment == null)
            {
                LogHelper.Error("GetEquipment Failed : {0}", id);
                return false;
            }

            _skillCtrl = _skillCtrl ?? new SkillCtrl(this);
            _skillCtrl.SetSkill(tableEquipment.SkillId);
            return true;
        }
    }
}