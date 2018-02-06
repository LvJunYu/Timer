﻿using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class
        RpgTaskManger
    {
        private static RpgTaskManger _instance;

        public static RpgTaskManger Instance
        {
            get { return _instance ?? (_instance = new RpgTaskManger()); }
        }

        static RpgTaskManger()
        {
            IintData();
        }

        private const int MaxTaskNum = 3;

        private readonly Dictionary<IntVec3, UnitExtraDynamic> _allNpcExtraData =
            new Dictionary<IntVec3, UnitExtraDynamic>();


        private static readonly Dictionary<int, int> KillMonstorNum = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> ColltionNum = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> KillMonstorNumTemp = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> ColltionNumtemp = new Dictionary<int, int>();

        private readonly Dictionary<IntVec3, NpcTaskDynamic> _npcTaskDynamics =
            new Dictionary<IntVec3, NpcTaskDynamic>();

        private readonly Dictionary<int, IntVec3> _deliverNpcDic = new Dictionary<int, IntVec3>();
        public readonly Dictionary<IntVec3, float> NpcTaskDynamicsTimeLimit = new Dictionary<IntVec3, float>();
        private readonly Dictionary<int, NpcTaskDynamic> _finishNpcTask = new Dictionary<int, NpcTaskDynamic>();
        private readonly Dictionary<IntVec3, bool> _controlDic = new Dictionary<IntVec3, bool>();
        private bool _haveNpcInScene;
        private Action _showDiaEvent;
        private NPCBase _curHitNpc;
        private IntVec3 _curNpcGuid;
        private const int JudgeInvterTime = 100;
        private int _coutTime = 0;

        private static void IintData()
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

        public void GetAllTask()
        {
            _allNpcExtraData.Clear();
            _npcTaskDynamics.Clear();
            _finishNpcTask.Clear();
            _controlDic.Clear();
            _deliverNpcDic.Clear();
            foreach (var colltion in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                ColltionNum[colltion.Value.Id] = 0;
            }
            foreach (var kill in TableManager.Instance.Table_NpcTaskTargetKillDic)
            {
                KillMonstorNum[kill.Value.Id] = 0;
            }
            NpcTaskDynamicsTimeLimit.Clear();
            _haveNpcInScene = false;
            using (var enumerator = DataScene2D.CurScene.UnitExtras.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.NpcSerialNumber != 0)
                    {
                        _allNpcExtraData.Add(enumerator.Current.Key, enumerator.Current.Value);
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
                        _deliverNpcDic.Add(enumerator.Current.Value.NpcSerialNumber, enumerator.Current.Key);
                    }
                }
                SocialGUIManager.Instance.GetUI<UICtrlSceneState>().SetNpcTaskPanelDis();
                if (_allNpcExtraData.Count <= 0)
                {
                    _haveNpcInScene = false;
                }
                else
                {
                    _haveNpcInScene = true;
                }
            }
            JudegeBeforeTask();
        }

        public void OnPlayHitNpc(IntVec3 npcguid)
        {
            if (JudeOldHitNpc(npcguid))
            {
                return;
            }
            if (!_haveNpcInScene)
            {
                return;
            }
            Judge();
            if (MidleTaskDia(npcguid))
            {
                return;
            }
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
        }

        //任务中的对话
        public bool MidleTaskDia(IntVec3 npcguid)
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
        public bool TaskMessageDia(IntVec3 npcguid)
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
                            UnitExtraDynamic finishTaskDia;
                            if (_allNpcExtraData.TryGetValue(npcguid, out finishTaskDia))
                            {
                                if (finishTaskDia.NpcSerialNumber == target.TargetNpcNum)
                                {
                                    ShowTip(npcguid);
                                    DictionaryListObject taskAfter = new DictionaryListObject();

                                    if (enumerator.Current.Value.TaskAfter.Count > 0)
                                    {
                                        taskAfter = enumerator.Current.Value.TaskAfter;
                                    }
                                    else
                                    {
                                        taskAfter.Add(
                                            GetTaskFinishDia(enumerator.Current.Value, enumerator.Current.Key));
                                    }
                                    var taskguid1 = enumerator.Current.Key;
                                    _showDiaEvent = () =>
                                    {
                                        _finishNpcTask.Add(enumerator.Current.Value.NpcTaskSerialNumber,
                                            enumerator.Current.Value);
                                        RemoveTask(taskguid1, false);
                                        SocialGUIManager.Instance.OpenUI<UICtrlShowNpcDia>(taskAfter);
                                    };
                                    canshow = true;
                                }
                            }
                        }
                    }
                }
            }
            return canshow;
        }

        // 任务后的对话
        public bool AfterTaskDia(IntVec3 npcguid)
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
                            DictionaryListObject diaList = new DictionaryListObject();
                            //找到任务的原guid
                            IntVec3 guid = IntVec3.zero;
                            foreach (var taskinfo in _npcTaskDynamics)
                            {
                                if (taskinfo.Value.TargetNpcSerialNumber == extraDynmic.NpcSerialNumber)
                                {
                                    guid = taskinfo.Key;
                                }
                            }
                            if (guid != IntVec3.zero)
                            {
                                if (enumerator.Current.Value.TaskAfter.Count > 0)
                                {
                                    diaList = enumerator.Current.Value.TaskAfter;
                                }
                                else
                                {
                                    diaList.Add(GetTaskFinishDia(enumerator.Current.Value,
                                        npcguid));
                                }
                                ShowTip(npcguid);
                                _showDiaEvent = () =>
                                {
                                    for (int j = 0; j < enumerator.Current.Value.TaskFinishAward.Count; j++)
                                    {
                                        var award =
                                            enumerator.Current.Value.TaskFinishAward.Get<NpcTaskTargetDynamic>(j);
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
                            }
                        }
                    }
                }
            }
            return canshow;
        }

        //加入新的任务
        public bool AddNewTask(IntVec3 npcguid)
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
                    for (int i = 0; i < extra.NpcTask.Count; i++)
                    {
                        NpcTaskDynamic taskDynamic = extra.NpcTask.Get<NpcTaskDynamic>(i);
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
                    if (UnitDefine.IsKey(award.TargetUnitID))
                    {
                        PlayMode.Instance.SceneState.AddKey();
                    }
                    if (UnitDefine.IsTeeth(award.TargetUnitID))
                    {
                        PlayMode.Instance.SceneState.GemGain++;
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
            if (!_haveNpcInScene)
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
            if (!_haveNpcInScene)
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
        public void OnControlFinish(IntVec3 guid)
        {
            if (!_haveNpcInScene)
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
//            List<IntVec3> removeGuid = new List<IntVec3>();
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
                                if (_controlDic.ContainsKey(target.TargetGuid))
                                {
                                    finishnum++;
                                }
                                break;
                        }
                    }
                    if (finishnum == enumerator.Current.Value.Targets.Count &&
                        !_finishNpcTask.ContainsKey(enumerator.Current.Value.NpcTaskSerialNumber))
                    {
                        _finishNpcTask.Add(enumerator.Current.Value.NpcTaskSerialNumber, enumerator.Current.Value);
                        IntVec3 deliverNpcGuid = _deliverNpcDic[enumerator.Current.Value.TargetNpcSerialNumber];
                        UnitBase unit;
                        NPCBase npcUnit;
                        ColliderScene2D.CurScene.TryGetUnit(deliverNpcGuid, out unit);
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
        private void JudegeBeforeTask()
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
                    ColliderScene2D.CurScene.TryGetUnit(enmoutor.Current.Key, out unit);
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

        //缓存数据的方法
        private void SetTemp(Dictionary<int, int> oriDic, Dictionary<int, int> temp)
        {
            foreach (var col in oriDic)
            {
                temp[col.Key] = col.Value;
            }
        }

        public void RemoveTask(IntVec3 guid, bool isovertime)
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
                ColliderScene2D.CurScene.TryGetUnit(guid, out unit);
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

        public void AddTask(IntVec3 guid, NpcTaskDynamic task)
        {
            _npcTaskDynamics.Add(guid, task);
            UnitBase unitold;
            NPCBase npcUnitold;
            ColliderScene2D.CurScene.TryGetUnit(guid, out unitold);
            npcUnitold = unitold as NPCBase;
            if (npcUnitold != null) npcUnitold.SetNoShow();
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
            NpcTaskDynamicsTimeLimit.Add(guid, GameRun.Instance.GameTimeSinceGameStarted);
            IntVec3 deliverNpcGuid = _deliverNpcDic[task.TargetNpcSerialNumber];
            UnitBase unit;
            NPCBase npcUnit;
            ColliderScene2D.CurScene.TryGetUnit(deliverNpcGuid, out unit);
            npcUnit = unit as NPCBase;
            if (npcUnit != null) npcUnit.SetInTask();
            SocialGUIManager.Instance.GetUI<UICtrlSceneState>().SetNpcTask(_npcTaskDynamics, _finishNpcTask);
        }

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

        private void ShowTip(IntVec3 guid)
        {
            UnitBase unit;
            NPCBase npcUnit;
            if (ColliderScene2D.CurScene.TryGetUnit(guid, out unit))
            {
                npcUnit = (NPCBase) unit;
                npcUnit.SetShowTip();
            }
        }

        private bool JudeOldHitNpc(IntVec3 npcguid)
        {
            _coutTime++;
            bool isOldNpcOrNoNpc = false;
            UnitBase unit;
            UnitBase unitOld;
            if (ColliderScene2D.CurScene.TryGetUnit(npcguid, out unit))
            {
                if (!UnitDefine.IsNpc(unit.Id))
                {
                    if (ColliderScene2D.CurScene.TryGetUnit(_curNpcGuid, out unitOld))
                    {
                        NPCBase oldNpc = unitOld as NPCBase;
                        if (oldNpc != null && _coutTime > JudgeInvterTime)
                        {
                            if (oldNpc.OldState != null)
                            {
                                oldNpc.OldState.Invoke();
                                JudegeBeforeTask();
                            }
                            _curNpcGuid = IntVec3.zero;
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
            if (_curNpcGuid == npcguid)
            {
                isOldNpcOrNoNpc = true;
                return isOldNpcOrNoNpc;
            }
            _curHitNpc = null;
            if (ColliderScene2D.CurScene.TryGetUnit(_curNpcGuid, out unitOld))
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
                    des = String.Format("传话给编号{0}的NPC", target.TargetNpcNum);
                    break;
            }
            return des;
        }

        private string GetTaskBeforeDiA(NpcTaskDynamic task, IntVec3 npcguid)
        {
            UnitBase unitbase;
            ColliderScene2D.CurScene.TryGetUnit(npcguid, out unitbase);
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

        private string GetTaskFinishDia(NpcTaskDynamic task, IntVec3 npcguid)
        {
            UnitBase unitbase;
            ColliderScene2D.CurScene.TryGetUnit(npcguid, out unitbase);
            var unit = unitbase.UnitDesc;
            UnitExtraDynamic extra = unitbase.GetUnitExtra();
            string dia = NpcDia.SetDiaData((int) NpcDia.GetNpcType(unit.Id),
                (int) ENpcFace.Happy, "再见！", extra.NpcName, NpcDia.brown,
                (int) EnpcWaggle.None);
            return dia;
        }
    }
}