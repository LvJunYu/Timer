namespace GameA.Game
{
    public class EffectSlowSpeed : EffectBase
    {
        private float _ratio;

        public override void Init(params object[] values)
        {
            _ratio = (float) values[0];
        }

        public override bool OnAttached(ActorBase target)
        {
            return base.OnAttached(target);
        }
    }
}