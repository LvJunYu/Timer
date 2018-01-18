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
    public class NpcTaskDynamic : DictionaryObject<NpcTaskDynamic>
    {
        static NpcTaskDynamic()
        {
            DefineField<ushort>(FieldTag.NpcSerialNumber, "NpcSerialNumber");
            DefineField<ushort>(FieldTag.TriggerTaskNumber, "TriggerTaskNumber");
            DefineField<ushort>(FieldTag.TriggerColOrKillNum, "TriggerColOrKillNum");
            DefineField<ushort>(FieldTag.TargetUnitID, "TargetUnitID");
            DefineField<byte>(FieldTag.TriggerType, "TriggerType");
            DefineFieldList<string>(FieldTag.TaskBefore, "TaskBefore");
            DefineFieldList<string>(FieldTag.TaskMiddle, "TaskMiddle");
            DefineFieldList<string>(FieldTag.TaskAfter, "TaskAfter");
            DefineFieldList<NpcTaskTargetDynamic>(FieldTag.Targets, "Targets");
            DefineFieldList<NpcTaskTargetDynamic>(FieldTag.TaskFinishAward, "TaskFinishAward");
            DefineFieldList<NpcTaskTargetDynamic>(FieldTag.BeforeTaskAward, "BeforeTaskAward");
        }

        public class FieldTag
        {
            private static int _nextId;
            public static readonly int NpcSerialNumber = _nextId++;
            public static readonly int TriggerTaskNumber = _nextId++;
            public static readonly int TriggerColOrKillNum = _nextId++;
            public static readonly int TargetUnitID = _nextId++;
            public static readonly int TriggerType = _nextId++;
            public static readonly int TaskBefore = _nextId++;
            public static readonly int TaskMiddle = _nextId++;
            public static readonly int TaskAfter = _nextId++;
            public static readonly int Targets = _nextId++;
            public static readonly int TaskFinishAward = _nextId++;
            public static readonly int BeforeTaskAward = _nextId++;
        }

        public ushort NpcSerialNumber
        {
            get { return Get<ushort>(FieldTag.NpcSerialNumber); }

            set { Set(value, FieldTag.NpcSerialNumber); }
        }

        public ushort TriggerTaskNumber
        {
            get { return Get<ushort>(FieldTag.TriggerTaskNumber); }

            set { Set(value, FieldTag.NpcSerialNumber); }
        }

        public ushort TriggerColOrKillNum
        {
            get { return Get<ushort>(FieldTag.TriggerColOrKillNum); }

            set { Set(value, FieldTag.TriggerColOrKillNum); }
        }

        public ushort TargetUnitID
        {
            get { return Get<ushort>(FieldTag.TargetUnitID); }

            set { Set(value, FieldTag.TargetUnitID); }
        }

        public ushort TriggerType
        {
            get { return Get<ushort>(FieldTag.TriggerType); }

            set { Set(value, FieldTag.TriggerType); }
        }

        public DictionaryListObject TaskBefore
        {
            get { return Get<DictionaryListObject>(FieldTag.TaskBefore); }

            set { Set(value, FieldTag.TaskBefore); }
        }

        public DictionaryListObject TaskMiddle
        {
            get { return Get<DictionaryListObject>(FieldTag.TaskMiddle); }

            set { Set(value, FieldTag.TaskMiddle); }
        }

        public DictionaryListObject TaskAfter
        {
            get { return Get<DictionaryListObject>(FieldTag.TaskAfter); }

            set { Set(value, FieldTag.TaskAfter); }
        }

        public DictionaryListObject TaskFinishAward
        {
            get { return Get<DictionaryListObject>(FieldTag.TaskFinishAward); }

            set { Set(value, FieldTag.TaskFinishAward); }
        }

        public DictionaryListObject BeforeTaskAward
        {
            get { return Get<DictionaryListObject>(FieldTag.BeforeTaskAward); }

            set { Set(value, FieldTag.BeforeTaskAward); }
        }


        public UnitExtraNpcTaskData ToUnitExtraNpcTaskData()
        {
            var msg = new UnitExtraNpcTaskData();
//            msg.NpcSerialNumber = NpcSerialNumber;
//            msg.TaskBefore.AddRange(TaskBefore.ToList());
//            msg.TaskMiddle.AddRange(TaskMiddle.ToList());
//            msg.TaskAfter.AddRange(TaskAfter.ToList());
//            msg.TaskTarget.AddRange(Targets.ToProtoDataList());
            return msg;
        }

        public void Set(UnitExtraNpcTaskData data)
        {
//            NpcSerialNumber = (ushort) data.NpcSerialNumber;
//            TaskBefore.Set(data.TaskBefore.ToArray());
//            TaskMiddle.Set(data.TaskMiddle.ToArray());
//            TaskAfter.Set(data.TaskAfter.ToArray());
//            Targets.SetProtoData(data.TaskTarget.ToArray());
        }
    }
}