using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlGameScreenEffect : UICtrlInGameBase<UIViewUIGuideBubble>
    {
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameStart;
        }

        public UIParticleItem EmitUIParticle(string itemName, Vector3 pos = default(Vector3))
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameScreenEffect>();
            }
            var uiparticle = GameParticleManager.Instance.EmitUIParticle(itemName, _cachedView.Trans, _groupId, pos);
            return uiparticle;
        }

        public UIParticleItem EmitUIParticle(string itemName, Transform parent, int groupId, float lifeTime, Vector3 pos = default(Vector3))
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameScreenEffect>();
            }
            var uiparticle =
                GameParticleManager.Instance.EmitUIParticle(itemName, _cachedView.Trans, _groupId, groupId, pos);
            return uiparticle;
        }
    }
}