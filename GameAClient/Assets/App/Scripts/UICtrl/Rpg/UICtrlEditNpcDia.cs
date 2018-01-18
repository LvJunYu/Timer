using System;
using System.Collections.Generic;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame, EUIAutoSetupType.Create)]
    public class UICtrlEditNpcDia : UICtrlAnimationBase<UIViewShowNpcDia>
    {
        private List<string> _diaList = new List<string>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
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

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);

            _diaList = (List<string>) parameter;
        }
    }
}