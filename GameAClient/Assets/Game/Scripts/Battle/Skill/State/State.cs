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
        protected int _effectOverlapCount;
        protected int _curDuration;
        protected bool _run;

        public Table_State TableState
        {
            get { return _tableState; }
        }
        
        public void OnFree()
        {
            _tableState = null;
            _duration = 0;
            _intervalTime = 0;
            _target = null;
            _timer = 0;
            _effectOverlapCount = 0;
            _curDuration = 0;
            _run = false;
        }

        public virtual bool OnAttached(Table_State tableState, ActorBase target)
        {
            _tableState = tableState;
            if (_tableState.EffectTypes.Length != _tableState.EffectValues.Length || _tableState.EffectTypes.Length != _tableState.EffectIds.Length)
            {
                LogHelper.Error("Wrong TableState. Types : {0}, Values: {1}, Ids: {2}", _tableState.EffectTypes.Length, _tableState.EffectValues.Length, _tableState.EffectIds.Length);
                return false;
            }
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
                        _target.SpeedRatio += (value * 0.01f) * (_effectOverlapCount + 1);
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
            for (int i = 0; i < _tableState.EffectTypes.Length; i++)
            {
                var value = _tableState.EffectValues[i];
                switch ((EEffectId) _tableState.EffectIds[i])
                {
                    case EEffectId.Hp:
                        _target.Hp -= value * (_effectOverlapCount + 1);
                        break;
                    case EEffectId.Speed:
                        _target.SpeedRatio -= (value * 0.01f) * (_effectOverlapCount + 1);
                        break;
                    case EEffectId.Ice:
                        break;
                    case EEffectId.HpMax:
                        break;
                }
            }
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

        public override string ToString()
        {
            return string.Format("Duration: {0}, IntervalTime: {1}, Target: {2}, Timer: {3}, EffectOverlapCount: {4}, CurDuration: {5}", _duration, _intervalTime, _target, _timer, _effectOverlapCount, _curDuration);
        }
    }
}