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
            UnitExtraHelper.SetUSCtrlSliderSetting(_usMaxHpSetting, EAdvanceAttribute.MaxHp,
                value => HasChanged = true);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usJumpAbilitySetting, EAdvanceAttribute.JumpAbility,
                value => HasChanged = true);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usMoveSpeedSetting, EAdvanceAttribute.MaxSpeedX,
                value => HasChanged = true);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usInjuredReduceSetting, EAdvanceAttribute.InjuredReduce,
                value => HasChanged = true);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usCureIncreaseSetting, EAdvanceAttribute.CureIncrease,
                value => HasChanged = true);
        }

        private void OnRestoreDefaultBtn()
        {
            if (!_isOpen) return;
            var table = TableManager.Instance.GetUnit(UnitDefine.MainPlayerId);
            _usMaxHpSetting.SetCur(table.Hp, false);
            _usJumpAbilitySetting.SetCur(table.JumpAbility, false);
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
            if (!_mainCtrl.IsMulti) return;
            if (HasChanged)
            {
                DataScene2D.CurScene.SetPlayerMaxHp(_usMaxHpSetting.Cur);
                DataScene2D.CurScene.SetPlayerJumpAbility(_usJumpAbilitySetting.Cur);
                DataScene2D.CurScene.SetPlayerMaxSpeedX(_usMoveSpeedSetting.Cur);
                DataScene2D.CurScene.SetPlayerInjuredReduce(_usInjuredReduceSetting.Cur);
                DataScene2D.CurScene.SetPlayerCureIncrease(_usCureIncreaseSetting.Cur);
                HasChanged = false;
            }
        }

        public void RefreshView()
        {
            _usMaxHpSetting.SetCur(DataScene2D.CurScene.PlayerExtra.MaxHp);
            _usJumpAbilitySetting.SetCur(DataScene2D.CurScene.PlayerExtra.JumpAbility);
            _usMoveSpeedSetting.SetCur(DataScene2D.CurScene.PlayerExtra.MaxSpeedX);
            _usInjuredReduceSetting.SetCur(DataScene2D.CurScene.PlayerExtra.InjuredReduce);
            _usCureIncreaseSetting.SetCur(DataScene2D.CurScene.PlayerExtra.CureIncrease);
        }
    }
}