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
        protected List<State> _currentStates = new List<State>();
        private Comparison<State> _comparisonState = SortState;

        protected EDieType _eDieType;
        protected int _attackedTimer;

        protected float _hpRecover = 200 * ConstDefineGM2D.FixedDeltaTime;
        protected int _hpRecoverTimer = 3 * ConstDefineGM2D.FixedFrameCount;

        protected SpineObject _effectIce;

        public int AttackedTimer
        {
            get { return _attackedTimer; }
        }

        public override EDieType EDieType
        {
            get { return _eDieType; }
        }

        public override bool IsActor
        {
            get { return true; }
        }

        protected override void Clear()
        {
            base.Clear();
            _canFanCross = true;
            _eDieType = EDieType.None;
            _attackedTimer = 0;
            GameParticleManager.FreeSpineObject(_effectIce);
            _effectIce = null;
            RemoveAllStates();
        }

        public override void UpdateLogic()
        {
            if (_isAlive && _isStart && !_isFreezed)
            {
                for (int i = 0; i < _currentStates.Count; i++)
                {
                    _currentStates[i].UpdateLogic();
                }
            }
        }

        public override void AddStates(params int[] ids)
        {
            if (!_isAlive)
            {
                return;
            }
            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var tableState = TableManager.Instance.GetState(id);
                if (tableState == null)
                {
                    continue;
                }
                //如果是减益buff 当前无敌时跳过。
                if (tableState.IsBuff == 0 && IsInvincible)
                {
                    continue;
                }
                //如果已存在，判断叠加属性
                State state;
                if (TryGetState(id, out state))
                {
                    ++state;
                    continue;
                }
                //如果不存在，判断是否同类替换
                if (tableState.IsReplace == 1)
                {
                    RemoveStateByType(tableState.StateType);
                }
                state = PoolFactory<State>.Get();
                if (state.OnAttached(tableState, this))
                {
                    _currentStates.Add(state);
                    LogHelper.Debug("{0} AddState: {1}",this, state.TableState.Id);
                    _currentStates.Sort(_comparisonState);
                    continue;
                }
                PoolFactory<State>.Free(state);
            }
        }

        public override void RemoveStates(params int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                State state;
                if (TryGetState(ids[i], out state))
                {
                    --state;
                }
            }
        }

        public override void RemoveState(State state)
        {
            if (_currentStates.Contains(state))
            {
                if (state.OnRemoved())
                {
                    _currentStates.Remove(state);
                    LogHelper.Debug("{0} RemoveState: {1}",this, state.TableState.Id);
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
                _currentStates[i].OnRemoved();
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
                    _currentStates[i].OnRemoved();
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

        public override void SetStateEffect(State state, bool within)
        {
            LogHelper.Debug("SetStateEffect : {0} | {1}", state.TableState.Id, within);
            switch ((EStateType) state.TableState.StateType)
            {
                case EStateType.Ice:
                    SetStateIce(state, within);
                    break;
                case EStateType.Fire:
                    SetStateFire(state, within);
                    break;
            }
        }

        private void SetStateIce(State state, bool within)
        {
            if (_effectIce == null)
            {
                _effectIce = GameParticleManager.Instance.EmitLoop(state.TableState.Particle, _trans);
            }
            _effectIce.Trans.localPosition = new Vector3(0, -0.1f, _curMoveDirection == EMoveDirection.Left ? 0.01f : -0.01f);
            _effectIce.Trans.rotation = Quaternion.identity;
            _effectIce.SetActive(within);
            if (within)
            {
                if (_animation != null)
                {
                    _animation.Reset();
                    _animation.PlayOnce("OnIce");
                }
            }
        }

        private void SetStateFire(State state, bool within)
        {
            if (within)
            {
                _eDieType = EDieType.Fire;
                if (_animation != null)
                {
                    _animation.PlayLoop("OnFire", 1, 1);
                }
            }
            else
            {
                _animation.Reset();
                _eDieType = EDieType.None;
            }
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

        internal override void InFan(IntVec2 force)
        {
            ExtraSpeed = force;
        }

        internal override void InWater()
        {
            if (_eDieType == EDieType.Fire)
            {
                //跳出水里
                ExtraSpeed.y = 240;
//                OutFire();
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
//                    unit.InFire();
                }
            }
        }
    }
}