using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlGoldEnergy : UICtrlGenericBase<UIViewGoldEnergy>
    {
        #region 常量与字段
        /// <summary>
        /// 下一次长体力的时间
        /// </summary>
        private long _nextEnergyGenerateTime;
        private readonly Stack<EStyle> _styleStack = new Stack<EStyle>(5);
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
            _styleStack.Push(EStyle.None);
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            OnEnergyChanged ();
            OnGoldChanged ();
            OnDiamondChanged ();
        }

        public void PushStyle(EStyle eStyle)
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGoldEnergy>();
            }
            _styleStack.Push(eStyle);
            SetStyle(eStyle);
        }

        public void PopStyle()
        {
            _styleStack.Pop();
            if (_styleStack.Count > 0)
            {
                SetStyle(_styleStack.Peek());
            }
            else
            {
                SetStyle(EStyle.None);
            }
        }

        private void SetStyle(EStyle eStyle)
        {
            var styleVal = (int) eStyle;
            _cachedView.Diamond.SetActiveEx((styleVal & 1<<(int) ESlot.Diamond) > 0);
            _cachedView.Gold.SetActiveEx((styleVal & 1<<(int) ESlot.Gold) > 0);
            _cachedView.Energy.SetActiveEx((styleVal & 1<<(int) ESlot.Energy) > 0);
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
            // energy refresh
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() > _nextEnergyGenerateTime) {
                OnEnergyChanged ();
            }
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

        private enum ESlot
        {
            Diamond,
            Gold,
            Energy
        }
        public enum EStyle
        {
            None = 0,
            EnergyGoldDiamond = 1<<ESlot.Diamond | 1<<ESlot.Gold | 1<<ESlot.Energy,
            GoldDiamond = 1<<ESlot.Diamond | 1<<ESlot.Gold
        }
    }
}
