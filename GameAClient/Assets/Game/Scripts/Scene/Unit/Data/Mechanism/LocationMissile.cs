using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5026, Type = typeof(LocationMissile))]
    public class LocationMissile : BlockBase
    {
        private const int GunId = 5029;
        private const string IconSpriteFormat = "M1LocationMissileIcon_{0}";
        private const string IconSprite = "M1LocationMissileIcon";
        private const int AttackAimTime = 20;
        private const int LineAngle = 5; //用于判断直线射程时的角度
        private const float RotateSpeed = 1;
        private const float AimSpeed = 5;
        private SkillCtrl _skillCtrl;
        private ERotateMode _eRotateType;
        private float _startAngle;
        private float _endAngle;
        private float _curAngle;
        private float _targetAngle;
        private int _attackInterval;
        private int _castRange;
        private int _teamId;
        private LocationMissileGun _gun;
        private EState _curState;
        private int _attackTimer;
        private bool _allCircle;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        public override float Angle
        {
            get { return _curAngle; }
        }

        private IntVec2 _bulletOffsetPos;

        public IntVec2 BulletOffsetPos
        {
            get { return _bulletOffsetPos; }
        }

        protected override void SetSkillValue()
        {
            _skillCtrl.CurrentSkills[0].SkillBase.SetValue(_attackInterval, _castRange, 10);
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _eRotateType = (ERotateMode) unitExtra.RotateMode;
            _startAngle = GM2DTools.GetAngle(unitExtra.ChildRotation);
            _endAngle = _eRotateType == ERotateMode.None ? _startAngle : GM2DTools.GetAngle(unitExtra.RotateValue);
            _allCircle = Util.IsFloatEqual(_startAngle, _endAngle) && _eRotateType != ERotateMode.None;
            //保证startAngle比endAngle大
            if (_startAngle < _endAngle || _allCircle)
            {
                _startAngle += 360;
            }

            _attackInterval = TableConvert.GetTime(unitExtra.TimeInterval);
            _castRange = TableConvert.GetRange(unitExtra.CastRange);
            _teamId = unitExtra.TeamId;
            return unitExtra;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            SetGunView();
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _curAngle = _startAngle;
            SetGunView();
            _skillCtrl = null;
            _curState = EState.Rotate;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            SetSkill();
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            if (_gun != null)
            {
                PlayMode.Instance.DestroyUnit(_gun);
                _gun = null;
            }
        }

        public override void UpdateLogic()
        {
            if (_attackTimer > 0)
            {
                _attackTimer--;
            }

            base.UpdateLogic();

            if (_eActiveState != EActiveState.Active)
            {
                return;
            }

            if (_skillCtrl != null)
            {
                _skillCtrl.UpdateLogic();
            }

            if (_curState == EState.Rotate)
            {
                if (GameRun.Instance.LogicFrameCnt % 5 == 0 && CheckFindTarget())
                {
                    ChangeState(EState.AimTarget);
                }
                else
                {
                    DoRotate();
                }
            }

            if (_curState == EState.AimTarget)
            {
                DoAimTarget();
                if (CheckCanAttack())
                {
                    ChangeState(EState.Fire);
                }
            }
            else if (_curState == EState.Fire)
            {
                if (_attackTimer > 0)
                {
                    if (_attackTimer == 1)
                    {
                        if (CheckFindTarget())
                        {
                            ChangeState(EState.AimTarget);
                        }
                        else
                        {
                            ChangeState(EState.Rotate);
                        }
                    }
                }
            }
        }

        public override bool CanHarm(UnitBase unit)
        {
            return !IsSameTeam(unit.TeamId);
        }

        private void DoAttack()
        {
            _attackTimer = 5;
            if (_skillCtrl != null && _skillCtrl.Fire(0))
            {
                _attackTimer = AttackAimTime;
                if (_gun != null)
                {
                    _gun.PlayAttackAnim();
                }
            }
        }

        private void DoAimTarget()
        {
            if (_eRotateType == ERotateMode.None)
            {
                return;
            }

            _curAngle = Mathf.MoveTowards(_curAngle, _targetAngle, AimSpeed);
            RefreshGunDir();
        }

        private void DoRotate()
        {
            if (_eRotateType == ERotateMode.None)
            {
                return;
            }

            if (_allCircle)
            {
                _eRotateType = ERotateMode.Anticlockwise;
            }
            else
            {
                if (_curAngle >= _startAngle)
                {
                    _eRotateType = ERotateMode.Anticlockwise;
                }
                else if (_curAngle <= _endAngle)
                {
                    _eRotateType = ERotateMode.Clockwise;
                }
            }

            switch (_eRotateType)
            {
                case ERotateMode.Clockwise:
                    _curAngle += RotateSpeed;
                    break;
                case ERotateMode.Anticlockwise:
                    _curAngle -= RotateSpeed;
                    break;
            }

            RefreshGunDir();
        }

        private bool CheckCanAttack()
        {
            return _eRotateType == ERotateMode.None || Util.IsFloatEqual(_curAngle, _targetAngle);
        }

        private bool CheckFindTarget()
        {
            var players = TeamManager.Instance.Players;
            float minDisSqr = int.MaxValue;
            bool findTarget = false;
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (player == null || !CanHarm(player))
                {
                    continue;
                }

                var relPos = player.CenterDownPos - CenterDownPos;
                var distanceSqr = relPos.SqrMagnitude();
                if (distanceSqr <= _castRange * _castRange)
                {
                    var angle = Vector2.Angle(Vector2.up, GM2DTools.TileToWorld(relPos));
                    Util.CorrectAngle360(ref angle);
                    if (relPos.x < 0)
                    {
                        angle = 360 - angle;
                    }

                    if (IsAngleValid(ref angle) && distanceSqr < minDisSqr)
                    {
                        findTarget = true;
                        minDisSqr = distanceSqr;
                        _targetAngle = angle;
                    }
                }
            }

            return findTarget;
        }

        private bool IsAngleValid(ref float angle)
        {
            //直线
            if (_eRotateType == ERotateMode.None)
            {
                return Mathf.Abs(_endAngle - angle) < LineAngle;
            }

            angle = (int) angle;
            if (angle < _endAngle)
            {
                angle += 360;
            }

            return angle <= _startAngle;
        }

        private void ChangeState(EState state)
        {
            if (_curState == state)
            {
                return;
            }

            _curState = state;
            if (_curState == EState.Fire)
            {
                DoAttack();
            }
        }

        private void SetGunView()
        {
            if (_view == null) return;
            if (_gun == null)
            {
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() =>
                {
                    CreateGun();
                    _bulletOffsetPos = GM2DTools.WorldToTile(new Vector2(-0.444f, -0.37f));
                    _gun.ChangeView(_teamId);
                    RefreshGunDir();
                }));
            }
        }

        private void RefreshGunDir()
        {
            if (_gun != null)
            {
                _gun.RefreshGunDir(_curAngle);
            }
        }

        private bool CreateGun()
        {
            if (_gun != null)
            {
                return false;
            }

            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(GunId);
            if (tableUnit == null)
            {
                LogHelper.Error("GetTableUnit Failed, {0}", GunId);
                return false;
            }

            IntVec3 guid = new IntVec3(_curPos.x, _curPos.y, GM2DTools.GetRuntimeCreatedUnitDepth());
            _gun = PlayMode.Instance.CreateUnit(new UnitDesc(GunId, guid, 0, Vector2.one)) as LocationMissileGun;
            if (_gun == null)
            {
                LogHelper.Error("CreateUnit Gun Faield, {0}", ToString());
                return false;
            }

            CommonTools.SetParent(_gun.Trans, _trans);
            _gun.Trans.localPosition = new Vector3(-0.144f, -0.07f, -0.01f);
            _gun.OnPlay();
            return true;
        }

        private void SetSkill()
        {
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this);
            _skillCtrl.SetSkill(_tableUnit.SkillId);
            SetSkillValue();
        }

        public static Sprite GetLocationMissileIconSprite(int teamId)
        {
            if (teamId == 0)
            {
                return JoyResManager.Instance.GetSprite(IconSprite);
            }

            return JoyResManager.Instance.GetSprite(string.Format(IconSpriteFormat,
                TeamManager.GetTeamColorName(teamId)));
        }

        public enum EState
        {
            Rotate,
            AimTarget,
            Fire
        }
    }
}