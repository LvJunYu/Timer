using System;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlUnitPropertyEditNpcDiaType : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        public enum EMenu
        {
            Dialog,
            Task
        }

        private bool _hasChanged;
        private bool _openAnim;
        private bool _completeAnim;

        //和对话有关的

        private USCtrlSliderSetting _dialogShowIntervalTimeSetting;
        private USCtrlUnitPropertyEditButton _npcCloseToShowBtn;
        private USCtrlUnitPropertyEditButton _npcIntervalShowBtn;

        private USCtrlUnitPropertyEditButton[] _showTypeBtnGroup;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AdvancePannel.SetActive(false);
            //显示时间的调整            
            _dialogShowIntervalTimeSetting = new USCtrlSliderSetting();
            _dialogShowIntervalTimeSetting.Init(_cachedView.DialogShowIntervalTimeSetting);
            UnitExtraHelper.SetUSCtrlSliderSetting(_dialogShowIntervalTimeSetting, EAdvanceAttribute.NpcIntervalTiem,
                value => _mainCtrl.EditData.UnitExtra.NpcShowInterval = (ushort) value);

            //显示类型选择
            _showTypeBtnGroup = new USCtrlUnitPropertyEditButton[(int) ENpcTriggerType.Max];
            _npcCloseToShowBtn = new USCtrlUnitPropertyEditButton();
            _npcCloseToShowBtn.Init(_cachedView.NpcCloseToShowBtn);
            _npcCloseToShowBtn.AddClickListener(() =>
            {
                _mainCtrl.EditData.UnitExtra.NpcShowType = (byte) ENpcTriggerType.Close;
                RefreshShowTypeMenu();
            });
            _showTypeBtnGroup[(int) ENpcTriggerType.Close] = _npcCloseToShowBtn;
            _npcIntervalShowBtn = new USCtrlUnitPropertyEditButton();
            _npcIntervalShowBtn.Init(_cachedView.NpcIntervalShowBtn);
            _npcIntervalShowBtn.AddClickListener(() =>
            {
                _mainCtrl.EditData.UnitExtra.NpcShowType = (byte) ENpcTriggerType.Interval;
                RefreshShowTypeMenu();
            });
            _showTypeBtnGroup[(int) ENpcTriggerType.Interval] = _npcIntervalShowBtn;

            //text加检测
            BadWordManger.Instance.InputFeidAddListen(_cachedView.NpcName);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.NpcDialog);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.NpcDiaLogDock.SetActiveEx(true);
            RefreshView();
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            _dialogShowIntervalTimeSetting.SetCur(_mainCtrl.EditData.UnitExtra.NpcShowInterval);
            _cachedView.NpcName.text = _mainCtrl.EditData.UnitExtra.NpcName;
            _cachedView.NpcDialog.text = _mainCtrl.EditData.UnitExtra.NpcDialog;
            _cachedView.TaskNpcName.text = _mainCtrl.EditData.UnitExtra.NpcName;
        }


        public override void Close()
        {
            // 关闭的时候设置名字和对话内容
            if (_cachedView.NpcName.text != _mainCtrl.EditData.UnitExtra.NpcName)
            {
                _mainCtrl.EditData.UnitExtra.NpcName = _cachedView.NpcName.text;
            }
            if (_cachedView.NpcDialog.text != _mainCtrl.EditData.UnitExtra.NpcDialog)
            {
                _mainCtrl.EditData.UnitExtra.NpcDialog = _cachedView.NpcDialog.text;
            }
            _cachedView.NpcDiaLogDock.SetActiveEx(false);
            base.Close();
        }

        private void RefreshShowTypeMenu()
        {
            var val = Mathf.Clamp(_mainCtrl.EditData.UnitExtra.NpcShowType - 1, 0, 1);
            for (int i = 0;
                i < _showTypeBtnGroup.Length;
                i++)

            {
                _showTypeBtnGroup[i].SetSelected(i == val);
            }
        }
    }
}