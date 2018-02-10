using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.Home)]
    public class UICtrlInfoNotificationRaw : UICtrlResManagedBase<UIViewInfoNotificationRaw>
    {
        private bool _openState;
        private Sequence _openSequence;
        private Sequence _closeSequence;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainFrame;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.UnfoldBtn.onClick.AddListener(OnUnfoldBtn);
            _cachedView.FoldBtn.onClick.AddListener(OnFoldBtn);
            _cachedView.InfoBtn.onClick.AddListener(OnInfoBtn);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            SetOpen(false);
        }

        private void SetOpen(bool value)
        {
            _openState = value;
            if (_openState)
            {
                OpenAnimation();
            }
            else
            {
                CloseAnimation();
            }
        }

        private void OnInfoBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlInfoNotification>();
        }

        private void OnUnfoldBtn()
        {
            SetOpen(true);
        }

        private void OnFoldBtn()
        {
            SetOpen(false);
        }

        private void OpenAnimation()
        {
            if (null == _openSequence)
            {
                CreateSequences();
            }

            if (_closeSequence.IsPlaying())
            {
                _closeSequence.Complete(true);
            }

            _cachedView.OpenPannelRtf.SetActiveEx(true);
            _cachedView.UnfoldBtn.SetActiveEx(false);
            _openSequence.Restart();
        }

        private void CloseAnimation()
        {
            if (null == _closeSequence)
            {
                CreateSequences();
            }

            if (_openSequence.IsPlaying())
            {
                _openSequence.Complete(true);
            }

            _closeSequence.PlayForward();
        }

        private void CreateSequences()
        {
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            _openSequence.Append(
                _cachedView.OpenPannelRtf.DOBlendableMoveBy(Vector3.right * 220, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause();
            _closeSequence.Append(_cachedView.OpenPannelRtf.DOBlendableMoveBy(Vector3.right * 220, 0.3f)
                .SetEase(Ease.InOutQuad)).OnComplete(OnAnimComplete).SetAutoKill(false).Pause();
        }

        private void OnAnimComplete()
        {
            _cachedView.OpenPannelRtf.SetActiveEx(false);
            _cachedView.UnfoldBtn.SetActiveEx(true);
            _closeSequence.Rewind();
        }
    }
}