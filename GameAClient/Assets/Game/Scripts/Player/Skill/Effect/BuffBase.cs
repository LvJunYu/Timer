using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 1, PreferedPoolSize = 30, MaxPoolSize = int.MaxValue)]
    public class BuffBase : IPoolableObject
    {
        protected int _time;
        protected EffectBase[] _effects;
        private ActorBase _target;
        protected EBuffType _eBuffType;

        public EffectBase[] Effects
        {
            get { return _effects; }
        }

        public EBuffType EBuffType
        {
            get { return _eBuffType; }
        }

        public bool IsGain
        {
            get
            {
                switch (_eBuffType)
                {
                        case EBuffType.Fire:
                        case EBuffType.Ice:
                        return false;
                }
                return false;
            }
        }

        public void Init(EBuffType eBuffType, int time, params EffectBase[] effects)
        {
            _eBuffType = eBuffType;
            _time = time;
            _effects = effects;
        }
        
        public virtual bool OnAttached(ActorBase target)
        {
            _target = target;
            for (int i = 0; i < _effects.Length; i++)
            {
                _effects[i].OnAttached(_target);
            }
            return true;
        }

        public virtual bool OnRemoved(ActorBase target)
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                _effects[i].OnRemoved(target);
            }
            return true;
        }

        public void Update()
        {
            _time--;
            if (_time == 0)
            {
                //移除buff
                _target.RemoveBuff(this);
                return;
            }
            for (int i = 0; i < _effects.Length; i++)
            {
                _effects[i].Update();
            }
        }

        public virtual void OnGet()
        {
        }

        public virtual void OnFree()
        {
            _eBuffType = EBuffType.None;
            _time = 0;
            _effects = null;
            _target = null;
        }

        public virtual void OnDestroyObject()
        {
        }
    }
}