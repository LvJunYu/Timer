using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class
        //任务后的奖励的选择
        UPCtrlUnitPropertyEditNpcFinishTaskAwardType : UpCtrlNpcAdvanceBase
    {
        private NpcTaskDynamic _taskDynamic;
        private NpcTaskTargetDynamic _target;
        private List<int> _colltionList = new List<int>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _panel = _cachedView.NpcTaskFinishAwardTypePanel;
            foreach (var VARIABLE in TableManager.Instance.Table_NpcTaskTargetColltionDic)
            {
                _colltionList.Add(VARIABLE.Key);
            }

            for (int i = 0; i < _cachedView.FinishAwardTypeTypeBtnGroup.Length; i++)
            {
                int index = i;
                switch (index)
                {
                    case 0:
                        index = (int) ENpcTargetType.Colltion;
                        break;
                    case 1:
                        index = (int) ENpcTargetType.Contorl;
                        break;
                }

                _cachedView.FinishAwardTypeTypeBtnGroup[i].onClick.AddListener(() => { ChooseTargetType(index); });
            }

            _cachedView.NpcTaskFinishAwardTypePanelExitBtn.onClick.AddListener(Close);
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
                    _taskDynamic.TaskFinishAward.Add(_target);
                    _target.ColOrKillNum = 1;
                    _mainCtrl.EditNpcTaskDock.RefreshView();
                    Close();
                    _mainCtrl.EditNpcTaskAwardColltionAdvance.OpenMenu(_target);
                    break;

                case (int) ENpcTargetType.Contorl:
                    //选择控制
                    if (_mainCtrl.IsInMap)
                    {
                        //打开连线界面
                        NpcTaskDataTemp.Intance.StartEditTargetControl(_taskDynamic,
                            _mainCtrl.EditData.UnitDesc.Guid, ETaskContype.AfterTask, _mainCtrl.EditData.UnitExtra,
                            _mainCtrl.EditData);
                        _mainCtrl.OnCloseBtnClick();
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

        public override void RefreshView()
        {
            if (!_isOpen) return;
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