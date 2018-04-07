using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 9100, Type = typeof(FlowerEffect))]
    public class FlowerEffect : EffectBase
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            GameParticleManager.Instance.PreLoadParticle("M1EffectHuaBan");
            return true;
        }

        protected override void OnTriggerEnter()
        {
            SocialGUIManager.Instance.GetUI<UICtrlGameScreenEffect>().EmitUIParticle("M1EffectHuaBan", _trans.position);
        }
    }
}