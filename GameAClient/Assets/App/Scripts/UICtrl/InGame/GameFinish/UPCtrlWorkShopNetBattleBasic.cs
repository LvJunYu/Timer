using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopNetBattleBasic : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        USCtrlSliderSetting _usPlayerCountSetting;
        USCtrlSliderSetting _usLifeCountSetting;
        USCtrlSliderSetting _usReviveTimeSetting;
        USCtrlSliderSetting _usReviveProtectTimeSetting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SureBtn_2.onClick.AddListener(OnSureBtn);
            _cachedView.SureBtn_3.onClick.AddListener(OnSureBtn);
            _cachedView.TitleInputField_3.onEndEdit.AddListener(_mainCtrl.OnTitleEndEdit);
            _cachedView.DescInputField_3.onEndEdit.AddListener(_mainCtrl.OnDescEndEdit);
            _cachedView.InfiniteLifeTog.onValueChanged.AddListener(OnInfiniteLifeTog);
            _usPlayerCountSetting = new USCtrlSliderSetting();
            _usLifeCountSetting = new USCtrlSliderSetting();
            _usReviveTimeSetting = new USCtrlSliderSetting();
            _usReviveProtectTimeSetting = new USCtrlSliderSetting();
            _usPlayerCountSetting.Init(_cachedView.PlayerCountSetting);
            _usLifeCountSetting.Init(_cachedView.LifeCountSetting);
            _usReviveTimeSetting.Init(_cachedView.ReviveTimeSetting);
            _usReviveProtectTimeSetting.Init(_cachedView.ReviveProtectTimeSetting);
            _usPlayerCountSetting.Set(1, 6, OnPlayerCountChanged);
            _usLifeCountSetting.Set(0, 30, OnLifeCountChanged, 5);
            _usReviveTimeSetting.Set(0, 10, OnReviveTimeChanged, 1, "{0}秒");
            _usReviveProtectTimeSetting.Set(0, 5, OnReviveProtectTimeChanged, 1, "{0}秒");
        }

        private void RefreshView()
        {
            _cachedView.TitleInputField_3.text = _mainCtrl.CurProject.Name;
            _cachedView.DescInputField_3.text = _mainCtrl.CurProject.Summary;

            _usPlayerCountSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleMaxPlayerCount);
            _usLifeCountSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleLifeCount);
            _usReviveTimeSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleReviveTime);
            _usReviveProtectTimeSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleReviveInvincibleTime);
        }

        private void OnInfiniteLifeTog(bool arg0)
        {
            if (arg0)
            {
                EditMode.Instance.MapStatistics.NetBattleMaxPlayerCount = -1;
            }
            else
            {
                EditMode.Instance.MapStatistics.NetBattleMaxPlayerCount = _usLifeCountSetting.Cur;
            }
        }

        private void OnPlayerCountChanged(int value)
        {
            if (!_cachedView.InfiniteLifeTog.isOn)
            {
                EditMode.Instance.MapStatistics.NetBattleMaxPlayerCount = value;
            }
        }

        private void OnLifeCountChanged(int value)
        {
            EditMode.Instance.MapStatistics.NetBattleLifeCount = value;
        }

        private void OnReviveTimeChanged(int value)
        {
            EditMode.Instance.MapStatistics.NetBattleReviveTime = value;
        }

        private void OnReviveProtectTimeChanged(int value)
        {
            EditMode.Instance.MapStatistics.NetBattleReviveInvincibleTime = value;
        }

        private void OnSureBtn()
        {
            if (!_mainCtrl.IsMulti) return;
        }

        public override void Open()
        {
            base.Open();
            _cachedView.NetBattleBasicPannel.SetActive(true);
            _cachedView.BtnDock1.SetActiveEx(true);
            _cachedView.BtnDock2.SetActiveEx(false);
            RefreshView();
        }

        public override void Close()
        {
            base.Close();
            _cachedView.NetBattleBasicPannel.SetActiveEx(false);
        }
    }
}