using System;
using System.Runtime.InteropServices;

#pragma warning disable 0660 0661

namespace GameA.Game
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct UnitAdvance : IEquatable<UnitAdvance>
    {
        public static UnitAdvance zero;

        public int TeamId;
        public int MaxHp;
        public int AttackPower;
        public int MaxSpeedX;
        public int Drops;
        public int AttackDistance;
        public int AttackInterval;
        public int ViewRange;
        public int JumpAbility;

        public bool Equals(UnitAdvance other)
        {
            return TeamId == other.TeamId &&
                   MaxHp == other.MaxHp &&
                   AttackPower == other.AttackPower &&
                   MaxSpeedX == other.MaxSpeedX &&
                   Drops == other.Drops &&
                   AttackDistance == other.AttackDistance &&
                   AttackInterval == other.AttackInterval &&
                   ViewRange == other.ViewRange &&
                   JumpAbility == other.JumpAbility;
        }

        public static bool operator ==(UnitAdvance a, UnitAdvance other)
        {
            return a.Equals(other);
        }

        public static bool operator !=(UnitAdvance a, UnitAdvance other)
        {
            return !(a == other);
        }
    }
}