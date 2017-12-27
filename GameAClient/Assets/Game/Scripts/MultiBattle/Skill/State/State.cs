using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 1, PreferedPoolSize = 30, MaxPoolSize = int.MaxValue)]
    public class State : IPoolableObject
    {
        protected Table_State _tableState;
        protected int _duration;
        protected int _intervalTime;
        protected UnitBase _target;
        protected UnitBase _sender; //释放状态的对象，用于计算击杀，可能为null
        protected int _timer;
        protected int _effectOverlapCount;
        protected int _curDuration;
        protected bool _run;
        protected UnityNativeParticleItem _effect;

        public Table_State TableState
        {
            get { return _tableState; }
        }

        public UnitBase Sender
        {
            get { return _sender; }
        }

        public void OnFree()
        {
            Clear();
        }

        private void Clear()
        {
            _tableState = null;
            _duration = 0;
            _intervalTime = 0;
            _target = null;
            _sender = null;
            _timer = 0;
            _effectOverlapCount = 0;
            _curDuration = 0;
            _run = false;
            GameParticleManager.FreeParticleItem(_effect);
            _effect = null;
        }

        public virtual bool OnAttached(Table_State tableState, ActorBase target, UnitBase sender)
        {
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("State {2} OnAttached from {0} to {1} ", _sender.Guid, _target.Guid, _tableState.Name));
            }
            _run = true;
            _tableState = tableState;
            _target = target;
            _sender = sender;
            if (_tableState.EffectTypes.Length != _tableState.EffectValues.Length ||
                _tableState.EffectTypes.Length != _tableState.EffectIds.Length)
            {
                LogHelper.Error("Wrong TableState. Types : {0}, Values: {1}, Ids: {2}", _tableState.EffectTypes.Length,
                    _tableState.EffectValues.Length, _tableState.EffectIds.Length);
                return false;
            }
            if (_tableState.Id == 61)
            {//出生无敌
                _duration = PlayMode.Instance.SceneState.Statistics.NetBattleReviveInvincibleTime *
                            ConstDefineGM2D.FixedFrameCount;
            }
            else
            {
                _duration = TableConvert.GetTime(_tableState.Duration);
            }
            _curDuration = _duration;
            _intervalTime = TableConvert.GetTime(_tableState.IntervalTime);
            Excute(EEffectType.Always);
            OnAddView();
            //0代表无穷
            if (_curDuration == 0)
            {
                _run = false;
            }
            return true;
        }

        private bool Excute(EEffectType eEffectType)
        {
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("State {2} Excute from {0} to {1} ", _sender.Guid, _target.Guid, _tableState.Name));
            }
            for (int i = 0; i < _tableState.EffectTypes.Length; i++)
            {
                if (_tableState.EffectTypes[i] != (int) eEffectType)
                {
                    continue;
                }
                var value = _tableState.EffectValues[i];
                switch ((EEffectId) _tableState.EffectIds[i])
                {
                    case EEffectId.Hp:
                        _target.OnHpChanged(value * (_effectOverlapCount + 1), _sender);
                        break;
                    case EEffectId.Speed:
                        _target.SpeedStateRatio += (value * 0.01f) * (_effectOverlapCount + 1);
                        break;
                    case EEffectId.Ice:
                        _target.AddEnvState(EEnvState.Ice);
                        break;
                    case EEffectId.HpMax:
                        break;
                    case EEffectId.Invincible:
                        break;
                    case EEffectId.Clay:
                        _target.AddEnvState(EEnvState.Clay);
                        break;
                    case EEffectId.Stun:
                        _target.AddEnvState(EEnvState.Stun);
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
            if (GameModeNetPlay.DebugEnable())
            {
                GameModeNetPlay.WriteDebugData(string.Format("State {2} OnRemoved from {0} to {1} ", _sender.Guid, _target.Guid, _tableState.Name));
            }
            for (int i = 0; i < _tableState.EffectTypes.Length; i++)
            {
                var value = _tableState.EffectValues[i];
                switch ((EEffectId) _tableState.EffectIds[i])
                {
                    case EEffectId.Hp:
                        break;
                    case EEffectId.Speed:
                        _target.SpeedStateRatio -= (value * 0.01f) * (_effectOverlapCount + 1);
                        break;
                    case EEffectId.Ice:
                        _target.RemoveEnvState(EEnvState.Ice);
                        break;
                    case EEffectId.HpMax:
                        break;
                    case EEffectId.Invincible:
                        break;
                    case EEffectId.Clay:
                        _target.RemoveEnvState(EEnvState.Clay);
                        if (_target.IsMonster)
                        {
                            ((MonsterBase) _target).IsClayOnWall = false;
                        }
                        break;
                    case EEffectId.Stun:
                        _target.RemoveEnvState(EEnvState.Stun);
                        break;
                }
            }
            OnRemovedView();
            Excute(EEffectType.End);
            return true;
        }

        public void UpdateLogic()
        {
            UpdateStateView();
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
            if (_timer >= _curDuration)
            {
                //移除
                _target.RemoveStates(_tableState.Id);
            }
            _timer++;
        }

        public void OnGet()
        {
            Clear();
        }

        public void OnDestroyObject()
        {
        }

        public static State operator ++(State state)
        {
            switch ((EOverlapType) state.TableState.OverlapType)
            {
                case EOverlapType.None:
                    break;
                case EOverlapType.TimeMax:
                    state._curDuration = state._duration + state._timer;
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
            switch ((EOverlapType) state.TableState.OverlapType)
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
            if (state._curDuration <= 0 || state._effectOverlapCount < 0)
            {
                state._target.RemoveState(state);
                return null;
            }
            return state;
        }

        public override string ToString()
        {
            return string.Format(
                "Duration: {0}, IntervalTime: {1}, Target: {2}, Timer: {3}, EffectOverlapCount: {4}, CurDuration: {5}",
                _duration, _intervalTime, _target, _timer, _effectOverlapCount, _curDuration);
        }

        #region view renderer

        private void OnAddView()
        {
            string path = _tableState.Particle;
            if (_tableState.StateType == (int) EStateType.Clay)
            {
                if (_target.IsMonster && ((MonsterBase) _target).IsClayOnWall)
                {
                    switch (((MonsterBase) _target).EClayOnWallDirection)
                    {
                        case EClayOnWallDirection.Down:
                            path += "Up";
                            break;
                        case EClayOnWallDirection.Left:
                            path += "Right";
                            break;
                        case EClayOnWallDirection.Right:
                            path += "Left";
                            break;
                        default:
                            path += "Down";
                            break;
                    }
                }
                else
                {
                    path += "Down";
                }
            }
            if (!string.IsNullOrEmpty(path))
            {
                _effect = GameParticleManager.Instance.GetUnityNativeParticleItem(path, _target.Trans);
                if (_effect != null)
                {
                    _effect.Play();
                }
            }
            if (_target.Animation != null && !string.IsNullOrEmpty(_tableState.Animation) &&
                _target.Animation.HasAnimation(_tableState.Animation))
            {
                _target.Animation.Reset();
                _target.Animation.PlayLoop(_tableState.Animation, 1, 1);
            }
            if (_tableState.StateType == (int) EStateType.Fire && _target.IsMain)
            {
                _target.View.SetRendererColor(Color.grey);
            }
        }

        private void OnRemovedView()
        {
            var view = _target.View;
            if (view == null)
            {
                return;
            }
            if (_tableState.StateType == (int) EStateType.Invincible || _tableState.StateType == (int) EStateType.Fire)
            {
                view.SetRendererColor(Color.white);
            }
            if (_target.Animation != null && !string.IsNullOrEmpty(_tableState.Animation) &&
                _target.Animation.HasAnimation(_tableState.Animation))
            {
                _target.Animation.Reset();
            }
        }

        private void UpdateStateView()
        {
            //利用相对位置设置Effect的层级
            if (_effect != null)
            {
                _effect.Trans.localPosition =
                    Vector3.forward * (_target.MoveDirection == EMoveDirection.Left ? 0.01f : -0.01f);
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