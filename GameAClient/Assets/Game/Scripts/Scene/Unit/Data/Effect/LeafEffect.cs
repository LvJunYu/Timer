using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 9103, Type = typeof(LeafEffect))]
    public class LeafEffect : EffectBase
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().GetUIParticle("M1EffectLuoYe");
            return true;
        }
        
        protected override void OnTriggerEnter()
        {
            SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().EmitUIParticle("M1EffectLuoYe");
        }
    }
}