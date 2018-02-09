using System;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{ // npc 触发任务
    public class
        UPCtrlUnitPropertyEditNpcTaskBeforeTask : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private GameObject _panel;
        private Sequence _openSequence;
        private Sequence _closeSequence;
        private bool _openAnim;
        private bool _completeAnim;
        private NpcTaskDynamic _target;
        private USCtrlSliderSetting _killNumSetting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskBeforeConditionTypePanel;
            BadWordManger.Instance.InputFeidAddListen(_cachedView.BeforeConditionInputField);
            _cachedView.BeforeConditionInputField.onEndEdit.AddListener((str) =>
            {
                if (str.Length > 0)
                {
                    int a = Int16.Parse(str);
                    ushort b = (ushort) a;
                    _target.TriggerTaskNumber = b;
                    Debug.Log(_target.TriggerTaskNumber);
                }
            });
            _cachedView.BeforeConditionInputField.onValueChanged.AddListener(
                (str) =>
                {
                    if (str.Length > 2)
                    {
                        _cachedView.BeforeConditionInputField.text = str.Substring(0, 2);
                    }
                });

            _cachedView.NpcTaskBeforeConditionTypePanelExitBtn.onClick.AddListener(Close);
        }

        public void OpenMenu(NpcTaskDynamic target)
        {
            _target = target;
            Open();
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
            OpenAnimation();
            if (_target.TriggerTaskNumber == 0)
            {
                _cachedView.BeforeConditionInputField.text = "";
            }
            else
            {
                _cachedView.BeforeConditionInputField.text = _target.TriggerTaskNumber.ToString();
            }
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