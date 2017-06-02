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
    public class UICtrlPurchase : UISocialCtrlBase<UIViewPurchase>
    {
        #region 常量与字段
        #endregion

        #region 属性
        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpDialog;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();

        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

            _cachedView.BuyDiamondBtn.onClick.AddListener (OnBuyDiamondBtn);
            _cachedView.BuyGoldBtn.onClick.AddListener (OnBuyGoldBtn);
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);

        }

        private void OnBuyGoldBtn () {
            // check energy full


        }

        private void OnBuyDiamondBtn ()
        {
            
        }

        private void OnCloseBtn () {
            SocialGUIManager.Instance.CloseUI <UICtrlBuyEnergy> ();
        }
        #endregion
    }
}
