/********************************************************************
** Filename : ActorBase
** Author : Dong
** Date : 2017/5/20 星期六 上午 10:51:33
** Summary : ActorBase
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public enum EDieType
    {
        None,
        /// <summary>
        /// 被机关之类的弄死
        /// </summary>
        Normal,
        Lazer,
        Water,
        Fire,
        OutofMap,
    }

    public class ActorBase : RigidbodyUnit
    {
        protected List<State > _currentStates = new List<State>();
        private Comparison<State> _comparisonState = SortState;

        protected EDieType _eDieType;
        protected int _attackedTimer;
        protected int _fireTimer;

        protected float _hpRecover = 200 * ConstDefineGM2D.FixedDeltaTime;
        protected int _hpRecoverTimer = 3 * ConstDefineGM2D.FixedFrameCount;

        public int AttackedTimer
        {
            get { return _attackedTimer; }
        }

        public override EDieType EDieType
        {
            get { return _eDieType; }
        }
        
        protected override void Clear()
        {
            base.Clear();
            _eDieType = EDieType.None;
            _attackedTimer = 0;
            _fireTimer = 0;
            RemoveAllStates();
        }
        
        public override void AddState(int id)
        {
            var tableState = TableManager.Instance.GetState(id);
            if (tableState == null)
            {
                return;
            }
            //如果已存在，判断叠加属性
            State state;
            if (TryGetState(id, out state))
            {
                switch ((EOverlapType)tableState.OverlapType)
                {
                    case EOverlapType.None:
                        return;
                    case EOverlapType.Time:
                        state.OverlapTime();
                        break;
                    case EOverlapType.Effect:
                        state.OverlapEffect();
                        break;
                    case EOverlapType.All:
                        state.OverlapEffect();
                        state.OverlapTime();
                        break;
                }
                return;
            }
            //如果不存在，判断是否同类替换
            if (tableState.IsReplace == 1)
            {
                RemoveStateByType(tableState.StateType);
            }
            state = PoolFactory<State>.Get();
            if (state.OnAttached(id, this))
            {
                _currentStates.Add(state);
                _currentStates.Sort(_comparisonState);
                return;
            }
            PoolFactory<State>.Free(state);
        }

        public override void RemoveState(State state)
        {
            if (_currentStates.Contains(state))
            {
                if (state.OnRemoved(this))
                {
                    _currentStates.Remove(state);
                    PoolFactory<State>.Free(state);
                }
            }
        }

        public bool TryGetState(int id, out State state)
        {
            for (int i = 0; i < _currentStates.Count; i++)
            {
                if (_currentStates[i].TableState.Id == id)
                {
                    state = _currentStates[i];
                    return true;
                }
            }
            state = null;
            return false;
        }
    
        public void RemoveStateByType(int stateType)
        {
            for (int i = _currentStates.Count - 1; i >= 0; i--)
            {
                if (_currentStates[i].TableState.StateType == stateType)
                {
                    RemoveState(_currentStates[i]);
                    _currentStates.RemoveAt(i);
                }
            }
        }
        
        public void RemoveAllStates()
        {
            for (int i = 0; i < _currentStates.Count; i++)
            {
                _currentStates[i].OnRemoved(this);
                PoolFactory<State>.Free(_currentStates[i]);
            }
            _currentStates.Clear();
        }
        
        public override void RemoveAllDebuffs()
        {
            for (int i = _currentStates.Count - 1; i >= 0; i--)
            {
                if (_currentStates[i].TableState.IsBuff == 0)
                {
                    _currentStates[i].OnRemoved(this);
                    _currentStates.RemoveAt(i);
                }
            }
        }
        
        private static int SortState(State one, State other)
        {
            int v = one.TableState.StatePriority.CompareTo(other.TableState.StatePriority);
            if (v == 0)
            {
                v = one.TableState.StateTypePriority.CompareTo(other.TableState.StateTypePriority);
            }
            return v;
        }

        internal override void InLazer()
        {
            if (!_isAlive || IsInvincible)
            {
                return;
            }
            _eDieType = EDieType.Lazer;
            OnDead();
            if (_animation != null)
            {
                _animation.PlayOnce("DeathLazer");
            }
        }

        internal override void InWater()
        {
            if (_eDieType == EDieType.Fire)
            {
                //跳出水里
                ExtraSpeed.y = 240;
                OutFire();
                return;
            }
            if (!_isAlive || IsInvincible)
            {
                return;
            }
            _eDieType = EDieType.Water;
            OnDead();
            if (_animation != null)
            {
                _animation.PlayOnce("DeathWater");
            }
        }

        internal override void OutFire()
        {
            if (_eDieType == EDieType.Fire)
            {
                _fireTimer = 0;
                _animation.Reset();
                _eDieType = EDieType.None;
            }
        }

        internal override void InFire()
        {
            if (!_isAlive || IsInvincible)
            {
                return;
            }
            if (_eDieType == EDieType.Fire)
            {
                return;
            }
            _eDieType = EDieType.Fire;
            if (_animation != null)
            {
                _animation.PlayLoop("OnFire", 1, 1);
            }
        }

        protected override bool CheckOutOfMap()
        {
            if (base.CheckOutOfMap())
            {
                _eDieType = EDieType.OutofMap;
                return true;
            }
            return false;
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (_isAlive && _eDieType == EDieType.Fire)
            {
                if (unit.IsAlive)
                {
                    unit.InFire();
                }
            }
        }
    }
}
