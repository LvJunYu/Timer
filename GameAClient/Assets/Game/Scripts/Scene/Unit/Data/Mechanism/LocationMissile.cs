using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    [Unit(Id = 5026, Type = typeof(LocationMissile))]
    public class LocationMissile : BlockBase
    {
        private const string SpriteFormat = "M1LocationMissile_{0}";
        private const string IconSprite = "M1LocationMissileIcon";
        private const float RotateSpeed = 1;
        private SkillCtrl _skillCtrl;
        private ERotateMode _eRotateType;
        private float _startAngle;
        private float _endAngle;
        private float _curAngle;
        private float _targetAngle;
        private int _attackInterval;
        private int _castRange;
        private int _teamId;
        private SpriteRenderer _gunRenderer;
        private EState _curState;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        public override float Angle
        {
            get { return _curAngle; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }

            SetSortingOrderBackground();
            return true;
        }

        protected override void SetSkillValue()
        {
            _skillCtrl.CurrentSkills[0].SkillBase.SetValue(_attackInterval, _castRange);
        }

        public override UnitExtraDynamic UpdateExtraData()
        {
            var unitExtra = base.UpdateExtraData();
            _eRotateType = (ERotateMode) unitExtra.RotateMode;
            _startAngle = GM2DTools.GetAngle(unitExtra.ChildRotation);
            _endAngle = _eRotateType == ERotateMode.None ? _startAngle : GM2DTools.GetAngle(unitExtra.RotateValue);
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
            RefreshGunDir();
            return true;
        }

        protected override void Clear()
        {
            _curAngle = _startAngle;
            RefreshGunDir();
            _skillCtrl = null;
            _curState = EState.Normal;
            base.Clear();
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            SetSkill();
        }

        internal override void OnObjectDestroy()
        {
            if (_gunRenderer != null)
            {
                Object.Destroy(_gunRenderer.gameObject);
                _gunRenderer = null;
            }

            base.OnObjectDestroy();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_eActiveState != EActiveState.Active)
            {
                return;
            }

            switch (_curState)
            {
                case EState.Normal:
                    if (_eRotateType != ERotateMode.None)
                    {
                        switch (_eRotateType)
                        {
                            case ERotateMode.Clockwise:
                                _curAngle += RotateSpeed;
                                break;
                            case ERotateMode.Anticlockwise:
                                _curAngle += -RotateSpeed;
                                break;
                        }

                        Util.CorrectAngle360(ref _curAngle);
                        if (!Util.IsFloatEqual(_startAngle, _endAngle))
                        {
                            if (Util.IsFloatEqual(_curAngle, _startAngle) || Util.IsFloatEqual(_curAngle, _endAngle))
                            {
                                _eRotateType = _eRotateType == ERotateMode.Clockwise
                                    ? ERotateMode.Anticlockwise
                                    : ERotateMode.Clockwise;
                            }
                        }

                        RefreshGunDir();
                        if (GameRun.Instance.LogicFrameCnt % 20 == 0 && CheckTarget())
                        {
                            ChangeState(EState.AimTarget);
                        }
                    }

                    break;
                case EState.AimTarget:
                    _curAngle = Mathf.MoveTowardsAngle(_curAngle, _targetAngle, RotateSpeed * 5);
                    RefreshGunDir();
                    if (Util.IsFloatEqual(_curAngle, _targetAngle))
                    {
                        ChangeState(EState.Fire);
                    }

                    break;
                case EState.Fire:
                    break;
            }
        }

        private bool CheckTarget()
        {
            var players = TeamManager.Instance.Players;
            float minDisSqr = int.MaxValue;
            bool findTarget = false;
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (player == null)
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

                    if (IsAngleValid(angle) && distanceSqr < minDisSqr)
                    {
                        findTarget = true;
                        minDisSqr = distanceSqr;
                        _targetAngle = angle;
                    }
                }
            }

            return findTarget;
        }

        private bool IsAngleValid(float angle)
        {
            if (_endAngle < _startAngle)
            {
                return angle >= _endAngle && angle <= _startAngle;
            }

            if (_endAngle > _startAngle)
            {
                return angle >= _endAngle || angle <= _startAngle;
            }

            return Util.IsFloatEqual(_startAngle, angle);
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
                if (_skillCtrl != null)
                {
                    _skillCtrl.UpdateLogic();
                    if (_skillCtrl.Fire(0))
                    {
//                    if (_animation != null)
//                    {
//                        _animation.PlayOnce("Start");
//                    }
                    }
                }
            }
        }

        private void SetGunView()
        {
            if (_view == null) return;
            if (_gunRenderer == null)
            {
                _gunRenderer = new GameObject("Gun").AddComponent<SpriteRenderer>();
                CommonTools.SetParent(_gunRenderer.transform, _trans);
                _gunRenderer.sortingOrder = (int) ESortingOrder.Item;
                _gunRenderer.transform.localPosition = new Vector3(0, 0, -0.01f);
            }

            _gunRenderer.sprite = GetLocationMissileSprite(_teamId);
        }

        private void RefreshGunDir()
        {
            if (_gunRenderer == null)
            {
                return;
            }

            _gunRenderer.transform.localEulerAngles = new Vector3(0, 0, 180 - _curAngle);
        }

        private void SetSkill()
        {
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this);
            _skillCtrl.SetSkill(_tableUnit.SkillId);
            SetSkillValue();
        }

        public static Sprite GetLocationMissileSprite(int teamId)
        {
            if (teamId == 0)
            {
                return JoyResManager.Instance.GetSprite(string.Format(SpriteFormat, 0));
            }

            return JoyResManager.Instance.GetSprite(string.Format(SpriteFormat, TeamManager.GetTeamColorName(teamId)));
        }

        public static Sprite GetLocationMissileIconSprite(int teamId)
        {
            if (teamId == 0)
            {
                return JoyResManager.Instance.GetSprite(IconSprite);
            }

            return JoyResManager.Instance.GetSprite(string.Format(SpriteFormat, TeamManager.GetTeamColorName(teamId)));
        }

        public enum EState
        {
            Normal,
            AimTarget,
            Fire
        }
    }
}