using DG.Tweening;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlUnitPropertyEditNpcTaskTypeAdvance : UpCtrlNpcAdvanceBase
    {
        private UnitExtraNpcTaskTarget _target;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskTypePanel;
            for (int i = 0; i < _cachedView.TargetTypeBtnGroup.Length; i++)
            {
                int index = i + 1;
                _cachedView.TargetTypeBtnGroup[i].onClick.AddListener(() =>
                {
                    _target.TaskType = index;
                    Close();
                });
            }
        }

        public void OpenMenu(UnitExtraNpcTaskTarget target)
        {
            _target = target;
            Open();
        }

        public override void Open()
        {
            base.Open();
            OpenAnimation();
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