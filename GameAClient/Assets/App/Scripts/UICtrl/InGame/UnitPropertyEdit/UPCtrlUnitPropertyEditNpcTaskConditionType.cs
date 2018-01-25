﻿using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace GameA
{
    public class
        //任务目标的选择
        UPCtrlUnitPropertyEditNpcTaskConditionType : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private GameObject _panel;
        private RectTransform _contentRtf;
        private Sequence _openSequence;
        private Sequence _closeSequence;
        private bool _openAnim;
        private bool _completeAnim;
        private NpcTaskDynamic _taskDynamic;
        private NpcTaskTargetDynamic _target;
        private List<int> _colltionList = new List<int>();
        private List<int> _killtionList = new List<int>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskConditionTypePanel;
            _contentRtf = _cachedView.NpcTaskConditionTypeContentRtf;
            foreach (var VARIABLE in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                _colltionList.Add(VARIABLE.Key);
            }
            foreach (var VARIABLE in TableManager.Instance.Table_NpcTaskTargetKillDic)
            {
                _killtionList.Add(VARIABLE.Key);
            }

            for (int i = 0; i < _cachedView.CondtionTypeBtnGroup.Length; i++)
            {
                int index = i + 1;
                _cachedView.CondtionTypeBtnGroup[i].onClick.AddListener(() => { ChooseTargetType(index); });
            }
        }

        public void ChooseTargetType(int index)
        {
            switch (index)
            {
                case (int) TrrigerTaskType.Kill:
                    //选择击杀
                    _taskDynamic.TriggerType = (int) TrrigerTaskType.Kill;
                    NpcTaskTargetDynamic targetData = new NpcTaskTargetDynamic();
                    targetData.TaskType = (byte) ENpcTargetType.Moster;
                    targetData.TargetUnitID = (ushort) _killtionList[0];
                    _taskDynamic.TriggerTask = targetData;
                    _mainCtrl.EditNpcTaskMonsterType.OpenMenu(_taskDynamic.TriggerTask);
                    _mainCtrl.EditNpcTaskDock.RefreshTask();
                    Close();
                    break;
                case (int) TrrigerTaskType.Colltion:
                    //选择收集
                    _taskDynamic.TriggerType = (int) TrrigerTaskType.Colltion;
                    NpcTaskTargetDynamic targetCol = new NpcTaskTargetDynamic();
                    targetCol.TaskType = (byte) ENpcTargetType.Colltion;
                    targetCol.TargetUnitID = (ushort) _colltionList[0];
                    _taskDynamic.TriggerTask = targetCol;
                    _mainCtrl.EditNpcTaskMonsterType.OpenMenu(_taskDynamic.TriggerTask);
                    _mainCtrl.EditNpcTaskDock.RefreshTask();
                    Close();
                    break;
                case (int) TrrigerTaskType.FinishOtherTask:
                    //选择其他的任务作为触发任务
                    _taskDynamic.TriggerType = (int) TrrigerTaskType.FinishOtherTask;
                    _mainCtrl.EditBeforeTask.OpenMenu(_taskDynamic);
                    break;
            }
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
            _mainCtrl.EditNpcTaskColltionType.Close();
            _mainCtrl.EditNpcTaskMonsterType.Close();
            OpenAnimation();
        }

        public void OpenMenu(NpcTaskDynamic taskData)
        {
            _taskDynamic = taskData;
            Open();
        }

        public void RefreshView()
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