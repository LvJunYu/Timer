using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 9101, Type = typeof(FireworkEffect))]
    public class FireworkEffect : EffectBase
    {
        protected override void OnTrigger()
        {
            LogHelper.Debug("FireworkEffect");
            GameParticleManager.Instance.Emit("M1EffectYanHua", _trans);
        }
    }
}