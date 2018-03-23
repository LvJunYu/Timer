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
        //npc怪兽的设置
        UPCtrlUnitPropertyEditNpcTaskMonsterAdvance : UpCtrlNpcAdvanceBase
    {
        private NpcTaskTargetDynamic _target;
        private List<UMCtrlHandNpcSelectTargetItem> _umList = new List<UMCtrlHandNpcSelectTargetItem>();
        private List<int> _idList = new List<int>();
        private USCtrlSliderSetting _killNumSetting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskMonsterPanel;
            foreach (var VARIABLE in TableManager.Instance.Table_NpcTaskTargetKillDic)
            {
                _idList.Add(VARIABLE.Key);
            }

            for (int i = 0;
                i < _idList.Count;
                i++)
            {
                int index = i;
                UMCtrlHandNpcSelectTargetItem item = new UMCtrlHandNpcSelectTargetItem();
                item.Init(_cachedView.ItemContent, EResScenary.Game);
                item.IintItem(_idList[index], _target, RefreshBtnGroup);
                _umList.Add(item);
            }

            _killNumSetting = new USCtrlSliderSetting();
            _killNumSetting.Init(_cachedView.KillNumSetting);
            UnitExtraHelper.SetUSCtrlSliderSetting(_killNumSetting, EAdvanceAttribute.MaxTaskKillOrColltionNum,
                value => _target.ColOrKillNum = (ushort) value);
            _cachedView.MonsterDownBtn.onClick.AddListener(() => { _cachedView.MonsterBar.value -= 0.1f; });
            _cachedView.MonsterUpBtn.onClick.AddListener(() => { _cachedView.MonsterBar.value += 0.1f; });
            _cachedView.NpcTaskMonsterPanelExitBtn.onClick.AddListener(Close);
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

            _killNumSetting.SetCur(_target.ColOrKillNum);

            RefreshBtnGroup();
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
            OpenAnimation();
        }

        public override void RefreshView()
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
    }
}