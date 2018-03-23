using System;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlUnitPropertyEditNpcDiaType : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        public const int NamMaxLength = 14;
        public const int DiaMaxLength = 100;

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
            _cachedView.NpcName.onValueChanged.AddListener(SetNameLength);
            _cachedView.NpcDialog.onValueChanged.AddListener(SetDiALength);
            _cachedView.NpcName.onEndEdit.AddListener((string str) => { SaveData(); });
            _cachedView.NpcDialog.onEndEdit.AddListener((string str) => { SaveData(); });
            BadWordManger.Instance.InputFeidAddListen(_cachedView.NpcName);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.NpcDialog);
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
            _cachedView.NpcDiaLogDock.SetActiveEx(true);
            RefreshView();
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            if (_mainCtrl.EditData.UnitExtra.NpcShowInterval == 0)
            {
                _mainCtrl.EditData.UnitExtra.NpcShowInterval = 1;
            }
            _dialogShowIntervalTimeSetting.SetCur(_mainCtrl.EditData.UnitExtra.NpcShowInterval, false);
            string name = _mainCtrl.EditData.UnitExtra.NpcName;
            _cachedView.NpcName.text = name;
            string dia = _mainCtrl.EditData.UnitExtra.NpcDialog;
            _cachedView.NpcDialog.text = dia;
            RefreshShowTypeMenu();
        }


        public override void Close()
        {
// 关闭的时候设置名字和对话内容
            _cachedView.NpcDiaLogDock.SetActiveEx(false);
//            SaveData();
            base.Close();
        }

        private void SaveData()
        {
            if (_cachedView.NpcName.text != _mainCtrl.EditData.UnitExtra.NpcName)
            {
                _mainCtrl.EditData.UnitExtra.NpcName = _cachedView.NpcName.text;
            }
            if (_cachedView.NpcDialog.text != _mainCtrl.EditData.UnitExtra.NpcDialog)
            {
                _mainCtrl.EditData.UnitExtra.NpcDialog = _cachedView.NpcDialog.text;
            }
            _mainCtrl.EditData.UnitExtra.NpcShowInterval = (ushort) _dialogShowIntervalTimeSetting.Cur;
        }

        private void RefreshShowTypeMenu()
        {
            var val = Mathf.Clamp(_mainCtrl.EditData.UnitExtra.NpcShowType, 0, 1);
            for (int i = 0;
                i < _showTypeBtnGroup.Length;
                i++)
            {
                _showTypeBtnGroup[i].SetSelected(i == val);
            }
            if (_mainCtrl.EditData.UnitExtra.NpcShowType == (int) ENpcTriggerType.Close)
            {
                _dialogShowIntervalTimeSetting.SetEnable(false);
            }
            else
            {
                _dialogShowIntervalTimeSetting.SetEnable(true);
            }
        }

        private void SetNameLength(string str)
        {
            if (str == null)
            {
                return;
            }
            _cachedView.NameTextNum.text = String.Format("{0}/{1}", GameATools.GetStrLength(str), NamMaxLength);
            if (GameATools.GetStrLength(str) > NamMaxLength)
            {
                _cachedView.NpcName.text = GameATools.GetMaxLengthStr(str, NamMaxLength);
            }
        }

        private void SetDiALength(string str)
        {
            if (str == null)
            {
                return;
            }
            _cachedView.DiaTextNum.text = String.Format("{0}/{1}", GameATools.GetStrLength(str), DiaMaxLength);
            if (GameATools.GetStrLength(str) > DiaMaxLength)
            {
                _cachedView.NpcDialog.text = GameATools.GetMaxLengthStr(str, DiaMaxLength);
            }
        }
    }
}