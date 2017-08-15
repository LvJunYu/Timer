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
    public class UICtrlGoldEnergy : UICtrlGenericBase<UIVIewGoldEnergy>
    {
        #region 常量与字段
        /// <summary>
        /// 下一次长体力的时间
        /// </summary>
        private long _nextEnergyGenerateTime;
        #endregion

        #region 属性
        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.FrontUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnEnergyChanged, OnEnergyChanged);
            RegisterEvent(EMessengerType.OnGoldChanged, OnGoldChanged);
            RegisterEvent(EMessengerType.OnDiamondChanged, OnDiamondChanged);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

            _cachedView.EnergyPlusBtn.onClick.AddListener (OnEnergyPlusBtn);
            _cachedView.GoldPlusBtn.onClick.AddListener (OnGoldPlusBtn);
            _cachedView.DiamondPlusBtn.onClick.AddListener (OnDiamondPlusBtn);
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            OnEnergyChanged ();
            OnGoldChanged ();
            OnDiamondChanged ();
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
            // energy refresh
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() > _nextEnergyGenerateTime) {
                OnEnergyChanged ();
            }
        }

        public void Show (bool showEnergy) {
            _cachedView.gameObject.SetActive (true);
            _cachedView.Energy.SetActive (showEnergy);
        }

        public void Hide () {
            _cachedView.gameObject.SetActive (false);
        }


        private void OnEnergyPlusBtn () {
            if (AppData.Instance.AdventureData.UserData.UserEnergyData.Energy >=
                AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity) {
                SocialGUIManager.ShowPopupDialog (
                    "体力已经满了",
                    null,
                    new KeyValuePair<string, Action> (
                        "确定", null)
                );
            } else {
                SocialGUIManager.Instance.OpenUI<UICtrlBuyEnergy> ();
            }
        }
        private void OnGoldPlusBtn () {
//            SocialGUIManager.Instance.OpenUI<UICtrlPurchase> ();
            SocialGUIManager.ShowPopupDialog("测试版还没有开启氪金功能哦~");
        }
        private void OnDiamondPlusBtn () {
//            SocialGUIManager.Instance.OpenUI<UICtrlPurchase> ();
            SocialGUIManager.ShowPopupDialog("测试版还没有开启氪金功能哦~");
        }

        private void OnEnergyChanged () {
            if (!IsOpen)
                return;
            AppData.Instance.AdventureData.UserData.UserEnergyData.LocalRefresh (false);
            int currentEnergy = AppData.Instance.AdventureData.UserData.UserEnergyData.Energy;
            int energyCapacity = AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity;
            _cachedView.EnergyNumber.text = string.Format("{0} / {1}",
                currentEnergy,
                energyCapacity
            );
            _cachedView.EnergyBar.fillAmount = (float)currentEnergy / energyCapacity;
            _nextEnergyGenerateTime = AppData.Instance.AdventureData.UserData.UserEnergyData.NextGenerateTime;
        }
        private void OnGoldChanged () {
            if (!IsOpen)
                return;
            _cachedView.GoldNumber.text = LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin.ToString();
        }
        private void OnDiamondChanged () {
            if (!IsOpen)
                return;
            _cachedView.DiamondNumber.text = LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond.ToString();
        }
        #endregion
    }
}
