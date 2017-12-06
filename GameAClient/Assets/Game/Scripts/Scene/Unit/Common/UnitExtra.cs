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
        public int AttackDistance;
        public int ViewRange;
        public int JumpAbility;
        public int BulletCount;

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
                   AttackDistance == other.AttackDistance &&
                   ViewRange == other.ViewRange &&
                   JumpAbility == other.JumpAbility &&
                   BulletCount == other.BulletCount;
        }

        public static bool operator ==(UnitExtra a, UnitExtra other)
        {
            return a.Equals(other);
        }

        public static bool operator !=(UnitExtra a, UnitExtra other)
        {
            return !(a == other);
        }
    }
}
