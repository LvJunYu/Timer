using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SoyEngine;

namespace GameA.Game
{
    public class RpgTaskManger
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

        private Dictionary<IntVec3, UnitExtraDynamic> _allNpcExtraData = new Dictionary<IntVec3, UnitExtraDynamic>();
        private Dictionary<IntVec3, NpcTaskDynamic> _npcTaskDynamics = new Dictionary<IntVec3, NpcTaskDynamic>();
        private static Dictionary<int, int> _killMonstorNum = new Dictionary<int, int>();
        private static Dictionary<int, int> _colltionNum = new Dictionary<int, int>();
        private Dictionary<int, NpcTaskDynamic> _finishNpcTask = new Dictionary<int, NpcTaskDynamic>();
        private Dictionary<IntVec3, bool> _controlDic = new Dictionary<IntVec3, bool>();
        private bool haveNpcInScene = false;

        private static void IintData()
        {
            foreach (var VARIABLE in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                _colltionNum.Add(VARIABLE.Value.Id, 0);
            }
            foreach (var VARIABLE in TableManager.Instance.Table_NpcTaskTargetKillDic)
            {
                _killMonstorNum.Add(VARIABLE.Value.Id, 0);
            }
        }

        public void GetAllTask()
        {
            _allNpcExtraData.Clear();
            _npcTaskDynamics.Clear();
            _finishNpcTask.Clear();
            _controlDic.Clear();
            haveNpcInScene = false;
            using (var enumerator = DataScene2D.CurScene.UnitExtras.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.NpcSerialNumber != 0)
                    {
                        _allNpcExtraData.Add(enumerator.Current.Key, enumerator.Current.Value);
                    }
                }
                SocialGUIManager.Instance.GetUI<UICtrlSceneState>().SetNpcTaskPanelDis();
                if (_allNpcExtraData.Count <= 0)
                {
                    haveNpcInScene = false;
                }
                else
                {
                    haveNpcInScene = true;
                }
            }
        }

        public void OnPlayHitNpc(IntVec3 npcguid)
        {
            if (!haveNpcInScene)
            {
                return;
            }
            //任务中的对话
            NpcTaskDynamic _task;
            if (_npcTaskDynamics.TryGetValue(npcguid, out _task))
            {
                if (_task.TaskMiddle.Count > 0)
                {
                    SocialGUIManager.Instance.OpenUI<UICtrlShowNpcDia>(_task.TaskMiddle);
                    if (SocialGUIManager.Instance.GetUI<UICtrlShowNpcDia>().IsOpen)
                    {
                        return;
                    }
                }
            }
            //任务是传话
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
                                    _finishNpcTask.Add(enumerator.Current.Value.NpcTaskSerialNumber,
                                        enumerator.Current.Value);
                                    _npcTaskDynamics.Remove(enumerator.Current.Key);

                                    if (_finishNpcTask[i].TaskAfter.Count > 0)
                                    {
                                        SocialGUIManager.Instance.OpenUI<UICtrlShowNpcDia>(_finishNpcTask[i].TaskAfter);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //任务后的对话
            UnitExtraDynamic extraDynmic;
            if (_allNpcExtraData.TryGetValue(npcguid, out extraDynmic))
            {
                for (int i = 0; i < extraDynmic.NpcTask.Count; i++)
                {
                    if (_finishNpcTask.ContainsKey(extraDynmic.NpcTask.Get<NpcTaskDynamic>(i).NpcTaskSerialNumber))
                    {
                        _npcTaskDynamics.Remove(npcguid);
                        if (extraDynmic.NpcTask.Get<NpcTaskDynamic>(i)
                                .TaskAfter.Count > 0)
                        {
                            SocialGUIManager.Instance.OpenUI<UICtrlShowNpcDia>(extraDynmic.NpcTask
                                .Get<NpcTaskDynamic>(i)
                                .TaskAfter);
                        }

                        return;
                    }
                }
            }
            //加入新的任务
            if (_npcTaskDynamics.Count >= 3)
            {
                return;
            }
            else
            {
                UnitExtraDynamic extra;
                if (_allNpcExtraData.TryGetValue(npcguid, out extra) && !_npcTaskDynamics.ContainsKey(npcguid))
                {
                    for (int i = 0; i < extra.NpcTask.Count; i++)
                    {
                        NpcTaskDynamic task = extra.NpcTask.Get<NpcTaskDynamic>(i);
                        if (!_finishNpcTask.ContainsKey(task.NpcTaskSerialNumber) && TriggerTaskFinish(task))
                        {
                            for (int j = 0; j < task.BeforeTaskAward.Count; j++)
                            {
                                var award = task.BeforeTaskAward.Get<NpcTaskTargetDynamic>(j);
                                GetAward(award);
                            }
                            _npcTaskDynamics.Add(npcguid, task);
                            //领取任务开始对话
                            if (task.Count > 0)
                            {
                                SocialGUIManager.Instance.OpenUI<UICtrlShowNpcDia>(task.TaskBefore);
                            }

                            break;
                        }
                    }
                }
            }
            SocialGUIManager.Instance.GetUI<UICtrlSceneState>().SetNpcTask(_npcTaskDynamics, _finishNpcTask);
        }

        public void GetAward(NpcTaskTargetDynamic award)
        {
            switch (award.TaskType)
            {
                case (byte) ENpcTargetType.Contorl:
                    UnitExtraDynamic extarData;
                    if (_allNpcExtraData.TryGetValue(award.TargetGuid, out extarData))
                    {
                        UnitBase unit;
                        if (ColliderScene2D.CurScene.TryGetUnit(award.TargetGuid, out unit))
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

        public bool TriggerTaskFinish(NpcTaskDynamic task)
        {
            bool finish = false;
            switch (task.TriggerType)
            {
                case (byte) TrrigerTaskType.None:
                    finish = true;
                    break;
                case (byte) TrrigerTaskType.Kill:
                    finish = _killMonstorNum[task.TriggerTask.TargetUnitID] > task.TriggerTask.ColOrKillNum;
                    break;
                case (byte) TrrigerTaskType.Colltion:
                    finish = _colltionNum[task.TriggerTask.TargetUnitID] > task.TriggerTask.ColOrKillNum;
                    finish = true;
                    break;
                case (byte) TrrigerTaskType.FinishOtherTask:
                    finish = _finishNpcTask.ContainsKey(task.TriggerTaskNumber);
                    break;
            }
            return finish;
        }

        public void AddColltion(int id)
        {
            if (!haveNpcInScene)
            {
                return;
            }
            if (_colltionNum.ContainsKey(id))
            {
                _colltionNum[id]++;
            }
            Judge();
        }

        public void AddKill(int id)
        {
            if (!haveNpcInScene)
            {
                return;
            }
            if (_killMonstorNum.ContainsKey(id))
            {
                _killMonstorNum[id]++;
            }
            Judge();
        }

        public void OnControlFinish(IntVec3 guid)
        {
            if (!haveNpcInScene)
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

        public void Judge()
        {
            bool finishTask = false;
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
                                if (_colltionNum.ContainsKey(target.TargetUnitID))
                                {
                                    if (_colltionNum[target.TargetUnitID] > target.ColOrKillNum)
                                    {
                                        _colltionNum[target.TargetUnitID] -= target.ColOrKillNum;
                                        finishnum++;
                                    }
                                }
                                break;
                            case ENpcTargetType.Moster:
                                if (_killMonstorNum.ContainsKey(target.TargetUnitID))
                                {
                                    if (_killMonstorNum[target.TargetUnitID] > target.ColOrKillNum)
                                    {
                                        _killMonstorNum[target.TargetUnitID] -= target.ColOrKillNum;
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
                    if (finishnum == enumerator.Current.Value.Targets.Count)
                    {
                        finishTask = true;
                        _finishNpcTask.Add(enumerator.Current.Value.NpcTaskSerialNumber, enumerator.Current.Value);
                        break;
                    }
                }
            }
        }
    }
}