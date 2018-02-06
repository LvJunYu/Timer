using System;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{ // npc 编辑对话
    public class
        UPCtrlUnitPropertyEditNpcTaskEditDia : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private GameObject _panel;
        private Sequence _openSequence;
        private Sequence _closeSequence;
        private bool _openAnim;
        private bool _completeAnim;
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
                SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>()
                    .SetNpcType(NpcDia.GetNpcType(_mainCtrl.EditData.UnitDesc.Id));
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
                    _task.TargetNpcSerialNumber = (ushort) Convert.ToInt32(str);
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
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
            OpenAnimation();
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
                    _beforeTaskAwardBtnGroup[i].SetSelectTarget(
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
                    _finishTaskAwardBtnGroup[i].SetSelectTarget(
                        _task.TaskFinishAward.Get<NpcTaskTargetDynamic>(i),
                        _task.TaskFinishAward,
                        OpenAdvencePanel, () => { RefreshTask(); });
                }
                else
                {
                    _finishTaskAwardBtnGroup[i].SetEnable(false);
                }
            }
            _cachedView.AddFinishTaskAwardBtn.SetActiveEx((_task.TaskFinishAward.Count !=
                                                           NpcTaskDynamic.MaxFinishTasAwardCout));
        }

        private void OpenAdvencePanel(NpcTaskTargetDynamic target)
        {
            switch ((ENpcTargetType) target.TaskType)
            {
                case ENpcTargetType.Colltion:
                    _mainCtrl.EditNpcTaskColltionType.OpenMenu(target);
                    break;
                case ENpcTargetType.Moster:
                    _mainCtrl.EditNpcTaskColltionType.OpenMenu(target);
                    break;
                case ENpcTargetType.Contorl:
                    break;
                case ENpcTargetType.Dialog:

                    break;
            }
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            RefreshTask();
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

        private void OpenAnimation()
        {
            if (null == _openSequence)
            {
                CreateSequences();
            }
            _closeSequence.Complete(true);
            _openSequence.Restart();
            _openAnim = true;
        }

        private void CloseAnimation()
        {
            if (null == _closeSequence)
            {
                CreateSequences();
            }

            if (_completeAnim)
            {
                _closeSequence.Complete(true);
                _completeAnim = false;
            }
            else
            {
                _closeSequence.PlayForward();
            }

            _openAnim = false;
        }

        private void CreateSequences()
        {
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            _openSequence.Append(
                _panel.transform.DOBlendableMoveBy(Vector3.left * 600, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause().PrependCallback(() =>
            {
                if (_closeSequence.IsPlaying())
                {
                    _closeSequence.Complete(true);
                }
                _panel.SetActiveEx(true);
            });
            _closeSequence.Append(_panel.transform.DOBlendableMoveBy(Vector3.left * 600, 0.3f)
                    .SetEase(Ease.InOutQuad)).OnComplete(OnCloseAnimationComplete).SetAutoKill(false).Pause()
                .PrependCallback(() =>
                {
                    if (_openSequence.IsPlaying())
                    {
                        _openSequence.Complete(true);
                    }
                });
        }

        public void CheckClose()
        {
            if (_isOpen)
            {
                _completeAnim = true;
                Close();
            }
        }

        private void OnCloseAnimationComplete()
        {
            _panel.SetActiveEx(false);
            _closeSequence.Rewind();
        }
    }
}