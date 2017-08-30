using System.Security.Permissions;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 2004, Type = typeof(MonsterJelly))]
    public class MonsterJelly : MonsterBase
    {
        public enum EMonsterState
        {
            Idle,
            Run,
            Dialog,
            Bang,
            Chase,
            Attack,
        }

        protected EMonsterState _eMonsterState;
        protected EMoveDirection _nextMoveDirection;
        protected int _timerBang;
        protected int _timerDialog;
        
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _maxSpeedX = 20;
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _timerBang = 0;
            _timerDialog = 0;
            _eMonsterState = EMonsterState.Idle;
            _nextMoveDirection = _moveDirection;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            ChangeState(EMonsterState.Run);
        }
   
        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                Jelly.OnEffect(other, EDirectionType.Up);
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }
        
        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                Jelly.OnEffect(other, EDirectionType.Down);
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                Jelly.OnEffect(other, EDirectionType.Left);
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                Jelly.OnEffect(other, EDirectionType.Right);
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }
        
        protected override void OnRightStampedEmpty()
        {
            ChangeState(EMonsterState.Bang);
            SetNextMoveDirection(EMoveDirection.Left);
        }

        protected override void OnLeftStampedEmpty()
        {
            ChangeState(EMonsterState.Bang);
            SetNextMoveDirection(EMoveDirection.Right);
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
            var pos =GM2DTools.TileToWorld(new IntVec2(_curMoveDirection == EMoveDirection.Right ? _colliderGrid.XMin : _colliderGrid.XMax,
                _colliderGrid.YMax)) ;
            switch (_eMonsterState)
            {
                case EMonsterState.Bang:
                    _timerBang = 75;
                    GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Bang, pos, 2, ESortingOrder.EffectItem);
                    break;
                case EMonsterState.Dialog:
                    _timerDialog = 125;
                    GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Dialog, pos, 3, ESortingOrder.EffectItem);
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
                    if (_timerBang==0)
                    {
                        ChangeState(EMonsterState.Run);
                        ChangeWay(_nextMoveDirection);
                    }
                    break;
                case EMonsterState.Dialog:
                    SetInput(EInputType.Right, false);
                    SetInput(EInputType.Left, false);
                    if (_timerDialog==0)
                    {
                        ChangeState(EMonsterState.Run);
                        ChangeWay(_nextMoveDirection);
                    }
                    break;
                case EMonsterState.Chase:
                    SetInput(_curMoveDirection == EMoveDirection.Right ? EInputType.Right : EInputType.Left, true);
                    break;
                case EMonsterState.Attack:
                    break;
            }
        }
    }
}