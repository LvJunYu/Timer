/********************************************************************
** Filename : UnitExtra
** Author : Dong
** Date : 2017/3/2 星期四 下午 1:52:53
** Summary : UnitExtra
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#pragma warning disable 0660 0661

namespace GameA.Game
{
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
        public int TeamId;
        public int MaxHp;
        public int Damage;
        public int MaxSpeedX;
        public List<int> Drops;
        public int AttackRange;
        public int ViewRange;
        public int JumpAbility;
        public int BulletCount;
        public int CastRange;
        public List<int> KnockbackForces;
        public List<int> AddStates;
        public int CastSpeed;
        
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
            BulletCount = skill.BulletCount;
            CastRange = skill.CastRange;
            CastSpeed = skill.ProjectileSpeed;
            if (KnockbackForces == null)
            {
                KnockbackForces = new List<int>(2);
            }
            else
            {
                KnockbackForces.Clear();
            }
            if (skill.KnockbackForces != null)
            {
                for (int i = 0; i < skill.KnockbackForces.Length; i++)
                {
                    KnockbackForces.Add(skill.KnockbackForces[i]);
                }
            }
            if (AddStates == null)
            {
                AddStates = new List<int>(2);
            }
            else
            {
                AddStates.Clear();
            }
            if (skill.AddStates != null)
            {
                for (int i = 0; i < skill.AddStates.Length; i++)
                {
                    AddStates.Add(skill.AddStates[i]);
                }
            }
        }
    }
}