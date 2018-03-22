using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace GameA
{
    public class
        //任务目标的选择
        UPCtrlUnitPropertyEditNpcTaskTargetType : UpCtrlNpcAdvanceBase
    {
        private NpcTaskDynamic _taskDynamic;
        private NpcTaskTargetDynamic _target;
        private List<int> _colltionList = new List<int>();
        private List<int> _killtionList = new List<int>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskTypePanel;
            foreach (var colltion in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                _colltionList.Add(colltion.Key);
            }

            foreach (var kill in TableManager.Instance.Table_NpcTaskTargetKillDic)
            {
                _killtionList.Add(kill.Key);
            }

            for (int i = 0; i < _cachedView.TargetTypeBtnGroup.Length; i++)
            {
                int index = i + 1;
                _cachedView.TargetTypeBtnGroup[i].onClick.AddListener(() => { ChooseTargetType(index); });
            }

            _cachedView.NpcTaskTypeExitBtn.onClick.AddListener(Close);
        }

        public void ChooseTargetType(int index)
        {
            switch (index)
            {
                case (int) ENpcTargetType.Colltion:
                    //选择收集
                    _target = new NpcTaskTargetDynamic();
                    _target.TaskType = (byte) ENpcTargetType.Colltion;
                    _target.TargetUnitID =
                        (ushort) TableManager.Instance.Table_NpcTaskTargetColltionDic[_colltionList[0]].Id;
                    _target.ColOrKillNum = 1;
                    _taskDynamic.Targets.Add(_target);
                    _mainCtrl.EditNpcTaskDock.RefreshView();
                    _mainCtrl.EditNpcTaskColltionType.OpenMenu(_target);
                    Close();
                    break;
                case (int) ENpcTargetType.Moster:
                    //选择击杀
                    _target = new NpcTaskTargetDynamic();
                    _target.TaskType = (byte) ENpcTargetType.Moster;
                    _target.ColOrKillNum = 1;
                    _target.TargetUnitID =
                        (ushort) TableManager.Instance.Table_NpcTaskTargetKillDic[_killtionList[0]].Id;
                    _taskDynamic.Targets.Add(_target);
                    _mainCtrl.EditNpcTaskDock.RefreshView();
                    _mainCtrl.EditNpcTaskMonsterType.OpenMenu(_target);
                    Close();
                    break;
                case (int) ENpcTargetType.Contorl:
                    //选择控制
                    if (_mainCtrl.IsInMap)
                    {
                        NpcTaskDataTemp.Intance.StartEditTargetControl(_taskDynamic, _mainCtrl.EditData.UnitDesc.Guid,
                            ETaskContype.Task, _mainCtrl.EditData.UnitExtra, _mainCtrl.EditData);
                        _mainCtrl.OnCloseBtnClick();
                        //打开连线界面
                    }
                    else
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "地块不在地图中！");
                    }

                    break;
                case (int) ENpcTargetType.Dialog:
                    //选择传话
                    if (_mainCtrl.IsInMap)
                    {
                        //输入传话的目标
                        _target = new NpcTaskTargetDynamic();
                        _target.TaskType = (byte) ENpcTargetType.Dialog;
                        _target.TargetNpcNum = 0;
                        _taskDynamic.Targets.Add(_target);
                        _mainCtrl.EditNpcTaskDock.RefreshView();
                        _mainCtrl.EditNpcTaregtDialog.OpenMenu(_target);
                        Close();
                    }
                    else
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameLog, "地块不在地图中！");
                    }

                    break;
            }
        }

        public override void Open()
        {
            _mainCtrl.CloseUpCtrlPanel();
            base.Open();
            OpenAnimation();
        }

        public void OpenMenu(NpcTaskDynamic taskData)
        {
            _taskDynamic = taskData;
            Open();
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