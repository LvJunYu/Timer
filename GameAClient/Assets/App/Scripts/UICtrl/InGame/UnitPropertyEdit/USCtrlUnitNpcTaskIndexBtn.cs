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
        private DictionaryListObject _taskDataList;
        private SeletNowTask _refreshcallback;

        public USViewUnitNpcTaskIndexBtn View
        {
            get { return _cachedView; }
        }

        public void InitData(SeletNowTask refreshnowTask, DictionaryListObject taskDataList,
            NpcTaskDynamic taskdata = null)
        {
            _cachedView.AddBtn.onClick.RemoveAllListeners();
            _cachedView.ChoseBtn.onClick.RemoveAllListeners();
            _cachedView.DelteBtn.onClick.RemoveAllListeners();
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
                if (NpcTaskDataTemp.Intance.GetNpcTaskSerialNum() == NpcTaskDataTemp.NoneNumMark)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog,
                        string.Format("任务数目达到上限"));
                }
                else
                {
                    _taskData = new NpcTaskDynamic();
                    _taskData.NpcTaskSerialNumber = (ushort) NpcTaskDataTemp.Intance.GetNpcTaskSerialNum();
                    NpcTaskDataTemp.Intance.SetNpcTaskSerialNum(_taskData.NpcTaskSerialNumber);
                    _taskDataList.Add(_taskData);
                    _refreshcallback.Invoke(_taskData);
                    _cachedView.AddBtn.SetActiveEx(false);
                    _cachedView.ChoseBtn.SetActiveEx(true);
                }
            });
            _cachedView.DelteBtn.onClick.AddListener(() =>
            {
                _cachedView.DelteBtn.SetActiveEx(false);
                _cachedView.ChoseBtn.SetActiveEx(false);
                _cachedView.AddBtn.SetActiveEx(true);
                int index = 0;
                for (int i = 0; i < _taskDataList.Count; i++)
                {
                    if (_taskDataList.Get<NpcTaskDynamic>(i).Equals(_taskData))
                    {
                        index = i;
                        break;
                    }
                }
                _taskDataList.RemoveAt(index);
                NpcTaskDataTemp.Intance.RecycleNpcTaskSerialNum(_taskData.NpcTaskSerialNumber);
                if (_taskDataList.Count > 0)
                {
                    _refreshcallback.Invoke(_taskDataList.Get<NpcTaskDynamic>(0));
                }
                else
                {
                    NpcTaskDynamic taskDta = new NpcTaskDynamic();
                    taskDta.NpcTaskSerialNumber = (ushort) NpcTaskDataTemp.Intance.GetNpcTaskSerialNum();
                    _taskDataList.Add(taskDta);
                    _refreshcallback.Invoke(_taskDataList.Get<NpcTaskDynamic>(0));
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