﻿using SoyEngine;

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
            SetValue();
            return true;
        }

        private void SetValue()
        {
            _skillCtrl.CurrentSkills[0].SetValue(50, TableConvert.GetRange(600));
        }

        protected override void UpdateMonsterAI()
        {
//            if (_timerAttack == 0)
//            {
            SetInput(EInputType.Skill1, true);
//                _timerAttack = 30;
//            }
//            else
//            {
//                SetInput(EInputType.Skill1, false);
//            }
            base.UpdateMonsterAI();
        }
    }
}