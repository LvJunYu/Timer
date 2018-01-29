using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlUnitPropertyEditAdvance : UPCtrlBase<UICtrlUnitPropertyEdit, UIViewUnitPropertyEdit>
    {
        public enum EMenu
        {
            ActorSetting,
            WeaponSetting,
            MonsterCave,
        }

        private Sequence _openSequence;
        private Sequence _closeSequence;
        private bool _hasChanged;
        private bool _openAnim;
        private bool _completeAnim;
        private USCtrlSliderSetting _usMaxHpSetting;
        private USCtrlSliderSetting _usJumpSetting;
        private USCtrlSliderSetting _usMoveSpeedSetting;
        private USCtrlSliderSetting _usDamageSetting;
        private USCtrlSliderSetting _usEffectRangeSetting;
        private USCtrlSliderSetting _usCastRangeSetting;
        private USCtrlSliderSetting _usDamageIntervalSetting;
        private USCtrlSliderSetting _usBulletSpeedSetting;
        private USCtrlSliderSetting _usBulletCountSetting;
        private USCtrlSliderSetting _usChargeTimeSetting;
        private USCtrlSliderSetting _usInjuredReduceSetting;
        private USCtrlSliderSetting _usCurIncreaseSetting;
        private USCtrlSliderSetting _usMonsterIntervalTimeSetting;
        private USCtrlSliderSetting _usMaxCreatedMonsterSetting;
        private USCtrlSliderSetting _usMaxAliveMonsterSetting;
        private USCtrlGameSettingItem _usCanMoveSetting;
        private USCtrlAddItem _usDropsSetting;
        private USCtrlAddItem _usAddStatesSetting;
        private USCtrlDropdownSetting _usSpawnSetting;
        private USCtrSetItem _usPlayerWeaponSetting;
        private EMenu _curMenu;
        private UnitExtraDynamic _curUnitExtra;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AdvancePannel.SetActive(false);
            _usMaxHpSetting = new USCtrlSliderSetting();
            _usJumpSetting = new USCtrlSliderSetting();
            _usMoveSpeedSetting = new USCtrlSliderSetting();
            _usDamageSetting = new USCtrlSliderSetting();
            _usEffectRangeSetting = new USCtrlSliderSetting();
            _usCastRangeSetting = new USCtrlSliderSetting();
            _usDamageIntervalSetting = new USCtrlSliderSetting();
            _usBulletSpeedSetting = new USCtrlSliderSetting();
            _usBulletCountSetting = new USCtrlSliderSetting();
            _usChargeTimeSetting = new USCtrlSliderSetting();
            _usInjuredReduceSetting = new USCtrlSliderSetting();
            _usCurIncreaseSetting = new USCtrlSliderSetting();
            _usMonsterIntervalTimeSetting = new USCtrlSliderSetting();
            _usMaxCreatedMonsterSetting = new USCtrlSliderSetting();
            _usMaxAliveMonsterSetting = new USCtrlSliderSetting();
            _usMaxHpSetting.Init(_cachedView.MaxHpSetting);
            _usJumpSetting.Init(_cachedView.JumpSetting);
            _usMoveSpeedSetting.Init(_cachedView.MoveSpeedSetting);
            _usDamageSetting.Init(_cachedView.DamageSetting);
            _usEffectRangeSetting.Init(_cachedView.EffectRangeSetting);
            _usCastRangeSetting.Init(_cachedView.CastRangeSetting);
            _usDamageIntervalSetting.Init(_cachedView.DamageIntervalSetting);
            _usBulletSpeedSetting.Init(_cachedView.BulletSpeedSetting);
            _usBulletCountSetting.Init(_cachedView.BulletCountSetting);
            _usChargeTimeSetting.Init(_cachedView.ChargeTimeSetting);
            _usInjuredReduceSetting.Init(_cachedView.InjuredReduceSetting);
            _usCurIncreaseSetting.Init(_cachedView.CurIncreaseSetting);
            _usMonsterIntervalTimeSetting.Init(_cachedView.MonsterIntervalTimeSetting);
            _usMaxCreatedMonsterSetting.Init(_cachedView.MaxCreatedMonsterSetting);
            _usMaxAliveMonsterSetting.Init(_cachedView.MaxAliveMonsterSetting);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usMaxHpSetting, EAdvanceAttribute.MaxHp,
                value => _mainCtrl.GetCurUnitExtra().MaxHp = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usJumpSetting, EAdvanceAttribute.JumpAbility,
                value => _mainCtrl.GetCurUnitExtra().JumpAbility = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usMoveSpeedSetting, EAdvanceAttribute.MaxSpeedX,
                value => _mainCtrl.GetCurUnitExtra().MaxSpeedX = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usDamageSetting, EAdvanceAttribute.Damage,
                value => _mainCtrl.GetCurUnitExtra().Damage = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usEffectRangeSetting, EAdvanceAttribute.EffectRange,
                value =>
                {
                    _mainCtrl.EditData.UnitExtra.EffectRange = (ushort) value;
                    if (_curMenu == EMenu.ActorSetting)
                    {
                        _mainCtrl.EditData.UnitExtra.CastRange = (ushort) value;
                    }
                });
            UnitExtraHelper.SetUSCtrlSliderSetting(_usCastRangeSetting, EAdvanceAttribute.CastRange,
                value => _mainCtrl.GetCurUnitExtra().CastRange = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usDamageIntervalSetting, EAdvanceAttribute.TimeInterval,
                value => _mainCtrl.GetCurUnitExtra().TimeInterval = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usBulletSpeedSetting, EAdvanceAttribute.BulletSpeed,
                value => _mainCtrl.GetCurUnitExtra().BulletSpeed = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usBulletCountSetting, EAdvanceAttribute.BulletCount,
                value => _mainCtrl.GetCurUnitExtra().BulletCount = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usChargeTimeSetting, EAdvanceAttribute.ChargeTime,
                value => _mainCtrl.GetCurUnitExtra().ChargeTime = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usInjuredReduceSetting, EAdvanceAttribute.InjuredReduce,
                value => _mainCtrl.GetCurUnitExtra().InjuredReduce = (byte) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usCurIncreaseSetting, EAdvanceAttribute.CureIncrease,
                value => _mainCtrl.GetCurUnitExtra().CureIncrease = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usMonsterIntervalTimeSetting, EAdvanceAttribute.MonsterIntervalTime,
                value => _mainCtrl.EditData.UnitExtra.MonsterIntervalTime = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usMaxCreatedMonsterSetting, EAdvanceAttribute.MaxCreatedMonster,
                value => _mainCtrl.EditData.UnitExtra.MaxCreatedMonster = (ushort) value);
            UnitExtraHelper.SetUSCtrlSliderSetting(_usMaxAliveMonsterSetting, EAdvanceAttribute.MaxAliveMonster,
                value => _mainCtrl.EditData.UnitExtra.MaxAliveMonster = (byte) value);
            _usCanMoveSetting = new USCtrlGameSettingItem();
            _usCanMoveSetting.Init(_cachedView.CanMoveSetting);
            _usDropsSetting = new USCtrlAddItem();
            _usDropsSetting.Init(_cachedView.DropsSetting);
            _usAddStatesSetting = new USCtrlAddItem();
            _usAddStatesSetting.Init(_cachedView.AddStatesSetting);
            _usSpawnSetting = new USCtrlDropdownSetting();
            _usSpawnSetting.Init(_cachedView.SpawnSetting);
            for (int i = 0; i < TeamManager.MaxTeamCount; i++)
            {
                int teamId = i + 1;
                _usSpawnSetting.SetSprite(i, TeamManager.GetSpawnSprite(teamId));
            }

            _usSpawnSetting.AddOnTeamChangedListener(index => _mainCtrl.OnTeamChanged(index + 1));
            _cachedView.MonsterSettingBtn.onClick.AddListener(() => _mainCtrl.OnMonsterSettingBtn());
            _cachedView.BackBtn.onClick.AddListener(() => _mainCtrl.Reset());
            _usPlayerWeaponSetting = new USCtrSetItem();
            _usPlayerWeaponSetting.Init(_cachedView.PlayerWeaponSetting);
            _usPlayerWeaponSetting.AddItemClickListener(index => _mainCtrl.OnPlayerWeaponSettingBtn(index));
            _usPlayerWeaponSetting.AddDeleteItemBtnListener
            (
                index =>
                {
                    var playerUnitExtra =
                        _mainCtrl.EditData.UnitExtra.InternalUnitExtras.Get<UnitExtraDynamic>(_mainCtrl
                            .CurSelectedPlayerIndex);
                    playerUnitExtra.InternalUnitExtras.Set<UnitExtraDynamic>(null, index);
                    _usPlayerWeaponSetting.SetCur(playerUnitExtra.InternalUnitExtras.ToList<UnitExtraDynamic>());
                });
        }

        public override void Open()
        {
            base.Open();
            _mainCtrl.CloseUpCtrlPanel(false);
            _cachedView.AdvancePannel.SetActive(true);
            _cachedView.AdvanceContentRtf.anchoredPosition = Vector2.zero;
            OpenAnimation();
            RefreshView();
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            int id = _mainCtrl.CurId;
            var table = TableManager.Instance.GetUnit(id);
            _usMaxHpSetting.SetEnable(_curMenu == EMenu.ActorSetting &&
                                      UnitExtraHelper.CanEdit(EAdvanceAttribute.MaxHp, id));
            _usJumpSetting.SetEnable(_curMenu == EMenu.ActorSetting &&
                                     UnitExtraHelper.CanEdit(EAdvanceAttribute.JumpAbility, id));
            _usMoveSpeedSetting.SetEnable(_curMenu == EMenu.ActorSetting &&
                                          UnitExtraHelper.CanEdit(EAdvanceAttribute.MaxSpeedX, id));
            _usEffectRangeSetting.SetEnable(_curMenu == EMenu.ActorSetting &&
                                            UnitExtraHelper.CanEdit(EAdvanceAttribute.EffectRange, id));
            _usCastRangeSetting.SetEnable(_curMenu == EMenu.WeaponSetting &&
                                          UnitExtraHelper.CanEdit(EAdvanceAttribute.CastRange, id));
            var b = _curMenu == EMenu.ActorSetting && table.SkillId > 0 ||
                    _curMenu == EMenu.WeaponSetting && table.ChildState != null;
            _usDamageSetting.SetEnable(b);
            _usDamageIntervalSetting.SetEnable(b);
            _usBulletSpeedSetting.SetEnable(_curMenu == EMenu.WeaponSetting &&
                                            UnitExtraHelper.CanEdit(EAdvanceAttribute.BulletSpeed, id));
            _usBulletCountSetting.SetEnable(_curMenu == EMenu.WeaponSetting &&
                                            UnitExtraHelper.CanEdit(EAdvanceAttribute.BulletCount, id));
            _usChargeTimeSetting.SetEnable(_curMenu == EMenu.WeaponSetting &&
                                           UnitExtraHelper.CanEdit(EAdvanceAttribute.ChargeTime, id));
            _usInjuredReduceSetting.SetEnable(_curMenu == EMenu.ActorSetting &&
                                              UnitExtraHelper.CanEdit(EAdvanceAttribute.InjuredReduce, id));
            _usCurIncreaseSetting.SetEnable(_curMenu == EMenu.ActorSetting &&
                                            UnitExtraHelper.CanEdit(EAdvanceAttribute.CureIncrease, id));
            _usMonsterIntervalTimeSetting.SetEnable(_curMenu == EMenu.MonsterCave);
            _usMaxCreatedMonsterSetting.SetEnable(_curMenu == EMenu.MonsterCave);
            _usMaxAliveMonsterSetting.SetEnable(_curMenu == EMenu.MonsterCave);
            _usCanMoveSetting.SetEnable(_curMenu == EMenu.ActorSetting &&
                                        UnitExtraHelper.CanEdit(EAdvanceAttribute.MaxSpeedX, id) &&
                                        !UnitDefine.IsSpawn(id));
            _cachedView.MonsterSettingBtn.SetActiveEx(_curMenu == EMenu.MonsterCave);
            _cachedView.BackBtn.SetActiveEx(_mainCtrl.CurEnterType != UICtrlUnitPropertyEdit.EEnterType.Normal);
//            _usDropsSetting.SetEnable(_curMenu == EMenu.ActorSetting &&
//                                      UnitExtraHelper.CanEdit(EAdvanceAttribute.Drops, id));
//            _usAddStatesSetting.SetEnable(b);
            _usAddStatesSetting.SetEnable(false);
            _usDropsSetting.SetEnable(false);
            _usPlayerWeaponSetting.SetEnable(UnitDefine.IsSpawn(id));
            _usSpawnSetting.SetEnable(
                UnitDefine.IsSpawn(id) && _mainCtrl.Project.ProjectType == EProjectType.PS_Compete);
            _usMaxHpSetting.SetCur(_mainCtrl.GetCurUnitExtra().MaxHp);
            _usJumpSetting.SetCur(_mainCtrl.EditData.UnitExtra.JumpAbility);
            var maxSpeedX = _mainCtrl.EditData.UnitExtra.MaxSpeedX;
            if (maxSpeedX == ushort.MaxValue)
            {
                if (table != null)
                {
                    maxSpeedX = (ushort) table.MaxSpeed;
                }
            }

            _usMoveSpeedSetting.SetCur(maxSpeedX);
            _usDamageSetting.SetCur(_mainCtrl.GetCurUnitExtra().Damage);
            var minEffectRange = UnitExtraHelper.GetMin(EAdvanceAttribute.EffectRange, _curMenu);
            _usEffectRangeSetting.SetCur(_mainCtrl.EditData.UnitExtra.EffectRange, true, minEffectRange);
            _usCastRangeSetting.SetCur(_mainCtrl.GetCurUnitExtra().CastRange);
            var minAttackInterval = UnitExtraHelper.GetMin(EAdvanceAttribute.TimeInterval, _curMenu);
            _usDamageIntervalSetting.SetCur(_mainCtrl.GetCurUnitExtra().TimeInterval, true, minAttackInterval);
            _usBulletSpeedSetting.SetCur(_mainCtrl.GetCurUnitExtra().BulletSpeed);
            _usBulletCountSetting.SetCur(_mainCtrl.GetCurUnitExtra().BulletCount);
            _usChargeTimeSetting.SetCur(_mainCtrl.GetCurUnitExtra().ChargeTime);
            _usInjuredReduceSetting.SetCur(_mainCtrl.GetCurUnitExtra().InjuredReduce);
            _usCurIncreaseSetting.SetCur(_mainCtrl.GetCurUnitExtra().CureIncrease);
            _usMonsterIntervalTimeSetting.SetCur(_mainCtrl.EditData.UnitExtra.MonsterIntervalTime);
            _usMaxCreatedMonsterSetting.SetCur(_mainCtrl.EditData.UnitExtra.MaxCreatedMonster);
            _usMaxAliveMonsterSetting.SetCur(_mainCtrl.EditData.UnitExtra.MaxAliveMonster);
            _usCanMoveSetting.SetData(_mainCtrl.EditData.UnitExtra.MaxSpeedX != ushort.MaxValue, value =>
            {
                _usMoveSpeedSetting.SetEnable(value);
                if (value)
                {
                    _mainCtrl.EditData.UnitExtra.MaxSpeedX = (ushort) _usMoveSpeedSetting.Cur;
                }
                else
                {
                    _mainCtrl.EditData.UnitExtra.MaxSpeedX = ushort.MaxValue;
                }
            });
            _usDropsSetting.Set(_mainCtrl.EditData.UnitExtra.Drops, USCtrlAddItem.EItemType.Drops);
            _usAddStatesSetting.Set(_mainCtrl.EditData.UnitExtra.AddStates, USCtrlAddItem.EItemType.States);
            if (_mainCtrl.CurEditType == EEditType.Spawn)
            {
                _usSpawnSetting.SetCur(_mainCtrl.GetCurUnitExtra().TeamId - 1);
                _usPlayerWeaponSetting.SetCur(_mainCtrl.GetCurUnitExtra().InternalUnitExtras
                    .ToList<UnitExtraDynamic>());
            }
        }

        public override void Close()
        {
            if (_openAnim)
            {
                CloseAnimation();
            }
            else if (_closeSequence == null || !_closeSequence.IsPlaying())
            {
                _cachedView.AdvancePannel.SetActiveEx(false);
            }

            base.Close();
        }

        private void OpenAnimation()
        {
            if (null == _openSequence)
            {
                CreateSequences();
            }

            if (_closeSequence.IsPlaying())
            {
                _closeSequence.Complete(true);
            }

            _cachedView.AdvancePannel.SetActiveEx(true);
            _openSequence.Restart();
            _openAnim = true;
        }

        private void CloseAnimation()
        {
            if (null == _closeSequence)
            {
                CreateSequences();
            }

            if (_openSequence.IsPlaying())
            {
                _openSequence.Complete(true);
            }

            if (_completeAnim)
            {
                _closeSequence.Complete(true);
                _completeAnim = false;
            }
            else
            {
                _closeSequence.PlayForward();
            }

            _openAnim = false;
        }

        private void CreateSequences()
        {
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            _openSequence.Append(
                _cachedView.AdvancePannel.transform.DOBlendableMoveBy(Vector3.left * 600, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause();
            _closeSequence.Append(_cachedView.AdvancePannel.transform.DOBlendableMoveBy(Vector3.left * 600, 0.3f)
                .SetEase(Ease.InOutQuad)).OnComplete(OnCloseAnimationComplete).SetAutoKill(false).Pause();
        }

        public void OpenMenu(EMenu eMenu)
        {
            _curMenu = eMenu;
            Open();
        }

        public void CheckClose()
        {
            if (_isOpen)
            {
                _completeAnim = true;
                Close();
            }
        }

        private void OnCloseAnimationComplete()
        {
            _cachedView.AdvancePannel.SetActiveEx(false);
            _closeSequence.Rewind();
        }

        public void OnChildIdChanged()
        {
            _usDamageSetting.SetCur(_mainCtrl.GetCurUnitExtra().Damage);
            _usCastRangeSetting.SetCur(_mainCtrl.GetCurUnitExtra().CastRange);
            _usDamageIntervalSetting.SetCur(_mainCtrl.GetCurUnitExtra().TimeInterval);
            _usBulletSpeedSetting.SetCur(_mainCtrl.GetCurUnitExtra().BulletSpeed);
            _usBulletCountSetting.SetCur(_mainCtrl.GetCurUnitExtra().BulletCount);
            _usChargeTimeSetting.SetCur(_mainCtrl.GetCurUnitExtra().ChargeTime);
            _usAddStatesSetting.Set(_mainCtrl.GetCurUnitExtra().AddStates, USCtrlAddItem.EItemType.States);
        }
    }
}