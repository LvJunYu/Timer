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

namespace GameA.Game
{
    public class NpcTaskTargetDynamic : DictionaryObject<NpcTaskTargetDynamic>
    {
        static NpcTaskTargetDynamic()
        {
            DefineField<byte>(FieldTag.TaskType, "TaskType");
            DefineField<ushort>(FieldTag.ColOrKillNum, "ColOrKillNum");
            DefineField<ushort>(FieldTag.TargetUnitID, "TriggerUnitID");
            DefineField<IntVec3>(FieldTag.TargetGuid, "TargetGuid");
            DefineField<ushort>(FieldTag.TargetNpcNum, "TargetNpcNum");
        }

        public class FieldTag
        {
            private static int _nextId;
            public static readonly int TaskType = _nextId++;
            public static readonly int ColOrKillNum = _nextId++;
            public static readonly int TargetUnitID = _nextId++;
            public static readonly int TargetGuid = _nextId++;
            public static readonly int TargetNpcNum = _nextId++;
        }


        public byte TaskType
        {
            get { return Get<byte>(FieldTag.TaskType); }
            set { Set(value, FieldTag.TaskType); }
        }

        public ushort ColOrKillNum
        {
            get { return Get<ushort>(FieldTag.ColOrKillNum); }
            set { Set(value, FieldTag.ColOrKillNum); }
        }

        public ushort TargetUnitID
        {
            get { return Get<ushort>(FieldTag.TargetUnitID); }
            set { Set(value, FieldTag.TargetUnitID); }
        }

        public ushort TargetNpcNum
        {
            get { return Get<ushort>(FieldTag.TargetNpcNum); }
            set { Set(value, FieldTag.TargetNpcNum); }
        }

        public IntVec3 TargetGuid
        {
            get { return Get<IntVec3>(FieldTag.TargetGuid); }
            set { Set(value, FieldTag.TargetGuid); }
        }

        public UnitExtraNpcTaskTarget ToUnitExtraNpcTaskTarget()
        {
            var msg = new UnitExtraNpcTaskTarget();
            msg.TargetGuid = GM2DTools.ToProto(TargetGuid);
            msg.TargetUnitID = TargetUnitID;
            msg.ColOrKillNum = ColOrKillNum;
            msg.TaskType = TaskType;
            msg.TargetNpcNum = TargetNpcNum;
            return msg;
        }

        public void Set(UnitExtraNpcTaskTarget data)
        {
            TaskType = (byte) data.TaskType;
            ColOrKillNum = (ushort) data.ColOrKillNum;
            TargetUnitID = (byte) data.TargetUnitID;
            TargetGuid = GM2DTools.ToEngine(data.TargetGuid);
            TargetNpcNum = (ushort) data.TargetNpcNum;
        }

        public bool Equals(NpcTaskTargetDynamic other)
        {
            return
                TaskType == other.TaskType &&
                ColOrKillNum == other.ColOrKillNum &&
                TargetUnitID == other.TargetUnitID &&
                TargetGuid == other.TargetGuid &&
                TargetNpcNum == other.TargetNpcNum;
        }

       
    }

    public enum ENpcTargetType
    {
        None = 0,
        Moster,
        Colltion,
        Dialog,
        Contorl
    }
}