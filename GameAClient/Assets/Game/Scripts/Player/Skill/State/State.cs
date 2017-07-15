using SoyEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 1, PreferedPoolSize = 30, MaxPoolSize = int.MaxValue)]
    public class State : IPoolableObject
    {
        public enum EEffectId
        {
            None,
            Hp,
            Speed,
            Ice,
            HpMax,
        }
        
        public enum EEffectType
        {
            None,
            Always,
            Interval,
            End,
        }
        
        protected Table_State _tableState;
        protected int _duration;
        protected int _intervalTime;
        protected UnitBase _target;
        protected int _timer;
        protected bool _run;
        protected int _effectOverlapCount;
        protected int _curDuration;

        public Table_State TableState
        {
            get { return _tableState; }
        }

        public virtual bool OnAttached(Table_State tableState, ActorBase target)
        {
            _tableState = tableState;
            _duration = TableConvert.GetTime(_tableState.Duration);
            _curDuration = _duration;
            _intervalTime = TableConvert.GetTime(_tableState.IntervalTime);
            Excute(EEffectType.Always);
            _run = true;
            return true;
        }

        private void Excute(EEffectType eEffectType)
        {
            for (int i = 0; i < _tableState.EffectTypes.Length; i++)
            {
                if (_tableState.EffectTypes[i] != (int)eEffectType)
                {
                    continue;
                }
                var value = _tableState.EffectValues[i];
                switch ((EEffectId) _tableState.EffectIds[i])
                {
                    case EEffectId.Hp:
                        _target.Hp += value * (_effectOverlapCount + 1);
                        break;
                    case EEffectId.Speed:
                        break;
                    case EEffectId.Ice:
                        break;
                    case EEffectId.HpMax:
                        break;
                }
            }
        }

        public virtual bool OnRemoved(ActorBase target)
        {
            Excute(EEffectType.End);
            return true;
        }
        
        public void UpdateLogic()
        {
            if (!_run)
            {
                return;
            }
            _timer++;
            if (_intervalTime > 0 && _timer % _intervalTime == 0)
            {
                Excute(EEffectType.Interval);
            }
            if (_timer == _curDuration)
            {
                //移除
                _target.RemoveStates(_tableState.Id);
            }
        }

        public void OnGet()
        {
        }

        public void OnFree()
        {
        }

        public void OnDestroyObject()
        {
        }

        public static State operator ++(State state)
        {
            switch ((EOverlapType)state.TableState.OverlapType)
            {
                case EOverlapType.None:
                    break;
                case EOverlapType.Time:
                    state._curDuration += state._duration;
                    break;
                case EOverlapType.Effect:
                    state._effectOverlapCount++;
                    break;
                case EOverlapType.All:
                    state._curDuration += state._duration;
                    state._effectOverlapCount++;
                    break;
            }
            return state;
        }

        public static State operator --(State state)
        {
            switch ((EOverlapType)state.TableState.OverlapType)
            {
                case EOverlapType.None:
                    break;
                case EOverlapType.Time:
                    state._curDuration -= state._duration;
                    break;
                case EOverlapType.Effect:
                    state._effectOverlapCount--;
                    break;
                case EOverlapType.All:
                    state._curDuration -= state._duration;
                    state._effectOverlapCount--;
                    break;
            }
            if (state._curDuration <= 0 || state._effectOverlapCount< 0)
            {
                state._target.RemoveState(state);
                return null;
            }
            return state;
        }
    }
}