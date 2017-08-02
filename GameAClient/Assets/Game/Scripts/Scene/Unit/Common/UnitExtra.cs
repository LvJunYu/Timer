/********************************************************************
** Filename : UnitExtra
** Author : Dong
** Date : 2017/3/2 星期四 下午 1:52:53
** Summary : UnitExtra
***********************************************************************/

using System;
using System.Runtime.InteropServices;

namespace GameA.Game
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct UnitExtra : IEquatable<UnitExtra>
    {
        public static UnitExtra zero;
        public EMoveDirection MoveDirection;
        public EMoveDirection RollerDirection;
        public string Msg;
        /// <summary>
        /// 0代表zero，所以赋值时候不能为0
        /// </summary>
        public byte UnitValue;
        public UnitChild Child;

        public bool IsDynamic()
        {
            return MoveDirection != EMoveDirection.None;
        }

        public bool Equals(UnitExtra other)
        {
            return MoveDirection == other.MoveDirection && Msg == other.Msg && RollerDirection == other.RollerDirection && UnitValue == other.UnitValue && 
                   Child.Equals(other.Child);
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

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct UnitChild : IEquatable<UnitChild>
    {
        public ushort Id;
        public byte Rotation;
        public EMoveDirection MoveDirection;

        public UnitChild(ushort id, byte rotation, EMoveDirection moveDirection)
        {
            Id = id;
            Rotation = rotation;
            MoveDirection = moveDirection;
        }

        public bool Equals(UnitChild other)
        {
            return Id == other.Id && Rotation == other.Rotation && MoveDirection == other.MoveDirection;
        }
    }
}
