using System;
using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame, EUIAutoSetupType.Create)]
    public class UICtrlEditNpcControl : UICtrlGenericBase<UIViewEditNpcControl>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ExitBtn.onClick.AddListener(Close);
        }


        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }


        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }


        public override void Close()
        {
            base.Close();
            if (NpcTaskDataTemp.Intance.EndEdit)
            {
                EditMode.Instance.StopSwitch();
                NpcTaskDataTemp.Intance.EndEdit = false;
            }

            EditHelper.TryEditUnitData(NpcTaskDataTemp.Intance.EditData.UnitDesc);
            SocialGUIManager.Instance.GetUI<UICtrlUnitPropertyEdit>().OnEditTypeMenuClick(EEditType.NpcTask);
            SocialGUIManager.Instance.GetUI<UICtrlUnitPropertyEdit>().EditNpcTaskDock
                .ChooseTaskBtnIndex(NpcTaskDataTemp.Intance.TaskTargetData.TargetGuid);
            switch (NpcTaskDataTemp.Intance.TaskType)
            {
                case ETaskContype.AfterTask:
                case ETaskContype.BeforeTask:
                    SocialGUIManager.Instance.GetUI<UICtrlUnitPropertyEdit>().EditNpcTaskDock.OpenEditPanel();
                    break;
            }
        }
    }
}