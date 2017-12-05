/********************************************************************
** Filename : UnitExtra
** Author : Dong
** Date : 2017/3/2 星期四 下午 1:52:53
** Summary : UnitExtra
***********************************************************************/

using System;
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
        Max,
    }
    
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct UnitExtra : IEquatable<UnitExtra>
    {
        public static UnitExtra zero;
        public UnitAdvance UnitAdvance;
        public EMoveDirection MoveDirection;
        public byte Active;
        public ushort ChildId;
        public byte ChildRotation;
        public byte RotateMode;
        public byte RotateValue;
        public ushort TimeDelay;
        public ushort TimeInterval;
        public string Msg;

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
                   Msg == other.Msg && UnitAdvance == other.UnitAdvance;
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
