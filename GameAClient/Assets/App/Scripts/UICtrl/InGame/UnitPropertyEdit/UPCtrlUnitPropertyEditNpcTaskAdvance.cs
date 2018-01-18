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
    public class UPCtrlUnitPropertyEditNpcTaskAdvance : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private ENpcTaskType _type;
        private GameObject _panel;
        private RectTransform _contentRtf;
        private Sequence _openSequence;
        private Sequence _closeSequence;
        private bool _openAnim;
        private bool _completeAnim;
        private UnitExtraNpcTaskTarget _npcTaskTarget;
        private UnitExtraNpcTaskData _npcTaskData = new UnitExtraNpcTaskData();
        private int _taskindex;
        private int _taskTargetindex;
        private List<int> _colletionList = new List<int>();
        private List<int> _mechanismList = new List<int>();

        private List<UMCtrlHandNpcSelectTargetItem> _umList = new List<UMCtrlHandNpcSelectTargetItem>();

        private USCtrlSliderSetting _collOrKillNumSetting;

        public USCtrlUnitNpcTaskTargetBtn[] SelectNpcTaskTargetBtns;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            SelectNpcTaskTargetBtns = new USCtrlUnitNpcTaskTargetBtn[_cachedView.SelectTargetBtnGroup.Length];
            for (int i = 0; i < SelectNpcTaskTargetBtns.Length; i++)
            {
                SelectNpcTaskTargetBtns[i] = new USCtrlUnitNpcTaskTargetBtn();
                SelectNpcTaskTargetBtns[i].Init(_cachedView.SelectTargetBtnGroup[i]);
                int num = i;
                SelectNpcTaskTargetBtns[i].AddNewTarget(() =>
                {
                    UnitExtraNpcTaskTarget target = CreataNewTarget((ENpcTaskType) (num + 1));
                    OpenMenu(target);
                });
            }
            _panel = _cachedView.NpcTaskSelectTypePanel;
            _contentRtf = _cachedView.NpcTaskSelectTypeContentRtf;
            BadWordManger.Instance.InputFeidAddListen(_cachedView.DiaTargetNpcNum);
            _panel.SetActiveEx(false);
            _cachedView.NpcTaskMonsterOrColPanel.SetActiveEx(false);
            _cachedView.NpcTaskDiaPanel.SetActiveEx(false);
            InitData();
            _collOrKillNumSetting = new USCtrlSliderSetting();
            _collOrKillNumSetting.Init(_cachedView.CollOrKillNumSetting);
            UnitExtraHelper.SetUSCtrlSliderSetting(_collOrKillNumSetting, EAdvanceAttribute.MaxTaskKillOrColltionNum,
                value => _npcTaskTarget.ColOrKillNum = (ushort) value);
        }

        public override void Open()
        {
            base.Open();
            _npcTaskData = _mainCtrl.UpCtrlUnitPropertyEditNpcTaskType.CurExtraNpcTaskData;
            _panel.SetActive(false);
            switch (_type)
            {
                case ENpcTaskType.None:
                    _panel = _cachedView.NpcTaskSelectTypePanel;
                    _contentRtf = _cachedView.NpcTaskSelectTypeContentRtf;
                    break;
                case ENpcTaskType.Colltion:
                    _panel = _cachedView.NpcTaskMonsterOrColPanel;
                    _contentRtf = _cachedView.NpcTaskMonsterOrColContentRtf;
                    CreateCollOrKillType(ENpcTaskType.Colltion);
                    break;
                case ENpcTaskType.Moster:
                    _panel = _cachedView.NpcTaskMonsterOrColPanel;
                    _contentRtf = _cachedView.NpcTaskMonsterOrColContentRtf;
                    CreateCollOrKillType(ENpcTaskType.Moster);
                    break;
                case ENpcTaskType.Contorl:

                    break;
                case ENpcTaskType.Dialog:
                    _panel = _cachedView.NpcTaskDiaPanel;
                    _contentRtf = _cachedView.NpcTaskDiaContentRtf;
                    break;
            }
            _panel.SetActive(true);
            _contentRtf.anchoredPosition = Vector2.zero;
            OpenAnimation();
            RefreshView();
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
            switch (_type)
            {
                case ENpcTaskType.None:
                    _panel = _cachedView.NpcTaskSelectTypePanel;
                    _contentRtf = _cachedView.NpcTaskSelectTypeContentRtf;
                    break;
                case ENpcTaskType.Colltion:
                    _panel = _cachedView.NpcTaskMonsterOrColPanel;
                    _contentRtf = _cachedView.NpcTaskMonsterOrColContentRtf;
                    CreateCollOrKillType(ENpcTaskType.Colltion);
                    break;
                case ENpcTaskType.Moster:
                    _panel = _cachedView.NpcTaskMonsterOrColPanel;
                    _contentRtf = _cachedView.NpcTaskMonsterOrColContentRtf;
                    CreateCollOrKillType(ENpcTaskType.Moster);
                    break;
                case ENpcTaskType.Contorl:

                    break;
                case ENpcTaskType.Dialog:
                    if (_npcTaskTarget.TargetUnitID != Convert.ToInt32(_cachedView.DiaTargetNpcNum.text))
                    {
                        _npcTaskTarget.TargetUnitID = Convert.ToUInt16(_cachedView.DiaTargetNpcNum.text);
                    }
                    break;
            }

            base.Close();
        }

        public void OpenMenu(UnitExtraNpcTaskTarget target)
        {
            _npcTaskTarget = target;
            _type = (ENpcTaskType) _npcTaskTarget.TaskType;
            Open();
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

        public void InitData()
        {
            foreach (var item in TableManager.Instance.Table_UnitDic)
            {
                switch (item.Value.UIType)
                {
                    case (int) ExplantionIndex.Collection:
                        _colletionList.Add(item.Key);
                        break;
                    case (int) ExplantionIndex.Role:
                        _mechanismList.Add(item.Key);
                        break;
                }
            }
        }

        //处理选择杀死和收集是的方法
        private void CreateCollOrKillType(ENpcTaskType type)
        {
            int num = _umList.Count;
            switch (type)
            {
                case ENpcTaskType.Colltion:

                    for (int i = 0; i < _colletionList.Count; i++)
                    {
                        if (i < num)
                        {
                            _umList[i].IintItem(_colletionList[i], _npcTaskTarget);
                        }
                        else
                        {
                            UMCtrlHandNpcSelectTargetItem item = new UMCtrlHandNpcSelectTargetItem();
                            item.Init(_cachedView.NpcTaskMonsterOrColContentRtf.rectTransform(), EResScenary.Game);
                            item.IintItem(_colletionList[i], _npcTaskTarget);
                            _umList.Add(item);
                        }
                    }
                    break;
                case ENpcTaskType.Moster:
                    for (int i = 0; i < _mechanismList.Count; i++)
                    {
                        if (i < num)
                        {
                            _umList[i].IintItem(_mechanismList[i], _npcTaskTarget);
                        }
                        else
                        {
                            UMCtrlHandNpcSelectTargetItem item = new UMCtrlHandNpcSelectTargetItem();
                            item.Init(_cachedView.NpcTaskMonsterOrColContentRtf.rectTransform(), EResScenary.Game);
                            item.IintItem(_mechanismList[i], _npcTaskTarget);
                            _umList.Add(item);
                        }
                    }
                    break;
            }
        }

        private UnitExtraNpcTaskTarget CreataNewTarget(ENpcTaskType type)
        {
            UnitExtraNpcTaskTarget target = new UnitExtraNpcTaskTarget();
            switch (type)
            {
                case ENpcTaskType.Moster:
                    target.ColOrKillNum = 1;
                    target.TaskType = (int) ENpcTaskType.Moster;
                    //默认的怪兽
                    target.TargetUnitID = 11000;
                    _npcTaskData.TaskTarget.Add(target);
                    RefrewTaskData();
                    break;
                case ENpcTaskType.Colltion:
                    target.ColOrKillNum = 1;
                    target.TaskType = (int) ENpcTaskType.Colltion;
                    _npcTaskData.TaskTarget.Add(target);
                    RefrewTaskData();
                    //默认的收集奖励
                    target.TargetUnitID = 11000;
                    break;
                case ENpcTaskType.Dialog:

                    target.TaskType = (int) ENpcTaskType.Dialog;
                    break;
                case ENpcTaskType.Contorl:
                    if (_mainCtrl.EditData.UnitDesc.Guid != IntVec3.zero)
                    {
                        target.TaskType = (int) ENpcTaskType.Contorl;
                        NpcTaskDataTemp.Intance.TaskData = _npcTaskData;
                        NpcTaskDataTemp.Intance.TaskTargetData = _npcTaskTarget;
                        NpcTaskDataTemp.Intance.TaskType = ETaskContype.Task;
                        NpcTaskDataTemp.Intance.NpcIntVec3 = _mainCtrl.EditData.UnitDesc.Guid;
                        NpcTaskDataTemp.Intance.IsEditNpcData = true;
                        _mainCtrl.Close();
                        EditMode.Instance.StartSwitch();
                    }


                    break;
            }

            return target;
        }

        //刷新任务面板
        private void RefrewTaskData()
        {
            _mainCtrl.UpCtrlUnitPropertyEditNpcTaskType.RefreshView();
        }
    }
}