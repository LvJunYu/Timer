using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public delegate void SeletTaskTargetType(UnitExtraNpcTaskTarget target);

    public delegate void SeletTaskType(UnitExtraNpcTaskData task);

    public class UPCtrlUnitPropertyEditNpcTaskType : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private bool _hasChanged;
        private USCtrlUnitNpcTaskIndexBtn[] _usCtrlUnitNpcTaskIndexGroup;
        private USCtrlUnitNpcTaskTargetBtn[] _usCtrlUnitNpcTargetGroup;
        private USCtrlUnitNpcTaskTargetBtn _usCtrlUnitAddNpcTarget;
        public UnitExtraNpcTaskData CurExtraNpcTaskData;

        //和任务有关的

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            CurExtraNpcTaskData = new UnitExtraNpcTaskData();

            //任务按钮
            _usCtrlUnitNpcTaskIndexGroup = new USCtrlUnitNpcTaskIndexBtn[_cachedView.IndexBtnGroup.Length];
            for (int i = 0; i < _usCtrlUnitNpcTaskIndexGroup.Length; i++)
            {
                _usCtrlUnitNpcTaskIndexGroup[i] = new USCtrlUnitNpcTaskIndexBtn();
                _usCtrlUnitNpcTaskIndexGroup[i].Init(_cachedView.IndexBtnGroup[i]);
            }

            //任务目标按钮
            _usCtrlUnitNpcTargetGroup = new USCtrlUnitNpcTaskTargetBtn[_cachedView.TargetBtnGroup.Length];
            for (int i = 0; i < _usCtrlUnitNpcTargetGroup.Length; i++)
            {
                _usCtrlUnitNpcTargetGroup[i] = new USCtrlUnitNpcTaskTargetBtn();
                _usCtrlUnitNpcTargetGroup[i].Init(_cachedView.TargetBtnGroup[i]);
            }
            //添加任务按钮
            _usCtrlUnitAddNpcTarget = new USCtrlUnitNpcTaskTargetBtn();
            _usCtrlUnitAddNpcTarget.Init(_cachedView.AddTargetBtn);
            _usCtrlUnitAddNpcTarget.AddNewTarget(() => { OpenAdvencePanel(new UnitExtraNpcTaskTarget()); });


            BadWordManger.Instance.InputFeidAddListen(_cachedView.TaskNpcName);
        }

        //刪除任務列表
        private void DeleteTask()
        {
            RefreshView();
        }

        public override void Open()
        {
            base.Open();
            _cachedView.NpcTaskDock.SetActiveEx(true);
            if (_mainCtrl.NpcTaskDatas.Count > 0)
            {
                CurExtraNpcTaskData = _mainCtrl.NpcTaskDatas[0];
            }
            RefreshView();
        }

        //刷新页面
        public void RefreshView()
        {
            if (!_isOpen) return;

            _cachedView.TaskNpcName.text = _mainCtrl.EditData.UnitExtra.NpcName;
            //设置任务的序列
            int taskNum = _mainCtrl.NpcTaskDatas.Count;
            for (int i = 0; i < _usCtrlUnitNpcTaskIndexGroup.Length; i++)
            {
                if (i < taskNum)
                {
                    _usCtrlUnitNpcTaskIndexGroup[i].SetEnable(true);
                    _usCtrlUnitNpcTaskIndexGroup[i].SetSelectTask(_mainCtrl.NpcTaskDatas[i], RefreshTask);
                }
                else
                {
                    _usCtrlUnitNpcTaskIndexGroup[i].SetEnable(false);
                }
            }


            // 选中任务后的刷新
            List<ENpcTaskType> listType = new List<ENpcTaskType>();
            for (int i = 0; i < _usCtrlUnitNpcTargetGroup.Length; i++)
            {
                if (i < CurExtraNpcTaskData.TaskTarget.Count)
                {
                    _usCtrlUnitNpcTargetGroup[i].SetEnable(true);
                    _usCtrlUnitNpcTargetGroup[i].SetSelectTarget(CurExtraNpcTaskData.TaskTarget[i],
                        CurExtraNpcTaskData,
                        OpenAdvencePanel, RefreshView);
                    listType.Add((ENpcTaskType) CurExtraNpcTaskData.TaskTarget[i].TaskType);
                }
                else
                {
                    _usCtrlUnitNpcTargetGroup[i].SetEnable(false);
                }
            }
            //最大的任务目标
            if (listType.Count >= 3)
            {
                _usCtrlUnitAddNpcTarget.SetEnable(false);
            }
            for (int i = 0; i < listType.Count; i++)
            {
                if (listType[i] == ENpcTaskType.Dialog)
                {
                    _usCtrlUnitAddNpcTarget.SetEnable(false);
                }
            }
        }

        public override void Close()
        {
            _cachedView.NpcTaskDock.SetActiveEx(false);


            // 关闭的时候设置名字和对话内容
            if (_cachedView.NpcName.text != _mainCtrl.EditData.UnitExtra.NpcName)
            {
                _mainCtrl.EditData.UnitExtra.NpcName = _cachedView.NpcName.text;
            }
            if (_cachedView.NpcDialog.text != _mainCtrl.EditData.UnitExtra.NpcDialog)
            {
                _mainCtrl.EditData.UnitExtra.NpcDialog = _cachedView.NpcDialog.text;
            }
            if (_cachedView.TaskNpcName.text != _mainCtrl.EditData.UnitExtra.NpcName)
            {
                _mainCtrl.EditData.UnitExtra.NpcName = _cachedView.TaskNpcName.text;
            }
            base.Close();
        }

        private void RefreshTask(UnitExtraNpcTaskData taskData)
        {
            CurExtraNpcTaskData = taskData;
            RefreshView();
        }

        private void OpenAdvencePanel(UnitExtraNpcTaskTarget target)
        {
            _mainCtrl.UpCtrlUnitPropertyEditNpcTaskAdvance.OpenMenu(target);
        }
    }
}