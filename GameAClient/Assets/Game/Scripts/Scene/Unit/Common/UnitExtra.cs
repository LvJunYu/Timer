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

#pragma warning disable 0660 0661

namespace GameA.Game
{
    /// <summary>
    /// 这个类不要加引用类型，赋值时会引用同一个对象
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
        public int MaxHp;
        public int Damage;
        public ushort MaxSpeedX;
        public MultiParam Drops;
        public ushort AttackRange;
        public ushort ViewRange;
        public ushort JumpAbility;
        public ushort BulletCount;
        public ushort CastRange;
        public MultiParam KnockbackForces;
        public MultiParam AddStates;
        public ushort CastSpeed;

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
                   AttackRange == other.AttackRange &&
                   ViewRange == other.ViewRange &&
                   JumpAbility == other.JumpAbility &&
                   BulletCount == other.BulletCount &&
                   CastRange == other.CastRange &&
                   KnockbackForces == other.KnockbackForces &&
                   CastSpeed == other.CastSpeed &&
                   AddStates == other.AddStates;
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
            Damage = skill.Damage;
            TimeInterval = (ushort) skill.CDTime;
            BulletCount = (ushort) skill.BulletCount;
            CastRange = (ushort) skill.CastRange;
            CastSpeed = (ushort) skill.ProjectileSpeed;
            if (skill.KnockbackForces != null)
            {
                KnockbackForces.Set(skill.KnockbackForces);
            }
            if (skill.AddStates != null)
            {
                AddStates.Set(skill.AddStates);
            }
        }
    }

    public struct MultiParam : IEquatable<MultiParam>
    {
        public bool HasSet;
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
            HasSet = true;
            for (int i = 0; i < list.Length; i++)
            {
                if (i == 0)
                {
                    Param1 = (ushort) list[0];
                }
                else if (i == 1)
                {
                    Param1 = (ushort) list[1];
                }
                else if (i == 2)
                {
                    Param2 = (ushort) list[2];
                }
                else if (i == 3)
                {
                    Param3 = (ushort) list[3];
                }
                else
                {
                    LogHelper.Error("list's length is bigger than param count");
                    return;
                }
            }
        }

        public List<int> ToList()
        {
            List<int> list = new List<int>();
            if (!HasSet) return list;
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
        Style,
        Attribute,
        Skill,
        Drop,
        Max,
    }
}