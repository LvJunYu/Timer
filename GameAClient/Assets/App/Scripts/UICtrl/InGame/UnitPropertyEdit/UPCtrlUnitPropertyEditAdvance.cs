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
            Camp,
            WeaponSetting,
            MonsterCave,
            Timer,
            SurpriseBox,
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
        private USCtrlSurpriseBoxItemsSetting _usSurpriseBoxItemSetting;
        private USCtrSetItem _usPlayerWeaponSetting;
        private EMenu _curMenu;
        private UnitExtraDynamic _curUnitExtra;
        private UPCtrlUnitPropertyAdvanceTimer _upCtrlTimer;

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
                    if (_curMenu == EMenu.Camp)
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

            _upCtrlTimer = new UPCtrlUnitPropertyAdvanceTimer();
            _upCtrlTimer.Init(_mainCtrl, _cachedView);

            _usSurpriseBoxItemSetting = new USCtrlSurpriseBoxItemsSetting();
            _usSurpriseBoxItemSetting.SetResScenary(_mainCtrl.ResScenary);
            _usSurpriseBoxItemSetting.Init(_cachedView.SurpriseBoxItemSetting);
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

        public void OpenMenu(EMenu eMenu)
        {
            _curMenu = eMenu;
            Open();
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            int id = _mainCtrl.CurId;
            var table = TableManager.Instance.GetUnit(id);
            if (CanEdit(EAdvanceAttribute.MaxHp))
            {
                _usMaxHpSetting.SetEnable(true);
                _usMaxHpSetting.SetCur(_mainCtrl.GetCurUnitExtra().MaxHp);
            }
            else
            {
                _usMaxHpSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.JumpAbility))
            {
                _usJumpSetting.SetEnable(true);
                _usJumpSetting.SetCur(_mainCtrl.GetCurUnitExtra().JumpAbility);
            }
            else
            {
                _usJumpSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.MaxSpeedX))
            {
                _usMoveSpeedSetting.SetEnable(true);
                var maxSpeedX = _mainCtrl.GetCurUnitExtra().MaxSpeedX;
                if (maxSpeedX == ushort.MaxValue)
                {
                    if (table != null)
                    {
                        maxSpeedX = (ushort) table.MaxSpeed;
                    }
                }

                _usMoveSpeedSetting.SetCur(maxSpeedX);
            }
            else
            {
                _usMoveSpeedSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.EffectRange))
            {
                _usEffectRangeSetting.SetEnable(true);
                var minEffectRange = UnitExtraHelper.GetMin(EAdvanceAttribute.EffectRange, _curMenu);
                _usEffectRangeSetting.SetCur(_mainCtrl.GetCurUnitExtra().EffectRange, true, minEffectRange);
            }
            else
            {
                _usEffectRangeSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.CastRange))
            {
                _usCastRangeSetting.SetEnable(true);
                _usCastRangeSetting.SetCur(_mainCtrl.GetCurUnitExtra().CastRange);
            }
            else
            {
                _usCastRangeSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.Damage))
            {
                _usDamageSetting.SetEnable(true);
                _usDamageSetting.SetCur(_mainCtrl.GetCurUnitExtra().Damage);
            }
            else
            {
                _usDamageSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.TimeInterval))
            {
                _usDamageIntervalSetting.SetEnable(true);
                var minAttackInterval = UnitExtraHelper.GetMin(EAdvanceAttribute.TimeInterval, _curMenu);
                _usDamageIntervalSetting.SetCur(_mainCtrl.GetCurUnitExtra().TimeInterval, true, minAttackInterval);
            }
            else
            {
                _usDamageIntervalSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.BulletSpeed))
            {
                _usBulletSpeedSetting.SetEnable(true);
                _usBulletSpeedSetting.SetCur(_mainCtrl.GetCurUnitExtra().BulletSpeed);
            }
            else
            {
                _usBulletSpeedSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.BulletCount))
            {
                _usBulletCountSetting.SetEnable(true);
                _usBulletCountSetting.SetCur(_mainCtrl.GetCurUnitExtra().BulletCount);
            }
            else
            {
                _usBulletCountSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.ChargeTime))
            {
                _usChargeTimeSetting.SetEnable(true);
                _usChargeTimeSetting.SetCur(_mainCtrl.GetCurUnitExtra().ChargeTime);
            }
            else
            {
                _usChargeTimeSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.InjuredReduce))
            {
                _usInjuredReduceSetting.SetEnable(true);
                _usInjuredReduceSetting.SetCur(_mainCtrl.GetCurUnitExtra().InjuredReduce);
            }
            else
            {
                _usInjuredReduceSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.CureIncrease))
            {
                _usCurIncreaseSetting.SetEnable(true);
                _usCurIncreaseSetting.SetCur(_mainCtrl.GetCurUnitExtra().CureIncrease);
            }
            else
            {
                _usCurIncreaseSetting.SetEnable(false);
            }

            if (CanEdit(EAdvanceAttribute.MaxSpeedX))
            {
                _usCanMoveSetting.SetEnable(true);
                _usCanMoveSetting.SetData(_mainCtrl.GetCurUnitExtra().MaxSpeedX != ushort.MaxValue, value =>
                {
                    _usMoveSpeedSetting.SetEnable(value);
                    if (value)
                    {
                        _mainCtrl.GetCurUnitExtra().MaxSpeedX = (ushort) _usMoveSpeedSetting.Cur;
                    }
                    else
                    {
                        _mainCtrl.GetCurUnitExtra().MaxSpeedX = ushort.MaxValue;
                    }
                });
            }
            else
            {
                _usCanMoveSetting.SetEnable(false);
            }

            if (_mainCtrl.CurEditType == EEditType.Spawn)
            {
                _usPlayerWeaponSetting.SetEnable(true);
                _usPlayerWeaponSetting.SetCur(_mainCtrl.GetCurUnitExtra().InternalUnitExtras
                    .ToList<UnitExtraDynamic>());
                if (_mainCtrl.Project.ProjectType == EProjectType.PT_Compete)
                {
                    _usSpawnSetting.SetCur(_mainCtrl.GetCurUnitExtra().TeamId - 1);
                    _usSpawnSetting.SetEnable(true);
                }
                else
                {
                    _usSpawnSetting.SetEnable(false);
                }
            }
            else
            {
                _usPlayerWeaponSetting.SetEnable(false);
                _usSpawnSetting.SetEnable(false);
            }

