﻿/********************************************************************
** Filename : UnitDesc
** Author : Dong
** Date : 2016/4/21 星期四 下午 3:46:51
** Summary : UnitDesc
***********************************************************************/

using System;
using System.Runtime.InteropServices;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [StructLayout(LayoutKind.Sequential)]
    [System.Serializable]
    public struct UnitDesc : IEquatable<UnitDesc>
    {
        public static UnitDesc zero = new UnitDesc();

        public int Id;
        public IntVec3 Guid;
        public byte Rotation;
        public Vector2 Scale;

        public UnitDesc(int id, IntVec3 guid, byte rotation, Vector2 scale)
        {
            Id = id;
            Guid = guid;
            Rotation = rotation;
            Scale = scale;
        }

        public IntVec3 GetUpPos(int z)
        {
            return new IntVec3(Guid.x, Guid.y + ConstDefineGM2D.ServerTileScale, z);
        }

        public IntVec3 GetDownPos(int z)
        {
            return new IntVec3(Guid.x, Guid.y - ConstDefineGM2D.ServerTileScale, z);
        }

        public IntVec3 GetLeftPos(int z)
        {
            return new IntVec3(Guid.x - ConstDefineGM2D.ServerTileScale, Guid.y, z);
        }

        public IntVec3 GetRightPos(int z)
        {
            return new IntVec3(Guid.x + ConstDefineGM2D.ServerTileScale, Guid.y , z);
        }

        public static bool operator ==(UnitDesc a, UnitDesc other)
        {
            return (a.Id == other.Id) && (a.Guid == other.Guid) && (a.Rotation == other.Rotation);
        }

        public static bool operator !=(UnitDesc a, UnitDesc other)
        {
            return !(a == other);
        }

        public bool Equals(UnitDesc other)
        {
            return (Id == other.Id) && (Guid == other.Guid) && (Rotation == other.Rotation);
        }

        public override bool Equals(object obj)
        {
            return Equals((UnitDesc)obj);
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Guid: {1}, Rotation: {2}, Scale: {3}", Id, Guid, Rotation, Scale);
        }
    }
}