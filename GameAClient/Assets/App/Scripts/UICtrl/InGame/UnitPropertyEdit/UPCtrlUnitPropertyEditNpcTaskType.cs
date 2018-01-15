using System;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlUnitPropertyEditNpcTaskType : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        public enum EMenu
        {
            Dialog,
            Task
        }

        private Sequence _openSequence;
        private Sequence _closeSequence;
        private bool _hasChanged;
        private bool _openAnim;
        private bool _completeAnim;

        //和任务有关的

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            BadWordManger.Instance.InputFeidAddListen(_cachedView.TaskNpcName);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.NpcTaskDock.SetActiveEx(true);
            OpenAnimation();
            RefreshView();
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            _cachedView.TaskNpcName.text = _mainCtrl.EditData.UnitExtra.NpcName;
        }


        public override void Close()
        {
            _cachedView.NpcTaskDock.SetActiveEx(false);
            if (_openAnim)
            {
                CloseAnimation();
            }
            else if (_closeSequence == null || !_closeSequence.IsPlaying())
            {
            }
            // 关闭的时候设置名字和对话内容
            if (_cachedView.NpcName.text != _mainCtrl.EditData.UnitExtra.NpcName)
            {
                _mainCtrl.EditData.UnitExtra.NpcName = _cachedView.NpcName.text;
            }
            if (_cachedView.NpcDialog.text != _mainCtrl.EditData.UnitExtra.NpcDialog)
            {
                _mainCtrl.EditData.UnitExtra.NpcDialog = _cachedView.NpcDialog.text;
            }
            if (_cachedView.TaskNpcName.text != _mainCtrl.EditData.UnitExtra.NpcName)
            {
                _mainCtrl.EditData.UnitExtra.NpcName = _cachedView.TaskNpcName.text;
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
                _cachedView.AdvancePannel.transform.DOBlendableMoveBy(Vector3.left * 600, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause().PrependCallback(() =>
            {
                if (_closeSequence.IsPlaying())
                {
                    _closeSequence.Complete(true);
                }
            });
            _closeSequence.Append(_cachedView.AdvancePannel.transform.DOBlendableMoveBy(Vector3.left * 600, 0.3f)
                    .SetEase(Ease.InOutQuad)).OnComplete(OnCloseAnimationComplete).SetAutoKill(false).Pause()
                .PrependCallback(() =>
                {
                    if (_openSequence.IsPlaying())
                    {
                        _openSequence.Complete(true);
                    }
                });
        }

        public void OpenMenu(EMenu eMenu)
        {
            RefreshView();
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
        }

        public void OnChildIdChanged()
        {
        }
    }
}