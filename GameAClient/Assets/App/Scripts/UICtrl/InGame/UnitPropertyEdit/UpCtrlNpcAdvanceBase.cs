using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UpCtrlNpcAdvanceBase : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        protected GameObject _panel;
        protected Sequence _openSequence;
        protected Sequence _closeSequence;
        protected bool _openAnim;
        protected bool _completeAnim;


        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
            OpenAnimation();
        }

        public virtual void RefreshView()
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

        protected void OpenAnimation()
        {
            if (null == _openSequence)
            {
                CreateSequences();
            }

            _closeSequence.Complete(true);

            _openSequence.Restart();
            _openAnim = true;
        }

        protected void CloseAnimation()
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

        protected void CreateSequences()
        {
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            _openSequence.Append(
                _panel.transform.DOBlendableLocalMoveBy(Vector3.left * 600, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause().PrependCallback(() =>
            {
                if (_closeSequence.IsPlaying())
                {
                    _closeSequence.Complete(true);
                }

                _panel.SetActiveEx(true);
            });
            _closeSequence.Append(_panel.transform.DOBlendableLocalMoveBy(Vector3.left * 600, 0.3f)
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

        protected void OnCloseAnimationComplete()
        {
            _panel.SetActiveEx(false);
            _closeSequence.Rewind();
        }
    }
}