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

        public UnitExtraNpcTaskData TaskData { get; set; }
        public UnitExtraNpcTaskTarget TaskTargetData { get; set; }
        public bool IsEditNpcData { get; set; }

        public bool IsEditNpcBerOrAfter()
        {
            return IsEditNpcData && (TaskType == ETaskContype.BeforeTask || TaskType == ETaskContype.AfterTask);
        }

        public bool IsEditNpcTarget()
        {
            return IsEditNpcData && TaskType == ETaskContype.Task;
        }

        public void FinishAddTarget(IntVec3 Pos)
        {
            TaskTargetData.TargetGuid =
                GM2DTools.ToProto(Pos);
            switch (TaskType)
            {
                case ETaskContype.AfterTask:
                    TaskData.TaskFinishAward.Add(TaskTargetData);
                    break;
                case ETaskContype.BeforeTask:
                    TaskData.BeforeTaskAward.Add(TaskTargetData);
                    break;
                case ETaskContype.Task:
                    TaskData.TaskTarget.Add(TaskTargetData);
                    break;
            }
        }

        public void OnUnitMoveUpdateSwitchData(UnitDesc oldUnitDesc)
        {
            IntVec3Proto pos = GM2DTools.ToProto(oldUnitDesc.Guid);
            using (var itor = ColliderScene2D.CurScene.Units.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    if (UnitDefine.IsNpc(itor.Current.Value.Id))
                    {
                        List<UnitExtraNpcTaskData> taskData =
                            itor.Current.Value.GetUnitExtra().NpcTask.ToListData();
                        for (int i = 0; i < taskData.Count; i++)
                        {
                            for (int j = 0; j < taskData[i].BeforeTaskAward.Count; j++)
                            {
                                if (taskData[i].BeforeTaskAward[i].TargetGuid == pos)
                                {
                                }
                            }
                            for (int j = 0; j < taskData[i].TaskFinishAward.Count; j++)
                            {
                            }
                            for (int j = 0; j < taskData[i].TaskTarget.Count; j++)
                            {
                            }
                        }
                    }
                }
            }
        }

        public void OnUnitDeleteUpdateSwitchData(UnitDesc unitDesc)
        {
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