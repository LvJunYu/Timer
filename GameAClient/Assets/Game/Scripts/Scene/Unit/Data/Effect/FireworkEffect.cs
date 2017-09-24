using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 9101, Type = typeof(FireworkEffect))]
    public class FireworkEffect : EffectBase
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().GetUIParticle("M1EffectYanHua");
            return true;
        }
        
        protected override void OnTriggerEnter()
        {
            SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().EmitUIParticle("M1EffectYanHua",_trans.position);
        }
    }
}