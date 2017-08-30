using SoyEngine;

namespace GameA.Game
{
    public class MonsterAI_2 : MonsterBase
    {
        protected EMonsterState _eMonsterState;
        protected EMoveDirection _nextMoveDirection;
        protected int _timerBang;
        protected int _timerDialog;

        protected override void Clear()
        {
            base.Clear();
            _eMonsterState = EMonsterState.Idle;
            _nextMoveDirection = _moveDirection;
            _timerBang = 0;
            _timerDialog = 0;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            ChangeState(EMonsterState.Run);
        }

        protected override void OnRightStampedEmpty()
        {
            if (_eMonsterState != EMonsterState.Chase)
            {
                ChangeState(EMonsterState.Bang);
                SetNextMoveDirection(EMoveDirection.Left);
            }
        }

        protected override void OnLeftStampedEmpty()
        {
            if (_eMonsterState != EMonsterState.Chase)
            {
                ChangeState(EMonsterState.Bang);
                SetNextMoveDirection(EMoveDirection.Right);
            }
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (unit.IsMonster)
            {
                ChangeState(EMonsterState.Dialog);
            }
            else
            {
                ChangeState(EMonsterState.Bang);
            }
            switch (eDirectionType)
            {
                case EDirectionType.Left:
                    SetNextMoveDirection(EMoveDirection.Right);
                    break;
                case EDirectionType.Right:
                    SetNextMoveDirection(EMoveDirection.Left);
                    break;
            }
            base.Hit(unit, eDirectionType);
        }

        protected virtual void SetNextMoveDirection(EMoveDirection eMoveDirection)
        {
            _nextMoveDirection = eMoveDirection;
        }

        protected virtual void ChangeState(EMonsterState eMonsterState)
        {
            if (_eMonsterState == eMonsterState)
            {
                return;
            }
            _eMonsterState = eMonsterState;
            var pos = GM2DTools.TileToWorld(new IntVec2(_curMoveDirection == EMoveDirection.Right ? _colliderGrid.XMin : _colliderGrid.XMax,
                _colliderGrid.YMax));
            switch (_eMonsterState)
            {
                case EMonsterState.Bang:
                    _timerBang = 75;
                    GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Bang, pos, 2,
                        ESortingOrder.EffectItem);
                    break;
                case EMonsterState.Dialog:
                    _timerDialog = 125;
                    GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Dialog, pos, 3,
                        ESortingOrder.EffectItem);
                    break;
            }
        }

        protected override void UpdateMonsterAI()
        {
            if (_timerBang > 0)
            {
                _timerBang--;
            }
            if (_timerDialog > 0)
            {
                _timerDialog--;
            }
            switch (_eMonsterState)
            {
                case EMonsterState.Idle:
                    SetInput(EInputType.Right, false);
                    SetInput(EInputType.Left, false);
                    break;
                case EMonsterState.Run:
                    SetInput(_curMoveDirection == EMoveDirection.Right ? EInputType.Right : EInputType.Left, true);
                    break;
                case EMonsterState.Bang:
                    SetInput(EInputType.Right, false);
                    SetInput(EInputType.Left, false);
                    if (_timerBang == 0)
                    {
                        ChangeState(EMonsterState.Run);
                        ChangeWay(_nextMoveDirection);
                    }
                    break;
                case EMonsterState.Dialog:
                    SetInput(EInputType.Right, false);
                    SetInput(EInputType.Left, false);
                    if (_timerDialog == 0)
                    {
                        ChangeState(EMonsterState.Run);
                        ChangeWay(_nextMoveDirection);
                    }
                    break;
                case EMonsterState.Chase:
                    SetInput(_curMoveDirection == EMoveDirection.Right ? EInputType.Right : EInputType.Left, true);
                    break;
                case EMonsterState.Attack:
                    SetInput(EInputType.Right, false);
                    SetInput(EInputType.Left, false);
                    break;
            }
        }
    }
}