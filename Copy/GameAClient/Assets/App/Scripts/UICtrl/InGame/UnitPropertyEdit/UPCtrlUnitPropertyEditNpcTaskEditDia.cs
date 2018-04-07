using System;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{ // npc 编辑对话
    public class
        UPCtrlUnitPropertyEditNpcTaskEditDia : UpCtrlNpcAdvanceBase
    {
        private NpcTaskDynamic _task;
        private USCtrlUnitNpcTaskTargetBtn[] _beforeTaskAwardBtnGroup;
        private USCtrlUnitNpcTaskTargetBtn[] _finishTaskAwardBtnGroup;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskEditDiaPanel;
            _beforeTaskAwardBtnGroup = new USCtrlUnitNpcTaskTargetBtn[_cachedView.BeforeTaskAward.Length];
            for (int i = 0; i < _cachedView.BeforeTaskAward.Length; i++)
            {
                USCtrlUnitNpcTaskTargetBtn item = new USCtrlUnitNpcTaskTargetBtn();
                item.Init(_cachedView.BeforeTaskAward[i]);
                _beforeTaskAwardBtnGroup[i] = item;
            }

            _finishTaskAwardBtnGroup = new USCtrlUnitNpcTaskTargetBtn[_cachedView.FinishTaskAward.Length];
            for (int i = 0; i < _cachedView.FinishTaskAward.Length; i++)
            {
                USCtrlUnitNpcTaskTargetBtn item = new USCtrlUnitNpcTaskTargetBtn();
                item.Init(_cachedView.FinishTaskAward[i]);
                _finishTaskAwardBtnGroup[i] = item;
            }

            _cachedView.TaskDiaBeforeBtn.onClick.AddListener(() =>
            {
                SocialGUIManager.Instance.OpenUI<UICtrlEditNpcDia>(_task.TaskBefore);
                SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>()
                    .SetNpcType(NpcDia.GetNpcType(_mainCtrl.EditData.UnitDesc.Id));
            });
            _cachedView.TaskMidBeforeBtn.onClick.AddListener(() =>
            {
                SocialGUIManager.Instance.OpenUI<UICtrlEditNpcDia>(_task.TaskMiddle);
                SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>()
                    .SetNpcType(NpcDia.GetNpcType(_mainCtrl.EditData.UnitDesc.Id));
            });
            _cachedView.TaskAfterBeforeBtn.onClick.AddListener(() =>
            {
                SocialGUIManager.Instance.OpenUI<UICtrlEditNpcDia>(_task.TaskAfter);
                int id = _mainCtrl.EditData.UnitDesc.Id;
                using (var allnpc = DataScene2D.CurScene.UnitExtras.GetEnumerator())
                {
                    while (allnpc.MoveNext())
                    {
                        if (allnpc.Current.Value.NpcSerialNumber == _task.TargetNpcSerialNumber)
                        {
                            UnitBase unit;
                            if (ColliderScene2D.CurScene.TryGetUnit(allnpc.Current.Key, out unit))
                            {
                                id = unit.Id;
                            }
                        }

                        for (int i = 0; i < _task.Targets.Count; i++)
                        {
                            if (_task.Targets.Get<NpcTaskTargetDynamic>(i).TaskType == (int) ENpcTargetType.Dialog)
                            {
                                if (allnpc.Current.Value.NpcSerialNumber ==
                                    _task.Targets.Get<NpcTaskTargetDynamic>(i).TargetNpcNum)
                                {
                                    UnitBase unit;
                                    if (ColliderScene2D.CurScene.TryGetUnit(allnpc.Current.Key, out unit))
                                    {
                                        id = unit.Id;
                                    }
                                }
                            }
                        }
                    }
                }

                SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>()
                    .SetNpcType(NpcDia.GetNpcType(id));
            });
            _cachedView.AddBeforeTaskAwardBtn.onClick.AddListener(() =>
            {
                _mainCtrl.EditBeforeTaskAward.OpenMenu(_task);
            });
            _cachedView.AddFinishTaskAwardBtn.onClick.AddListener(() =>
            {
                _mainCtrl.EditFinishTaskAward.OpenMenu(_task);
            });
            _cachedView.NpcTaskEditDiaPanelExitBtn.onClick.AddListener(Close);
            _cachedView.TargetTaskNpc.onEndEdit.AddListener((str) =>
            {
                if (NpcTaskDataTemp.Intance.NpcSerialNumberDic.ContainsKey(Convert.ToInt32(str)))
                {
                    if (NpcTaskDataTemp.Intance.NpcSerialNumberDic[Convert.ToInt32(str)])
                    {
                        _task.TargetNpcSerialNumber = (ushort) Convert.ToInt32(str);
                    }
                    else
                    {
                        _task.TargetNpcSerialNumber = _mainCtrl.EditData.UnitExtra.NpcSerialNumber;
                    }
                }
                else
                {
                    _task.TargetNpcSerialNumber = _mainCtrl.EditData.UnitExtra.NpcSerialNumber;
                }
            });
            BadWordManger.Instance.InputFeidAddListen(_cachedView.TargetTaskNpc);
        }

        public void OpenMenu(NpcTaskDynamic task)
        {
            _task = task;
            Open();
        }

        public override void Open()
        {
            base.Open();
            RefreshView();
        }

        private void RefreshTask(NpcTaskDynamic taskData = null)
        {
            if (taskData != null)
            {
                _task = taskData;
            }

            // 刷新任务前后的奖励
            for (int i = 0; i < _cachedView.BeforeTaskAward.Length; i++)
            {
                if (i < _task.BeforeTaskAward.Count)
                {
                    _beforeTaskAwardBtnGroup[i].SetEnable(true);
                    _beforeTaskAwardBtnGroup[i].SetSelectTarget(_mainCtrl.EditData.UnitDesc.Guid,
                        _task.BeforeTaskAward.ToList<NpcTaskTargetDynamic>()[i],
                        _task.BeforeTaskAward,
                        OpenAdvencePanel, () => { RefreshTask(); });
                }
                else
                {
                    _beforeTaskAwardBtnGroup[i].SetEnable(false);
                }
            }

            _cachedView.AddBeforeTaskAwardBtn.SetActiveEx((_task.BeforeTaskAward.Count !=
                                                           NpcTaskDynamic.MaxBeforeTasAwardCout));

            for (int i = 0; i < _cachedView.FinishTaskAward.Length; i++)
            {
                if (i < _task.TaskFinishAward.Count)
                {
                    _finishTaskAwardBtnGroup[i].SetEnable(true);
                    _finishTaskAwardBtnGroup[i].SetSelectTarget(_mainCtrl.EditData.UnitDesc.Guid,
                        _task.TaskFinishAward.Get<NpcTaskTargetDynamic>(i),
                        _task.TaskFinishAward,
                        OpenAdvencePanel, () => { RefreshTask(); });
                }
                else
                {
                    _finishTaskAwardBtnGroup[i].SetEnable(false);
                }
            }

            _cachedView.TargetTaskNpc.text = _task.TargetNpcSerialNumber.ToString();
            _cachedView.AddFinishTaskAwardBtn.SetActiveEx((_task.TaskFinishAward.Count !=
                                                           NpcTaskDynamic.MaxFinishTasAwardCout));
        }

        private void OpenAdvencePanel(NpcTaskTargetDynamic target)
        {
            switch ((ENpcTargetType) target.TaskType)
            {
                case ENpcTargetType.Colltion:
                    _mainCtrl.EditNpcTaskAwardColltionAdvance.OpenMenu(target);
                    break;
                case ENpcTargetType.Moster:
                    _mainCtrl.EditNpcTaskMonsterType.OpenMenu(target);
                    break;
                case ENpcTargetType.Contorl:
                    break;
                case ENpcTargetType.Dialog:

                    break;
            }
        }

        public override void RefreshView()
        {
            if (!_isOpen) return;
            RefreshTask();
        }
    }
}