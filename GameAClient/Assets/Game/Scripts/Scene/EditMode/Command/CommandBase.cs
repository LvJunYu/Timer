/********************************************************************
** Filename : CommandBase
** Author : Dong
** Date : 2015/7/8 星期三 下午 5:32:19
** Summary : CommandBase
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class CommandBase
    {
        protected List<UnitEditData> _buffers = new List<UnitEditData>();
        protected bool _pushFlag;

        protected bool CheckCanAddChild(Table_Unit child, UnitDesc parent)
        {
            if (child == null || parent == UnitDesc.zero)
            {
                return false;
            }
            Table_Unit tableParent = UnitManager.Instance.GetTableUnit(parent.Id);
            if (tableParent == null)
            {
                return false;
            }
            // check if parent and child in same group
            if ((child.ChildType & tableParent.ParentType) == 0)
            {
                return false;
            }
            return true;
        }

        protected bool CheckCanBindMagic(Table_Unit child, UnitDesc parent)
        {
            if (child == null || parent == UnitDesc.zero)
            {
                return false;
            }
            Table_Unit tableParent = UnitManager.Instance.GetTableUnit(parent.Id);
            if (tableParent == null)
            {
                return false;
            }
            if (child.Id == ConstDefineGM2D.BlueStoneId && tableParent.OriginMagicDirection!=0)
            {
                return true;
            }
            return false;
        }

        protected bool CheckMask(byte rotation,int mask)
        {
            return (mask & (byte)(1 << rotation)) != 0;
        }
    }

    public struct UnitEditData : IEquatable<UnitEditData>
    {
        public UnitDesc UnitDesc;
        public UnitExtra UnitExtra;

        public UnitEditData(UnitDesc unitDesc, UnitExtra unitExtra)
        {
            UnitDesc = unitDesc;
            UnitExtra = unitExtra;
        }
        public static bool operator ==(UnitEditData a, UnitEditData other)
        {
            return (a.UnitDesc == other.UnitDesc) && (a.UnitExtra == other.UnitExtra);
        }

        public static bool operator !=(UnitEditData a, UnitEditData other)
        {
            return !(a == other);
        }

        public bool Equals (UnitEditData other)
        {
            return (UnitDesc == other.UnitDesc) && (UnitExtra == other.UnitExtra);
        }
    }
}