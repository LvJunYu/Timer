using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlGoldEnergy : UICtrlAnimationBase<UIViewGoldEnergy>
    {
        #region 常量与字段

        /// <summary>
        /// 下一次长体力的时间
        /// </summary>
        private long _nextEnergyGenerateTime;

        private readonly Stack<EStyle> _styleStack = new Stack<EStyle>(5);

        private int  _thousand = 1000;
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI2;
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
            _cachedView.EnergyPlusBtn.onClick.AddListener(OnEnergyPlusBtn);
            _cachedView.GoldPlusBtn.onClick.AddListener(OnGoldPlusBtn);
            _cachedView.DiamondPlusBtn.onClick.AddListener(OnDiamondPlusBtn);
            _cachedView.SettingBtn.onClick.AddListener(OnSettingBtn);
            _cachedView.MailBtn.onClick.AddListener(OnMailBtn);
//            _styleStack.Push(EStyle.None);
            _startPos = new Vector3(0, 100, 0);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            OnEnergyChanged();
            OnGoldChanged();
            OnDiamondChanged();

            if (_styleStack.Count > 0)
            {
                SetStyle(_styleStack.Peek());
            }
            else
            {
                SetStyle(EStyle.None);
            }
        }

        protected override void SetAnimationType()
        {
            base.SetAnimationType();
            _animationType = EAnimationType.MoveFromUp;
        }

        protected override void OnCloseAnimationComplete()
        {
            base.OnCloseAnimationComplete();
            SocialGUIManager.Instance.OpenUI<UICtrlGoldEnergy>();
        }

        public void PushStyle(EStyle eStyle)
        {
            _styleStack.Push(eStyle);
            if (_isOpen)
            {
                //先显示移出动画，再移入
                SocialGUIManager.Instance.CloseUI<UICtrlGoldEnergy>();
            }
            else
            {
                SetStyle(eStyle);
            }
        }

        public void PopStyle()
        {
            _styleStack.Pop();
            SocialGUIManager.Instance.CloseUI<UICtrlGoldEnergy>();
        }

        private void SetStyle(EStyle eStyle)
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGoldEnergy>();
            }
            var styleVal = (int) eStyle;
            _cachedView.Diamond.SetActiveEx((styleVal & 1 << (int) ESlot.Diamond) > 0);
            _cachedView.Gold.SetActiveEx((styleVal & 1 << (int) ESlot.Gold) > 0);
            _cachedView.Energy.SetActiveEx((styleVal & 1 << (int) ESlot.Energy) > 0);
            _cachedView.SettingBtn.gameObject.SetActiveEx((styleVal & 1 << (int) ESlot.Setting) > 0);
//            _cachedView.MailBtn.gameObject.SetActiveEx((styleVal & 1 << (int) ESlot.Mail) > 0);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            // energy refresh
            if (DateTimeUtil.GetServerTimeNowTimestampMillis() > _nextEnergyGenerateTime)
            {
                OnEnergyChanged();
            }
        }

        public void OnSettingBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlGameSetting>().ChangeToSettingAtHome();
        }

        public void OnMailBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlMail>();
        }

        private void OnEnergyPlusBtn()
        {
            if (AppData.Instance.AdventureData.UserData.UserEnergyData.Energy >=
                AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity)
            {
                SocialGUIManager.ShowPopupDialog(
                    "体力已经满了",
                    null,
                    new KeyValuePair<string, Action>(
                        "确定", null)
                );
            }
            else
            {
                SocialGUIManager.Instance.OpenUI<UICtrlBuyEnergy>();
            }
        }

        private void OnGoldPlusBtn()
        {
//            SocialGUIManager.Instance.OpenUI<UICtrlPurchase> ();
            SocialGUIManager.ShowPopupDialog("测试版还没有开启氪金功能哦~");
        }

        private void OnDiamondPlusBtn()
        {
//            SocialGUIManager.Instance.OpenUI<UICtrlPurchase> ();
            SocialGUIManager.ShowPopupDialog("测试版还没有开启氪金功能哦~");
        }

        private void OnEnergyChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            AppData.Instance.AdventureData.UserData.UserEnergyData.LocalRefresh(false);
            int currentEnergy = AppData.Instance.AdventureData.UserData.UserEnergyData.Energy;
            int energyCapacity = AppData.Instance.AdventureData.UserData.UserEnergyData.EnergyCapacity;
            _cachedView.EnergyNumber.text = string.Format("{0} / {1}",
                currentEnergy,
                energyCapacity
            );
            _cachedView.EnergyBar.fillAmount = (float) currentEnergy / energyCapacity;
            _nextEnergyGenerateTime = AppData.Instance.AdventureData.UserData.UserEnergyData.NextGenerateTime;
        }

        private void OnGoldChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            if (LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin < _thousand)
            {
                _cachedView.GoldNumber.text = LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin.ToString();
            }
            else
            {
                _cachedView.GoldNumber.text = (LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin/_thousand).ToString();
            }
        }

        private void OnDiamondChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            if (LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin < _thousand)
            {
                _cachedView.DiamondNumber.text = LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond.ToString();
            }
            else
            {
                _cachedView.DiamondNumber.text = (LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond/_thousand).ToString();
            }
        }

        #endregion

        private enum ESlot
        {
            Diamond,
            Gold,
            Energy,
            Setting,
            Mail
        }

        public enum EStyle
        {
            None = 0,
            EnergyGoldDiamond = 1 << ESlot.Diamond | 1 << ESlot.Gold | 1 << ESlot.Energy,
            GoldDiamond = 1 << ESlot.Diamond | 1 << ESlot.Gold,
            GoldDiamondSetting = 1 << ESlot.Diamond | 1 << ESlot.Gold | 1 << ESlot.Setting
        }
    }
}