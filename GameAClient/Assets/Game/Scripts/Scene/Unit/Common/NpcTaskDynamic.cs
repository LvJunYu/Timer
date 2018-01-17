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
            DefineFieldList<string>(FieldTag.TaskBefore, "TaskBefore");
            DefineFieldList<string>(FieldTag.TaskMiddle, "TaskMiddle");
            DefineFieldList<string>(FieldTag.TaskAfter, "TaskAfter");
            DefineFieldList<NpcTaskTargetDynamic>(FieldTag.Targets, "Targets");
        }
        public class FieldTag
        {
            private static int _nextId;
            public static readonly int NpcSerialNumber = _nextId++;
            public static readonly int TaskBefore = _nextId++;
            public static readonly int TaskMiddle = _nextId++;
            public static readonly int TaskAfter = _nextId++;
            public static readonly int Targets = _nextId++;
        }
        public ushort NpcSerialNumber
        {
            get { return Get<ushort>(FieldTag.NpcSerialNumber); }
            set { Set(value, FieldTag.NpcSerialNumber);}
        }
        public DictionaryListObject TaskBefore
        {
            get { return Get<DictionaryListObject>(FieldTag.TaskBefore); }
            set { Set(value, FieldTag.TaskBefore);}
        }
        public DictionaryListObject TaskMiddle
        {
            get { return Get<DictionaryListObject>(FieldTag.TaskMiddle); }
            set { Set(value, FieldTag.TaskMiddle);}
        }
        public DictionaryListObject TaskAfter
        {
            get { return Get<DictionaryListObject>(FieldTag.TaskAfter); }
            set { Set(value, FieldTag.TaskAfter);}
        }
        public DictionaryListObject Targets
        {
            get { return Get<DictionaryListObject>(FieldTag.Targets); }
            set { Set(value, FieldTag.Targets);}
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