using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopNetBattlePlayerSetting : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        private USCtrlSliderSetting _usMaxHpSetting;
        private USCtrlSliderSetting _usJumpAbilitySetting;
        private USCtrlSliderSetting _usMoveSpeedSetting;
        private USCtrlSliderSetting _usInjuredReduceSetting;
        private USCtrlSliderSetting _usCureIncreaseSetting;
        public bool HasChanged;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
//            _cachedView.SureBtn_2.onClick.AddListener(Save);
            _cachedView.RestoreDefaultBtn.onClick.AddListener(OnRestoreDefaultBtn);

            _usMaxHpSetting = new USCtrlSliderSetting();
            _usJumpAbilitySetting = new USCtrlSliderSetting();
            _usMoveSpeedSetting = new USCtrlSliderSetting();
            _usInjuredReduceSetting = new USCtrlSliderSetting();
            _usCureIncreaseSetting = new USCtrlSliderSetting();
            _usMaxHpSetting.Init(_cachedView.MaxHpSetting);
            _usJumpAbilitySetting.Init(_cachedView.JumpAbilitySetting);
            _usMoveSpeedSetting.Init(_cachedView.MoveSpeedSetting);
            _usInjuredReduceSetting.Init(_cachedView.InjuredReduceSetting);
            _usCureIncreaseSetting.Init(_cachedView.CureIncreaseSetting);
            _usMaxHpSetting.Set(1, 2000, value => HasChanged = true, 100);
            _usJumpAbilitySetting.Set(100, 300, value => HasChanged = true, 10);
            _usMoveSpeedSetting.Set(20, 100, value => HasChanged = true, 10);
            _usInjuredReduceSetting.Set(0, 100, value => HasChanged = true, 10, "{0}%");
            _usCureIncreaseSetting.Set(0, 500, value => HasChanged = true, 10, "{0}%");
        }

        private void OnRestoreDefaultBtn()
        {
            if (!_isOpen) return;
            var table = TableManager.Instance.GetUnit(UnitDefine.MainPlayerId);
            _usMaxHpSetting.SetCur(table.Hp, false);
            _usJumpAbilitySetting.SetCur(table.JumpAblity, false);
            _usMoveSpeedSetting.SetCur(table.MaxSpeed, false);
            _usInjuredReduceSetting.SetCur(0, false);
            _usCureIncreaseSetting.SetCur(0, false);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.NetBattlePlayerSettingPannel.SetActive(true);
            _cachedView.BtnDock1.SetActiveEx(false);
            _cachedView.BtnDock2.SetActiveEx(true);
            RefreshView();
            HasChanged = false;
        }

        public override void Close()
        {
            base.Close();
            _cachedView.NetBattlePlayerSettingPannel.SetActiveEx(false);
            Save();
        }

        public void Save()
        {
            if (!_mainCtrl.IsMulti || !HasChanged) return;
            DataScene2D.Instance.SetPlayerCommonValue("MaxHp", _usMaxHpSetting.Cur);
            DataScene2D.Instance.SetPlayerCommonValue("JumpAbility", (ushort) _usJumpAbilitySetting.Cur);
            DataScene2D.Instance.SetPlayerCommonValue("MaxSpeedX", (ushort) _usMoveSpeedSetting.Cur);
            DataScene2D.Instance.SetPlayerCommonValue("InjuredReduce", (byte) _usInjuredReduceSetting.Cur);
            DataScene2D.Instance.SetPlayerCommonValue("CureIncrease", (ushort) _usCureIncreaseSetting.Cur);
            HasChanged = false;
        }

        public void RefreshView()
        {
            _usMaxHpSetting.SetCur(DataScene2D.Instance.PlayerExtra.MaxHp);
            _usJumpAbilitySetting.SetCur(DataScene2D.Instance.PlayerExtra.JumpAbility);
            _usMoveSpeedSetting.SetCur(DataScene2D.Instance.PlayerExtra.MaxSpeedX);
            _usInjuredReduceSetting.SetCur(DataScene2D.Instance.PlayerExtra.InjuredReduce);
            _usCureIncreaseSetting.SetCur(DataScene2D.Instance.PlayerExtra.CureIncrease);
        }
    }
}