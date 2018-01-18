/********************************************************************
** Filename : NpcTask
** Author : Zhp
** Date : 2018/1/12 星期五 下午 17:49:06
** Summary : NpcTask
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SoyEngine;
using SoyEngine.Proto;
using Win32;

#pragma warning disable 0660 0661
namespace GameA.Game
{
    /// <summary>
    /// 这个类不要加引用类型，方便赋值
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct NpcTaskTarget : IEquatable<NpcTaskTarget>
    {
        public byte TaskType;
        public ushort ColOrKillNum;
        public ushort TargetUnitID;
        public IntVec3 TargetGuid;


        public static bool operator ==(NpcTaskTarget a, NpcTaskTarget other)
        {
            return a.Equals(other);
        }

        public static bool operator !=(NpcTaskTarget a, NpcTaskTarget other)
        {
            return !(a == other);
        }


        public UnitExtraNpcTaskTarget ToUnitExtraNpcTaskTarget()
        {
            var msg = new UnitExtraNpcTaskTarget();
            msg.TargetGuid = GM2DTools.ToProto(TargetGuid);
            msg.TargetUnitID = TargetUnitID;
            msg.ColOrKillNum = ColOrKillNum;
            msg.TaskType = TaskType;
            return msg;
        }

        public void Set(UnitExtraNpcTaskTarget data)
        {
            TaskType = (byte) data.TaskType;
            ColOrKillNum = (ushort) data.ColOrKillNum;
            TargetUnitID = (byte) data.TargetUnitID;
            TargetGuid = GM2DTools.ToEngine(data.TargetGuid);
        }

        public bool Equals(NpcTaskTarget other)
        {
            return
                TaskType == other.TaskType &&
                ColOrKillNum == other.ColOrKillNum &&
                TargetUnitID == other.TargetUnitID &&
                TargetGuid == other.TargetGuid;
        }
    }

    public enum ENpcTaskType
    {
        None = 0,
        Colltion,
        Moster,
        Dialog,
        Contorl
    }
}