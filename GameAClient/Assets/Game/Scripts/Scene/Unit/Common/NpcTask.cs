/********************************************************************
** Filename : NpcTask
** Author : Zhp
** Date : 2018/1/12 星期五 下午 17:49:06
** Summary : NpcTask
***********************************************************************/

using System;
using System.Runtime.InteropServices;
using SoyEngine.Proto;

#pragma warning disable 0660 0661
namespace GameA.Game
{
    /// <summary>
    /// 这个类不要加引用类型，方便赋值
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct NpcTask : IEquatable<NpcTask>
    {
        public byte NpcType;
        public string NpcName;
        public string NpcDialog;
        public byte NpcShowType;

        public bool Equals(UnitExtra other)
        {
            return
                NpcName == other.NpcName &&
                NpcType == other.NpcType &&
                NpcShowType == other.NpcShowType;
        }

        public static bool operator ==(NpcTask a, NpcTask other)
        {
            return a.Equals(other);
        }

        public static bool operator !=(NpcTask a, NpcTask other)
        {
            return !(a == other);
        }


        public UnitExtraNpcTaskData ToUnitExtraNpcTaskData()
        {
            var msg = new UnitExtraNpcTaskData();

            return msg;
        }

        public void Set(UnitExtraNpcTaskData data)
        {
        }

        public bool Equals(NpcTask other)
        {
            return
                NpcDialog == other.NpcDialog &&
                NpcName == other.NpcName &&
                NpcType == other.NpcType &&
                NpcShowType == other.NpcShowType;
        }
    }
}