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
            var unitExtra = DataScene2D.Instance.GetUnitExtra(_guid);
            _weaponId = unitExtra.ChildId;
            SetWeapon(_weaponId);
        }

        public override void StartSkill()
        {
            _timerAttack = 40;
//            SpeedX = 0;
            if (_animation != null && !_animation.IsPlaying("Attack", 1))
            {
                _animation.PlayOnce("Attack", 1, 1);
            }
        }

        public override bool SetWeapon(int id)
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
            _skillCtrl.SetSkill(tableEquipment.MonsterSkillId);
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
    }
}