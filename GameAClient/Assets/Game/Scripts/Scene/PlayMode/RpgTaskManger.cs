using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public struct UnitSceneGuid : IEquatable<UnitSceneGuid>
    {
        public UnitSceneGuid(IntVec3 guid, int index)
        {
            Guid = guid;
            Index = index;
        }

        public IntVec3 Guid;
        public int Index;
        public static UnitSceneGuid zore = new UnitSceneGuid(IntVec3.zero, 0);

        public bool Equals(UnitSceneGuid other)
        {
            return Guid.Equals(other.Guid) && Index == other.Index;
        }
    }

    public class RpgTaskMangerData
    {
        private RpgTaskMangerData()
        {
            IintData();
        }

        private static RpgTaskMangerData _instance;

        public static RpgTaskMangerData Instance
        {
            get { return _instance ?? (_instance = new RpgTaskMangerData()); }
        }

        public bool HaveNpcInScene;

        public readonly Dictionary<UnitSceneGuid, NpcTaskDynamic> NpcTaskDynamics =
            new Dictionary<UnitSceneGuid, NpcTaskDynamic>();

        public readonly Dictionary<UnitSceneGuid, UnitExtraDynamic> AllNpcExtraData =
            new Dictionary<UnitSceneGuid, UnitExtraDynamic>();

        public readonly Dictionary<int, UnitSceneGuid> DeliverNpcDic = new Dictionary<int, UnitSceneGuid>();

        public readonly Dictionary<UnitSceneGuid, float> NpcTaskDynamicsTimeLimit =
            new Dictionary<UnitSceneGuid, float>();

        public readonly Dictionary<int, NpcTaskDynamic> FinishNpcTask = new Dictionary<int, NpcTaskDynamic>();
        public readonly Dictionary<UnitSceneGuid, bool> ControlDic = new Dictionary<UnitSceneGuid, bool>();
        public readonly Dictionary<int, int> KillMonstorNum = new Dictionary<int, int>();
        public readonly Dictionary<int, int> ColltionNum = new Dictionary<int, int>();
        public readonly Dictionary<int, int> KillMonstorNumTemp = new Dictionary<int, int>();
        public readonly Dictionary<int, int> ColltionNumtemp = new Dictionary<int, int>();

        private void IintData()
        {
            foreach (var colltion in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                ColltionNum.Add(colltion.Value.Id, 0);
            }

            foreach (var kill in TableManager.Instance.Table_NpcTaskTargetKillDic)
            {
                KillMonstorNum.Add(kill.Value.Id, 0);
            }

            foreach (var colltion in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                ColltionNumtemp.Add(colltion.Value.Id, 0);
            }

            foreach (var kill in TableManager.Instance.Table_NpcTaskTargetKillDic)
            {
                KillMonstorNumTemp.Add(kill.Value.Id, 0);
            }
        }

        public void ClearData()
        {
            AllNpcExtraData.Clear();
            NpcTaskDynamics.Clear();
            DeliverNpcDic.Clear();
            NpcTaskDynamicsTimeLimit.Clear();
            FinishNpcTask.Clear();
            ControlDic.Clear();
            foreach (var colltion in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                ColltionNum[colltion.Value.Id] = 0;
            }

            foreach (var kill in TableManager.Instance.Table_NpcTaskTargetKillDic)
            {
                KillMonstorNum[kill.Value.Id] = 0;
            }

            HaveNpcInScene = false;
        }
    }

    public class RpgTaskManger
    {
        private const int MaxTaskNum = 3;

        private Dictionary<UnitSceneGuid, UnitExtraDynamic> _allNpcExtraData;
        private Dictionary<int, int> KillMonstorNum;
        private Dictionary<int, int> ColltionNum;
        private Dictionary<int, int> KillMonstorNumTemp;
        private Dictionary<int, int> ColltionNumtemp;

        private Dictionary<UnitSceneGuid, NpcTaskDynamic> _npcTaskDynamics;

        private Dictionary<int, UnitSceneGuid> _deliverNpcDic;
        public Dictionary<UnitSceneGuid, float> NpcTaskDynamicsTimeLimit;
        private Dictionary<int, NpcTaskDynamic> _finishNpcTask;
        private Dictionary<UnitSceneGuid, bool> _controlDic;

        private Action _showDiaEvent;
        private NPCBase _curHitNpc;
        private UnitSceneGuid _curNpcGuid;
        private const int JudgeInvterTime = 100;
        private int _coutTime;

        public void GetAllTask()
        {
            ColltionNum = RpgTaskMangerData.Instance.ColltionNum;
            KillMonstorNum = RpgTaskMangerData.Instance.KillMonstorNum;
            KillMonstorNumTemp = RpgTaskMangerData.Instance.KillMonstorNumTemp;
            ColltionNumtemp = RpgTaskMangerData.Instance.ColltionNumtemp;
            _allNpcExtraData = RpgTaskMangerData.Instance.AllNpcExtraData;
            _npcTaskDynamics = RpgTaskMangerData.Instance.NpcTaskDynamics;
            _finishNpcTask = RpgTaskMangerData.Instance.FinishNpcTask;
            _controlDic = RpgTaskMangerData.Instance.ControlDic;
            _deliverNpcDic = RpgTaskMangerData.Instance.DeliverNpcDic;
            NpcTaskDynamicsTimeLimit = RpgTaskMangerData.Instance.NpcTaskDynamicsTimeLimit;
            using (var enumerator = DataScene2D.CurScene.UnitExtras.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.NpcSerialNumber != 0)
                    {
                        _allNpcExtraData.Add(
                            new UnitSceneGuid(enumerator.Current.Key, Scene2DManager.Instance.CurSceneIndex),
                            enumerator.Current.Value);
                        UnitBase unit;
                        NPCBase npcUnit;
                        if (ColliderScene2D.CurScene.TryGetUnit(enumerator.Current.Key, out unit))
                        {
                            npcUnit = unit as NPCBase;
                            if (npcUnit != null)
                            {
                                npcUnit.SetNoShow();
                            }
                        }

                        _deliverNpcDic.Add(enumerator.Current.Value.NpcSerialNumber,
                            new UnitSceneGuid(enumerator.Current.Key, Scene2DManager.Instance.CurSceneIndex));
                    }
                }

                SocialGUIManager.Instance.GetUI<UICtrlSceneState>().SetNpcTaskPanelDis();
                if (_allNpcExtraData.Count <= 0)
                {
                    RpgTaskMangerData.Instance.HaveNpcInScene = false;
                }
                else
                {
                    RpgTaskMangerData.Instance.HaveNpcInScene = true;
                }
            }

            JudegeBeforeTask();
        }

        public void OnPlayHitNpc(UnitSceneGuid npcguid)
        {
            if (JudeOldHitNpc(npcguid))
            {
                return;
            }

            if (!RpgTaskMangerData.Instance.HaveNpcInScene)
            {
                return;
            }

            Judge();

            //任务是传话
            if (TaskMessageDia(npcguid))
            {
                return;
            }

            //任务后的对话
            if (AfterTaskDia(npcguid))
            {
                return;
            }

            //加入新的任务
            if (AddNewTask(npcguid))
            {
                return;
            }

            //任务中的对话放到最后去判断
            if (MidleTaskDia(npcguid))
            {
            }
        }

        //任务中的对话
        public bool MidleTaskDia(UnitSceneGuid npcguid)
        {
            bool canshow = false;
            //任务中的对话
            NpcTaskDynamic task;
            if (_npcTaskDynamics.TryGetValue(npcguid, out task))
            {
                if (!_finishNpcTask.ContainsKey(task.NpcTaskSerialNumber))
                {
                    if (task.TaskMiddle.Count > 0)
                    {
                        ShowTip(npcguid);
                        _showDiaEvent = () => { SocialGUIManager.Instance.OpenUI<UICtrlShowNpcDia>(task.TaskMiddle); };
                        canshow = true;
                    }
                }
            }

            return canshow;
        }

        // 任务是传话
        public bool TaskMessageDia(UnitSceneGuid npcguid)
        {
            bool canshow = false;
            using (var enumerator = _npcTaskDynamics.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    for (int i = 0; i < enumerator.Current.Value.Targets.Count; i++)
                    {
                        NpcTaskTargetDynamic target = enumerator.Current.Value.Targets.Get<NpcTaskTargetDynamic>(i);
                        if (target.TaskType == (byte) ENpcTargetType.Dialog)
                        {
                            NpcTaskDynamic task = enumerator.Current.Value;
                            UnitExtraDynamic finishTaskDia;
                            if (_allNpcExtraData.TryGetValue(npcguid, out finishTaskDia))
                            {
                                if (finishTaskDia.NpcSerialNumber == target.TargetNpcNum)
                                {
                                    ShowTip(npcguid);
                                    DictionaryListObject taskAfter = new DictionaryListObject();

                                    if (task.TaskAfter.Count > 0)
                                    {
//                                        ChangeDiaIcon(npcguid, enumerator.Current.Value.TaskAfter);
                                        taskAfter = task.TaskAfter;
                                    }
                                    else
                                    {
                                        taskAfter.Add(
                                            GetTaskFinishDia(task, npcguid));
                                    }

                                    var key = enumerator.Current.Key;
                                    var taskguid1 = key;
                                    DictionaryListObject awardlist;
                                    awardlist = GetAward(task.TaskFinishAward);
                                    _showDiaEvent = () =>
                                    {
                                        for (int j = 0; j < awardlist.Count; j++)
                                        {
                                            GetAward(awardlist.Get<NpcTaskTargetDynamic>(j));
                                        }

                                        _finishNpcTask.Add(task.NpcTaskSerialNumber,
                                            task);
                                        RemoveTask(taskguid1, false);
                                        SocialGUIManager.Instance.OpenUI<UICtrlShowNpcDia>(taskAfter);
                                    };
                                    canshow = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return canshow;
        }

        // 任务后的对话
        public bool AfterTaskDia(UnitSceneGuid npcguid)
        {
            bool canshow = false;
            using (var enumerator = _finishNpcTask.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    UnitExtraDynamic extraDynmic;
                    if (_allNpcExtraData.TryGetValue(npcguid, out extraDynmic))
                    {
                        if (enumerator.Current.Value.TargetNpcSerialNumber == extraDynmic.NpcSerialNumber)
                        {
                            NpcTaskDynamic task = enumerator.Current.Value;
                            if (!_npcTaskDynamics.ContainsValue(task))
                            {
                                continue;
                            }

                            DictionaryListObject diaList = new DictionaryListObject();
                            //找到任务的原guid
                            UnitSceneGuid guid = new UnitSceneGuid(IntVec3.zero, 0);
                            foreach (var taskinfo in _npcTaskDynamics)
                            {
                                if (taskinfo.Value.TargetNpcSerialNumber == extraDynmic.NpcSerialNumber)
                                {
                                    guid = taskinfo.Key;
                                }
                            }

                            if (!guid.Equals(UnitSceneGuid.zore))
                            {
                                if (task.TaskAfter.Count > 0)
                                {
//                                    ChangeDiaIcon(npcguid, enumerator.Current.Value.TaskAfter);
                                    diaList = task.TaskAfter;
                                }
                                else
                                {
                                    diaList.Add(GetTaskFinishDia(task,
                                        npcguid));
                                }

                                ShowTip(npcguid);
                                DictionaryListObject awardlist;
                                awardlist = GetAward(task.TaskFinishAward);
                                _showDiaEvent = () =>
                                {
                                    for (int j = 0; j < awardlist.Count; j++)
                                    {
                                        var award =
                                            awardlist.Get<NpcTaskTargetDynamic>(j);
                                        GetAward(award);
                                    }

                                    if (_npcTaskDynamics.ContainsKey(guid))
                                    {
                                        RemoveTask(guid, false);
                                    }

                                    JudegeBeforeTask();
                                    SocialGUIManager.Instance.OpenUI<UICtrlShowNpcDia>(diaList);
                                    canshow = true;
                                };
                                break;
                            }
                        }
                    }
                }
            }

            return canshow;
        }

        //加入新的任务
        public bool AddNewTask(UnitSceneGuid npcguid)
        {
            bool canshow = false;
            if (_npcTaskDynamics.Count >= MaxTaskNum)
            {
            }
            else
            {
                UnitExtraDynamic extra;
                if (_allNpcExtraData.TryGetValue(npcguid, out extra) && !_npcTaskDynamics.ContainsKey(npcguid))
                {
                    NpcTaskDynamic taskDynamic;
                    for (int i = 0; i < extra.NpcTask.Count; i++)
                    {
                        taskDynamic = extra.NpcTask.Get<NpcTaskDynamic>(i);
                        if (!_finishNpcTask.ContainsKey(taskDynamic.NpcTaskSerialNumber) &&
                            TriggerTaskFinish(taskDynamic))
                        {
                            DictionaryListObject diaList = new DictionaryListObject();
                            //领取任务开始对话
                            if (taskDynamic.TaskBefore.Count > 0)
                            {
                                diaList = taskDynamic.TaskBefore;
                            }
                            else
                            {
                                diaList.Add(GetTaskBeforeDiA(taskDynamic, npcguid));
                            }

                            ShowTip(npcguid);
                            _showDiaEvent = () =>
                            {
                                for (int j = 0; j < taskDynamic.BeforeTaskAward.Count; j++)
                                {
                                    var award = taskDynamic.BeforeTaskAward.Get<NpcTaskTargetDynamic>(j);
                                    GetAward(award);
                                }

                                AddTask(npcguid, taskDynamic);
                                SocialGUIManager.Instance.OpenUI<UICtrlShowNpcDia>(diaList);
                            };
                            canshow = true;
                            break;
                        }
                    }
                }
            }

            return canshow;
        }


        //获得奖励
        public void GetAward(NpcTaskTargetDynamic award)
        {
            switch (award.TaskType)
            {
                case (byte) ENpcTargetType.Contorl:
                    UnitBase unit;
                    if (ColliderScene2D.CurScene.TryGetUnit(award.TargetGuid, out unit))
                    {
                        if (UnitDefine.IsGate(unit.UnitDesc.Id))
                        {
                            Gate gate = unit as Gate;
                            gate.DirectOpen();
                        }
                        else
                        {
                            unit.OnCtrlBySwitch();
                        }
                    }

                    break;
                case (byte) ENpcTargetType.Colltion:
                    SocialGUIManager.Instance.GetUI<UICtrlSceneState>().GetAwardKeyCheck(award.TargetUnitID);
                    if (UnitDefine.IsKey(award.TargetUnitID))
                    {
                        for (int i = 0; i < award.ColOrKillNum; i++)
                        {
                            PlayMode.Instance.SceneState.AddKey();
                        }
                    }

                    if (UnitDefine.IsTeeth(award.TargetUnitID))
                    {
                        for (int i = 0; i < award.ColOrKillNum; i++)
                        {
                            PlayMode.Instance.SceneState.GemGain++;
                        }
                    }

                    break;
                case (byte) ENpcTargetType.Moster:

                    break;
                case (byte) ENpcTargetType.Dialog:

                    break;
            }
        }

        // 判断是否完成了触发任务完成任务
        public bool TriggerTaskFinish(NpcTaskDynamic task)
        {
            bool finish = false;
            switch (task.TriggerType)
            {
                case (byte) TrrigerTaskType.None:
                    finish = true;
                    break;
                case (byte) TrrigerTaskType.Kill:
                    finish = KillMonstorNum[task.TriggerTask.TargetUnitID] >= task.TriggerTask.ColOrKillNum;
                    break;
                case (byte) TrrigerTaskType.Colltion:
                    finish = ColltionNum[task.TriggerTask.TargetUnitID] >= task.TriggerTask.ColOrKillNum;
                    break;
                case (byte) TrrigerTaskType.FinishOtherTask:
                    finish = _finishNpcTask.ContainsKey(task.TriggerTaskNumber);
                    break;
            }

            return finish;
        }

        // 收集物品增加
        public void AddColltion(int id)
        {
            if (!RpgTaskMangerData.Instance.HaveNpcInScene)
            {
                return;
            }

            if (ColltionNum.ContainsKey(id))
            {
                ColltionNum[id]++;
            }

            Judge();
        }

        // 击杀增加
        public void AddKill(int id)
        {
            if (!RpgTaskMangerData.Instance.HaveNpcInScene)
            {
                return;
            }

            if (KillMonstorNum.ContainsKey(id))
            {
                KillMonstorNum[id]++;
            }

            Judge();
        }

        // 控制完成
        public void OnControlFinish(UnitSceneGuid guid)
        {
            if (!RpgTaskMangerData.Instance.HaveNpcInScene)
            {
                return;
            }

            if (_controlDic.ContainsKey(guid))
            {
                _controlDic[guid] = true;
            }
            else
            {
                _controlDic.Add(guid, true);
            }

            Judge();
        }

        // 判断任务是否完成首先判断的时间
        private void Judge()
        {
            JudegeBeforeTask();
            SetTemp(ColltionNum, ColltionNumtemp);
            SetTemp(KillMonstorNum, KillMonstorNumTemp);
            //判断时间的放到每个人物中判断
//            List<UnitSceneGuid> removeGuid = new List<UnitSceneGuid>();
//            foreach (var task in _npcTaskDynamics)
//            {
//                var time = (NpcTaskDynamicsTimeLimit[task.Key] + (float) task.Value.TaskimeLimit);
//                var b = time < GameRun.Instance.GameTimeSinceGameStarted;
//                if (!b)
//                {
//                    removeGuid.Add(task.Key);
//                }
//            }
//            for (int i = 0; i < removeGuid.Count; i++)
//            {
//                _npcTaskDynamics.Remove(removeGuid[i]);
//                NpcTaskDynamicsTimeLimit.Remove(removeGuid[i]);
//            }
            using (var enumerator = _npcTaskDynamics.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    int finishnum = 0;
                    for (int i = 0; i < enumerator.Current.Value.Targets.Count; i++)
                    {
                        NpcTaskTargetDynamic target = enumerator.Current.Value.Targets.Get<NpcTaskTargetDynamic>(i);
                        switch ((ENpcTargetType) target.TaskType)
                        {
                            case ENpcTargetType.Colltion:
                                if (ColltionNum.ContainsKey(target.TargetUnitID))
                                {
                                    if (ColltionNum[target.TargetUnitID] >= target.ColOrKillNum)
                                    {
                                        ColltionNum[target.TargetUnitID] -= target.ColOrKillNum;
                                        finishnum++;
                                    }
                                }

                                break;
                            case ENpcTargetType.Moster:
                                if (KillMonstorNum.ContainsKey(target.TargetUnitID))
                                {
                                    if (KillMonstorNum[target.TargetUnitID] >= target.ColOrKillNum)
                                    {
                                        KillMonstorNum[target.TargetUnitID] -= target.ColOrKillNum;
                                        finishnum++;
                                    }
                                }

                                break;
                            case ENpcTargetType.Contorl:
                                if (_controlDic != null)
                                {
                                    if (_controlDic.ContainsKey(new UnitSceneGuid(target.TargetGuid,
                                        Scene2DManager.Instance.CurSceneIndex)))
                                    {
                                        finishnum++;
                                    }
                                }

                                break;
                        }
                    }

                    if (finishnum == enumerator.Current.Value.Targets.Count &&
                        !_finishNpcTask.ContainsKey(enumerator.Current.Value.NpcTaskSerialNumber))
                    {
                        _finishNpcTask.Add(enumerator.Current.Value.NpcTaskSerialNumber, enumerator.Current.Value);
                        UnitSceneGuid deliverNpcGuid = _deliverNpcDic[enumerator.Current.Value.TargetNpcSerialNumber];
                        UnitBase unit;
                        NPCBase npcUnit;
                        Scene2DManager.Instance.TryGetUnit(deliverNpcGuid, out unit);
                        npcUnit = unit as NPCBase;
                        if (npcUnit != null)
                        {
                            npcUnit.SetFinishTask();
                        }

                        SocialGUIManager.Instance.GetUI<UICtrlSceneState>()
                            .SetNpcTask(_npcTaskDynamics, _finishNpcTask);
                        break;
                    }
                    else
                    {
                        SetTemp(ColltionNumtemp, ColltionNum);
                        SetTemp(KillMonstorNumTemp, KillMonstorNum);
                    }
                }
            }
        }

        //判断触发任务是否完成
        public void JudegeBeforeTask()
        {
            using (var enmoutor = _allNpcExtraData.GetEnumerator())
            {
                while (enmoutor.MoveNext())
                {
                    if (_npcTaskDynamics.ContainsKey(enmoutor.Current.Key))
                    {
                        return;
                    }

                    bool isready = false;
                    var extra = enmoutor.Current.Value;
                    for (int i = 0; i < extra.NpcTask.Count; i++)
                    {
                        NpcTaskDynamic task = extra.NpcTask.Get<NpcTaskDynamic>(i);
                        if (TriggerTaskFinish(task) && !_finishNpcTask.ContainsKey(task.NpcTaskSerialNumber))
                        {
                            isready = true;
                            break;
                        }
                    }

                    UnitBase unit;
                    if (Scene2DManager.Instance.TryGetUnit(enmoutor.Current.Key, out unit))
                    {
                        NPCBase npc = unit as NPCBase;

                        if (isready)
                        {
                            npc.SetReady();
                        }
                        else
                        {
                            npc.SetNoShow();
                        }
                    }
                }
            }
        }

        //缓存数据的方法
        private void SetTemp(Dictionary<int, int> oriDic, Dictionary<int, int> temp)
        {
            foreach (var col in oriDic)
            {
                temp[col.Key] = col.Value;
            }
        }

        //删除任务
        public void RemoveTask(UnitSceneGuid guid, bool isovertime)
        {
            if (isovertime && _finishNpcTask.ContainsKey(_npcTaskDynamics[guid].NpcTaskSerialNumber))
            {
                NpcTaskDynamic task = _finishNpcTask[_npcTaskDynamics[guid].NpcTaskSerialNumber];
                for (int i = 0;
                    i < task.Targets.Count;
                    i++)
                {
                    NpcTaskTargetDynamic target = task.Targets.Get<NpcTaskTargetDynamic>(i);
                    switch ((ENpcTargetType) target.TaskType)
                    {
                        case ENpcTargetType.Colltion:
                            if (ColltionNum.ContainsKey(target.TargetUnitID))
                            {
                                ColltionNum[target.TargetUnitID] += target.ColOrKillNum;
                            }

                            break;
                        case ENpcTargetType.Moster:
                            if (KillMonstorNum.ContainsKey(target.TargetUnitID))
                            {
                                KillMonstorNum[target.TargetUnitID] += target.ColOrKillNum;
                            }

                            break;
                    }
                }

                _finishNpcTask.Remove(_npcTaskDynamics[guid].NpcTaskSerialNumber);
            }

            _npcTaskDynamics.Remove(guid);
            NpcTaskDynamicsTimeLimit.Remove(guid);

//修改标志
            UnitExtraDynamic oriExtra;
            if (_allNpcExtraData.TryGetValue(guid, out oriExtra))
            {
                bool allFinish = true;
                for (int i = 0;
                    i < oriExtra.NpcTask.Count;
                    i++)
                {
                    if (!_finishNpcTask.ContainsKey(oriExtra.NpcTask.Get<NpcTaskDynamic>(i)
                        .NpcTaskSerialNumber))
                    {
                        allFinish = false;
                    }
                }

                UnitBase unit;
                NPCBase npcUnit;
                Scene2DManager.Instance.TryGetUnit(guid, out unit);
                npcUnit = unit as NPCBase;
                if (npcUnit != null)
                {
                    if (allFinish)
                    {
                        npcUnit.SetNoShow();
                    }
                    else
                    {
                        npcUnit.SetReady();
                    }
                }
            }

            SocialGUIManager.Instance.GetUI<UICtrlSceneState>().SetNpcTask(_npcTaskDynamics, _finishNpcTask);
        }

        //添加任务
        public void AddTask(UnitSceneGuid guid, NpcTaskDynamic task)
        {
            _npcTaskDynamics.Add(guid, task);
            UnitBase unitold;
            NPCBase npcUnitold;
            Scene2DManager.Instance.TryGetUnit(guid, out unitold);
            npcUnitold = unitold as NPCBase;
            if (npcUnitold != null) npcUnitold.SetNoShow();
            if (task.TriggerType == (int) TrrigerTaskType.Colltion ||
                task.TriggerType == (int) TrrigerTaskType.Colltion)
            {
                NpcTaskTargetDynamic TriggerTask = task.TriggerTask;
                switch ((ENpcTargetType) TriggerTask.TaskType)
                {
                    case ENpcTargetType.Colltion:
                        if (ColltionNum.ContainsKey(TriggerTask.TargetUnitID))
                        {
                            ColltionNum[TriggerTask.TargetUnitID] -= TriggerTask.ColOrKillNum;
                        }

                        break;
                    case ENpcTargetType.Moster:
                        if (KillMonstorNum.ContainsKey(TriggerTask.TargetUnitID))
                        {
                            KillMonstorNum[TriggerTask.TargetUnitID] -= TriggerTask.ColOrKillNum;
                        }

                        break;
                }
            }

            NpcTaskDynamicsTimeLimit.Add(guid, GameRun.Instance.GameTimeSinceGameStarted);
            UnitSceneGuid deliverNpcGuid = _deliverNpcDic[task.TargetNpcSerialNumber];
            UnitBase unit;
            NPCBase npcUnit;
            Scene2DManager.Instance.TryGetUnit(deliverNpcGuid, out unit);
            npcUnit = unit as NPCBase;
            if (npcUnit != null)
                if (task.Targets.Get<NpcTaskTargetDynamic>(0).TaskType == (int) ENpcTargetType.Dialog)
                {
                    npcUnit.SetFinishTask();
                }
                else
                {
                    npcUnit.SetInTask();
                }

            SocialGUIManager.Instance.GetUI<UICtrlSceneState>().SetNpcTask(_npcTaskDynamics, _finishNpcTask);
        }

        //控制键
        public void AssitConShowDiaEvent()
        {
            if (_curHitNpc == null)
            {
                return;
            }

            if (_showDiaEvent != null)
            {
                _showDiaEvent.Invoke();
                _showDiaEvent = null;
            }
        }

        //显示tip
        private void ShowTip(UnitSceneGuid guid)
        {
            UnitBase unit;
            NPCBase npcUnit;
            if (Scene2DManager.Instance.TryGetUnit(guid, out unit))
            {
                npcUnit = (NPCBase) unit;
                npcUnit.SetShowTip();
            }
        }

        // 判断是否是上一次碰到的npc
        private bool JudeOldHitNpc(UnitSceneGuid npcguid)
        {
            _coutTime++;
            bool isOldNpcOrNoNpc = false;
            UnitBase unit;
            UnitBase unitOld;
            if (Scene2DManager.Instance.TryGetUnit(npcguid, out unit))
            {
                if (!UnitDefine.IsNpc(unit.Id))
                {
                    if (Scene2DManager.Instance.TryGetUnit(_curNpcGuid, out unitOld))
                    {
                        NPCBase oldNpc = unitOld as NPCBase;
                        if (oldNpc != null && _coutTime > JudgeInvterTime)
                        {
                            if (oldNpc.OldState != null)
                            {
                                oldNpc.OldState.Invoke();
                                JudegeBeforeTask();
                            }

                            _curNpcGuid = new UnitSceneGuid(IntVec3.zero, 0);
                        }
                    }

                    isOldNpcOrNoNpc = true;
                    return isOldNpcOrNoNpc;
                }
            }
            else
            {
                isOldNpcOrNoNpc = true;
                return isOldNpcOrNoNpc;
            }

            if (_curNpcGuid.Equals(npcguid))
            {
                isOldNpcOrNoNpc = true;
                return isOldNpcOrNoNpc;
            }

            _curHitNpc = null;
            if (Scene2DManager.Instance.TryGetUnit(_curNpcGuid, out unitOld))
            {
                NPCBase oldNpc = unitOld as NPCBase;
                if (oldNpc.OldState != null)
                {
                    oldNpc.OldState.Invoke();
                }
            }

            _curNpcGuid = npcguid;
            _coutTime = 0;
            _curHitNpc = unit as NPCBase;
            _showDiaEvent = null;
            return isOldNpcOrNoNpc;
        }

        //获得目标的描述
        private string GetTargetDes(NpcTaskTargetDynamic target)
        {
            string des = "";
            switch (target.TaskType)
            {
                case (int) ENpcTargetType.Colltion:
                    des = String.Format("收集{0}{1}", TableManager.Instance.Table_UnitDic[target.TargetUnitID].Name,
                        target.ColOrKillNum);
                    break;
                case (int) ENpcTargetType.Moster:
                    des = String.Format("击杀{0}{1}", TableManager.Instance.Table_UnitDic[target.TargetUnitID].Name,
                        target.ColOrKillNum);
                    break;
                case (int) ENpcTargetType.Contorl:
                    UnitBase unit;
                    if (ColliderScene2D.CurScene.TryGetUnit(target.TargetGuid, out unit))
                    {
                        des = String.Format("控制{0}", TableManager.Instance.Table_UnitDic[unit.Id].Name);
                    }

                    break;
                case (int) ENpcTargetType.Dialog:
                    des = String.Format("传话给名字是{0}的NPC", GetNpcNameByNum(target.TargetNpcNum));
                    break;
            }

            return des;
        }

        //获得任务前的对话
        private string GetTaskBeforeDiA(NpcTaskDynamic task, UnitSceneGuid npcguid)
        {
            UnitBase unitbase;
            Scene2DManager.Instance.TryGetUnit(npcguid, out unitbase);
            var unit = unitbase.UnitDesc;
            UnitExtraDynamic extra = unitbase.GetUnitExtra();
            string diaContent = "";
            for (int j = 0; j < task.Targets.Count; j++)
            {
                NpcTaskTargetDynamic target = task.Targets.Get<NpcTaskTargetDynamic>(j);
                diaContent += GetTargetDes(target);
            }

            string dia = NpcDia.SetDiaData((int) NpcDia.GetNpcType(unit.Id),
                (int) ENpcFace.Happy, diaContent, extra.NpcName, NpcDia.brown,
                (int) EnpcWaggle.None);
            return dia;
        }

        // 获得任务后的对话
        private string GetTaskFinishDia(NpcTaskDynamic task, UnitSceneGuid npcguid)
        {
            UnitBase unitbase;
            Scene2DManager.Instance.TryGetUnit(npcguid, out unitbase);
            var unit = unitbase.UnitDesc;
            UnitExtraDynamic extra = unitbase.GetUnitExtra();
            string dia = NpcDia.SetDiaData((int) NpcDia.GetNpcType(unit.Id),
                (int) ENpcFace.Happy, "再见！", extra.NpcName, NpcDia.brown,
                (int) EnpcWaggle.None);
            return dia;
        }

        //根据编号获得名字
        public string GetNpcNameByNum(int num)
        {
            string name = "";
            using (var allnpc = _allNpcExtraData.GetEnumerator())
            {
                while (allnpc.MoveNext())
                {
                    if (allnpc.Current.Value.NpcSerialNumber == num)
                    {
                        if (allnpc.Current.Value.NpcName != null)
                        {
                            name += allnpc.Current.Value.NpcName;
                            break;
                        }
                    }
                }
            }

            return name;
        }

        //改变对话的头像
        private void ChangeDiaIcon(UnitSceneGuid npcguid, DictionaryListObject diaList)
        {
            if (diaList == null)
            {
                return;
            }

            DictionaryListObject newDictionaryListObject = new DictionaryListObject();
            for (int i = 0; i < diaList.Count; i++)
            {
                string dia = diaList.Get<string>(i);
                NpcDia npcDia = new NpcDia();
                npcDia.AnalysisNpcDia(dia);
                UnitBase unitbase;
                Scene2DManager.Instance.TryGetUnit(npcguid, out unitbase);
                var unit = unitbase.UnitDesc;
                npcDia.NpcId = NpcDia.GetNpcType(unit.Id);
                newDictionaryListObject.Add(npcDia.ToString());
            }

            diaList = newDictionaryListObject;
        }

//        TaskType = _nextId++;
//        public static readonly int ColOrKillNum = _nextId++;
//        public static readonly int TargetUnitID = _nextId++;
//        public static readonly int TargetGuid = _nextId++;
//        public static readonly int TargetNpcNum
        //copy数据

        // 获得奖励
        private DictionaryListObject GetAward(DictionaryListObject awardlist)
        {
            DictionaryListObject newawardlist = new DictionaryListObject();
            for (int i = 0; i < awardlist.Count; i++)
            {
                NpcTaskTargetDynamic target = new NpcTaskTargetDynamic();
                target.TaskType = awardlist.Get<NpcTaskTargetDynamic>(i).TaskType;
                target.ColOrKillNum =
                    awardlist.Get<NpcTaskTargetDynamic>(i).ColOrKillNum;
                target.TargetUnitID = awardlist.Get<NpcTaskTargetDynamic>(i).TargetUnitID;
                target.TargetGuid = awardlist.Get<NpcTaskTargetDynamic>(i).TargetGuid;
                target.TargetNpcNum = awardlist.Get<NpcTaskTargetDynamic>(i).TargetNpcNum;
                newawardlist.Add(target);
            }

            return newawardlist;
        }
    }
}