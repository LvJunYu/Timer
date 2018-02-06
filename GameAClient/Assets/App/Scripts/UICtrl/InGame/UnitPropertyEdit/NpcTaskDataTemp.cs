using System.Collections.Generic;
using GameA.Game;
using Microsoft.Win32;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class NpcTaskDataTemp
    {
        public const int MaxNpcTargetSerialNum = 99;
        public const int NoneNumMark = -1;
        public const int MaxNpcSerialNum = 99;
        private static NpcTaskDataTemp _intance;
        private DictionaryListObject _npcTaskList = new DictionaryListObject();

        public static NpcTaskDataTemp Intance
        {
            get { return _intance ?? (_intance = new NpcTaskDataTemp()); }
        }

        private NpcTaskDataTemp()
        {
            Init();
        }

        public void Init()
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

        public void Clear()
        {
            for (int i = 0; i < MaxNpcTargetSerialNum; i++)
            {
                _npcTaskSerialNumberDic[i + 1] = false;
            }
            for (int i = 0; i < MaxNpcSerialNum; i++)
            {
                _npcSerialNumberDic[i + 1] = false;
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
        public bool EndEdit;

        public bool IsEditNpcTarget(IntVec3 guid)
        {
            return IsEditNpcData && guid == NpcIntVec3;
        }

        public void StartEditTargetControl(NpcTaskDynamic task, IntVec3 guid, ETaskContype type, UnitExtraDynamic extra)
        {
            EditMode.Instance.StopSwitch();
            TaskData = task;
            _npcTaskList.Clear();
            for (int i = 0; i < extra.NpcTask.Count; i++)
            {
                _npcTaskList.Add(extra.NpcTask.Get<NpcTaskDynamic>(i));
            }
            TaskTargetData = new NpcTaskTargetDynamic();
            TaskTargetData.TaskType = (byte) ENpcTargetType.Contorl;
            IsEditNpcData = true;
            TaskType = type;
            NpcIntVec3 = guid;
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
                    break;
                case ETaskContype.BeforeTask:
                    TaskData.BeforeTaskAward.Add(TaskTargetData);
                    break;
                case ETaskContype.Task:
                    TaskData.Targets.Add(TaskTargetData);
                    break;
            }
            CheckNpcTaskNum(_npcTaskList, unitextra.NpcSerialNumber);
            unitextra.NpcTask = _npcTaskList;
            EndEdit = true;
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
            if (!_npcTaskSerialNumberDic.ContainsKey(num))
            {
                return;
            }
            _npcTaskSerialNumberDic[num] = false;
        }

        public void RecycleNpcSerialNum(int num)
        {
            if (!_npcSerialNumberDic.ContainsKey(num))
            {
                return;
            }
            _npcSerialNumberDic[num] = false;
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

        public void RemoveNpc(UnitDesc unitDesc)
        {
            UnitExtraDynamic extra = DataScene2D.CurScene.GetUnitExtra(unitDesc.Guid);
            if (UnitDefine.IsNpc(unitDesc.Id))
            {
                if (extra.NpcSerialNumber != 0)
                {
                    RecycleNpcSerialNum(extra.NpcSerialNumber);
                }
                if (extra.NpcType == (int) ENpcType.Task)
                {
                    for (int i = 0; i < extra.NpcTask.Count; i++)
                    {
                        if (extra.NpcTask.Get<NpcTaskDynamic>(i).NpcTaskSerialNumber != 0)
                        {
                        }
                        else
                        {
                            RecycleNpcTaskSerialNum(extra.NpcTask.Get<NpcTaskDynamic>(i).NpcTaskSerialNumber);
                        }
                    }
                }
                UnitBase unit;
                NPCBase npcUnit;
                if (ColliderScene2D.CurScene.TryGetUnit(unitDesc.Guid, out unit))
                {
                    npcUnit = unit as NPCBase;
                    if (npcUnit != null && npcUnit.StateBar != null) npcUnit.SetNpcNum();
                }
            }
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
                        if (extra.NpcTask.Get<NpcTaskDynamic>(i).TargetNpcSerialNumber == 0)
                        {
                            extra.NpcTask.Get<NpcTaskDynamic>(i).TargetNpcSerialNumber = extra.NpcSerialNumber;
                        }
                    }
                }
                UnitBase unit;
                NPCBase npcUnit;
                if (ColliderScene2D.CurScene.TryGetUnit(unitDesc.Guid, out unit))
                {
                    npcUnit = unit as NPCBase;
                    if (npcUnit != null && npcUnit.StateBar != null) npcUnit.SetNpcNum();
                }
            }
        }

        public void CheckNpcTaskNum(DictionaryListObject tasklist, int npcSerialNum)
        {
            int num = tasklist.Count;
            for (int i = 0; i < num; i++)
            {
                if (tasklist.Get<NpcTaskDynamic>(i).Targets.Count == 0)
                {
                    RecycleNpcTaskSerialNum(tasklist.Get<NpcTaskDynamic>(i)
                        .NpcTaskSerialNumber);
                    tasklist.RemoveAt(i);
                }
                else
                {
                    if (tasklist.Get<NpcTaskDynamic>(i).TargetNpcSerialNumber == 0)
                    {
                        tasklist.Get<NpcTaskDynamic>(i).TargetNpcSerialNumber = (ushort) npcSerialNum;
                    }
                }
            }
        }

        public ETaskContype GetTaskContype(IntVec3 switchGuid, IntVec3 unitGuid)
        {
            ETaskContype type = ETaskContype.None;
            UnitExtraDynamic unitextra;
            unitextra = DataScene2D.CurScene.GetUnitExtra(switchGuid);
            NpcTaskDynamic task;
            NpcTaskTargetDynamic target;
            for (int i = 0; i < unitextra.NpcTask.Count; i++)
            {
                task = unitextra.NpcTask.Get<NpcTaskDynamic>(i);

                for (int j = 0; j < task.Targets.Count; j++)
                {
                    target = task.Targets.Get<NpcTaskTargetDynamic>(i);
                    if (target.TargetGuid == unitGuid)
                    {
                        type = ETaskContype.Task;
                    }
                }
                for (int j = 0; j < task.BeforeTaskAward.Count; j++)
                {
                    target = task.BeforeTaskAward.Get<NpcTaskTargetDynamic>(i);
                    if (target.TargetGuid == unitGuid)
                    {
                        type = ETaskContype.BeforeTask;
                    }
                }
                for (int j = 0; j < task.TaskFinishAward.Count; j++)
                {
                    target = task.TaskFinishAward.Get<NpcTaskTargetDynamic>(i);
                    if (target.TargetGuid == unitGuid)
                    {
                        type = ETaskContype.AfterTask;
                    }
                }
            }
            return type;
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