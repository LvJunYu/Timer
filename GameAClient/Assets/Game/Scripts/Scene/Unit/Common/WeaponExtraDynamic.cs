using SoyEngine;

namespace GameA.Game
{
    public class WeaponExtraDynamic : DictionaryObject<WeaponExtraDynamic>
    {
        static WeaponExtraDynamic()
        {
            DefineField<ushort>(FieldTag.ChildId, "ChildId");
            DefineField<ushort>(FieldTag.Damage, "Damage");
            DefineField<ushort>(FieldTag.CastRange, "CastRange");
            DefineField<ushort>(FieldTag.AttackInterval, "AttackInterval");
            DefineField<ushort>(FieldTag.BulletCount, "BulletCount");
            DefineField<ushort>(FieldTag.BulletSpeed, "BulletSpeed");
            DefineField<ushort>(FieldTag.ChargeTime, "ChargeTime");
        }

        public class FieldTag
        {
            private static int _nextId;
            public static readonly int ChildId = _nextId++;
            public static readonly int Damage = _nextId++;
            public static readonly int CastRange = _nextId++;
            public static readonly int AttackInterval = _nextId++;
            public static readonly int BulletCount = _nextId++;
            public static readonly int BulletSpeed = _nextId++;
            public static readonly int ChargeTime = _nextId++;
        }

        public ushort ChildId
        {
            get { return Get<ushort>(FieldTag.ChildId); }
            set { Set(value, FieldTag.ChildId);}
        }
        public ushort Damage
        {
            get { return Get<ushort>(FieldTag.Damage); }
            set { Set(value, FieldTag.Damage);}
        }
        public ushort CastRange
        {
            get { return Get<ushort>(FieldTag.CastRange); }
            set { Set(value, FieldTag.CastRange);}
        }
        public ushort AttackInterval
        {
            get { return Get<ushort>(FieldTag.AttackInterval); }
            set { Set(value, FieldTag.AttackInterval);}
        }
        public ushort BulletCount
        {
            get { return Get<ushort>(FieldTag.BulletCount); }
            set { Set(value, FieldTag.BulletCount);}
        }
        public ushort BulletSpeed
        {
            get { return Get<ushort>(FieldTag.BulletSpeed); }
            set { Set(value, FieldTag.BulletSpeed);}
        }
        public ushort ChargeTime
        {
            get { return Get<ushort>(FieldTag.ChargeTime); }
            set { Set(value, FieldTag.ChargeTime);}
        }
        
        public void UpdateDefaultValueFromChildId()
        {
            int skillId = TableManager.Instance.GetEquipment(ChildId).SkillId;
            var skill = TableManager.Instance.GetSkill(skillId);
            Damage = (ushort) skill.Damage;
            AttackInterval = (ushort) skill.CDTime;
            BulletCount = (ushort) skill.BulletCount;
            CastRange = (ushort) skill.CastRange;
            ChargeTime = (ushort) skill.ChargeTime;
            BulletSpeed = (ushort) skill.ProjectileSpeed;
        }
        
        public static WeaponExtraDynamic GetDefaultValue()
        {
            var weaponExtraDynamic = new WeaponExtraDynamic();
            weaponExtraDynamic.ChildId = UnitDefine.WaterGun;
            weaponExtraDynamic.UpdateDefaultValueFromChildId();
            return weaponExtraDynamic;
        }
    }
}