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
        /// <summary>
        /// 预定要买的数量
        /// </summary>
        private int _buyCnt;

        private long _nextEnergyGenerateTime;
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

            _cachedView.BuyBtn.onClick.AddListener (OnBuyBtn);
            _cachedView.AccBtn.onClick.AddListener (OnAccBtn);
            _cachedView.RaiseBtn.onClick.AddListener (OnRaiseBtn);
            _cachedView.DropBtn.onClick.AddListener (OnDropBtn);
            _cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            if (null != parameter) {
                _buyCnt = (int)parameter;
            } else {
                _buyCnt = 1;
            }
            if (_buyCnt > AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity - AppData.Instance.AdventureData.UserData.UserEnergyData.Energy) {
                _buyCnt = AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity - AppData.Instance.AdventureData.UserData.UserEnergyData.Energy;
            }
            if (_buyCnt < 1) {
                _buyCnt = 1;
            }

            // todo
            _cachedView.UnitPrice.text = "998";
            _cachedView.BuyCnt.text = _buyCnt.ToString ();
            _cachedView.TotalPrice.text = (998 * _buyCnt).ToString ();
            _cachedView.AccPrice.text = "1998";
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
            if (DateTimeUtil.GetServerTimeNowTimestampMillis () > _nextEnergyGenerateTime) {
                AppData.Instance.AdventureData.UserData.UserEnergyData.LocalRefresh (false);
                _nextEnergyGenerateTime = AppData.Instance.AdventureData.UserData.UserEnergyData.NextGenerateTime;
                if (_buyCnt > AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity - AppData.Instance.AdventureData.UserData.UserEnergyData.Energy) {
                    _buyCnt = AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity - AppData.Instance.AdventureData.UserData.UserEnergyData.Energy;
                }
                if (_buyCnt < 1) {
                    _buyCnt = 1;
                }
                _cachedView.BuyCnt.text = _buyCnt.ToString ();
                _cachedView.TotalPrice.text = (998 * _buyCnt).ToString ();
            }
        }

        private void OnBuyBtn () {
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
            SocialGUIManager.ShowPopupDialog("测试版还没有开启这个功能哦，请耐心等待正式版吧~");
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
