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
    public class UICtrlPurchase : UICtrlGenericBase<UIViewPurchase>
    {
        #region 常量与字段
        #endregion

        #region 属性
        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.Purchase;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            Messenger.AddListener (EMessengerType.OnGoldChanged, OnGoldChanged);
            Messenger.AddListener (EMessengerType.OnDiamondChanged, OnDiamondChanged);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

            _cachedView.BuyDiamondBtn.onClick.AddListener (OnBuyDiamondBtn);
            _cachedView.BuyGoldBtn.onClick.AddListener (OnBuyGoldBtn);
            _cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            OnDiamondChanged ();
            OnGoldChanged ();
        }

        private void OnBuyGoldBtn ()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "稍等");
            RemoteCommands.ExecuteCommand (
                LocalUser.Instance.UserGuid,
                "add gold 100",
                msg => {
                    if (msg.ResultCode == (int)EExecuteCommandCode.ECC_Success) {
                        GameATools.LocalAddGold (100);
                    } else {
                        
                    }
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                },
                code => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                }
            );
        }

        private void OnBuyDiamondBtn ()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "稍等");
            RemoteCommands.ExecuteCommand (
                LocalUser.Instance.UserGuid,
                "add diamond 10",
                msg => {
                    if (msg.ResultCode == (int)EExecuteCommandCode.ECC_Success) {
                        GameATools.LocalAddDiamond (10);
                    } else {

                    }
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                },
                code => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                }
            );
        }

        private void OnCloseBtn () {
            SocialGUIManager.Instance.CloseUI <UICtrlPurchase> ();
        }

        private void OnDiamondChanged ()
        {
            if (!_isOpen) return;
            _cachedView.CurrentDiamond.text = LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond.ToString ();
        }
        private void OnGoldChanged ()
        {
            if (!_isOpen) return;
            _cachedView.CurrentGold.text = LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin.ToString ();
        }
        #endregion
    }
}
