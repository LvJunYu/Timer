using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 9100, Type = typeof(FlowerEffect))]
    public class FlowerEffect : EffectBase
    {
        protected override void OnTrigger()
        {
            LogHelper.Debug("FlowerEffect");
            var effect = GameParticleManager.Instance.GetUIParticleItem("M1EffectHuaBan", SocialGUIManager.Instance.UIRoot.Trans, (int) EUIGroupType.InGameStart);
            if (effect != null)
            {
                effect.Particle.Play();
            }
        }
    }
}