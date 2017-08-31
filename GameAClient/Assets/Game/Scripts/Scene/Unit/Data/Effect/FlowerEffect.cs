using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 9100, Type = typeof(FlowerEffect))]
    public class FlowerEffect : EffectBase
    {
        protected override void OnTrigger()
        {
            SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().EmitUIParticle("M1EffectHuaBan",5f);
        }
    }
}