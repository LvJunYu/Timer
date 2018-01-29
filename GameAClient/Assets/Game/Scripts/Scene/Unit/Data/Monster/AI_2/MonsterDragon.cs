using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 2007, Type = typeof(MonsterDragon))]
    public class MonsterDragon : MonsterAI_2
    {
        private int _weaponId;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _intelligenc = 3;
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            SetWeapon(_weaponId);
        }

        public override void StartSkill()
        {
            _timerAttack = 40;
//            SpeedX = 0;
            if (_animation != null && !_animation.IsPlaying(Attack, 1))
            {
                _animation.PlayOnce(Attack, 1, 1);
            }
        }

        public override bool SetWeapon(int id, UnitExtraDynamic unitExtra = null, int slot = -1)
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

        protected override void UpdateMonsterAI()
        {
            base.UpdateMonsterAI();

            SetInput(EInputType.Skill1, true);
        }

        protected override void OnFire()
        {
            base.OnFire();
            SetInput(EInputType.Skill1, false);
        }

        public override UnitExtraDynamic UpdateExtraData()
        {
            var unitExtra = base.UpdateExtraData();
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
    }
}