//            _usDropsSetting.SetEnable(_curMenu == EMenu.ActorSetting &&
//                                      UnitExtraHelper.CanEdit(EAdvanceAttribute.Drops, id));
//            _usAddStatesSetting.SetEnable(b);
            _usAddStatesSetting.SetEnable(false);
            _usDropsSetting.SetEnable(false);
//            _usDropsSetting.Set(_mainCtrl.EditData.UnitExtra.Drops, USCtrlAddItem.EItemType.Drops);
//            _usAddStatesSetting.Set(_mainCtrl.EditData.UnitExtra.AddStates, USCtrlAddItem.EItemType.States);

            _usMonsterIntervalTimeSetting.SetEnable(_curMenu == EMenu.MonsterCave);
            _usMaxCreatedMonsterSetting.SetEnable(_curMenu == EMenu.MonsterCave);
            _usMaxAliveMonsterSetting.SetEnable(_curMenu == EMenu.MonsterCave);
            _cachedView.MonsterSettingBtn.SetActiveEx(_curMenu == EMenu.MonsterCave);
            if (_curMenu == EMenu.MonsterCave)
            {
                _usMonsterIntervalTimeSetting.SetCur(_mainCtrl.EditData.UnitExtra.MonsterIntervalTime);
                _usMaxCreatedMonsterSetting.SetCur(_mainCtrl.EditData.UnitExtra.MaxCreatedMonster);
                _usMaxAliveMonsterSetting.SetCur(_mainCtrl.EditData.UnitExtra.MaxAliveMonster);
            }

            _cachedView.BackBtn.SetActiveEx(_mainCtrl.CurEnterType != UICtrlUnitPropertyEdit.EEnterType.Normal);

            if (_curMenu == EMenu.SurpriseBox)
            {
                _usSurpriseBoxItemSetting.SetUnitExtra(_mainCtrl.GetCurUnitExtra());
                _usSurpriseBoxItemSetting.Open();
            }
            else
            {
                _usSurpriseBoxItemSetting.Close();
            }

            if (_curMenu == EMenu.Timer)
            {
                _upCtrlTimer.Open();
            }
            else
            {
                _upCtrlTimer.Close();
            }
        }

        public void CheckClose()
        {
            if (_isOpen)
            {
                _completeAnim = true;
                Close();
            }
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

        private void OnCloseAnimationComplete()
        {
            _cachedView.AdvancePannel.SetActiveEx(false);
            _closeSequence.Rewind();
        }

        private bool CanEdit(EAdvanceAttribute eAdvanceAttribute)
        {
            int id = _mainCtrl.CurId;
            var table = TableManager.Instance.GetUnit(id);
            if (table == null)
            {
                LogHelper.Error("cant get unit which id == {0}", id);
                return false;
            }

            switch (eAdvanceAttribute)
            {
                case EAdvanceAttribute.TimeInterval:
                case EAdvanceAttribute.Damage:
                    return _curMenu == EMenu.Camp && table.SkillId > 0 ||
                           _curMenu == EMenu.WeaponSetting && table.ChildState != null;
                case EAdvanceAttribute.Drops:
                    return UnitDefine.IsMonster(id);
                case EAdvanceAttribute.EffectRange:
                    return _curMenu == EMenu.Camp && table.SkillId > 0 && id != UnitDefine.LocationMissileId;
                case EAdvanceAttribute.ViewRange:
                    return false;
                case EAdvanceAttribute.BulletSpeed:
                case EAdvanceAttribute.CastRange:
                    return _curMenu == EMenu.WeaponSetting && table.ChildState != null ||
                           id == UnitDefine.LocationMissileId;
                case EAdvanceAttribute.BulletCount:
                case EAdvanceAttribute.ChargeTime:
                    return _curMenu == EMenu.WeaponSetting && UnitDefine.IsWeaponPool(id);
                case EAdvanceAttribute.AddStates:
                    return table.SkillId > 0 || table.ChildState != null;
                case EAdvanceAttribute.MaxSpeedX:
                    return _curMenu == EMenu.Camp && table.MaxSpeed > 0 && !UnitDefine.IsSpawn(id);
                case EAdvanceAttribute.JumpAbility:
                    return _curMenu == EMenu.Camp && table.JumpAbility > 0 && !UnitDefine.IsSpawn(id);
                case EAdvanceAttribute.MaxHp:
                case EAdvanceAttribute.InjuredReduce:
                case EAdvanceAttribute.CureIncrease:
                    return _curMenu == EMenu.Camp && table.Hp > 0;
            }

            return false;
        }
    }
}