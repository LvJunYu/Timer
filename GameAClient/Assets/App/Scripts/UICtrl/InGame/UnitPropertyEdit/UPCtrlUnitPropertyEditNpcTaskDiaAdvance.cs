using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{ // npc 编辑对话
    public class
        UpCtrlUnitPropertyEditNpcTaskDiaAdvance : UpCtrlNpcAdvanceBase
    {
        private NpcTaskTargetDynamic _target = new NpcTaskTargetDynamic();
        private string _tipContent = "这个NPC还没创建，设置无效!";

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskDiaPanel;
            BadWordManger.Instance.InputFeidAddListen(_cachedView.DiaTargetNpcNum);
            _cachedView.DiaTargetNpcNum.onValueChanged.AddListener(str =>
            {
                if (str.Length > 0)
                {
                    _cachedView.NoNpcTipText.text = "";
                    ushort num = (ushort) int.Parse(str);
                    if (NpcTaskDataTemp.Intance.HavetNpcSerialNum(num))
                    {
                        _target.TargetNpcNum = num;
                    }
                    else
                    {
                        _target.TargetNpcNum = num;
                        _cachedView.NoNpcTipText.text = _tipContent;
                    }
                }
            });
//            _cachedView.DiaTargetNpcNum.onEndEdit.AddListener(str =>
//            {
//                CheckTargetType(_mainCtrl.EditNpcTaskDock.CurExtraNpcTaskData);
//                _mainCtrl.EditNpcTaskDock.RefreshTask();
//            });
            _cachedView.NpcTaskDiaPanelExitBtn.onClick.AddListener(
                () =>
                {
                    if (_isOpen)
                    {
                        CheckTargetType(_mainCtrl.EditNpcTaskDock.CurExtraNpcTaskData);
                        _mainCtrl.EditNpcTaskDock.RefreshTask();
                    }

                    Close();
                }
            );
        }

        public void OpenMenu(NpcTaskTargetDynamic target)
        {
            _target = target;
            Open();
            _cachedView.DiaTargetNpcNum.text = _target.TargetNpcNum.ToString();
            _cachedView.NoNpcTipText.text = "";
        }

        public void CheckTargetType(NpcTaskDynamic task)
        {
            DictionaryListObject targets = new DictionaryListObject();
            if (task == null)
            {
                return;
            }

            if (task.Targets == null)
            {
                return;
            }

            for (int i = 0; i < task.Targets.Count; i++)
            {
                if (task.Targets.Get<NpcTaskTargetDynamic>(i).TaskType == (int) ENpcTargetType.Dialog)
                {
                    targets.Clear();
                    if (NpcTaskDataTemp.Intance.HavetNpcSerialNum(
                        task.Targets.Get<NpcTaskTargetDynamic>(i).TargetNpcNum))
                    {
                        targets.Add(task.Targets.Get<NpcTaskTargetDynamic>(i));
                    }

                    task.Targets = targets;
                    break;
                }
                else
                {
                    targets.Add(task.Targets.Get<NpcTaskTargetDynamic>(i));
                }
            }
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
            OpenAnimation();
        }

        public override void RefreshView()
        {
            if (!_isOpen) return;
        }

        public override void Close()
        {
            if (_openAnim)
            {
                CloseAnimation();
            }
            else if (_closeSequence == null || !_closeSequence.IsPlaying())
            {
                _panel.SetActiveEx(false);
            }

            base.Close();
        }
    }
}