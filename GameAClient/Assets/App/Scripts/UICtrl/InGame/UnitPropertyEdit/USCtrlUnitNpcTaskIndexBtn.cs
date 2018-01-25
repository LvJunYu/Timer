using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.Events;

namespace GameA
{
    public class USCtrlUnitNpcTaskIndexBtn : USCtrlBase<USViewUnitNpcTaskIndexBtn>
    {
        private NpcTaskDynamic _taskData;
        private List<NpcTaskDynamic> _taskDataList;
        private SeletNowTask _refreshcallback;

        public USViewUnitNpcTaskIndexBtn View
        {
            get { return _cachedView; }
        }

        public void InitData(SeletNowTask refreshnowTask, List<NpcTaskDynamic> taskDataList,
            NpcTaskDynamic taskdata = null)
        {
            _taskData = taskdata;
            _taskDataList = taskDataList;
            _refreshcallback = refreshnowTask;
            if (taskdata == null)
            {
                _cachedView.ChoseBtn.SetActiveEx(false);
                _cachedView.DelteBtn.SetActiveEx(false);
                _cachedView.AddBtn.SetActiveEx(true);
            }
            else
            {
                _cachedView.ChoseBtn.SetActiveEx(true);
                _cachedView.DelteBtn.SetActiveEx(false);
                _cachedView.AddBtn.SetActiveEx(false);
            }
            _cachedView.ChoseBtn.onClick.AddListener(() => { _refreshcallback.Invoke(_taskData); });
            _cachedView.AddBtn.onClick.AddListener(() =>
            {
                _taskData = new NpcTaskDynamic();
                _taskData.NpcSerialNumber = (ushort) NpcTaskDataTemp.Intance.GetNpcTaskSerialNum();
                _taskDataList.Add(_taskData);
                _refreshcallback.Invoke(_taskData);
                _cachedView.AddBtn.SetActiveEx(false);
                _cachedView.ChoseBtn.SetActiveEx(true);
            });
            _cachedView.DelteBtn.onClick.AddListener(() =>
            {
                _cachedView.DelteBtn.SetActiveEx(false);
                _cachedView.ChoseBtn.SetActiveEx(false);
                _cachedView.AddBtn.SetActiveEx(true);
                _taskDataList.Remove(_taskData);
                NpcTaskDataTemp.Intance.RecycleNpcTaskSerialNum(_taskData.NpcSerialNumber);
                if (_taskDataList.Count > 0)
                {
                    _refreshcallback.Invoke(_taskDataList[0]);
                }
                else
                {
                    NpcTaskDynamic taskDta = new NpcTaskDynamic();
                    taskDta.NpcSerialNumber = (ushort) NpcTaskDataTemp.Intance.GetNpcTaskSerialNum();
                    _taskDataList.Add(taskDta);
                    _refreshcallback.Invoke(_taskDataList[0]);
                }
            });
            _cachedView.RightClick.RightMouseCallback = (() =>
            {
                _cachedView.DelteBtn.gameObject.SetActiveEx(!_cachedView.DelteBtn.gameObject.activeSelf);
            });
        }

        public void SetSelected(NpcTaskDynamic taskData)
        {
            _cachedView.ChoseBtn.interactable = (taskData != _taskData);
        }

        public void SetEnable(bool enable)
        {
            _cachedView.SetActiveEx(enable);
        }
    }
}