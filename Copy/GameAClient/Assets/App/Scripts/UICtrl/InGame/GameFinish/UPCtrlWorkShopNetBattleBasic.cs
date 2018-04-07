using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopNetBattleBasic : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        USCtrlSliderSetting _usMinPlayerCountSetting;
        USCtrlSliderSetting _usLifeCountSetting;
        USCtrlSliderSetting _usReviveTimeSetting;
        USCtrlSliderSetting _usReviveProtectTimeSetting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _usMinPlayerCountSetting = new USCtrlSliderSetting();
            _usLifeCountSetting = new USCtrlSliderSetting();
            _usReviveTimeSetting = new USCtrlSliderSetting();
            _usReviveProtectTimeSetting = new USCtrlSliderSetting();
            _usMinPlayerCountSetting.Init(_cachedView.MinPlayerCountSetting);
            _usLifeCountSetting.Init(_cachedView.LifeCountSetting);
            _usReviveTimeSetting.Init(_cachedView.ReviveTimeSetting);
            _usReviveProtectTimeSetting.Init(_cachedView.ReviveProtectTimeSetting);
            _usMinPlayerCountSetting.Set(2, 6, value => EditMode.Instance.MapStatistics.NetBattleMinPlayerCount = value);
            _usLifeCountSetting.Set(1, 20, value => EditMode.Instance.MapStatistics.NetBattleLifeCount = value);
            _usReviveTimeSetting.Set(0, 10, value => EditMode.Instance.MapStatistics.NetBattleReviveTime = value, 1,
                "{0}秒");
            _usReviveProtectTimeSetting.Set(0, 10,
                value => EditMode.Instance.MapStatistics.NetBattleReviveInvincibleTime = value, 1, "{0}秒");

            _cachedView.InfiniteLifeTog.onValueChanged.AddListener(
                b => EditMode.Instance.MapStatistics.InfiniteLife = b);
            //复活方式
            for (int i = 0; i < _cachedView.ReviveTypeTogs.Length; i++)
            {
                var inx = i;
                _cachedView.ReviveTypeTogs[i].onValueChanged.AddListener(value =>
                {
                    if (value)
                    {
                        EditMode.Instance.MapStatistics.NetBattleReviveType = inx;
                    }
                });
            }
            //伤害类型
            for (int i = 0; i < _cachedView.HarmTypeTogs.Length; i++)
            {
                var inde = i;
                _cachedView.HarmTypeTogs[i].onValueChanged.AddListener(value =>
                {
                    EditMode.Instance.MapStatistics.SetHarmType((EHarmType) inde, value);
                });
            }
        }

        private void RefreshView()
        {
            _cachedView.TitleInputField_3.text = _mainCtrl.CurProject.Name;
            _cachedView.DescInputField_3.text = _mainCtrl.CurProject.Summary;

            _cachedView.InfiniteLifeTog.isOn = EditMode.Instance.MapStatistics.InfiniteLife;
            for (int i = 0; i < _cachedView.ReviveTypeTogs.Length; i++)
            {
                _cachedView.ReviveTypeTogs[i].isOn = EditMode.Instance.MapStatistics.NetBattleReviveType == i;
            }
            for (int i = 0; i < _cachedView.HarmTypeTogs.Length; i++)
            {
                _cachedView.HarmTypeTogs[i].isOn = EditMode.Instance.MapStatistics.CanHarmType((EHarmType) i);
            }
            _usMinPlayerCountSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleMinPlayerCount);
            _usLifeCountSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleLifeCount);
            _usReviveTimeSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleReviveTime);
            _usReviveProtectTimeSetting.SetCur(EditMode.Instance.MapStatistics.NetBattleReviveInvincibleTime);
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