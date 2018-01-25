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
    //选择npc任务种类
    public class UPCtrlUnitPropertyEditNpcTaskAdvance : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private ENpcTargetType _type;
        private GameObject _panel;
        private RectTransform _contentRtf;
        private Sequence _openSequence;
        private Sequence _closeSequence;
        private bool _openAnim;
        private bool _completeAnim;
        private NpcTaskTargetDynamic _npcTaskTarget;
        private NpcTaskDynamic _npcTaskData = new NpcTaskDynamic();
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
                    NpcTaskTargetDynamic target = CreataNewTarget((ENpcTargetType) (num + 1));
                    OpenMenu(target);
                });
            }
            _panel = _cachedView.NpcTaskTypePanel;
            _contentRtf = _cachedView.NpcTaskTypeContentRtf;
            BadWordManger.Instance.InputFeidAddListen(_cachedView.DiaTargetNpcNum);
            _panel.SetActiveEx(false);
            _cachedView.NpcTaskColltionPanel.SetActiveEx(false);
            _cachedView.NpcTaskDiaPanel.SetActiveEx(false);
            InitData();
            _collOrKillNumSetting = new USCtrlSliderSetting();
            _collOrKillNumSetting.Init(_cachedView.ColltionNumSetting);
            UnitExtraHelper.SetUSCtrlSliderSetting(_collOrKillNumSetting, EAdvanceAttribute.MaxTaskKillOrColltionNum,
                value => _npcTaskTarget.ColOrKillNum = (ushort) value);
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
            _npcTaskData = _mainCtrl.EditNpcTaskDock.CurExtraNpcTaskData;
            _panel.SetActive(false);
            switch (_type)
            {
                case ENpcTargetType.None:
                    _panel = _cachedView.NpcTaskTypePanel;
                    _contentRtf = _cachedView.NpcTaskTypeContentRtf;
                    break;
                case ENpcTargetType.Colltion:
                    _panel = _cachedView.NpcTaskColltionPanel;
                    _contentRtf = _cachedView.NpcTaskColltionContentRtf;
                    CreateCollOrKillType(ENpcTargetType.Colltion);
                    break;
                case ENpcTargetType.Moster:
                    _panel = _cachedView.NpcTaskColltionPanel;
                    _contentRtf = _cachedView.NpcTaskColltionContentRtf;
                    CreateCollOrKillType(ENpcTargetType.Moster);
                    break;
                case ENpcTargetType.Contorl:

                    break;
                case ENpcTargetType.Dialog:
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
                case ENpcTargetType.None:
                    _panel = _cachedView.NpcTaskTypePanel;
                    _contentRtf = _cachedView.NpcTaskTypeContentRtf;
                    break;
                case ENpcTargetType.Colltion:
                    _panel = _cachedView.NpcTaskColltionPanel;
                    _contentRtf = _cachedView.NpcTaskColltionContentRtf;
                    CreateCollOrKillType(ENpcTargetType.Colltion);
                    break;
                case ENpcTargetType.Moster:
                    _panel = _cachedView.NpcTaskColltionPanel;
                    _contentRtf = _cachedView.NpcTaskColltionContentRtf;
                    CreateCollOrKillType(ENpcTargetType.Moster);
                    break;
                case ENpcTargetType.Contorl:

                    break;
                case ENpcTargetType.Dialog:
                    if (_npcTaskTarget.TargetUnitID != Convert.ToInt32(_cachedView.DiaTargetNpcNum.text))
                    {
                        _npcTaskTarget.TargetUnitID = Convert.ToUInt16(_cachedView.DiaTargetNpcNum.text);
                    }
                    break;
            }

            base.Close();
        }

        public void OpenMenu(NpcTaskTargetDynamic target)
        {
            _npcTaskTarget = target;
            _type = (ENpcTargetType) _npcTaskTarget.TaskType;
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
        private void CreateCollOrKillType(ENpcTargetType type)
        {
            int num = _umList.Count;
            switch (type)
            {
                case ENpcTargetType.Colltion:

                    for (int i = 0; i < _colletionList.Count; i++)
                    {
                        if (i < num)
                        {
//                            _umList[i].IintItem(_colletionList[i], _npcTaskTarget);
                        }
                        else
                        {
                            UMCtrlHandNpcSelectTargetItem item = new UMCtrlHandNpcSelectTargetItem();
                            item.Init(_cachedView.NpcTaskColltionContentRtf.rectTransform(), EResScenary.Game);
//                            item.IintItem(_colletionList[i], _npcTaskTarget);
                            _umList.Add(item);
                        }
                    }
                    break;
                case ENpcTargetType.Moster:
                    for (int i = 0; i < _mechanismList.Count; i++)
                    {
                        if (i < num)
                        {
//                            _umList[i].IintItem(_mechanismList[i], _npcTaskTarget);
                        }
                        else
                        {
                            UMCtrlHandNpcSelectTargetItem item = new UMCtrlHandNpcSelectTargetItem();
                            item.Init(_cachedView.NpcTaskColltionContentRtf.rectTransform(), EResScenary.Game);
//                            item.IintItem(_mechanismList[i], _npcTaskTarget);
                            _umList.Add(item);
                        }
                    }
                    break;
            }
        }

        private NpcTaskTargetDynamic CreataNewTarget(ENpcTargetType type)
        {
            NpcTaskTargetDynamic target = new NpcTaskTargetDynamic();
            switch (type)
            {
                case ENpcTargetType.Moster:
                    target.ColOrKillNum = 1;
                    target.TaskType = (int) ENpcTargetType.Moster;
                    //默认的怪兽
                    target.TargetUnitID = 11000;
                    _npcTaskData.Targets.Add(target);
                    RefrewTaskData();
                    break;
                case ENpcTargetType.Colltion:
                    target.ColOrKillNum = 1;
                    target.TaskType = (int) ENpcTargetType.Colltion;
                    _npcTaskData.Targets.Add(target);
                    RefrewTaskData();
                    //默认的收集奖励
                    target.TargetUnitID = 11000;
                    break;
                case ENpcTargetType.Dialog:

                    target.TaskType = (int) ENpcTargetType.Dialog;
                    break;
                case ENpcTargetType.Contorl:
                    if (_mainCtrl.EditData.UnitDesc.Guid != IntVec3.zero)
                    {
                        target.TaskType = (int) ENpcTargetType.Contorl;
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
            _mainCtrl.EditNpcTaskDock.RefreshView();
        }
    }
}