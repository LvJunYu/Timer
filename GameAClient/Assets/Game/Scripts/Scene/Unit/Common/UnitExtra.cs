/********************************************************************
** Filename : UnitExtra
** Author : Dong
** Date : 2017/3/2 星期四 下午 1:52:53
** Summary : UnitExtra
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SoyEngine;
using SoyEngine.Proto;

#pragma warning disable 0660 0661

namespace GameA.Game
{
    /// <summary>
    /// 这个类不要加引用类型，方便赋值
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct UnitExtra : IEquatable<UnitExtra>
    {
        public static UnitExtra zero;
        public EMoveDirection MoveDirection;
        public byte Active;
        public ushort ChildId;
        public byte ChildRotation;
        public byte RotateMode;
        public byte RotateValue;
        public ushort TimeDelay;
        public ushort TimeInterval;
        public string Msg;
        public byte TeamId;
        public ushort Damage;
        public MultiParam Drops;
        public ushort EffectRange; //技能作用范围
        public ushort CastRange; //子弹射程
        public ushort ViewRange;
        public ushort BulletCount;
        public MultiParam KnockbackForces;
        public MultiParam AddStates;
        public ushort BulletSpeed;
        public ushort ChargeTime;
        public ushort MaxHp;
        public ushort MaxSpeedX;
        public ushort JumpAbility;
        public byte InjuredReduce;
        public ushort CureIncrease;

        //Npc相关

        public byte NpcType;
        public string NpcName;
        public string NpcDialog;
        public byte NpcShowType;
        public ushort NpcShowInterval;
        public MultiParamTask NpcTask;


        public bool IsDynamic()
        {
            return MoveDirection != EMoveDirection.None;
        }

        public bool Equals(UnitExtra other)
        {
            return MoveDirection == other.MoveDirection &&
                   Active == other.Active &&
                   ChildId == other.ChildId && ChildRotation == other.ChildRotation &&
                   RotateMode == other.RotateMode && RotateValue == other.RotateValue &&
                   TimeDelay == other.TimeDelay && TimeInterval == other.TimeInterval &&
                   Msg == other.Msg &&
                   TeamId == other.TeamId &&
                   MaxHp == other.MaxHp &&
                   Damage == other.Damage &&
                   MaxSpeedX == other.MaxSpeedX &&
                   Drops == other.Drops &&
                   EffectRange == other.EffectRange &&
                   CastRange == other.CastRange &&
                   ViewRange == other.ViewRange &&
                   JumpAbility == other.JumpAbility &&
                   BulletCount == other.BulletCount &&
                   ChargeTime == other.ChargeTime &&
                   KnockbackForces == other.KnockbackForces &&
                   AddStates == other.AddStates &&
                   BulletSpeed == other.BulletSpeed &&
                   InjuredReduce == other.InjuredReduce &&
                   CureIncrease == other.CureIncrease &&
                   NpcShowInterval == other.NpcShowInterval &&
                   NpcDialog == other.NpcDialog &&
                   NpcName == other.NpcName &&
                   NpcType == other.NpcType &&
                   NpcShowType == other.NpcShowType &&
                   NpcTask == other.NpcTask;
        }

        public static bool operator ==(UnitExtra a, UnitExtra other)
        {
            return a.Equals(other);
        }

        public static bool operator !=(UnitExtra a, UnitExtra other)
        {
            return !(a == other);
        }

        public void UpdateFromChildId()
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
                KnockbackForces.Set(skill.KnockbackForces);
            }
            if (skill.AddStates != null)
            {
                AddStates.Set(skill.AddStates);
            }
        }

        public Msg_Preinstall ToUnitPreInstall()
        {
            var msg = new Msg_Preinstall();
            msg.Data = GameMapDataSerializer.Instance.Serialize(GM2DTools.ToProto(IntVec3.zero, this));
            return msg;
        }

        public void Set(Preinstall data)
        {
            var unitExtraKeyValuePair = GameMapDataSerializer.Instance.Deserialize<UnitExtraKeyValuePair>(data.Data);
            this = GM2DTools.ToEngine(unitExtraKeyValuePair);
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
                    if (eMenu == UPCtrlUnitPropertyEditAdvance.EMenu.ActorSetting)
                    {
                        return 10;
                    }
                    return GetMin(eAdvanceAttribute);
                case EAdvanceAttribute.TimeInterval:
                    if (eMenu == UPCtrlUnitPropertyEditAdvance.EMenu.ActorSetting)
                    {
                        return 800;
                    }
                    return GetMin(eAdvanceAttribute);
            }
            return GetMin(eAdvanceAttribute);
        }

        private static int GetMin(EAdvanceAttribute eAdvanceAttribute)
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
                    return 0;
            }
            return 0;
        }

        private static int GetMax(EAdvanceAttribute eAdvanceAttribute)
        {
            switch (eAdvanceAttribute)
            {
                case EAdvanceAttribute.TimeInterval:
                    return 5000;
                case EAdvanceAttribute.Damage:
                    return 1000;
                case EAdvanceAttribute.EffectRange:
                    return 50;
                case EAdvanceAttribute.ViewRange:
                    return 300;
                case EAdvanceAttribute.BulletCount:
                    return 99;
                case EAdvanceAttribute.CastRange:
                    return 300;
                case EAdvanceAttribute.BulletSpeed:
                    return 20;
                case EAdvanceAttribute.ChargeTime:
                    return 10000;
                case EAdvanceAttribute.MaxHp:
                    return 2000;
                case EAdvanceAttribute.MaxSpeedX:
                    return 120;
                case EAdvanceAttribute.JumpAbility:
                    return 260;
                case EAdvanceAttribute.InjuredReduce:
                    return 100;
                case EAdvanceAttribute.CureIncrease:
                    return 500;
                case EAdvanceAttribute.NpcIntervalTiem:
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
                    return 100;
                case EAdvanceAttribute.Damage:
                case EAdvanceAttribute.EffectRange:
                case EAdvanceAttribute.ViewRange:
                case EAdvanceAttribute.BulletCount:
                case EAdvanceAttribute.CastRange:
                case EAdvanceAttribute.BulletSpeed:
                case EAdvanceAttribute.MaxHp:
                case EAdvanceAttribute.MaxSpeedX:
                case EAdvanceAttribute.JumpAbility:
                case EAdvanceAttribute.InjuredReduce:
                case EAdvanceAttribute.CureIncrease:
                    return 1;
            }
            return 0;
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
                    return "{0:f1}秒";
            }
            return "{0}";
        }

        private static int GetConvertValue(EAdvanceAttribute eAdvanceAttribute)
        {
            switch (eAdvanceAttribute)
            {
                case EAdvanceAttribute.TimeInterval:
                case EAdvanceAttribute.ChargeTime:
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

        public static bool CanEdit(EAdvanceAttribute eAdvanceAttribute, int id)
        {
            var table = TableManager.Instance.GetUnit(id);
            if (table == null)
            {
                LogHelper.Error("cant get unit which id == {0}", id);
                return false;
            }
            switch (eAdvanceAttribute)
            {
                case EAdvanceAttribute.TimeInterval:
                    return table.SkillId > 0 || table.ChildState != null;
                case EAdvanceAttribute.Damage:
                    return table.SkillId > 0 || table.ChildState != null;
                case EAdvanceAttribute.Drops:
                    return UnitDefine.IsMonster(id);
                case EAdvanceAttribute.EffectRange:
                    return table.SkillId > 0;
                case EAdvanceAttribute.ViewRange:
                    return false;
                case EAdvanceAttribute.BulletSpeed:
                case EAdvanceAttribute.CastRange:
                    return table.ChildState != null;
                case EAdvanceAttribute.BulletCount:
                case EAdvanceAttribute.ChargeTime:
                    return UnitDefine.IsEnergyPool(id);
                case EAdvanceAttribute.AddStates:
                    return table.SkillId > 0 || table.ChildState != null;
                case EAdvanceAttribute.MaxHp:
                    return table.Hp > 0;
                case EAdvanceAttribute.MaxSpeedX:
                    return table.MaxSpeed > 0;
                case EAdvanceAttribute.JumpAbility:
                    return table.JumpAbility > 0;
                case EAdvanceAttribute.InjuredReduce:
                    return table.Hp > 0;
                case EAdvanceAttribute.CureIncrease:
                    return table.Hp > 0;
            }
            return false;
        }
    }

    public struct MultiParam : IEquatable<MultiParam>
    {
        public bool HasContent;
        public ushort Param0;
        public ushort Param1;
        public ushort Param2;
        public ushort Param3;

        public bool Equals(MultiParam other)
        {
            return Param0 == other.Param0 &&
                   Param1 == other.Param1 &&
                   Param2 == other.Param2 &&
                   Param3 == other.Param3;
        }

        public static bool operator ==(MultiParam a, MultiParam other)
        {
            return a.Equals(other);
        }

        public static bool operator !=(MultiParam a, MultiParam other)
        {
            return !(a == other);
        }

        public void Set(int[] list)
        {
            if (list == null || list.Length == 0) return;
            HasContent = true;
            for (int i = 0; i < list.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        Param0 = (ushort) list[0];
                        break;
                    case 1:
                        Param1 = (ushort) list[1];
                        break;
                    case 2:
                        Param2 = (ushort) list[2];
                        break;
                    case 3:
                        Param3 = (ushort) list[3];
                        break;
                    default:
                        LogHelper.Error("list's length is bigger than param count");
                        break;
                }
            }
        }

        public List<int> ToList()
        {
            List<int> list = new List<int>();
            if (!HasContent) return list;
            if (Param0 != 0)
            {
                list.Add(Param0);
            }
            if (Param1 != 0)
            {
                list.Add(Param1);
            }
            if (Param2 != 0)
            {
                list.Add(Param2);
            }
            if (Param3 != 0)
            {
                list.Add(Param3);
            }
            return list;
        }
    }

    public struct MultiParamTask : IEquatable<MultiParamTask>
    {
        public bool HasContent;
        public NpcTask Param0;
        public NpcTask Param1;
        public NpcTask Param2;

        public bool Equals(MultiParamTask other)
        {
            return Param0 == other.Param0 &&
                   Param1 == other.Param1 &&
                   Param2 == other.Param2;
        }

        public static bool operator ==(MultiParamTask a, MultiParamTask other)
        {
            return a.Equals(other);
        }

        public static bool operator !=(MultiParamTask a, MultiParamTask other)
        {
            return !(a == other);
        }

        public void Set(NpcTask[] list)
        {
            if (list == null || list.Length == 0) return;
            HasContent = true;
            for (int i = 0; i < list.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        Param0 = (NpcTask) list[0];
                        break;
                    case 1:
                        Param1 = (NpcTask) list[1];
                        break;
                    case 2:
                        Param2 = (NpcTask) list[2];
                        break;
                    default:
                        LogHelper.Error("list's length is bigger than param count");
                        break;
                }
            }
        }

        public void Set(UnitExtraNpcTaskData[] list)
        {
            if (list == null || list.Length == 0) return;
            HasContent = true;
            for (int i = 0; i < list.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        Param0.Set(list[0]);
                        break;
                    case 1:
                        Param1.Set(list[1]);
                        break;
                    case 2:
                        Param2.Set(list[2]);
                        break;
                    default:
                        LogHelper.Error("list's length is bigger than param count");
                        break;
                }
            }
        }

        public List<UnitExtraNpcTaskData> ToListData()
        {
            List<UnitExtraNpcTaskData> list = new List<UnitExtraNpcTaskData>();
            if (!HasContent) return list;
            if (Param0.NpcSerialNumber > 0)
            {
                list.Add(Param0.ToUnitExtraNpcTaskData());
            }
            if (Param1.NpcSerialNumber > 0)
            {
                list.Add(Param1.ToUnitExtraNpcTaskData());
            }
            if (Param2.NpcSerialNumber > 0)
            {
                list.Add(Param2.ToUnitExtraNpcTaskData());
            }
            return list;
        }

        public List<NpcTask> ToList()
        {
            List<NpcTask> list = new List<NpcTask>();
            if (!HasContent) return list;
            if (Param0.NpcSerialNumber > 0)
            {
                list.Add(Param0);
            }
            if (Param1.NpcSerialNumber > 0)
            {
                list.Add(Param1);
            }
            if (Param2.NpcSerialNumber > 0)
            {
                list.Add(Param2);
            }
            return list;
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
        TimeDelay,
        TimeInterval,
        Text,
        Camp,
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
        Max
    }
}