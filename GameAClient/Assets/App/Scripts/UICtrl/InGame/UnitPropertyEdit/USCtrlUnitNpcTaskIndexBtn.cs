using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.Events;

namespace GameA
{
    public class USCtrlUnitNpcTaskIndexBtn : USCtrlBase<USViewUnitNpcTaskIndexBtn>
    {
        private UnitExtraNpcTaskData _taskData;

        public USViewUnitNpcTaskIndexBtn View
        {
            get { return _cachedView; }
        }

        public void SetSelectTask(UnitExtraNpcTaskData taskdata, SeletTaskType task)
        {
            _taskData = taskdata;
            _cachedView.ChoseBtn.onClick.AddListener(() => { task.Invoke(_taskData); });
        }

        public void SetText2(string text)
        {
            USViewUnitNpcTaskIndexBtn btn = _cachedView as USViewUnitNpcTaskIndexBtn;
            if (btn != null)
            {
                DictionaryTools.SetContentText(btn.IndexNum, text);
            }
        }

        public void SetSelected(bool selected)
        {
            _cachedView.ChoseBtn.interactable = !selected;
        }

        public void SetEnable(bool enable)
        {
            _cachedView.SetActiveEx(enable);
        }
    }
}