using SoyEngine;

namespace GameA.Game
{
    public class PlayerUnitExtraDynamic : DictionaryObject<PlayerUnitExtraDynamic>
    {
        static PlayerUnitExtraDynamic()
        {
            DefineField<byte>(FieldTag.TeamId, "TeamId");
            DefineField<ushort>(FieldTag.MaxHp, "MaxHp");
            DefineField<byte>(FieldTag.InjuredReduce, "InjuredReduce");
            DefineField<ushort>(FieldTag.CureIncrease, "CureIncrease");
            DefineFieldList<WeaponExtraDynamic>(FieldTag.Weapons, "Weapons");
        }

        public class FieldTag
        {
            private static int _nextId;
            public static readonly int TeamId = _nextId++;
            public static readonly int MaxHp = _nextId++;
            public static readonly int InjuredReduce = _nextId++;
            public static readonly int CureIncrease = _nextId++;
            public static readonly int Weapons = _nextId++;
        }

        public byte TeamId
        {
            get { return Get<byte>(FieldTag.TeamId); }
            set { Set(value, FieldTag.TeamId);}
        }
        public ushort MaxHp
        {
            get { return Get<ushort>(FieldTag.MaxHp); }
            set { Set(value, FieldTag.MaxHp);}
        }
        public byte InjuredReduce
        {
            get { return Get<byte>(FieldTag.InjuredReduce); }
            set { Set(value, FieldTag.InjuredReduce);}
        }
        public ushort CureIncrease
        {
            get { return Get<ushort>(FieldTag.CureIncrease); }
            set { Set(value, FieldTag.CureIncrease);}
        }
        public DictionaryListObject Weapons
        {
            get { return Get<DictionaryListObject>(FieldTag.Weapons); }
            set { Set(value, FieldTag.Weapons);}
        }

        public static PlayerUnitExtraDynamic GetDefaultValue()
        {
            var table = TableManager.Instance.GetUnit(UnitDefine.MainPlayerId);
            var playerUnitExtraDynamic = new PlayerUnitExtraDynamic();
            playerUnitExtraDynamic.TeamId = 1;
            playerUnitExtraDynamic.MaxHp = (ushort) table.Hp;
            return playerUnitExtraDynamic;
        }
    }
}