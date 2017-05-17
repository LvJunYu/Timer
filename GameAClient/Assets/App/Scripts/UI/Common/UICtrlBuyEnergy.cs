using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlBuyEnergy : UICtrlGenericBase<UIViewBuyEnergy>
    {
        #region 常量与字段
        #endregion

        #region 属性
        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();

        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

            //_cachedView.EnergyPlusBtn.onClick.AddListener (OnEnergyPlusBtn);
            //_cachedView.GoldPlusBtn.onClick.AddListener (OnGoldPlusBtn);
            //_cachedView.DiamondPlusBtn.onClick.AddListener (OnDiamondPlusBtn);
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();

        }


        #endregion
    }
}
