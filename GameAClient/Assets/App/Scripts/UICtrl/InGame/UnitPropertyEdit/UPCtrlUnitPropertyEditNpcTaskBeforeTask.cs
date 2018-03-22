using System;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{ // npc 触发任务
    public class
        UPCtrlUnitPropertyEditNpcTaskBeforeTask : UpCtrlNpcAdvanceBase
    {
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
    }
}