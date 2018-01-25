using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public delegate void SeletTaskTargetType(NpcTaskTargetDynamic target);

    public delegate void SeletNowTask(NpcTaskDynamic task);

    public enum EAdvanceType
    {
        ChooseTargetType,
        Monster,
        Colltion,
        SendDia,
        EditDia,
        ChooseBeforeTaskAward,
        AddConditon,
        BeforeTaskAward,
        ChooseTrigerCondtion,
        TrigerMonster,
        TrigerColltion,
    }

//    public USViewSliderSetting TaskTimeLimitSetting;
//    public Button RemoveTaskTimeLimitBtn;
    public class UPCtrlUnitPropertyEditNpcTaskDock : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        private bool _hasChanged;
        private USCtrlUnitNpcTaskIndexBtn[] _usCtrlUnitNpcTaskIndexGroup;
        private USCtrlUnitNpcTaskTargetBtn[] _usCtrlUnitNpcTargetGroup;
        private USCtrlUnitNpcTaskTargetBtn _usCtrlUnitNpcCondtionBtn;
        private USCtrlSliderSetting _timeLimitSliderSetting;
        public NpcTaskDynamic CurExtraNpcTaskData;

        public DictionaryListObject NpcTaskDatas;
        //和任务有关的

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
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
            _cachedView.AddTargetBtn.onClick.AddListener(() =>
            {
                _mainCtrl.EditNpcTaskTargetType.OpenMenu(CurExtraNpcTaskData);
            });


            //添加目标按钮
            _usCtrlUnitNpcCondtionBtn = new USCtrlUnitNpcTaskTargetBtn();
            _usCtrlUnitNpcCondtionBtn.Init(_cachedView.TriggerConditonBtn);
