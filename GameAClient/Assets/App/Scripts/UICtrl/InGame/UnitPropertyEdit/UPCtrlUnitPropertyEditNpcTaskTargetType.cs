using System;
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
        UPCtrlUnitPropertyEditNpcTaskTargetType : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
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
            _panel = _cachedView.NpcTaskTypePanel;
            _contentRtf = _cachedView.NpcTaskTypeContentRtf;
            foreach (var VARIABLE in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                _colltionList.Add(VARIABLE.Key);
            }
            foreach (var VARIABLE in TableManager.Instance.Table_NpcTaskTargetKillDic)
            {
                _killtionList.Add(VARIABLE.Key);
            }

            for (int i = 0; i < _cachedView.TargetTypeBtnGroup.Length; i++)
            {
                int index = i + 1;
                _cachedView.TargetTypeBtnGroup[i].onClick.AddListener(() => { ChooseTargetType(index); });
            }
        }

        public void ChooseTargetType(int index)
        {
            switch (index)
            {
                case (int) ENpcTargetType.Colltion:
                    //选择收集
                    _target = new NpcTaskTargetDynamic();
                    _target.TaskType = (byte) ENpcTargetType.Colltion;
                    _target.TargetUnitID =
                        (ushort) TableManager.Instance.Table_NpcTaskTargetColltionDic[_colltionList[0]].Id;
                    _mainCtrl.EditNpcTaskColltionType.OpenMenu(_target);
                    _taskDynamic.Targets.Add(_target);
                    _mainCtrl.EditNpcTaskDock.RefreshView();
                    Close();
                    break;
                case (int) ENpcTargetType.Moster:
                    //选择击杀
                    _target = new NpcTaskTargetDynamic();
                    _target.TaskType = (byte) ENpcTargetType.Moster;
                    _target.TargetUnitID =
                        (ushort) TableManager.Instance.Table_NpcTaskTargetKillDic[_killtionList[0]].Id;
                    _mainCtrl.EditNpcTaskMonsterType.OpenMenu(_target);
                    _taskDynamic.Targets.Add(_target);
                    _mainCtrl.EditNpcTaskDock.RefreshView();
                    Close();
                    break;
                case (int) ENpcTargetType.Contorl:
                    //选择控制
                    if (_mainCtrl.IsInMap)
                    {
                        //打开连线界面
                    }
                    else
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "地块不在地图中！");
                    }
                    break;
                case (int) ENpcTargetType.Dialog:
                    //选择传话
                    if (_mainCtrl.IsInMap)
                    {
                        //输入传话的目标
                        _target = new NpcTaskTargetDynamic();
                        _target.TaskType = (byte) ENpcTargetType.Dialog;
                        _taskDynamic.Targets.Add(_target);
                        _mainCtrl.EditNpcTaregtDialog.OpenMenu(_target);
                        Close();
                    }
                    else
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "地块不在地图中！");
                    }
                    break;
            }
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
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