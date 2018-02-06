using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class
        //npc收集的设置
        UPCtrlUnitPropertyEditNpcTaskColltionAdvance : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private GameObject _panel;
        private Sequence _openSequence;
        private Sequence _closeSequence;
        private bool _openAnim;
        private bool _completeAnim;
        private NpcTaskTargetDynamic _target;
        private List<UMCtrlHandNpcSelectTargetItem> _umList = new List<UMCtrlHandNpcSelectTargetItem>();
        private List<int> _idList = new List<int>();
        private USCtrlSliderSetting _colltionNumSetting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskColltionPanel;
            foreach (var VARIABLE in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                _idList.Add(VARIABLE.Key);
            }
            for (int i = 0;
                i < _idList.Count;
                i++)
            {
                int index = i;
                UMCtrlHandNpcSelectTargetItem item = new UMCtrlHandNpcSelectTargetItem();
                item.Init(_cachedView.ColltionItemContent, EResScenary.Game);
                item.IintItem(_idList[index], _target, RefreshBtnGroup);
                _umList.Add(item);
            }

            _colltionNumSetting = new USCtrlSliderSetting();
            _colltionNumSetting.Init(_cachedView.ColltionNumSetting);
            UnitExtraHelper.SetUSCtrlSliderSetting(_colltionNumSetting, EAdvanceAttribute.MaxTaskKillOrColltionNum,
                value => _target.ColOrKillNum = (ushort) value);
            _cachedView.ColltionDownBtn.onClick.AddListener(() => { _cachedView.ColltionMBar.value -= 0.1f; });
            _cachedView.ColltionUpBtn.onClick.AddListener(() => { _cachedView.ColltionMBar.value += 0.1f; });
            _cachedView.NpcTaskColltionPanelExitBtn.onClick.AddListener(Close);
        }

        private void RefreshBtnGroup()
        {
            for (int i = 0; i < _umList.Count; i++)
            {
                _umList[i].SetSelected(_umList[i].UnitId == _target.TargetUnitID);
            }
        }

        public void OpenMenu(NpcTaskTargetDynamic target)
        {
            _target = target;
            Open();
            for (int i = 0;
                i < _idList.Count;
                i++)
            {
                int index = i;
                _umList[i].IintItem(_idList[index], _target, RefreshBtnGroup);
            }

            RefreshBtnGroup();
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
            OpenAnimation();
            _colltionNumSetting.SetCur(_target.ColOrKillNum);
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