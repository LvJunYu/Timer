using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{ // npc 编辑对话
    public class
        UPCtrlUnitPropertyEditNpcTaskAddCondition : UpCtrlNpcAdvanceBase
    {
        private RectTransform _contentRtf;

        public RectTransform ContentRtf
        {
            get { return _contentRtf; }
            set { _contentRtf = value; }
        }

        private NpcTaskDynamic _taskData = new NpcTaskDynamic();
        private USCtrlSliderSetting _killNumSetting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskAddConditionPanel;
            _contentRtf = _cachedView.NpcTaskAddConditionContentRtf;
            _cachedView.TriggerCondtionBtn.onClick.AddListener(() =>
            {
                if (_taskData.TriggerType != (int) TrrigerTaskType.None)
                {
                    Close();
                }
                else
                {
                    _mainCtrl.EditNpcConditionType.OpenMenu(_taskData);
                    Close();
                }
            });
            _cachedView.TaskTimeLimit.onClick.AddListener(() =>
            {
                if (_taskData.TaskimeLimit > 0)
                {
                    Close();
                }
                else
                {
                    _taskData.TaskimeLimit = (ushort) UnitExtraHelper.GetMin(EAdvanceAttribute.MaxTaskTimeLimit);
                    _mainCtrl.EditNpcTaskDock.RefreshTask();
                    Close();
                }
            });
            _cachedView.NpcTaskAddConditionPanelExitBtn.onClick.AddListener(Close);
        }

        public void OpenMenu(NpcTaskDynamic taskData)
        {
            _taskData = taskData;
            Open();
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            OpenAnimation();
            base.Open();
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