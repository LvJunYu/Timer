using System;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlUnitPropertyEditNpcTask : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
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

        //和对话有关的

        private USCtrlSliderSetting _dialogShowIntervalTimeSetting;
        private USCtrlUnitPropertyEditButton _npcCloseToShowBtn;
        private USCtrlUnitPropertyEditButton _npcIntervalShowBtn;

        private EMenu _curMenu;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AdvancePannel.SetActive(false);
            _dialogShowIntervalTimeSetting = new USCtrlSliderSetting();
            _dialogShowIntervalTimeSetting.Init(_cachedView.DialogShowIntervalTimeSetting);

            UnitExtraHelper.SetUSCtrlSliderSetting(_dialogShowIntervalTimeSetting, EAdvanceAttribute.NpcIntervalTiem,
                value => _mainCtrl.EditData.UnitExtra.NpcShowInterval = (ushort) value);

            _npcCloseToShowBtn = new USCtrlUnitPropertyEditButton();
            _npcCloseToShowBtn.Init(_cachedView.NpcCloseToShowBtn);
            _npcCloseToShowBtn.AddClickListener(() =>
            {
                _mainCtrl.EditData.UnitExtra.NpcShowType = (ushort) ENpcTriggerType.Close;
            });

            _npcIntervalShowBtn = new USCtrlUnitPropertyEditButton();
            _npcIntervalShowBtn.Init(_cachedView.NpcIntervalShowBtn);
            _npcIntervalShowBtn.AddClickListener(() =>
            {
                _mainCtrl.EditData.UnitExtra.NpcShowType = (ushort) ENpcTriggerType.Interval;
            });
            BadWordManger.Instance.InputFeidAddListen(_cachedView.NpcName);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.NpcDialog);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.AdvancePannel.SetActive(true);
            _cachedView.AdvanceContentRtf.anchoredPosition = Vector2.zero;
            OpenAnimation();
            RefreshView();
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            var id = _mainCtrl.EditData.UnitDesc.Id;
            var table = TableManager.Instance.GetUnit(id);
        }

        public override void Close()
        {
            if (_openAnim)
            {
                CloseAnimation();
            }
            else if (_closeSequence == null || !_closeSequence.IsPlaying())
            {
                _cachedView.AdvancePannel.SetActiveEx(false);
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
            _curMenu = eMenu;
            Open();
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