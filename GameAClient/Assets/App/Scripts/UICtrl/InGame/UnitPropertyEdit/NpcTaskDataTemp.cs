using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class NpcTaskDataTemp
    {
        public const int MaxNpcTargetSerialNum = 99;
        public const int NoneNumMark = -1;
        public const int MaxNpcSerialNum = 99;
        private static NpcTaskDataTemp _intance;

        public static NpcTaskDataTemp Intance
        {
            get { return _intance ?? (_intance = new NpcTaskDataTemp()); }
        }

        private NpcTaskDataTemp()
        {
            _npcTaskSerialNumberDic = new Dictionary<int, bool>();
            for (int i = 0; i < MaxNpcTargetSerialNum; i++)
            {
                _npcTaskSerialNumberDic.Add(i + 1, false);
            }
            _npcSerialNumberDic = new Dictionary<int, bool>();
            for (int i = 0; i < MaxNpcSerialNum; i++)
            {
                _npcSerialNumberDic.Add(i + 1, false);
            }
        }

        private Dictionary<int, bool> _npcTaskSerialNumberDic;

        public Dictionary<int, bool> NpcTaskSerialNumberDic1
        {
            get { return _npcTaskSerialNumberDic; }
            set { _npcTaskSerialNumberDic = value; }
        }

        private Dictionary<int, bool> _npcSerialNumberDic;

        public Dictionary<int, bool> NpcSerialNumberDic
        {
            get { return _npcSerialNumberDic; }
            set { _npcSerialNumberDic = value; }
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

        public int GetNpcTaskSerialNum()
        {
            int SerialNUm = NoneNumMark;
            for (int i = 0; i < MaxNpcTargetSerialNum; i++)
            {
                if (!_npcTaskSerialNumberDic[i + 1])
                {
                    SerialNUm = i + 1;
                    break;
                }
            }
            return SerialNUm;
        }

        public void RecycleNpcTaskSerialNum(int num)
        {
            _npcTaskSerialNumberDic[num] = false;
        }

        public bool SetNpcTaskSerialNum(int num)
        {
            bool sucess;
            if (_npcTaskSerialNumberDic.ContainsKey(num))
            {
                _npcTaskSerialNumberDic[num] = true;
                sucess = true;
            }
            else
            {
                sucess = false;
            }
            return sucess;
        }

        public bool SetSecenTaskSerialNum(int num)
        {
            bool sucess;
            if (_npcTaskSerialNumberDic.ContainsKey(num))
            {
                _npcTaskSerialNumberDic[num] = true;
                sucess = true;
            }
            else
            {
                sucess = false;
            }
            return sucess;
        }

        public bool SetSecenAllTaskSerialNum(List<int> numList)
        {
            for (int i = 0; i < MaxNpcTargetSerialNum; i++)
            {
                _npcTaskSerialNumberDic[i + 1] = false;
            }
            bool sucess = true;
            for (int i = 0; i < numList.Count; i++)
            {
                if (_npcTaskSerialNumberDic.ContainsKey(numList[i]))
                {
                    _npcTaskSerialNumberDic[numList[i]] = false;
                }
                else
                {
                    sucess = false;
                    break;
                }
            }

            return sucess;
        }

        public int GetNpcSerialNum()
        {
            int SerialNUm = NoneNumMark;
            for (int i = 0; i < MaxNpcTargetSerialNum; i++)
            {
                if (!_npcSerialNumberDic[i + 1])
                {
                    SerialNUm = i + 1;
                    break;
                }
            }
            return SerialNUm;
        }

        public bool SetNpcSerialNum(int num)
        {
            bool sucess;
            if (_npcSerialNumberDic.ContainsKey(num))
            {
                _npcSerialNumberDic[num] = true;
                sucess = true;
            }
            else
            {
                sucess = false;
            }
            return sucess;
        }

        public bool SetSecenSerialNum(int num)
        {
            bool sucess;
            if (_npcSerialNumberDic.ContainsKey(num))
            {
                _npcSerialNumberDic[num] = true;
                sucess = true;
            }
            else
            {
                sucess = false;
            }
            return sucess;
        }

        public bool SetSecenAllSerialNum(List<int> numList)
        {
            for (int i = 0; i < MaxNpcTargetSerialNum; i++)
            {
                _npcSerialNumberDic[i + 1] = false;
            }
            bool sucess = true;
            for (int i = 0; i < numList.Count; i++)
            {
                if (_npcSerialNumberDic.ContainsKey(numList[i]))
                {
                    _npcSerialNumberDic[numList[i]] = false;
                }
                else
                {
                    sucess = false;
                    break;
                }
            }

            return sucess;
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