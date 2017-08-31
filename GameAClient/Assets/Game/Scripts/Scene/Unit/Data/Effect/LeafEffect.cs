using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 9103, Type = typeof(LeafEffect))]
    public class LeafEffect : EffectBase
    {
        protected override void OnTrigger()
        {
            LogHelper.Debug("Leaf");
            GameParticleManager.Instance.Emit("M1EffectLuoYe", _trans);
        }
    }
}