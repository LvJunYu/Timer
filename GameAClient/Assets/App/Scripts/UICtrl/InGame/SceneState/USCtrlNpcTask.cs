﻿using System;
using System.Collections.Generic;
using System.IO;
using GameA.Game;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USCtrlNpcTask : USCtrlBase<UsViewNpcTask>
    {
        private UnitExtraDynamic _extarDynamicData;
        private NpcTaskDynamic _taskData;
        public List<GameObject> TargeObjGroup = new List<GameObject>();
        public List<Text> TargetTypeTextGroup = new List<Text>();
        public List<Text> TargetNumTextGroup = new List<Text>();
        private UnitSceneGuid _guid;
        private bool _isFinish;

        public override void Init(UsViewNpcTask view)
        {
            base.Init(view);
            TargeObjGroup.Add(_cachedView.Target1);
            TargeObjGroup.Add(_cachedView.Target2);
            TargeObjGroup.Add(_cachedView.Target3);
            TargetTypeTextGroup.Add(_cachedView.TargetTypeText1);
            TargetTypeTextGroup.Add(_cachedView.TargetTypeText2);
            TargetTypeTextGroup.Add(_cachedView.TargetTypeText3);
            TargetNumTextGroup.Add(_cachedView.TargetNumText1);
            TargetNumTextGroup.Add(_cachedView.TargetNumText2);
            TargetNumTextGroup.Add(_cachedView.TargetNumText3);
        }

        public void SetNpcTask(UnitSceneGuid guid, UnitExtraDynamic extraData, NpcTaskDynamic taskData, bool isFinish)
        {
            _isFinish = isFinish;
            _guid = guid;
            _cachedView.SetActiveEx(true);
            _extarDynamicData = extraData;
            _taskData = taskData;
            _cachedView.NpcName.text = _extarDynamicData.NpcName;
            if (isFinish)
            {
                _cachedView.FinishBtn.SetActiveEx(true);
            }
            else
            {
                _cachedView.FinishBtn.SetActiveEx(false);
            }

            for (int i = 0; i < TargeObjGroup.Count; i++)
            {
                if (i < _taskData.Targets.Count)
                {
                    TargeObjGroup[i].SetActiveEx(true);
                    if (_taskData.Targets.Get<NpcTaskTargetDynamic>(i).TaskType == (int) ENpcTargetType.Contorl)
                    {
                        TargetNumTextGroup[i].SetActiveEx(false);
                        UnitBase unit;
                        if (ColliderScene2D.CurScene.TryGetUnit(
                            _taskData.Targets.Get<NpcTaskTargetDynamic>(i).TargetGuid,
                            out unit))
                        {
                            if (TableManager.Instance.Table_UnitDic.ContainsKey(unit.Id))
                            {
                                TargetTypeTextGroup[i].text = TableManager.Instance.Table_UnitDic[unit.Id].Name;
                            }
                        }
                    }
                    else
                    {
                        TargetNumTextGroup[i].SetActiveEx(true);
                    }


                    if (_taskData.TaskimeLimit <= 0)
                    {
                        _cachedView.TimeText1.SetActiveEx(false);
                    }
                    else
                    {
                        _cachedView.TimeText1.SetActiveEx(true);
                        _cachedView.TimeText1.text = String.Format("{0}秒", _taskData.TaskimeLimit);
                    }

                    if (_taskData.Targets.Get<NpcTaskTargetDynamic>(i).TaskType == (int) ENpcTargetType.Dialog)
                    {
                        TargetTypeTextGroup[i].text = "传话";
//                        TargetNumTextGroup[i].SetActiveEx(false);
                        TargetNumTextGroup[i].text =
                            Scene2DManager.Instance.GetCurScene2DEntity().RpgManger.GetNpcNameByNum(_taskData.Targets
                                .Get<NpcTaskTargetDynamic>(i)
                                .TargetNpcNum);
//                        RpgTaskManger.Instance.GetNpcNameByNum(_taskData.Targets.Get<NpcTaskTargetDynamic>(i)
//                            .TargetNpcNum);
                    }
                    else
                    {
                        if (_taskData.Targets.Get<NpcTaskTargetDynamic>(i).TaskType != (int) ENpcTargetType.Contorl)
                        {
                            TargetNumTextGroup[i].SetActiveEx(true);
                            int targetNum = _taskData.Targets.Get<NpcTaskTargetDynamic>(i).ColOrKillNum;
                            int nowNum = Scene2DManager.Instance.GetCurScene2DEntity().RpgManger
                                .GetKillOrColltionNum(_taskData.Targets.Get<NpcTaskTargetDynamic>(i).TargetUnitID);
                            if (isFinish)
                            {
                                TargetNumTextGroup[i].text = String.Format("{0}/{1}", targetNum, targetNum);
                            }
                            else
                            {
                                TargetNumTextGroup[i].text = String.Format("{0}/{1}", nowNum, targetNum);
                            }
                        }
                    }

                    if (TableManager.Instance.Table_UnitDic.ContainsKey(_taskData.Targets.Get<NpcTaskTargetDynamic>(i)
                        .TargetUnitID))
                    {
                        TargetTypeTextGroup[i].text = TableManager.Instance.Table_UnitDic[_taskData.Targets
                            .Get<NpcTaskTargetDynamic>(i)
                            .TargetUnitID].Name;
//                        TargetNumTextGroup[i].text = _taskData.Targets
//                            .Get<NpcTaskTargetDynamic>(i).ColOrKillNum.ToString();
                    }
                }
                else
                {
                    TargeObjGroup[i].SetActiveEx(false);
                }
            }
        }

        public void UpdataTimeLimit()
        {
            if (!HasInited)
            {
                return;
            }

            if (_taskData == null)
            {
                return;
            }

            if (_taskData.TaskimeLimit <= 0)
            {
                return;
            }

            if (_isFinish)
            {
                return;
            }

            if (Scene2DManager.Instance.GetCurScene2DEntity().RpgManger.NpcTaskDynamicsTimeLimit.ContainsKey(_guid))
            {
                float lastTime = GameRun.Instance.GameTimeSinceGameStarted -
                                 Scene2DManager.Instance.GetCurScene2DEntity().RpgManger
                                     .NpcTaskDynamicsTimeLimit[_guid];
                if (lastTime < _taskData.TaskimeLimit)
                {
                    _cachedView.TimeText1.text = String.Format("{0}秒", (int) (_taskData.TaskimeLimit - lastTime));
                }
                else
                {
                    Scene2DManager.Instance.GetCurScene2DEntity().RpgManger.RemoveTask(_guid, true);
                }
            }
        }

        public void SetDisable()
        {
            _cachedView.SetActiveEx(false);
        }
    }
}