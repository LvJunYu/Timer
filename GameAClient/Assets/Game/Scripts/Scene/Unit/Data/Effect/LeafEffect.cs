using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 9103, Type = typeof(LeafEffect))]
    public class LeafEffect : EffectBase
    {
        protected override void OnTrigger()
        {
            SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().EmitUIParticle("M1EffectLuoYe",5f);
        }
    }
}