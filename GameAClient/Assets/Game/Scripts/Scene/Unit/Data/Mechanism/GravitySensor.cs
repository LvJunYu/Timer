using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5027, Type = typeof(GravitySensor))]
    public class GravitySensor : BlockBase
    {
        private const int MoveSpeed = 20;
        private bool _trigger;
        private int _oriY;

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

        private EMoveDirection _lastDirection;

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _lastDirection = _moveDirection;
            return unitExtra;
        }

        public override void UpdateLogic()
        {
            SpeedY = 0;
            if (_moveDirection == EMoveDirection.Down || _moveDirection == EMoveDirection.Up)
            {
                _moveDirection = _lastDirection;
            }

            if (_trigger)
            {
                _lastDirection = _moveDirection;
                _moveDirection = EMoveDirection.Down;
                SpeedY = -MoveSpeed;
                _trigger = false;
            }
            else
            {
                if (_curPos.y < _oriY)
                {
                    _lastDirection = _moveDirection;
                    _moveDirection = EMoveDirection.Up;
                    SpeedY = Mathf.Min(MoveSpeed, _oriY - _curPos.y) ;
                }
            }

            base.UpdateLogic();
        }

        protected override bool CheckMagicPassAfterHit(UnitBase unit)
        {
            return false;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsRigidbody)
            {
                _trigger = true;
            }

            return base.OnUpHit(other, ref y, checkOnly);
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            base.Hit(unit, eDirectionType);
            if (eDirectionType == EDirectionType.Up && unit.IsRigidbody)
            {
                _speed = IntVec2.zero;
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

        protected override void Clear()
        {
            base.Clear();
            _trigger = false;
        }
    }
}