﻿using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class UnitExtraDynamic : DictionaryObject<UnitExtraDynamic>
    {
        static UnitExtraDynamic()
        {
            DefineField<EMoveDirection>(FieldTag.MoveDirection, "MoveDirection");
            DefineField<byte>(FieldTag.Active, "Active");
            DefineField<ushort>(FieldTag.ChildId, "ChildId");
            DefineField<byte>(FieldTag.ChildRotation, "ChildRotation");
            DefineField<byte>(FieldTag.RotateMode, "RotateMode");
            DefineField<byte>(FieldTag.RotateValue, "RotateValue");
            DefineField<ushort>(FieldTag.TimeDelay, "TimeDelay");
            DefineField<ushort>(FieldTag.TimeInterval, "TimeInterval");
            DefineField<string>(FieldTag.Msg, "Msg");
            DefineField<byte>(FieldTag.TeamId, "TeamId");
            DefineField<ushort>(FieldTag.Damage, "Damage");
            DefineFieldList<ushort>(FieldTag.Drops, "Drops");
            DefineField<ushort>(FieldTag.EffectRange, "EffectRange");
            DefineField<ushort>(FieldTag.CastRange, "CastRange");
            DefineField<ushort>(FieldTag.ViewRange, "ViewRange");
            DefineField<ushort>(FieldTag.BulletCount, "BulletCount");
            DefineFieldList<ushort>(FieldTag.KnockbackForces, "KnockbackForces");
            DefineFieldList<ushort>(FieldTag.AddStates, "AddStates");
            DefineFieldList<ushort>(FieldTag.SurpriseBoxItems, "SurpriseBoxItems");
            DefineField<ushort>(FieldTag.BulletSpeed, "BulletSpeed");
            DefineField<ushort>(FieldTag.ChargeTime, "ChargeTime");
            DefineField<ushort>(FieldTag.MaxHp, "MaxHp");
            DefineField<ushort>(FieldTag.MaxSpeedX, "MaxSpeedX");
            DefineField<ushort>(FieldTag.JumpAbility, "JumpAbility");
            DefineField<byte>(FieldTag.InjuredReduce, "InjuredReduce");
            DefineField<ushort>(FieldTag.CureIncrease, "CureIncrease");
            DefineField<ushort>(FieldTag.MonsterIntervalTime, "MonsterIntervalTime");
            DefineField<ushort>(FieldTag.MaxCreatedMonster, "MaxCreatedMonster");
            DefineField<byte>(FieldTag.MaxAliveMonster, "MaxAliveMonster");
            DefineField<byte>(FieldTag.TimerSecond, "TimerSecond");
            DefineField<byte>(FieldTag.TimerMinSecond, "TimerMinSecond");
            DefineField<byte>(FieldTag.TimerMaxSecond, "TimerMaxSecond");
            DefineField<bool>(FieldTag.IsRandom, "TimerRandom");
            DefineField<bool>(FieldTag.TimerCirculation, "TimerCirculation");
            DefineField<byte>(FieldTag.SurpriseBoxInterval, "SurpriseBoxInterval");
            DefineField<bool>(FieldTag.SurpriseBoxCountLimit, "SurpriseBoxCountLimit");
            DefineField<byte>(FieldTag.SurpriseBoxMaxCount, "SurpriseBoxMaxCount");
            DefineField<ushort>(FieldTag.MonsterId, "MonsterId");
            DefineField<ushort>(FieldTag.CommonValue, "CommonValue");
            DefineField<byte>(FieldTag.CycleInterval, "CycleInterval");
            DefineField<byte>(FieldTag.NpcType, "NpcType");
            DefineField<string>(FieldTag.NpcName, "NpcName");
            DefineField<ushort>(FieldTag.NpcSerialNumber, "NpcSerialNumber");
            DefineField<byte>(FieldTag.NpcShowType, "NpcShowType");
            DefineField<string>(FieldTag.NpcDialog, "NpcDialog");
            DefineField<ushort>(FieldTag.NpcShowInterval, "NpcShowInterval");
            DefineFieldList<NpcTaskDynamic>(FieldTag.NpcTask, "NpcTask");
            DefineFieldList<UnitExtraDynamic>(FieldTag.InternalUnitExtras, "InternalUnitExtras");
        }

        public class FieldTag
        {
            private static int _nextId;
            public static readonly int MoveDirection = _nextId++;
            public static readonly int Active = _nextId++;
            public static readonly int ChildId = _nextId++;
            public static readonly int ChildRotation = _nextId++;
            public static readonly int RotateMode = _nextId++;
            public static readonly int RotateValue = _nextId++;
            public static readonly int TimeDelay = _nextId++;
            public static readonly int TimeInterval = _nextId++;
            public static readonly int Msg = _nextId++;
            public static readonly int TeamId = _nextId++;
            public static readonly int Damage = _nextId++;
            public static readonly int Drops = _nextId++;
            public static readonly int EffectRange = _nextId++;
            public static readonly int CastRange = _nextId++;
            public static readonly int ViewRange = _nextId++;
            public static readonly int BulletCount = _nextId++;
            public static readonly int KnockbackForces = _nextId++;
            public static readonly int AddStates = _nextId++;
            public static readonly int BulletSpeed = _nextId++;
            public static readonly int ChargeTime = _nextId++;
            public static readonly int MaxHp = _nextId++;
            public static readonly int MaxSpeedX = _nextId++;
            public static readonly int JumpAbility = _nextId++;
            public static readonly int InjuredReduce = _nextId++;
            public static readonly int CureIncrease = _nextId++;
            public static readonly int MonsterIntervalTime = _nextId++;
            public static readonly int MaxCreatedMonster = _nextId++;
            public static readonly int MaxAliveMonster = _nextId++;
            public static readonly int TimerSecond = _nextId++;
            public static readonly int TimerMinSecond = _nextId++;
            public static readonly int TimerMaxSecond = _nextId++;
            public static readonly int IsRandom = _nextId++;
            public static readonly int TimerCirculation = _nextId++;
            public static readonly int SurpriseBoxInterval = _nextId++;
            public static readonly int SurpriseBoxCountLimit = _nextId++;
            public static readonly int SurpriseBoxMaxCount = _nextId++;
            public static readonly int SurpriseBoxItems = _nextId++;
            public static readonly int MonsterId = _nextId++;
            public static readonly int CommonValue = _nextId++;
            public static readonly int CycleInterval = _nextId++;
            public static readonly int NpcType = _nextId++;
            public static readonly int NpcName = _nextId++;
            public static readonly int NpcSerialNumber = _nextId++;
            public static readonly int NpcDialog = _nextId++;
            public static readonly int NpcShowType = _nextId++;
            public static readonly int NpcShowInterval = _nextId++;
            public static readonly int NpcTask = _nextId++;
            public static readonly int InternalUnitExtras = _nextId++;
        }

        public EMoveDirection MoveDirection
        {
            get { return Get<EMoveDirection>(FieldTag.MoveDirection); }
            set { Set(value, FieldTag.MoveDirection); }
        }

        public byte Active
        {
            get { return Get<byte>(FieldTag.Active); }
            set { Set(value, FieldTag.Active); }
        }

        public ushort ChildId
        {
            get { return Get<ushort>(FieldTag.ChildId); }
            set { Set(value, FieldTag.ChildId); }
        }

        public byte ChildRotation
        {
            get { return Get<byte>(FieldTag.ChildRotation); }
            set { Set(value, FieldTag.ChildRotation); }
        }

        public byte RotateMode
        {
            get { return Get<byte>(FieldTag.RotateMode); }
            set { Set(value, FieldTag.RotateMode); }
        }

        public byte RotateValue
        {
            get { return Get<byte>(FieldTag.RotateValue); }
            set { Set(value, FieldTag.RotateValue); }
        }

        public ushort TimeDelay
        {
            get { return Get<ushort>(FieldTag.TimeDelay); }
            set { Set(value, FieldTag.TimeDelay); }
        }

        public ushort TimeInterval
        {
            get { return Get<ushort>(FieldTag.TimeInterval); }
            set { Set(value, FieldTag.TimeInterval); }
        }

        public string Msg
        {
            get { return Get<string>(FieldTag.Msg); }
            set { Set(value, FieldTag.Msg); }
        }

        public byte TeamId
        {
            get { return Get<byte>(FieldTag.TeamId); }
            set { Set(value, FieldTag.TeamId); }
        }

        public ushort Damage
        {
            get { return Get<ushort>(FieldTag.Damage); }
            set { Set(value, FieldTag.Damage); }
        }

        public DictionaryListObject Drops
        {
            get { return Get<DictionaryListObject>(FieldTag.Drops); }
            set { Set(value, FieldTag.Drops); }
        }

        /// <summary>
        /// 技能作用范围
        /// </summary>
        public ushort EffectRange
        {
            get { return Get<ushort>(FieldTag.EffectRange); }
            set { Set(value, FieldTag.EffectRange); }
        }

        /// <summary>
        /// 子弹射程
        /// </summary>
        public ushort CastRange
        {
            get { return Get<ushort>(FieldTag.CastRange); }
            set { Set(value, FieldTag.CastRange); }
        }

        public ushort ViewRange
        {
            get { return Get<ushort>(FieldTag.ViewRange); }
            set { Set(value, FieldTag.ViewRange); }
        }

        public ushort BulletCount
        {
            get { return Get<ushort>(FieldTag.BulletCount); }
            set { Set(value, FieldTag.BulletCount); }
        }

        public DictionaryListObject KnockbackForces
        {
            get { return Get<DictionaryListObject>(FieldTag.KnockbackForces); }
            set { Set(value, FieldTag.KnockbackForces); }
        }

        public DictionaryListObject AddStates
        {
            get { return Get<DictionaryListObject>(FieldTag.AddStates); }
            set { Set(value, FieldTag.AddStates); }
        }

        public DictionaryListObject SurpriseBoxItems
        {
            get { return Get<DictionaryListObject>(FieldTag.SurpriseBoxItems); }
            set { Set(value, FieldTag.SurpriseBoxItems); }
        }

        public ushort BulletSpeed
        {
            get { return Get<ushort>(FieldTag.BulletSpeed); }
            set { Set(value, FieldTag.BulletSpeed); }
        }

        public ushort ChargeTime
        {
            get { return Get<ushort>(FieldTag.ChargeTime); }
            set { Set(value, FieldTag.ChargeTime); }
        }

        public ushort MaxHp
        {
            get { return Get<ushort>(FieldTag.MaxHp); }
            set { Set(value, FieldTag.MaxHp); }
        }

        public ushort MaxSpeedX
        {
            get { return Get<ushort>(FieldTag.MaxSpeedX); }
            set { Set(value, FieldTag.MaxSpeedX); }
        }

        public ushort JumpAbility
        {
            get { return Get<ushort>(FieldTag.JumpAbility); }
            set { Set(value, FieldTag.JumpAbility); }
        }

        public byte InjuredReduce
        {
            get { return Get<byte>(FieldTag.InjuredReduce); }
            set { Set(value, FieldTag.InjuredReduce); }
        }

        public ushort CureIncrease
        {
            get { return Get<ushort>(FieldTag.CureIncrease); }
            set { Set(value, FieldTag.CureIncrease); }
        }

        public ushort MonsterIntervalTime
        {
            get { return Get<ushort>(FieldTag.MonsterIntervalTime); }
            set { Set(value, FieldTag.MonsterIntervalTime); }
        }

        public ushort MaxCreatedMonster
        {
            get { return Get<ushort>(FieldTag.MaxCreatedMonster); }
            set { Set(value, FieldTag.MaxCreatedMonster); }
        }

        public byte MaxAliveMonster
        {
            get { return Get<byte>(FieldTag.MaxAliveMonster); }
            set { Set(value, FieldTag.MaxAliveMonster); }
        }

        public byte TimerSecond
        {
            get { return Get<byte>(FieldTag.TimerSecond); }
            set { Set(value, FieldTag.TimerSecond); }
        }

        public byte TimerMinSecond
        {
            get { return Get<byte>(FieldTag.TimerMinSecond); }
            set { Set(value, FieldTag.TimerMinSecond); }
        }

        public byte TimerMaxSecond
        {
            get { return Get<byte>(FieldTag.TimerMaxSecond); }
            set { Set(value, FieldTag.TimerMaxSecond); }
        }

        public bool IsRandom
        {
            get { return Get<bool>(FieldTag.IsRandom); }
            set { Set(value, FieldTag.IsRandom); }
        }

        public bool TimerCirculation
        {
            get { return Get<bool>(FieldTag.TimerCirculation); }
            set { Set(value, FieldTag.TimerCirculation); }
        }

        public byte SurpriseBoxInterval
        {
            get { return Get<byte>(FieldTag.SurpriseBoxInterval); }
            set { Set(value, FieldTag.SurpriseBoxInterval); }
        }

        public bool SurpriseBoxCountLimit
        {
            get { return Get<bool>(FieldTag.SurpriseBoxCountLimit); }
            set { Set(value, FieldTag.SurpriseBoxCountLimit); }
        }

        public byte SurpriseBoxMaxCount
        {
            get { return Get<byte>(FieldTag.SurpriseBoxMaxCount); }
            set { Set(value, FieldTag.SurpriseBoxMaxCount); }
        }

        public ushort MonsterId
        {
            get { return Get<ushort>(FieldTag.MonsterId); }
            set { Set(value, FieldTag.MonsterId); }
        }

        public ushort CommonValue
        {
            get { return Get<ushort>(FieldTag.CommonValue); }
            set { Set(value, FieldTag.CommonValue); }
        }

        public byte CycleInterval
        {
            get { return Get<byte>(FieldTag.CycleInterval); }
            set { Set(value, FieldTag.CycleInterval); }
        }

        public byte NpcType
        {
            get { return Get<byte>(FieldTag.NpcType); }
            set { Set(value, FieldTag.NpcType); }
        }

        public string NpcName
        {
            get { return Get<string>(FieldTag.NpcName); }
            set { Set(value, FieldTag.NpcName); }
        }

        public ushort NpcSerialNumber
        {
            get { return Get<ushort>(FieldTag.NpcSerialNumber); }
            set { Set(value, FieldTag.NpcSerialNumber); }
        }

        public string NpcDialog
        {
            get { return Get<string>(FieldTag.NpcDialog); }
            set { Set(value, FieldTag.NpcDialog); }
        }

        public byte NpcShowType
        {
            get { return Get<byte>(FieldTag.NpcShowType); }
            set { Set(value, FieldTag.NpcShowType); }
        }

        public ushort NpcShowInterval
        {
            get { return Get<ushort>(FieldTag.NpcShowInterval); }
            set { Set(value, FieldTag.NpcShowInterval); }
        }

        public DictionaryListObject NpcTask
        {
            get { return Get<DictionaryListObject>(FieldTag.NpcTask); }
            set { Set(value, FieldTag.NpcTask); }
        }

        public DictionaryListObject InternalUnitExtras
        {
            get { return Get<DictionaryListObject>(FieldTag.InternalUnitExtras); }
            set { Set(value, FieldTag.InternalUnitExtras); }
        }

        public bool IsDynamic()
        {
            return MoveDirection != EMoveDirection.None;
        }

        public void UpdateDefaultValueFromChildId()
        {
            int skillId = TableManager.Instance.GetEquipment(ChildId).SkillId;
            var skill = TableManager.Instance.GetSkill(skillId);
            Damage = (ushort) skill.Damage;
            TimeInterval = (ushort) skill.CDTime;
            BulletCount = (ushort) skill.BulletCount;
            CastRange = (ushort) skill.CastRange;
            ChargeTime = (ushort) skill.ChargeTime;
            BulletSpeed = (ushort) skill.ProjectileSpeed;
            if (skill.KnockbackForces != null)
            {
                if (KnockbackForces != null)
                {
                    KnockbackForces.Clear();
                }

                for (int i = 0; i < skill.KnockbackForces.Length; i++)
                {
                    Set((ushort) skill.KnockbackForces[i], FieldTag.KnockbackForces, i);
                }
            }

            if (skill.AddStates != null)
            {
                if (AddStates != null)
                {
                    AddStates.Clear();
                }

                for (int i = 0; i < skill.AddStates.Length; i++)
                {
                    Set((ushort) skill.AddStates[i], FieldTag.AddStates, i);
                }
            }
        }

        public void UpdateFromMonsterId()
        {
            var monsterTable = TableManager.Instance.GetUnit(MonsterId);
            MaxHp = (ushort) monsterTable.Hp;
            MaxSpeedX = (ushort) monsterTable.MaxSpeed;
            InjuredReduce = 0;
            CureIncrease = 0;
            ChargeTime = 0;
            int skillId = monsterTable.SkillId;
            var skill = TableManager.Instance.GetSkill(skillId);
            if (skill != null)
            {
                Damage = (ushort) skill.Damage;
                TimeInterval = (ushort) skill.CDTime;
                CastRange = (ushort) skill.CastRange;
                if (skill.EffectValues != null && skill.EffectValues.Length > 0)
                {
                    EffectRange = (ushort) skill.EffectValues[0];
                }

                if (skill.KnockbackForces != null &&
                    !(skill.KnockbackForces.Length == 1 && skill.KnockbackForces[0] == 0))
                {
                    for (int i = 0; i < skill.KnockbackForces.Length; i++)
                    {
                        Set((ushort) skill.KnockbackForces[i], FieldTag.KnockbackForces, i);
                    }
                }
                else
                {
                    KnockbackForces = null;
                }

                if (skill.AddStates != null && !(skill.AddStates.Length == 1 && skill.AddStates[0] == 0))
                {
                    for (int i = 0; i < skill.AddStates.Length; i++)
                    {
                        Set((ushort) skill.AddStates[i], FieldTag.AddStates, i);
                    }
                }
                else
                {
                    AddStates = null;
                }
            }
        }

        public Msg_Preinstall ToUnitPreInstall()
        {
            var msg = new Msg_Preinstall();
            msg.Data = ClientProtoSerializer.Instance.Serialize(GM2DTools.ToProto(IntVec3.zero, this));
            return msg;
        }

        public void Set(UnitExtraKeyValuePair unitExtraKeyValuePair)
        {
            GM2DTools.ToEngine(unitExtraKeyValuePair, this);
        }

        public static UnitExtraDynamic GetDefaultPlayerValue(int index = 0,
            EProjectType projectTpye = EProjectType.PT_Single, UnitExtraDynamic unitExtraDynamic = null)
        {
            var table = TableManager.Instance.GetUnit(UnitDefine.MainPlayerId);
            if (unitExtraDynamic == null)
            {
                unitExtraDynamic = new UnitExtraDynamic();
            }

            if (projectTpye == EProjectType.PT_Compete)
            {
                unitExtraDynamic.TeamId = (byte) (index + 1);
            }
            else
            {
                unitExtraDynamic.TeamId = 1;
            }

            unitExtraDynamic.MaxHp = (ushort) table.Hp;
            unitExtraDynamic.JumpAbility = (ushort) table.JumpAbility;
            unitExtraDynamic.MaxSpeedX = (ushort) table.MaxSpeed;
            return unitExtraDynamic;
        }
    }

    public class UnitExtraHelper
    {
        public static int GetMin(EAdvanceAttribute eAdvanceAttribute,
            UPCtrlUnitPropertyEditAdvance.EMenu eMenu)
        {
            switch (eAdvanceAttribute)
            {
                case EAdvanceAttribute.EffectRange:
                    if (eMenu == UPCtrlUnitPropertyEditAdvance.EMenu.Camp)
                    {
                        return 10;
                    }

                    return GetMin(eAdvanceAttribute);
                case EAdvanceAttribute.TimeInterval:
                    if (eMenu == UPCtrlUnitPropertyEditAdvance.EMenu.Camp)
                    {
                        return 800;
                    }

                    return GetMin(eAdvanceAttribute);
            }

            return GetMin(eAdvanceAttribute);
        }

        public static int GetMin(EAdvanceAttribute eAdvanceAttribute)
        {
            switch (eAdvanceAttribute)
            {
                case EAdvanceAttribute.TimeInterval:
                    return 100;
                case EAdvanceAttribute.Damage:
                    return 1;
                case EAdvanceAttribute.EffectRange:
                    return 10;
                case EAdvanceAttribute.ViewRange:
                    return 10;
                case EAdvanceAttribute.BulletCount:
                    return 1;
                case EAdvanceAttribute.CastRange:
                    return 20;
                case EAdvanceAttribute.ChargeTime:
                    return 500;
                case EAdvanceAttribute.BulletSpeed:
                    return 5;
                case EAdvanceAttribute.MaxHp:
                    return 1;
                case EAdvanceAttribute.MaxSpeedX:
                    return 20;
                case EAdvanceAttribute.JumpAbility:
                    return 100;
                case EAdvanceAttribute.InjuredReduce:
                    return 0;
                case EAdvanceAttribute.CureIncrease:
                    return 0;
                case EAdvanceAttribute.NpcIntervalTiem:
                    return 1;
                case EAdvanceAttribute.MonsterIntervalTime:
                    return 500;
                case EAdvanceAttribute.MaxCreatedMonster:
                    return 1;
                case EAdvanceAttribute.MaxAliveMonster:
                    return 1;
                case EAdvanceAttribute.TimerSecond:
                case EAdvanceAttribute.TimerMinSecond:
                case EAdvanceAttribute.TimerMaxSecond:
                case EAdvanceAttribute.SurpriseBoxInterval:
                case EAdvanceAttribute.SurpriseBoxMaxCount:
                case EAdvanceAttribute.CycleInterval:
                    return 1;
                case EAdvanceAttribute.MaxTaskKillOrColltionNum:
                    return 1;
                case EAdvanceAttribute.MaxTaskTimeLimit:
                    return 10;
            }

            return 0;
        }

        private static int GetMax(EAdvanceAttribute eAdvanceAttribute)
        {
            switch (eAdvanceAttribute)
            {
                case EAdvanceAttribute.ChargeTime:
                    return 10000;
                case EAdvanceAttribute.TimeInterval:
                case EAdvanceAttribute.MonsterIntervalTime:
                    return 5000;
                case EAdvanceAttribute.MaxHp:
                    return 2000;
                case EAdvanceAttribute.Damage:
                    return 1000;
                case EAdvanceAttribute.CureIncrease:
                    return 500;
                case EAdvanceAttribute.ViewRange:
                case EAdvanceAttribute.CastRange:
                case EAdvanceAttribute.MaxCreatedMonster:
                case EAdvanceAttribute.MaxTaskTimeLimit:
                    return 300;
                case EAdvanceAttribute.JumpAbility:
                    return 260;
                case EAdvanceAttribute.MaxSpeedX:
                    return 120;
                case EAdvanceAttribute.InjuredReduce:
                case EAdvanceAttribute.SurpriseBoxMaxCount:
                    return 100;
                case EAdvanceAttribute.BulletCount:
                case EAdvanceAttribute.TimerSecond:
                case EAdvanceAttribute.TimerMinSecond:
                case EAdvanceAttribute.TimerMaxSecond:
                case EAdvanceAttribute.MaxTaskKillOrColltionNum:
                    return 99;
                case EAdvanceAttribute.CycleInterval:
                    return 60;
                case EAdvanceAttribute.EffectRange:
                    return 50;
                case EAdvanceAttribute.BulletSpeed:
                case EAdvanceAttribute.SurpriseBoxInterval:
                    return 20;
                case EAdvanceAttribute.NpcIntervalTiem:
                case EAdvanceAttribute.MaxAliveMonster:
                    return 10;
            }

            return 0;
        }

        private static int GetDelta(EAdvanceAttribute eAdvanceAttribute)
        {
            switch (eAdvanceAttribute)
            {
                case EAdvanceAttribute.TimeInterval:
                case EAdvanceAttribute.ChargeTime:
                case EAdvanceAttribute.MonsterIntervalTime:
                    return 100;
            }

            return 1;
        }

        private static string GetFormat(EAdvanceAttribute eAdvanceAttribute)
        {
            switch (eAdvanceAttribute)
            {
                case EAdvanceAttribute.InjuredReduce:
                case EAdvanceAttribute.CureIncrease:
                    return "{0}%";
                case EAdvanceAttribute.TimeInterval:
                case EAdvanceAttribute.ChargeTime:
                case EAdvanceAttribute.MonsterIntervalTime:
                    return "{0:f1}秒";
                case EAdvanceAttribute.NpcIntervalTiem:
                case EAdvanceAttribute.MaxTaskTimeLimit:
                case EAdvanceAttribute.SurpriseBoxInterval:
                case EAdvanceAttribute.CycleInterval:
                    return "{0}秒";
            }

            return "{0}";
        }

        private static int GetConvertValue(EAdvanceAttribute eAdvanceAttribute)
        {
            switch (eAdvanceAttribute)
            {
                case EAdvanceAttribute.TimeInterval:
                case EAdvanceAttribute.ChargeTime:
                case EAdvanceAttribute.MonsterIntervalTime:
                    return 1000;
            }

            return 1;
        }

        public static void SetUSCtrlSliderSetting(USCtrlSliderSetting usCtrlSliderSetting,
            EAdvanceAttribute eAdvanceAttribute, Action<int> callBack)
        {
            usCtrlSliderSetting.Set(GetMin(eAdvanceAttribute), GetMax(eAdvanceAttribute), callBack,
                GetDelta(eAdvanceAttribute), GetFormat(eAdvanceAttribute),
                GetConvertValue(eAdvanceAttribute));
        }
    }

    public enum EEditType
    {
        None = -1,

        /// <summary>
        /// 存在Node里面。
        /// </summary>
        Direction,
        MoveDirection,
        Active,
        Child,
        Rotate,
        Angel,
        TimeDelay,
        TimeInterval,
        Text,
        Camp,
        MonsterCave,
        Spawn,
        SurpriseBox,
        WoodCase,
        PasswordDoor,
        Bomb,
        Cycle,
        Style,
        NpcType,
        NpcTask,
        Max,
    }

    public enum EAdvanceAttribute
    {
        TimeInterval,
        Damage,
        Drops,
        EffectRange,
        ViewRange,
        BulletCount,
        CastRange,
        AddStates,
        BulletSpeed,
        MaxHp,
        MaxSpeedX,
        JumpAbility,
        ChargeTime,
        InjuredReduce,
        CureIncrease,
        NpcIntervalTiem,
        MonsterIntervalTime,
        MaxCreatedMonster,
        MaxAliveMonster,
        TimerSecond,
        TimerMinSecond,
        TimerMaxSecond,
        SurpriseBoxInterval,
        SurpriseBoxMaxCount,
        MaxTaskKillOrColltionNum,
        MaxTaskTimeLimit,
        CycleInterval,
        Spawn,
        Max
    }
}