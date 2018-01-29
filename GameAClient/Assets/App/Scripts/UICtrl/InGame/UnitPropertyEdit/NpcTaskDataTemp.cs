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
            Clear();
        }

        public void Clear()
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
        public UnitExtraDynamic CurExtraDynamic { get; set; }

        public bool IsEditNpcTarget(IntVec3 guid)
        {
            return IsEditNpcData && guid == NpcIntVec3;
        }

        public void StartEditTargetControl(NpcTaskDynamic task, IntVec3 guid, ETaskContype type, UnitExtraDynamic extra)
        {
            TaskData = task;
            TaskTargetData = new NpcTaskTargetDynamic();
            TaskTargetData.TaskType = (byte) ENpcTargetType.Contorl;
            IsEditNpcData = true;
            TaskType = type;
            NpcIntVec3 = guid;
            CurExtraDynamic = extra;
            EditMode.Instance.StartSwitch();
        }

        public void FinishAddTarget(IntVec3 guid)
        {
            UnitExtraDynamic unitextra;
            unitextra = DataScene2D.CurScene.GetUnitExtra(NpcIntVec3);
            TaskTargetData.TargetGuid = guid;
            switch (TaskType)
            {
                case ETaskContype.AfterTask:
                    TaskData.TaskFinishAward.Add(TaskTargetData);
                    unitextra.NpcTask = CurExtraDynamic.NpcTask;
                    break;
                case ETaskContype.BeforeTask:
                    TaskData.BeforeTaskAward.Add(TaskTargetData);
                    unitextra.NpcTask = CurExtraDynamic.NpcTask;
                    break;
                case ETaskContype.Task:
                    TaskData.Targets.Add(TaskTargetData);
                    unitextra.NpcTask = CurExtraDynamic.NpcTask;
                    break;
            }
//            EditMode.Instance.StopSwitch();
        }

        //开关改变是查找npc改变目标guid
        public void OnUnitMoveUpdateSwitchData(UnitDesc oldUnitDesc, UnitDesc newUnitDesc)
        {
            if (UnitDefine.IsNpc(oldUnitDesc.Id))
                return;
            IntVec3 oldGuid = oldUnitDesc.Guid;
            IntVec3 newGuid = newUnitDesc.Guid;
            using (var itor = ColliderScene2D.CurScene.Units.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    if (UnitDefine.IsNpc(itor.Current.Value.Id))
                    {
                        DictionaryListObject taskData =
                            itor.Current.Value.GetUnitExtra().NpcTask;
                        for (int i = 0; i < taskData.Count; i++)
                        {
                            var beforeTaskAward = taskData.Get<NpcTaskDynamic>(i).BeforeTaskAward;
                            ChangeGuid(oldGuid, newGuid, beforeTaskAward);
                            var TaskFinishAward = taskData.Get<NpcTaskDynamic>(i).Targets;
                            ChangeGuid(oldGuid, newGuid, TaskFinishAward);
                            var TaskTarget = taskData.Get<NpcTaskDynamic>(i).TaskFinishAward;
                            ChangeGuid(oldGuid, newGuid, TaskTarget);
                        }
                    }
                }
            }
        }

        public void ChangeGuid(IntVec3 oldGuid, IntVec3 newGuid, DictionaryListObject targetDynamics)
        {
            for (int i = 0; i < targetDynamics.Count; i++)
            {
                if (targetDynamics.Get<NpcTaskTargetDynamic>(i).TargetGuid == oldGuid)
                {
                    targetDynamics.Get<NpcTaskTargetDynamic>(i).TargetGuid = newGuid;
                }
            }
        }

        //去除npc关联的开关的方法
        public void OnUnitDelteUpdateSwitchData(UnitDesc unitDesc)
        {
            if (UnitDefine.IsNpc(unitDesc.Id))
                return;
            IntVec3 guid = unitDesc.Guid;
            using (var itor = ColliderScene2D.CurScene.Units.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    if (UnitDefine.IsNpc(itor.Current.Value.Id))
                    {
                        DictionaryListObject taskData =
                            itor.Current.Value.GetUnitExtra().NpcTask;
                        for (int i = 0; i < taskData.Count; i++)
                        {
                            NpcTaskDynamic task = taskData.Get<NpcTaskDynamic>(i);
                            for (int j = 0; j < task.BeforeTaskAward.Count; j++)
                            {
                                if (task.BeforeTaskAward.Get<NpcTaskTargetDynamic>(j).TargetGuid == guid)
                                {
                                    task.BeforeTaskAward.RemoveAt(j);
                                }
                            }
                            for (int j = 0; j < task.TaskFinishAward.Count; j++)
                            {
                                if (task.TaskFinishAward.Get<NpcTaskTargetDynamic>(j).TargetGuid == guid)
                                {
                                    task.TaskFinishAward.RemoveAt(j);
                                }
                            }
                            for (int j = 0; j < task.Targets.Count; j++)
                            {
                                if (task.Targets.Get<NpcTaskTargetDynamic>(j).TargetGuid == guid)
                                {
                                    task.Targets.RemoveAt(j);
                                    if (task.Targets.Count == 0)
                                    {
                                        RecycleNpcTaskSerialNum(task.NpcTaskSerialNumber);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public int GetNpcTaskSerialNum()
        {
            int serialNUm = NoneNumMark;
            for (int i = 0; i < MaxNpcTargetSerialNum; i++)
            {
                if (!_npcTaskSerialNumberDic[i + 1])
                {
                    serialNUm = i + 1;
                    break;
                }
            }
            return serialNUm;
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

        public int GetNpcSerialNum()
        {
            int serialNUm = NoneNumMark;
            for (int i = 0; i < MaxNpcTargetSerialNum; i++)
            {
                if (!_npcSerialNumberDic[i + 1])
                {
                    serialNUm = i + 1;
                    break;
                }
            }
            return serialNUm;
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

        public void AddNpc(UnitDesc unitDesc)
        {
            UnitExtraDynamic extra = DataScene2D.CurScene.GetUnitExtra(unitDesc.Guid);
            if (UnitDefine.IsNpc(unitDesc.Id))
            {
                if (extra.NpcSerialNumber != 0)
                {
                    SetNpcSerialNum(extra.NpcSerialNumber);
                }
                else
                {
                    int num = GetNpcSerialNum();
                    SetNpcSerialNum(num);
                    extra.NpcSerialNumber = (ushort) num;
                }
                if (extra.NpcType == (int) ENpcType.Task)
                {
                    for (int i = 0; i < extra.NpcTask.Count; i++)
                    {
                        if (extra.NpcTask.Get<NpcTaskDynamic>(i).NpcTaskSerialNumber == 0)
                        {
                            int taskNum = GetNpcTaskSerialNum();
                            if (SetNpcTaskSerialNum(taskNum))
                            {
                                extra.NpcTask.Get<NpcTaskDynamic>(i).NpcTaskSerialNumber = (ushort) taskNum;
                            }
                            else
                            {
                                extra.NpcTask.RemoveAt(i);
                            }
                        }
                        else
                        {
                            SetNpcTaskSerialNum(extra.NpcTask.Get<NpcTaskDynamic>(i).NpcTaskSerialNumber);
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