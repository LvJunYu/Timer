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
        public NpcTaskTargetDynamic TargetData { get; private set; }
        public NpcTaskDynamic TaskData { get; private set; }

        public USViewUnitNpcTaskTarget View
        {
            get { return _cachedView; }
        }

        public void SetSelectTarget(NpcTaskTargetDynamic targetData, NpcTaskDynamic taskData,
            SeletTaskTargetType callback, UnityAction refresh)
        {
            TargetData = targetData;
            TaskData = taskData;
            _cachedView.ChoseBtn.onClick.AddListener(() => { callback.Invoke(targetData); });
            _cachedView.DelteBtn.onClick.AddListener(() =>
            {
                int index = 0;
                for (int i = 0; i < taskData.Targets.Count; i++)
                {
                    if (taskData.Targets.ToList<NpcTaskTargetDynamic>()[i].Equals(targetData))
                    {
                        index = i;
                    }
                }
                taskData.Targets.Remove(index);
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