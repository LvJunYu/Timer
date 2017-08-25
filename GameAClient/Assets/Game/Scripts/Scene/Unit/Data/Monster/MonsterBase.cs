/********************************************************************
** Filename : MonsterBase
** Author : Dong
** Date : 2017/4/18 星期二 下午 9:05:44
** Summary : MonsterBase
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class MonsterBase : ActorBase
    {
        protected static IntVec2 SeekRange = new IntVec2(13, 4) * ConstDefineGM2D.ServerTileScale;
        protected static IntVec2 AttackRange = new IntVec2(1, 1) * ConstDefineGM2D.ServerTileScale;
        public static IntVec2 PathRange = new IntVec2(3, 2);
        protected const int MaxStuckFrames = 30;
        protected const int MaxReSeekFrames = 5;
        protected List<IntVec2> _path = new List<IntVec2>();

        protected IntVec2 _lastPos;
        protected int _fireTimer;
        protected EMonsterState _eState;
        
        protected int _thinkTimer;
        protected int _stuckTimer;
        protected int _reSeekTimer;

        protected override bool IsCheckClimb()
        {
            return false;
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _isMonster = true;
            _maxSpeedX = 40;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _view.StatusBar.ShowHP();
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _input = _input ?? new InputBase();
            _input.Clear();
            _lastPos = _curPos;
            _fireTimer = 0;
            _eState = EMonsterState.Think;
            _path.Clear();
            _thinkTimer = 0;
            _stuckTimer = 0;
            _reSeekTimer = 0;
        }

        protected override void CheckrInput()
        {
            
        }

        protected override void CalculateMotor()
        {
            base.CalculateMotor();
            if (HasStateType(EStateType.Fire))
            {
                _speedRatio *= SpeedFireRatio;
                OnFire();
                ChangeState(EMonsterState.Idle);
            }
            else
            {
                _fireTimer = 0;
                UpdateMonsterAI();
            }
            if (_grounded)
            {
                // 判断左右踩空
                if (_downUnits.Count == 1)
                {
                    if (SpeedX > 0 && _downUnits[0].ColliderGrid.XMax < _colliderGrid.XMax)
                    {
                        OnRightStampedEmpty();
                    }
                    else if (SpeedX < 0 && _downUnits[0].ColliderGrid.XMin > _colliderGrid.XMin)
                    {
                        OnLeftStampedEmpty();
                    }
                }
            }
        }

        protected virtual void ChangeState(EMonsterState state)
        {
            _eState = state;
            //LogHelper.Debug("ChangeState : {0}", _eState);
        }

        protected virtual void UpdateMonsterAI()
        {
            _curMaxSpeedX = (int)(_maxSpeedX * _speedRatio * _speedStateRatio);
            if (_thinkTimer > 0)
            {
                _thinkTimer--;
            }
            ChangeState(EMonsterState.Idle);
            IntVec2 rel = CenterDownPos - PlayMode.Instance.MainPlayer.CenterDownPos;
            if (PlayMode.Instance.MainPlayer.CanMove)
            {
                if (ConditionAttack(rel))
                {
                    ChangeState(EMonsterState.Attack);
                }
                else if(ConditionSeek(rel))
                {
                    ChangeState(EMonsterState.Seek);
                }
                else if(ConditionThink(rel))
                {
                    ChangeState(EMonsterState.Think);
                }
            }
            if (_eState != EMonsterState.Seek)
            {
                SetInput(EInputType.Right, false);
                SetInput(EInputType.Left, false);
            }
            SetInput(EInputType.Skill1, false);
            
            switch (_eState)
            {
                case EMonsterState.Think:
                    OnThink();
                    break;
                case EMonsterState.Seek:
                    OnSeek();
                    break;
                case EMonsterState.Attack:
                    OnAttack();
                    break;
            }
#if UNITY_EDITOR
            if (_path != null)
            {
                for (int i = 0; i < _path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(_path[i].x + 0.5f, _path[i].y + 0.5f), new Vector3(_path[i + 1].x + 0.5f, _path[i + 1].y + 0.5f));
                }
            }
#endif
        }

        protected virtual void OnThink()
        {
        }

        protected virtual void OnSeek()
        {
            
        }

        protected virtual void OnAttack()
        {
            SetInput(EInputType.Skill1, true);
        }

        protected virtual void OnFire()
        {
            _fireTimer++;
            if (_fireTimer % 80 == 0)
            {
                ChangeWay(_curMoveDirection == EMoveDirection.Left ? EMoveDirection.Right : EMoveDirection.Left);
            }
            CheckWay();
        }

        protected override void UpdateDynamicView(float deltaTime)
        {
            base.UpdateDynamicView(deltaTime);
            if (_isAlive)
            {
                UpdateMonsterView(deltaTime);
            }
            else
            {
                _dieTime ++;
                if (_dieTime == 100)
                {
                    PlayMode.Instance.DestroyUnit(this);
                }
            }
        }

        protected virtual void UpdateMonsterView(float deltaTime)
        {
            if (_animation != null)
            {
                if (_speed.x == 0)
                {
                    if (CanMove)
                    {
                        _animation.PlayLoop("Idle");
                    }
                }
                else
                {
                    float speed = (int)(SpeedX * 1f);
                    speed = Math.Abs(speed);
                    speed = Mathf.Clamp(speed, 30, 100) * deltaTime;
                    _animation.PlayLoop("Run", speed);
                }
            }
        }
        
        protected virtual void OnRightStampedEmpty()
        {
        }

        protected virtual void OnLeftStampedEmpty()
        {
        }

        protected bool CheckWay()
        {
            if (_hitUnits != null)
            {
                if (_curMoveDirection == EMoveDirection.Left && _hitUnits[(int)EDirectionType.Left] != null)
                {
                    _fireTimer = 0;
                    return ChangeWay(EMoveDirection.Right);
                }
                else if (_curMoveDirection == EMoveDirection.Right && _hitUnits[(int)EDirectionType.Right] != null)
                {
                    _fireTimer = 0;
                    return ChangeWay(EMoveDirection.Left);
                }
            }
            return false;
        }

        public override bool ChangeWay(EMoveDirection eMoveDirection)
        {
            if (!_isMonster)
            {
                return false;
            }
            SetInput(eMoveDirection == EMoveDirection.Right ? EInputType.Right : EInputType.Left, true);
            SetInput(eMoveDirection == EMoveDirection.Right ? EInputType.Left : EInputType.Right, false);
            return true;
        }

        protected override void OnDead ()
        {
            base.OnDead ();
            Messenger<EDieType>.Broadcast (EMessengerType.OnMonsterDead, _eDieType);
        }

        protected void SetInput(EInputType eInputType, bool value)
        {
            _input.CurAppliedInputKeyAry[(int)eInputType] = value;
        }
        
        protected virtual bool ConditionAttack(IntVec2 rel)
        {
            if (!CanAttack)
            {
                return false;
            }
            if (Mathf.Abs(rel.x) > AttackRange.x || Mathf.Abs(rel.y) > AttackRange.y)
            {
                return false;
            }
            return true;
        }

        protected virtual bool ConditionSeek(IntVec2 rel)
        {
            if (!CanMove)
            {
                return false;
            }
            if (_path.Count <= 1)
            {
                return false;
            }
            return true;
        }
        
        protected virtual bool ConditionThink(IntVec2 rel)
        {
            if (!CanMove)
            {
                return false;
            }
            if (_thinkTimer > 0)
            {
                return false;
            }
            _thinkTimer = 50;
            if (Mathf.Abs(rel.x) > SeekRange.x || Mathf.Abs(rel.y) > SeekRange.y)
            {
                return false;
            }
            return true;
        }
    }
}
