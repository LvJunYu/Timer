using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopNetBattlePlayerSetting : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SureBtn_2.onClick.AddListener(OnSureBtn);
            _cachedView.SureBtn_3.onClick.AddListener(OnSureBtn);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.NetBattlePlayerSettingPannel.SetActive(true);
            _cachedView.BtnDock1.SetActiveEx(true);
            _cachedView.BtnDock2.SetActiveEx(false);
            UpdateAll();
        }

        public override void Close()
        {
            base.Close();
            _cachedView.NetBattlePlayerSettingPannel.SetActiveEx(false);
        }

        private void OnSureBtn()
        {
            if (!_mainCtrl.IsMulti) return;
        }

        public void UpdateAll()
        {
            
        }
    }
}