using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlGameBottomTip : UICtrlInGameAnimationBase<UIViewGameBottomTip>
    {
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameTip;
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            _openSequence.Append(_cachedView.ContentDock.DOScale(Vector3.zero, 0.3f).From().SetEase(Ease.OutBack));
            _closeSequence.Append(_cachedView.ContentDock.DOScale(Vector3.zero, 0.15f).SetEase(Ease.Linear));
        }

        public void ShowTip(string tip)
        {
            if (_isOpen)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlGameBottomTip>();
            }
            SocialGUIManager.Instance.OpenUI<UICtrlGameBottomTip>();
            DictionaryTools.SetContentText(_cachedView.ContentText, tip);
        }

        public void CloseTip()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGameBottomTip>();
        }
    }
}
