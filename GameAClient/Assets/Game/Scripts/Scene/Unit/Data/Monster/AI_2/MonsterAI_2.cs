using SoyEngine;
using UnityEngine;
namespace GameA.Game
{
    public class MonsterAI_2 : MonsterBase
    {
        protected EMonsterState _lastEMonsterState;
        protected EMonsterState _eMonsterState;
        protected EMoveDirection _nextMoveDirection;
        protected int _timerBang;
        protected int _timerDialog;
        protected int _timerDetectStay;
        protected int _timerAttack;
        protected int _timerRun;
        protected int _timerStupid;
        protected int _intelligenc = 10; //智商，决定出问号概率，0每走几步就会出问好，10不会出问号

        protected override void Clear()
        {
            base.Clear();
            _eMonsterState = EMonsterState.Idle;
            _nextMoveDirection = _moveDirection;
            _timerBang = 0;
            _timerDialog = 0;
            _timerDetectStay = 0;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            ChangeState(EMonsterState.Run);
        }

        protected override void OnRightStampedEmpty()
        {
            if (_eMonsterState != EMonsterState.Chase && _eMonsterState != EMonsterState.Brake)
            {
                ChangeState(EMonsterState.Bang);
                SetNextMoveDirection(EMoveDirection.Left);
            }
        }

        protected override void OnLeftStampedEmpty()
        {
            if (_eMonsterState != EMonsterState.Chase && _eMonsterState != EMonsterState.Brake)
            {
                ChangeState(EMonsterState.Bang);
                SetNextMoveDirection(EMoveDirection.Right);
            }
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (eDirectionType == EDirectionType.Up || eDirectionType == EDirectionType.Down)
            {
                return;
            }
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
//            LogHelper.Debug(_nextMoveDirection+"~~~~");
        }

        protected virtual void ChangeState(EMonsterState eMonsterState)
        {
            if (_eMonsterState == eMonsterState)
            {
                return;
            }
            _lastEMonsterState = _eMonsterState;
            _eMonsterState = eMonsterState;
            var pos = GM2DTools.TileToWorld(new IntVec2(
                _moveDirection == EMoveDirection.Right ? _colliderGrid.XMin : _colliderGrid.XMax,
                _colliderGrid.YMax));
            switch (_eMonsterState)
            {
                case EMonsterState.Bang:
                    _timerBang = 75;
                    GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Bang, pos,
                        ESortingOrder.EffectItem);
                    break;
                case EMonsterState.Dialog:
                    _timerDialog = 125;
                    GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Dialog, pos,
                        ESortingOrder.EffectItem);
                    break;
                case EMonsterState.Stupid:
                    OnChangeStupid(pos);
                    break;
                case EMonsterState.Run:
                    OnChangeRun();
                    break;
            }
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("MonsterAi_2 ChangeState {0}", _eMonsterState.ToString()));
            }
        }

        protected virtual void OnChangeRun()
        {
            _timerRun = RandomDependFrame(90, 140);
        }

        protected virtual void OnChangeStupid(Vector3 pos)
        {
            _timerStupid = RandomDependFrame(0, 2) == 0 ? 150 : 225;
            GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Question, pos,
                ESortingOrder.EffectItem);
        }

        protected override void UpdateMonsterAI()
        {
            base.UpdateMonsterAI();
            if (_timerBang > 0)
            {
                _timerBang--;
            }
            if (_timerDialog > 0)
            {
                _timerDialog--;
            }
            if (_timerDetectStay > 0)
            {
                _timerDetectStay--;
            }
            if (_timerAttack > 0)
            {
                _timerAttack--;
            }
            if (_timerRun > 0)
            {
                _timerRun--;
            }
            if (_timerStupid > 0)
            {
                _timerStupid--;
            }
            if (_isClayOnWall) return;
            switch (_eMonsterState)
            {
                case EMonsterState.Idle:
                    SetInput(EInputType.Right, false);
                    SetInput(EInputType.Left, false);
                    break;
                case EMonsterState.Run:
                    SetInput(_moveDirection == EMoveDirection.Right ? EInputType.Right : EInputType.Left, true);
                    if (_timerRun == 0)
                    {
                        int value = GameRun.Instance.LogicFrameCnt % 11;
                        if (_intelligenc <= value)
                        {
                            ChangeState(EMonsterState.Stupid);
                        }
                        else
                        {
                            _timerRun = 30;
                        }
                    }
                    break;
                case EMonsterState.Stupid:
                    SetInput(EInputType.Right, false);
                    SetInput(EInputType.Left, false);
                    if (_timerStupid == 0)
                    {
                        ChangeState(EMonsterState.Run);
                    }
                    else if (_timerStupid % 75 == 0)
                    {
                        SetFacingDir(_moveDirection == EMoveDirection.Left
                            ? EMoveDirection.Right
                            : EMoveDirection.Left);
                        var pos = GM2DTools.TileToWorld(new IntVec2(
                            _moveDirection == EMoveDirection.Right ? _colliderGrid.XMin : _colliderGrid.XMax,
                            _colliderGrid.YMax));
                        GameParticleManager.Instance.Emit(ParticleNameConstDefineGM2D.Question, pos,
                            ESortingOrder.EffectItem);
                    }
                    break;
                case EMonsterState.Bang:
                    SetInput(EInputType.Right, false);
                    SetInput(EInputType.Left, false);
                    if (_timerBang == 0)
                    {
                        if (_timerDetectStay > 0)
                        {
                            ChangeState(EMonsterState.Chase);
                        }
                        else
                        {
                            ChangeState(EMonsterState.Run);
                            ChangeWay(_nextMoveDirection);
                        }
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
                    SetInput(_moveDirection == EMoveDirection.Right ? EInputType.Right : EInputType.Left, true);
                    break;
                case EMonsterState.Attack:
                    SetInput(EInputType.Right, false);
                    SetInput(EInputType.Left, false);
                    SetInput(EInputType.Skill1, true);
                    break;
            }
        }

        private int RandomDependFrame(int min, int max)
        {
            return Mathf.Clamp(GameRun.Instance.LogicFrameCnt % (max - min) + min, min, max);
        }
    }
}