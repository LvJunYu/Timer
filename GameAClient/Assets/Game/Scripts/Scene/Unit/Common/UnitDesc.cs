/********************************************************************
** Filename : UnitDesc
** Author : Dong
** Date : 2016/4/21 星期四 下午 3:46:51
** Summary : UnitDesc
***********************************************************************/

using System;
using System.Runtime.InteropServices;
using SoyEngine;
using UnityEngine;

#pragma warning disable 0660 0661

namespace GameA.Game
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct UnitDesc : IEquatable<UnitDesc>
    {
        public static UnitDesc zero;

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

        public IntVec2 GetUpPos()
        {
            return new IntVec2(Guid.x, Guid.y + ConstDefineGM2D.ServerTileScale);
        }

        public IntVec3 GetDownPos(int z)
        {
            return new IntVec3(Guid.x, Guid.y - ConstDefineGM2D.ServerTileScale, z);
        }

        public IntVec3 GetLeftPos(int z)
        {
            return new IntVec3(Guid.x - ConstDefineGM2D.ServerTileScale, Guid.y, z);
        }
        
        public IntVec2 GetLeftPos()
        {
            return new IntVec2(Guid.x - ConstDefineGM2D.ServerTileScale, Guid.y);
        }

        public IntVec3 GetRightPos(int z)
        {
            return new IntVec3(Guid.x + ConstDefineGM2D.ServerTileScale, Guid.y , z);
        }
        
        public IntVec2 GetRightPos()
        {
            return new IntVec2(Guid.x + ConstDefineGM2D.ServerTileScale, Guid.y);
        }
      
        public override string ToString()
        {
            return string.Format("Id: {0}, Guid: {1}, Rotation: {2}, Scale: {3}", Id, Guid, Rotation, Scale);
        }
        
        public static bool operator ==(UnitDesc a, UnitDesc other)
        {
            return a.Equals(other);
        }

        public static bool operator !=(UnitDesc a, UnitDesc other)
        {
            return !(a == other);
        }

        public bool Equals(UnitDesc other)
        {
            return Id == other.Id && Guid == other.Guid && Rotation == other.Rotation &&Util.IsVector2Equal(Scale, other.Scale);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ Guid.GetHashCode();
                hashCode = (hashCode * 397) ^ Rotation.GetHashCode();
                hashCode = (hashCode * 397) ^ Scale.GetHashCode();
                return hashCode;
            }
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct ColliderDesc : IEquatable<ColliderDesc>
    {
        public Grid2D Grid;
        public byte Depth;
        public byte Layer;
        public bool IsDynamic;

        public bool Equals(ColliderDesc other)
        {
            return Grid.Equals(other.Grid) && Depth == other.Depth && Layer == other.Layer && IsDynamic == other.IsDynamic;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ColliderDesc && Equals((ColliderDesc) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Grid.GetHashCode();
                hashCode = (hashCode * 397) ^ Depth.GetHashCode();
                hashCode = (hashCode * 397) ^ Layer.GetHashCode();
                hashCode = (hashCode * 397) ^ IsDynamic.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Grid: {0}, Depth: {1}, Layer: {2}, IsDynamic: {3}", Grid, Depth, Layer, IsDynamic);
        }
    }

    public class SceneUnitDesc
    {
        public UnitDesc UnitDesc;
        public int SceneIndex;

        public SceneUnitDesc(UnitDesc unitDesc, int sceneIndex)
        {
            UnitDesc = unitDesc;
            SceneIndex = sceneIndex;
        }
        
        public bool Equals(SceneUnitDesc other)
        {
            return other.UnitDesc.Guid == UnitDesc.Guid && other.SceneIndex == SceneIndex;
        }
    }
}