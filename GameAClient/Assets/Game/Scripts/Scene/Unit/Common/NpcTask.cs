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
    public struct NpcTask : IEquatable<NpcTask>
    {
        public ushort NpcSerialNumber;
        public ushort TriggerTaskNumber;
        public ushort TriggerColOrKillNum;
        public ushort TargetUnitID;
        public byte TriggerType;
        public MultiParamDia TaskBefore;
        public MultiParamDia TaskMiddle;
        public MultiParamDia TaskAfter;
        public MultiParamTarget Targets;
        public MultiParamTarget TaskFinishAward;
        public MultiParamTarget BeforeTaskAward;

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
            msg.NpcSerialNumber = NpcSerialNumber;
            msg.TaskBefore.AddRange(TaskBefore.ToList());
            msg.TaskMiddle.AddRange(TaskMiddle.ToList());
            msg.TaskAfter.AddRange(TaskAfter.ToList());
            msg.TaskTarget.AddRange(Targets.ToProtoDataList());
            msg.TargetUnitID = TargetUnitID;
            msg.TriggerType = TriggerType;
            msg.TriggerColOrKillNum = TriggerColOrKillNum;
            msg.TriggerTaskNumber = TriggerTaskNumber;
            msg.BeforeTaskAward.AddRange(BeforeTaskAward.ToProtoDataList());
            msg.TaskFinishAward.AddRange(TaskFinishAward.ToProtoDataList());
            return msg;
        }

        public void Set(UnitExtraNpcTaskData data)
        {
            NpcSerialNumber = (ushort) data.NpcSerialNumber;
            TaskBefore.Set(data.TaskBefore.ToArray());
            TaskMiddle.Set(data.TaskMiddle.ToArray());
            TaskAfter.Set(data.TaskAfter.ToArray());
            Targets.SetProtoData(data.TaskTarget.ToArray());
            TriggerTaskNumber = (ushort) data.TriggerTaskNumber;
            TriggerType = (byte) data.TriggerType;
            TargetUnitID = (ushort) data.TargetUnitID;
            TriggerColOrKillNum = (ushort) data.TriggerColOrKillNum;
            BeforeTaskAward.SetProtoData(data.BeforeTaskAward.ToArray());
            TaskFinishAward.SetProtoData(data.TaskFinishAward.ToArray());
        }

        public bool Equals(NpcTask other)
        {
            return
                NpcSerialNumber == other.NpcSerialNumber &&
                TaskBefore == other.TaskBefore &&
                TaskMiddle == other.TaskMiddle &&
                TaskAfter == other.TaskAfter &&
                TriggerType == other.TriggerType &&
                TargetUnitID == other.TargetUnitID &&
                TriggerTaskNumber == other.TriggerTaskNumber &&
                TriggerColOrKillNum == other.TriggerColOrKillNum &&
                TaskFinishAward == other.TaskFinishAward &&
                BeforeTaskAward == other.BeforeTaskAward;
        }
    }

    public struct MultiParamDia : IEquatable<MultiParamDia>
    {
        public bool HasContent;
        public string Param0;
        public string Param1;
        public string Param2;
        public string Param3;
        public string Param4;

        public bool Equals(MultiParamDia other)
        {
            return Param0 == other.Param0 &&
                   Param1 == other.Param1 &&
                   Param2 == other.Param2 &&
                   Param3 == other.Param3 &&
                   Param4 == other.Param4;
        }

        public static bool operator ==(MultiParamDia a, MultiParamDia other)
        {
            return a.Equals(other);
        }

        public static bool operator !=(MultiParamDia a, MultiParamDia other)
        {
            return !(a == other);
        }

        public void Set(string[] list)
        {
            if (list == null || list.Length == 0) return;
            HasContent = true;
            for (int i = 0; i < list.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        Param0 = (string) list[0];
                        break;
                    case 1:
                        Param1 = (string) list[1];
                        break;
                    case 2:
                        Param2 = (string) list[2];
                        break;
                    case 3:
                        Param3 = (string) list[3];
                        break;
                    case 4:
                        Param4 = (string) list[4];
                        break;
                    default:
                        LogHelper.Error("list's length is bigger than param count");
                        break;
                }
            }
        }

        public List<string> ToList()
        {
            List<string> list = new List<string>();
            if (!HasContent) return list;
            if (Param0 != null)
            {
                list.Add(Param0);
            }
            if (Param1 != null)
            {
                list.Add(Param1);
            }
            if (Param2 != null)
            {
                list.Add(Param2);
            }
            if (Param3 != null)
            {
                list.Add(Param3);
            }
            if (Param4 != null)
            {
                list.Add(Param4);
            }
            return list;
        }
    }

    public struct MultiParamTarget : IEquatable<MultiParamTarget>
    {
        public bool HasContent;
        public bool HasContentProtoData;
        private const int _sum = 3;
        public NpcTaskTarget Param0;
        public NpcTaskTarget Param1;
        public NpcTaskTarget Param2;

        public bool Equals(MultiParamTarget other)
        {
            return Param0 == other.Param0 &&
                   Param1 == other.Param1 &&
                   Param2 == other.Param2;
        }

        public static bool operator ==(MultiParamTarget a, MultiParamTarget other)
        {
            return a.Equals(other);
        }

        public static bool operator !=(MultiParamTarget a, MultiParamTarget other)
        {
            return !(a == other);
        }

        public void Set(NpcTaskTarget[] list)
        {
            if (list == null || list.Length == 0) return;
            HasContent = true;
            for (int i = 0; i < _sum; i++)
            {
                switch (i)
                {
                    case 0:
                        if (i < list.Length)
                        {
                            Param0 = list[0];
                        }
                        else
                        {
                            Param0 = new NpcTaskTarget();
                        }
                        break;
                    case 1:
                        if (i < list.Length)
                        {
                            Param0 = list[1];
                        }
                        else
                        {
                            Param1 = new NpcTaskTarget();
                        }
                        break;
                    case 2:
                        if (i < list.Length)
                        {
                            Param2 = list[2];
                        }
                        else
                        {
                            Param2 = new NpcTaskTarget();
                        }
                        break;
                    default:
                        LogHelper.Error("list's length is bigger than param count");
                        break;
                }
            }
        }

        public List<NpcTaskTarget> ToList()
        {
            List<NpcTaskTarget> list = new List<NpcTaskTarget>();
            if (!HasContent) return list;
            if (Param0.TaskType != 0)
            {
                list.Add(Param0);
            }
            if (Param1.TaskType != 0)
            {
                list.Add(Param1);
            }
            if (Param2.TaskType != 0)
            {
                list.Add(Param2);
            }
            return list;
        }

        public void SetProtoData(UnitExtraNpcTaskTarget[] list)
        {
            if (list == null || list.Length == 0) return;
            HasContentProtoData = true;
            for (int i = 0; i < _sum; i++)
            {
                switch (i)
                {
                    case 0:
                        if (i < list.Length)
                        {
                            Param0.Set(list[0]);
                        }
                        else
                        {
                            Param0 = new NpcTaskTarget();
                        }
                        break;
                    case 1:
                        if (i < list.Length)
                        {
                            Param1.Set(list[1]);
                        }
                        else
                        {
                            Param1 = new NpcTaskTarget();
                        }
                        break;
                    case 2:
                        if (i < list.Length)
                        {
                            Param2.Set(list[2]);
                        }
                        else
                        {
                            Param2 = new NpcTaskTarget();
                        }
                        break;
                    default:
                        LogHelper.Error("list's length is bigger than param count");
                        break;
                }
            }
        }

        public List<UnitExtraNpcTaskTarget> ToProtoDataList()
        {
            List<UnitExtraNpcTaskTarget> list = new List<UnitExtraNpcTaskTarget>();
            if (!HasContentProtoData) return list;
            if (Param0.TaskType != 0)
            {
                list.Add(Param0.ToUnitExtraNpcTaskTarget());
            }
            if (Param1.TaskType != 0)
            {
                list.Add(Param1.ToUnitExtraNpcTaskTarget());
            }
            if (Param2.TaskType != 0)
            {
                list.Add(Param2.ToUnitExtraNpcTaskTarget());
            }
            return list;
        }
    }

    public enum TrrigerTaskType
    {
        None = 0,
        KillOrGet = 1,
        FinishOtherTask = 2
    }
}