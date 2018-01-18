using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class NpcTaskDataTemp
    {
        private static NpcTaskDataTemp _intance;

        public static NpcTaskDataTemp Intance
        {
            get { return _intance ?? (_intance = new NpcTaskDataTemp()); }
        }

        private NpcTaskDataTemp()
        {
        }


        public IntVec3 NpcIntVec3 { get; set; }
        public ETaskContype TaskType { get; set; }

        public NpcTaskDynamic TaskData { get; set; }
        public NpcTaskTargetDynamic TaskTargetData { get; set; }
        public bool IsEditNpcData { get; set; }

        public bool IsEditNpcBerOrAfter()
        {
            return IsEditNpcData && (TaskType == ETaskContype.BeforeTask || TaskType == ETaskContype.AfterTask);
        }

        public bool IsEditNpcTarget()
        {
            return IsEditNpcData && TaskType == ETaskContype.Task;
        }

        public void FinishAddTarget(IntVec3 guid)
        {
            TaskTargetData.TargetGuid = guid;
            switch (TaskType)
            {
                case ETaskContype.AfterTask:
                    TaskData.TaskFinishAward.Add(TaskTargetData);
                    break;
                case ETaskContype.BeforeTask:
                    TaskData.BeforeTaskAward.Add(TaskTargetData);
                    break;
                case ETaskContype.Task:
                    TaskData.Targets.Add(TaskTargetData);
                    break;
            }
        }

        //开关改变是查找npc改变目标guid
        public void OnUnitMoveUpdateSwitchData(UnitDesc oldUnitDesc, UnitDesc newUnitDesc)
        {
            IntVec3 oldGuid = oldUnitDesc.Guid;
            IntVec3 newGuid = newUnitDesc.Guid;
            using (var itor = ColliderScene2D.CurScene.Units.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    if (UnitDefine.IsNpc(itor.Current.Value.Id))
                    {
                        List<NpcTaskDynamic> taskData =
                            itor.Current.Value.GetUnitExtra().NpcTask.ToList<NpcTaskDynamic>();
                        for (int i = 0; i < taskData.Count; i++)
                        {
                            var beforeTaskAward = taskData[i].BeforeTaskAward.ToList<NpcTaskTargetDynamic>();
                            ChangeGuid(oldGuid, newGuid, beforeTaskAward);
                            var TaskFinishAward = taskData[i].TaskFinishAward.ToList<NpcTaskTargetDynamic>();
                            ChangeGuid(oldGuid, newGuid, TaskFinishAward);
                            var TaskTarget = taskData[i].Targets.ToList<NpcTaskTargetDynamic>();
                            ChangeGuid(oldGuid, newGuid, TaskTarget);
                        }
                    }
                }
            }
        }

        public void ChangeGuid(IntVec3 oldGuid, IntVec3 newGuid, List<NpcTaskTargetDynamic> targetDynamics)
        {
            for (int i = 0; i < targetDynamics.Count; i++)
            {
                if (targetDynamics[i].TargetGuid == oldGuid)
                {
                    targetDynamics[i].TargetGuid = newGuid;
                }
            }
        }

        //去除npc关联的开关的方法
        public void OnUnitDelteUpdateSwitchData(UnitDesc unitDesc)
        {
            IntVec3 guid = unitDesc.Guid;
            using (var itor = ColliderScene2D.CurScene.Units.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    if (UnitDefine.IsNpc(itor.Current.Value.Id))
                    {
                        List<NpcTaskDynamic> taskData =
                            itor.Current.Value.GetUnitExtra().NpcTask.ToList<NpcTaskDynamic>();
                        for (int i = 0; i < taskData.Count; i++)
                        {
                            for (int j = 0; j < taskData[i].BeforeTaskAward.Count; j++)
                            {
                                if (taskData[i].BeforeTaskAward.ToList<NpcTaskTargetDynamic>()[j].TargetGuid == guid)
                                {
                                    taskData[i].BeforeTaskAward.Remove(j);
                                }
                            }
                            for (int j = 0; j < taskData[i].TaskFinishAward.Count; j++)
                            {
                                if (taskData[i].TaskFinishAward.ToList<NpcTaskTargetDynamic>()[j].TargetGuid == guid)
                                {
                                    taskData[i].TaskFinishAward.Remove(j);
                                }
                            }
                            for (int j = 0; j < taskData[i].Targets.Count; j++)
                            {
                                if (taskData[i].Targets.ToList<NpcTaskTargetDynamic>()[j].TargetGuid == guid)
                                {
                                    taskData[i].Targets.Remove(j);
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    public enum ETaskContype
    {
        None = 0,
        BeforeTask,
        AfterTask,
        Task
    }
}