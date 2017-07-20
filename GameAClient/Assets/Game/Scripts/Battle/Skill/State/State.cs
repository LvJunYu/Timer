using SoyEngine;
using UnityEngine;

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
            BanAttack,
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
        protected UnityNativeParticleItem _effect;

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
            GameParticleManager.FreeParticleItem(_effect);
            _effect = null;
        }

        public virtual bool OnAttached(Table_State tableState, ActorBase target)
        {
            _tableState = tableState;
            _target = target;
            if (_tableState.EffectTypes.Length != _tableState.EffectValues.Length || _tableState.EffectTypes.Length != _tableState.EffectIds.Length)
            {
                LogHelper.Error("Wrong TableState. Types : {0}, Values: {1}, Ids: {2}", _tableState.EffectTypes.Length, _tableState.EffectValues.Length, _tableState.EffectIds.Length);
                return false;
            }
            _duration = TableConvert.GetTime(_tableState.Duration);
            _curDuration = _duration;
            _intervalTime = TableConvert.GetTime(_tableState.IntervalTime);
            if (!string.IsNullOrEmpty(_tableState.Particle))
            {
                _effect = GameParticleManager.Instance.GetUnityNativeParticleItem(_tableState.Particle, _target.Trans);
                _effect.Play();
            }
            Excute(EEffectType.Always);
            OnAddView();
            _run = true;
            return true;
        }

        private bool Excute(EEffectType eEffectType)
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
                        _target.OnHpChanged(value * (_effectOverlapCount + 1));
                        break;
                    case EEffectId.Speed:
                        _target.SpeedRatio += (value * 0.01f) * (_effectOverlapCount + 1);
                        break;
                    case EEffectId.BanAttack:
                        _target.CanAttack = false;
                        break;
                    case EEffectId.HpMax:
                        break;
                }
                if (!_run)
                {
                    return false;
                }
            }
            return true;
        }

        public virtual bool OnRemoved()
        {
            for (int i = 0; i < _tableState.EffectTypes.Length; i++)
            {
                var value = _tableState.EffectValues[i];
                switch ((EEffectId) _tableState.EffectIds[i])
                {
                    case EEffectId.Hp:
                        break;
                    case EEffectId.Speed:
                        _target.SpeedRatio -= (value * 0.01f) * (_effectOverlapCount + 1);
                        break;
                    case EEffectId.BanAttack:
                        _target.CanAttack = true;
                        break;
                    case EEffectId.HpMax:
                        break;
                }
            }
            OnRemovedView();
            Excute(EEffectType.End);
            return true;
        }
        
        public void UpdateLogic()
        {
            if (!_run)
            {
                return;
            }
            if (_intervalTime > 0 && _timer % _intervalTime == 0)
            {
                if (!Excute(EEffectType.Interval))
                {
                    return;
                }
            }
            UpdateStateView();
            if (_timer >= _curDuration)
            {
                //移除
                _target.RemoveStates(_tableState.Id);
            }
            _timer++;
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
                case EOverlapType.TimeMax:
                    state._curDuration += state._duration - state._timer;
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
                case EOverlapType.TimeMax:
                    state._curDuration = 0;
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
        
        #region view renderer

        private void OnAddView()
        {
            if (_target.Animation != null && !string.IsNullOrEmpty(_tableState.Animation) && _target.Animation.HasAnimation(_tableState.Animation))
            {
                _target.Animation.Reset();
                _target.Animation.PlayLoop(_tableState.Animation);
            }
        }

        private void OnRemovedView()
        {
            var view = _target.View;
            if (view == null)
            {
                return;
            }
            if (_tableState.StateType == (int)EStateType.Invincible)
            {
                view.SetRendererColor(Color.white);
            }
            if (_target.Animation != null && !string.IsNullOrEmpty(_tableState.Animation) && _target.Animation.HasAnimation(_tableState.Animation))
            {
                _target.Animation.Reset();
            }
        }

        private void UpdateStateView()
        {
            //利用相对位置设置Effect的层级
            if (_effect != null)
            {
                _effect.Trans.localPosition = Vector3.forward * (_target.CurMoveDirection == EMoveDirection.Left ? 0.01f : -0.01f);
                _effect.Trans.rotation = Quaternion.identity;
            }
            if (_tableState.Id == 61)
            {
                FlashRenderer();
            }
            else if (_tableState.Id == 62)
            {
                Chameleon();
            }
        }
        
        protected void FlashRenderer()
        {
            var view = _target.View;
            if (view == null)
            {
                return;
            }
            int t = _timer % 20;
            var a = t > 9 ? Mathf.Clamp01((t - 10) * 0.1f + 0.3f) : Mathf.Clamp01(1f - t * 0.1f + 0.3f);
            view.SetRendererColor(new Color(1f, 1f, 1f, a));
        }

        protected void Chameleon()
        {
            var view = _target.View;
            if (view == null)
            {
                return;
            }
            var a = new Color(1f, 0.8235f, 0.804f, 0.804f);
            var b = new Color(0.9f, 0.41f, 0.804f, 0.804f);
            var c = new Color(1f, 0.745f, 0.63f, 0.804f);
            const int interval = 5;
            int t = GameRun.Instance.LogicFrameCnt % (3 * interval);
            if (t < interval)
            {
                view.SetRendererColor(Color.Lerp(a, b, t * (1f / interval)));
            }
            else if (t < 2 * interval)
            {
                view.SetRendererColor(Color.Lerp(c, b, (2f * interval - t) * (1f / interval)));
            }
            else
            {
                view.SetRendererColor(Color.Lerp(a, c, (3f * interval - t) * (1f / interval)));
            }
        }
        
        #endregion
    }
}