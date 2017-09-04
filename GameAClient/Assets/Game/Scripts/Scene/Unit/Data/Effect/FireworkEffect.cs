using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 9101, Type = typeof(FireworkEffect))]
    public class FireworkEffect : EffectBase
    {
        protected override void OnTrigger()
        {
            SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().EmitUIParticle("M1EffectYanHua",5f, _trans.position);
        }
    }
}