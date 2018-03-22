using System.Collections.Generic;
using GameA.Game;

namespace GameA
{
    public class
        //npc收集的设置
        UPCtrlUnitPropertyEditNpcTaskColltionAdvance : UpCtrlNpcAdvanceBase
    {
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
    }
}