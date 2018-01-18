using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.Events;

namespace GameA
{
    public class USCtrlUnitNpcTaskTargetBtn : USCtrlBase<USViewUnitNpcTaskTarget>
    {
        private Sprite _sprite;
        private int _targetIndex;
        private ENpcTaskType _type;
        private UnitExtraNpcTaskTarget _targetData;
        private UnitExtraNpcTaskData _taskData;

        public USViewUnitNpcTaskTarget View
        {
            get { return _cachedView; }
        }

        public void SetSelectTarget(UnitExtraNpcTaskTarget targetData, UnitExtraNpcTaskData taskData,
            SeletTaskTargetType callback, UnityAction refresh)
        {
            _targetData = targetData;
            _taskData = taskData;
            _cachedView.ChoseBtn.onClick.AddListener(() => { callback.Invoke(targetData); });
            _cachedView.DelteBtn.onClick.AddListener(() =>
            {
                taskData.TaskTarget.Remove(targetData);
                refresh.Invoke();
            });
        }

        public void AddNewTarget(UnityAction callback)
        {
            _cachedView.ChoseBtn.onClick.AddListener(callback);
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