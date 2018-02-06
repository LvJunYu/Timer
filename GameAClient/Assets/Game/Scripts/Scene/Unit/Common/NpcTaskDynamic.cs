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
            DefineField<ushort>(FieldTag.NpcTaskSerialNumber, "NpcTaskSerialNumber");
            DefineField<ushort>(FieldTag.TriggerTaskNumber, "TriggerTaskNumber");
            DefineField<NpcTaskTargetDynamic>(FieldTag.TriggerTask, "TriggerTask");
            DefineField<byte>(FieldTag.TriggerType, "TriggerType");
            DefineFieldList<string>(FieldTag.TaskBefore, "TaskBefore");
            DefineFieldList<string>(FieldTag.TaskMiddle, "TaskMiddle");
            DefineFieldList<string>(FieldTag.TaskAfter, "TaskAfter");
            DefineFieldList<NpcTaskTargetDynamic>(FieldTag.Targets, "Targets");
            DefineFieldList<NpcTaskTargetDynamic>(FieldTag.TaskFinishAward, "TaskFinishAward");
            DefineFieldList<NpcTaskTargetDynamic>(FieldTag.BeforeTaskAward, "BeforeTaskAward");
            DefineField<ushort>(FieldTag.TaskimeLimit, "TaskimeLimit");
            DefineField<ushort>(FieldTag.TargetNpcSerialNumber, "TargetNpcSerialNumber");
        }

        public class FieldTag
        {
            private static int _nextId;
            public static readonly int NpcTaskSerialNumber = _nextId++;
            public static readonly int TriggerTaskNumber = _nextId++;
            public static readonly int TriggerTask = _nextId++;
            public static readonly int TriggerType = _nextId++;
            public static readonly int TaskBefore = _nextId++;
            public static readonly int TaskMiddle = _nextId++;
            public static readonly int TaskAfter = _nextId++;
            public static readonly int Targets = _nextId++;
            public static readonly int TaskFinishAward = _nextId++;
            public static readonly int BeforeTaskAward = _nextId++;
            public static readonly int TaskimeLimit = _nextId++;
            public static readonly int TargetNpcSerialNumber = _nextId++;
        }

        public ushort NpcTaskSerialNumber
        {
            get { return Get<ushort>(FieldTag.NpcTaskSerialNumber); }

            set { Set(value, FieldTag.NpcTaskSerialNumber); }
        }

        public NpcTaskTargetDynamic TriggerTask
        {
            get { return Get<NpcTaskTargetDynamic>(FieldTag.TriggerTask); }

            set { Set(value, FieldTag.TriggerTask); }
        }

        public ushort TriggerTaskNumber
        {
            get { return Get<ushort>(FieldTag.TriggerTaskNumber); }

            set { Set(value, FieldTag.TriggerTaskNumber); }
        }


        public byte TriggerType
        {
            get { return Get<byte>(FieldTag.TriggerType); }

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

        public DictionaryListObject Targets
        {
            get { return Get<DictionaryListObject>(FieldTag.Targets); }

            set { Set(value, FieldTag.Targets); }
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

        public ushort TaskimeLimit
        {
            get { return Get<ushort>(FieldTag.TaskimeLimit); }

            set { Set(value, FieldTag.TaskimeLimit); }
        }

        public ushort TargetNpcSerialNumber
        {
            get { return Get<ushort>(FieldTag.TargetNpcSerialNumber); }

            set { Set(value, FieldTag.TargetNpcSerialNumber); }
        }


        public UnitExtraNpcTaskData ToUnitExtraNpcTaskData()
        {
            var msg = new UnitExtraNpcTaskData();
            msg.NpcTaskSerialNumber = NpcTaskSerialNumber;
            msg.TriggerTaskNumber = TriggerTaskNumber;
            msg.TriggerTask = TriggerTask.ToUnitExtraNpcTaskTarget();
            msg.TriggerType = TriggerType;
            msg.TaskBefore.AddRange(TaskBefore.ToList<string>());
            msg.TaskMiddle.AddRange(TaskMiddle.ToList<string>());
            msg.TaskAfter.AddRange(TaskAfter.ToList<string>());
            msg.TargetNpcSerialNumber = TargetNpcSerialNumber;
            //清一下列表
            msg.TaskTarget.Clear();
            for (int i = 0, count = Targets.Count; i < count; i++)
            {
                UnitExtraNpcTaskTarget val = Targets.Get<NpcTaskTargetDynamic>(i).ToUnitExtraNpcTaskTarget();
                if (val != null)
                {
                    msg.TaskTarget.Add(val);
                }
            }
            msg.BeforeTaskAward.Clear();
            for (int i = 0, count = BeforeTaskAward.Count; i < count; i++)
            {
                UnitExtraNpcTaskTarget val = BeforeTaskAward.Get<NpcTaskTargetDynamic>(i).ToUnitExtraNpcTaskTarget();
                if (val != null)
                {
                    msg.BeforeTaskAward.Add(val);
                }
            }
            msg.TaskFinishAward.Clear();
            for (int i = 0, count = TaskFinishAward.Count; i < count; i++)
            {
                UnitExtraNpcTaskTarget val = TaskFinishAward.Get<NpcTaskTargetDynamic>(i).ToUnitExtraNpcTaskTarget();
                if (val != null)
                {
                    msg.TaskFinishAward.Add(val);
                }
            }
            msg.TaskimeLimit = TaskimeLimit;
            return msg;
        }

        public void Set(UnitExtraNpcTaskData data)
        {
            NpcTaskSerialNumber = (ushort) data.NpcTaskSerialNumber;
            TriggerTaskNumber = (ushort) data.TriggerTaskNumber;
            TriggerTask.Set(data.TriggerTask);
            TriggerType = (byte) data.TriggerType;
            TargetNpcSerialNumber = (ushort) data.TargetNpcSerialNumber;
            for (int i = 0; i < data.TaskBefore.Count; i++)
            {
                Set(data.TaskBefore[i], FieldTag.TaskBefore, i);
            }
            for (int i = 0; i < data.TaskMiddle.Count; i++)
            {
                Set(data.TaskMiddle[i], FieldTag.TaskMiddle, i);
            }
            for (int i = 0; i < data.TaskAfter.Count; i++)
            {
                Set(data.TaskAfter[i], FieldTag.TaskAfter, i);
            }
            for (int i = 0; i < data.TaskTarget.Count; i++)
            {
                var taskTarget = new NpcTaskTargetDynamic();
                taskTarget.Set(data.TaskTarget[i]);
                Set(taskTarget, FieldTag.Targets, i);
            }
            for (int i = 0; i < data.BeforeTaskAward.Count; i++)
            {
                var taskTarget = new NpcTaskTargetDynamic();
                taskTarget.Set(data.TaskTarget[i]);
                Set(taskTarget, FieldTag.BeforeTaskAward, i);
            }
            for (int i = 0; i < data.TaskFinishAward.Count; i++)
            {
                var taskTarget = new NpcTaskTargetDynamic();
                taskTarget.Set(data.TaskTarget[i]);
                Set(taskTarget, FieldTag.TaskFinishAward, i);
            }
            TaskimeLimit = (ushort) data.TaskimeLimit;
        }

        public const int MaxBeforeTasAwardCout = 2;
        public const int MaxFinishTasAwardCout = 3;
    }

    public enum TrrigerTaskType
    {
        None = 0,
        Kill = 1,
        Colltion = 2,
        FinishOtherTask = 3
    }
}