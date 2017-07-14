using SoyEngine;

namespace GameA.Game
{
    public class EffectPersistent : EffectBase, IPoolableObject
    {
        protected int _intervalTime;
        protected EffectBase _effect;

        private int _timer;
        private ActorBase _target;

        public override void Init(params object[] values)
        {
            _intervalTime = (int)values[0];
            _effect = (EffectBase)values[1];
        }

        public override bool OnAttached(ActorBase target)
        {
            _target = target;
            return base.OnAttached(target);
        }

        public override void Update()
        {
            _timer++;
            if (_intervalTime >0 && _timer % _intervalTime == 0)
            {
                _effect.OnAttached(_target);
            }
        }

        public void OnGet()
        {
        }

        public void OnFree()
        {
            _intervalTime = 0;
            _effect = null;
            _timer = 0;
            _target = null;
        }

        public void OnDestroyObject()
        {
        }
    }
}