//            _usCtrlUnitNpcCondtionBtn.AddNewTarget(() =>
//            {
//                if (CurExtraNpcTaskData != null)
//                {
//                    OpenAdvencePanel(new NpcTaskTargetDynamic());
//                }
//            });
            //添加条件按钮
            _cachedView.AddConditionBtn.onClick.AddListener(() =>
            {
                _mainCtrl.EditNpcAddCondition.OpenMenu(CurExtraNpcTaskData);
            });
            //编辑对话按钮
            _cachedView.DialogBtn.onClick.AddListener(() => { _mainCtrl.EditNpcDia.OpenMenu(CurExtraNpcTaskData); });
            //npc名字限制长度
            BadWordManger.Instance.InputFeidAddListen(_cachedView.TaskNpcName);
            _cachedView.TaskNpcName.onValueChanged.AddListener(SetNameLength);
            _cachedView.RemoveTaskTimeLimitBtn.onClick.AddListener(() =>
            {
                CurExtraNpcTaskData.TaskimeLimit = 0;
                RefreshTask();
            });
            //任务限时
            _timeLimitSliderSetting = new USCtrlSliderSetting();
            _timeLimitSliderSetting.Init(_cachedView.TaskTimeLimitSetting);
            UnitExtraHelper.SetUSCtrlSliderSetting(_timeLimitSliderSetting, EAdvanceAttribute.MaxTaskTimeLimit,
                value => { CurExtraNpcTaskData.TaskimeLimit = (ushort) value; });
        }

        public override void Open()
        {
            base.Open();
            _mainCtrl.CloseUpCtrlPanel();
            NpcTaskDatas = _mainCtrl.EditData.UnitExtra.NpcTask;
            _cachedView.NpcTaskDock.SetActiveEx(true);
            if (NpcTaskDatas.Count > 0)
            {
                CurExtraNpcTaskData = NpcTaskDatas.Get<NpcTaskDynamic>(0);
            }
            else
            {
                CurExtraNpcTaskData = new NpcTaskDynamic();
                NpcTaskDatas.Add(CurExtraNpcTaskData);
            }
            RefreshView();
        }

        //刷新页面
        public void RefreshView()
        {
            if (!_isOpen) return;
            //名字
            _cachedView.TaskNpcName.text = _mainCtrl.EditData.UnitExtra.NpcName;
            _cachedView.TaskSerialNumText.text = CurExtraNpcTaskData.NpcSerialNumber.ToString();
            //设置任务的序列
            int taskNum = NpcTaskDatas.Count;
            for (int i = 0; i < _usCtrlUnitNpcTaskIndexGroup.Length; i++)
            {
                if (i < taskNum)
                {
                    _usCtrlUnitNpcTaskIndexGroup[i].SetEnable(true);
                    _usCtrlUnitNpcTaskIndexGroup[i]
                        .InitData(RefreshTask, NpcTaskDatas, NpcTaskDatas.Get<NpcTaskDynamic>(i));
                }
                else
                {
                    _usCtrlUnitNpcTaskIndexGroup[i].InitData(RefreshTask, NpcTaskDatas);
                }
            }

            RefreshTask();
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


        public void RefreshTask(NpcTaskDynamic taskData = null)
        {
            if (taskData != null)
            {
                CurExtraNpcTaskData = taskData;
            }
            //刷新任务列表
            for (int i = 0; i < _usCtrlUnitNpcTaskIndexGroup.Length; i++)
            {
                _usCtrlUnitNpcTaskIndexGroup[i].SetSelected(CurExtraNpcTaskData);
            }

            // 选中任务后的刷新
            List<ENpcTargetType> listType = new List<ENpcTargetType>();
            for (int i = 0; i < _usCtrlUnitNpcTargetGroup.Length; i++)
            {
                if (i < CurExtraNpcTaskData.Targets.Count)
                {
                    _usCtrlUnitNpcTargetGroup[i].SetEnable(true);
                    _usCtrlUnitNpcTargetGroup[i].SetSelectTarget(
                        CurExtraNpcTaskData.Targets.ToList<NpcTaskTargetDynamic>()[i],
                        CurExtraNpcTaskData.Targets,
                        OpenAdvencePanel, RefreshView);
                    listType.Add(
                        (ENpcTargetType) CurExtraNpcTaskData.Targets.ToList<NpcTaskTargetDynamic>()[i].TaskType);
                }
                else
                {
                    _usCtrlUnitNpcTargetGroup[i].SetEnable(false);
                }
            }
            //最大的任务目标
            if (listType.Count >= 3)
            {
                _cachedView.AddTargetBtn.SetActiveEx(false);
            }
            for (int i = 0; i < listType.Count; i++)
            {
                if (listType[i] == ENpcTargetType.Dialog)
                {
                    _cachedView.AddTargetBtn.SetActiveEx(false);
                }
            }

            //任务条件
            if (CurExtraNpcTaskData.TriggerType == (int) ENpcTargetType.None)
            {
                _cachedView.TriggerConditonObj.SetActiveEx(false);
            }
            else
            {
                _cachedView.TriggerConditonObj.SetActiveEx(true);
                _usCtrlUnitNpcCondtionBtn.SetTriggerCondition(CurExtraNpcTaskData,
                    EditTriggerTaskEdit, () => { RefreshTask(); });
            }
            if (CurExtraNpcTaskData.TaskimeLimit == 0)
            {
                _cachedView.TaskTimeLimitObj.SetActiveEx(false);
            }
            else
            {
                _cachedView.TaskTimeLimitObj.SetActiveEx(true);
                _timeLimitSliderSetting.SetCur(CurExtraNpcTaskData.TaskimeLimit);
            }
            _cachedView.AddConditionBtn.SetActiveEx(!(CurExtraNpcTaskData.TriggerType != (int) ENpcTargetType.None &&
                                                      CurExtraNpcTaskData.TaskimeLimit > 0));
        }

        private void EditTriggerTaskEdit(TrrigerTaskType type)
        {
            switch (type)
            {
                case TrrigerTaskType.Colltion:
                    _mainCtrl.EditNpcTaskColltionType.OpenMenu(CurExtraNpcTaskData.TriggerTask);
                    break;
                case TrrigerTaskType.Kill:
                    _mainCtrl.EditNpcTaskMonsterType.OpenMenu(CurExtraNpcTaskData.TriggerTask);
                    break;
                case TrrigerTaskType.FinishOtherTask:
                    _mainCtrl.EditBeforeTask.OpenMenu(CurExtraNpcTaskData);

                    break;
            }
        }

        private void OpenAdvencePanel(NpcTaskTargetDynamic target)
        {
            switch ((ENpcTargetType) target.TaskType)
            {
                case ENpcTargetType.Colltion:
                    _mainCtrl.EditNpcTaskColltionType.OpenMenu(target);
                    break;
                case ENpcTargetType.Moster:
                    _mainCtrl.EditNpcTaskColltionType.OpenMenu(target);
                    break;
                case ENpcTargetType.Contorl:
                    break;
                case ENpcTargetType.Dialog:

                    break;
            }
        }


        private void SetNameLength(string str)
        {
            _cachedView.NameNumText.text =
                String.Format("{0}/{1}", str.Length, UPCtrlUnitPropertyEditNpcDiaType.NamMaxLength);
            if (str.Length > UPCtrlUnitPropertyEditNpcDiaType.NamMaxLength)
            {
                _cachedView.TaskNpcName.text = str.Substring(0, UPCtrlUnitPropertyEditNpcDiaType.NamMaxLength);
            }
        }
    }
}