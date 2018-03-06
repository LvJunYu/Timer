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
        private SpriteRenderer _gunRenderer;
        private EState _curState;
        private int _attackTimer;

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
            _skillCtrl.CurrentSkills[0].SkillBase.SetValue(_attackInterval, _castRange);
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
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
            _curState = EState.Rotate;
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

        private void DoAttack()
        {
            _attackTimer = 5;
            if (_skillCtrl != null && _skillCtrl.Fire(0))
            {
                _attackTimer = AttackAimTime;
//                if (_animation != null)
//                {
//                    _animation.PlayOnce("Start");
//                }
            }
        }

        private void DoAimTarget()
        {
            if (_eRotateType == ERotateMode.None)
            {
                return;
            }

            _curAngle = Mathf.MoveTowardsAngle(_curAngle, _targetAngle, AimSpeed);
            RefreshGunDir();
        }

        private void DoRotate()
        {
            if (_eRotateType == ERotateMode.None)
            {
                return;
            }

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

            if (findTarget)
            {
                _targetAngle = (int) _targetAngle;
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

            //直线
            if (_eRotateType == ERotateMode.None)
            {
                return Mathf.Abs(_startAngle - angle) < LineAngle;
            }

            //360°旋转
            return true;
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
            if (_gunRenderer == null)
            {
                _gunRenderer = new GameObject("Gun").AddComponent<SpriteRenderer>();
                CommonTools.SetParent(_gunRenderer.transform, _trans);
                _gunRenderer.sortingOrder = (int) ESortingOrder.Item;
                _gunRenderer.transform.localPosition = new Vector3(-0.144f, -0.07f, -0.01f);
                _bulletOffsetPos = GM2DTools.WorldToTile(new Vector2(-0.444f, -0.37f));
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