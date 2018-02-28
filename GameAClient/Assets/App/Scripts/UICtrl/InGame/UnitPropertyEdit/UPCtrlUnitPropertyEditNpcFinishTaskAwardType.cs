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
        //任务后的奖励的选择
        UPCtrlUnitPropertyEditNpcFinishTaskAwardType : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private GameObject _panel;
        private Sequence _openSequence;
        private Sequence _closeSequence;
        private bool _openAnim;
        private bool _completeAnim;
        private NpcTaskDynamic _taskDynamic;
        private NpcTaskTargetDynamic _target;
        private List<int> _colltionList = new List<int>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskFinishAwardTypePanel;
            foreach (var VARIABLE in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                _colltionList.Add(VARIABLE.Key);
            }
            for (int i = 0; i < _cachedView.FinishAwardTypeTypeBtnGroup.Length; i++)
            {
                int index = i;
                switch (index)
                {
                    case 0:
                        index = (int) ENpcTargetType.Colltion;
                        break;
                    case 1:
                        index = (int) ENpcTargetType.Contorl;
                        break;
                }
                _cachedView.FinishAwardTypeTypeBtnGroup[i].onClick.AddListener(() => { ChooseTargetType(index); });
            }
            _cachedView.NpcTaskFinishAwardTypePanelExitBtn.onClick.AddListener(Close);
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
                    _taskDynamic.TaskFinishAward.Add(_target);
                    _target.ColOrKillNum = 1;
                    _mainCtrl.EditNpcTaskDock.RefreshView();
                    Close();
                    _mainCtrl.EditNpcTaskColltionType.OpenMenu(_target);
                    break;

                case (int) ENpcTargetType.Contorl:
                    //选择控制
                    if (_mainCtrl.IsInMap)
                    {
                        //打开连线界面
                        NpcTaskDataTemp.Intance.StartEditTargetControl(_taskDynamic,
                            _mainCtrl.EditData.UnitDesc.Guid, ETaskContype.AfterTask, _mainCtrl.EditData.UnitExtra,_mainCtrl.EditData);
                        _mainCtrl.OnCloseBtnClick();
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