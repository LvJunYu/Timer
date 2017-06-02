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

            AppData.Instance.AdventureData.UserData.UserEnergyData.LocalRefresh (false);
            if (_buyCnt > (
                AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity -
                AppData.Instance.AdventureData.UserData.UserEnergyData.Energy)
               ) {
                SocialGUIManager.ShowPopupDialog (
                    "购买的体力超过上限了", 
                    "不能这样买",
                    new KeyValuePair<string, Action> (
                        "确定", null)
                );
                SocialGUIManager.Instance.CloseUI<UICtrlBuyEnergy> ();
                return;
            }
            // todo
            if (GameATools.CheckDiamond (998 * _buyCnt)) {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "...");
                RemoteCommands.BuyEnergy (
                    _buyCnt,
                    msg => {
                        if ((int)EBuyEnergyCode.BEC_Success == msg.ResultCode) {
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this); 
                            GameATools.LocalSetEnergy (msg.CurEnergy);
                            GameATools.LocalUseDiamond(998 * _buyCnt);
                            SocialGUIManager.Instance.CloseUI<UICtrlBuyEnergy> ();
                        } else {
                            // todo error handle
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        }
                    },
                    code => {
                        // todo error handle
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    }
                );
            }
        }

        private void OnAccBtn () {
        }

        private void OnRaiseBtn () {
            _buyCnt++;
            _buyCnt = Mathf.Clamp (
                    _buyCnt,
                    1,
                    AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity -
                    AppData.Instance.AdventureData.UserData.UserEnergyData.Energy);
            _cachedView.BuyCnt.text = _buyCnt.ToString ();
            _cachedView.TotalPrice.text = (998 * _buyCnt).ToString ();
        }

        private void OnDropBtn () {
            _buyCnt--;
            _buyCnt = Mathf.Clamp (
                    _buyCnt,
                    1,
                    AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity -
                    AppData.Instance.AdventureData.UserData.UserEnergyData.Energy);
            _cachedView.BuyCnt.text = _buyCnt.ToString ();
            _cachedView.TotalPrice.text = (998 * _buyCnt).ToString ();
        }

        private void OnCloseBtn () {
            SocialGUIManager.Instance.CloseUI <UICtrlBuyEnergy> ();
        }
        #endregion
    }
}
