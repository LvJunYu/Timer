using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5027, Type = typeof(GravitySensor))]
    public class GravitySensor : BlockBase
    {
        private bool _trigger;
        private int _oriY;
        private EMoveDirection _lastDirection;
        private int _lastSpeedX;
        private int _activeSpeedX;
        private bool _isUpDown;
        private bool _canControllledBySwitch;

        public override bool CanControlledBySwitch
        {
            get { return _canControllledBySwitch; }
        }

        public override bool UseMagic()
        {
            return true;
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }

            _oriY = _curPos.y;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            if (GameRun.Instance.IsPlaying && _animation != null)
            {
                _animation.PlayLoop("Open");
            }

            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            if (_animation != null)
            {
                _animation.PlayLoop("Open");
            }
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _lastDirection = _moveDirection;
            _canControllledBySwitch = _moveDirection != EMoveDirection.None;
            return unitExtra;
        }

        public override void UpdateLogic()
        {
            if (!_isAlive) return;
            if (_isUpDown)
            {
                IsUpDown(false);
            }

            if (_trigger)
            {
                IsUpDown(true, EMoveDirection.Down);
                _trigger = false;
            }
            else
            {
                if (_curPos.y < _oriY)
                {
                    IsUpDown(true, EMoveDirection.Up);
                }
            }

            if (Speed == IntVec2.zero && _eActiveState == EActiveState.Active)
            {
                CheckMagicDir();
            }

            if (Speed != IntVec2.zero)
            {
                DoMagic();
            }

            if (!_isUpDown)
            {
                _lastSpeedX = SpeedX;
            }
        }

        public override void UpdateView(float deltaTime)
        {
            if (_isAlive)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
                if (GameModeBase.DebugEnable())
                {
                    GameModeBase.WriteDebugData(string.Format("Type = {2}, Guid == {0} _trans.position = {1} ", Guid,
                        _trans.position, GetType().Name));
                }
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsRigidbody)
            {
                if (other.DownUnits.Count <= 1)
                {
                    _trigger = true;
                }
            }

            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            if (!_enabled)
            {
                return IntVec2.zero;
            }

            _deltaImpactPos = _deltaPos;
            return _deltaImpactPos;
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            if (GameRun.Instance.IsPlaying)
            {
                if (_eActiveState == EActiveState.Active)
                {
                    _lastSpeedX = SpeedX = _activeSpeedX;
                }
                else
                {
                    _activeSpeedX = _lastSpeedX;
                    _lastSpeedX = SpeedX = 0;
                }
            }
        }

        protected override void Clear()
        {
            base.Clear();
            _isUpDown = false;
            _trigger = false;
            _lastSpeedX = SpeedX;
            _activeSpeedX = 0;
        }

        private void IsUpDown(bool value, EMoveDirection dir = EMoveDirection.None)
        {
            _isUpDown = value;
            if (value)
            {
                SpeedX = 0;
                _lastDirection = _moveDirection;
                switch (dir)
                {
                    case EMoveDirection.Up:
                        SpeedY = Mathf.Min(_velocity, _oriY - _curPos.y);
                        _moveDirection = EMoveDirection.Up;
                        break;
                    case EMoveDirection.Down:
                        SpeedY = -_velocity;
                        _moveDirection = EMoveDirection.Down;
                        break;
                }
            }
            else
            {
                SpeedX = _lastSpeedX;
                _moveDirection = _lastDirection;
                SpeedY = 0;
            }
        }
    }
}