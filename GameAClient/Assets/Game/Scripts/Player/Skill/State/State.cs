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

        public Table_State TableState
        {
            get { return _tableState; }
        }

        public virtual bool OnAttached(int id, ActorBase target)
        {
            _target = target;
            _tableState = TableManager.Instance.GetState(id);
            if (_tableState == null)
            {
                LogHelper.Error("GetState Failed : {0}", id);
                return false;
            }
            _duration = TableConvert.GetTime(_tableState.Duration);
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
                        _target.Hp += value * _effectOverlapCount;
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
            if (_timer == _duration)
            {
                //移除
                _target.RemoveState(this);
                Excute(EEffectType.End);
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

        public void OverlapTime()
        {
            _duration += _duration;
        }

        public void OverlapEffect()
        {
            _effectOverlapCount++;
        }
    }
}