using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlGameBottomTip : UICtrlInGameAnimationBase<UIViewGameBottomTip>
    {
        #region 常量与字段
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameTip;
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            _openSequence.Append(_cachedView.ContentDock.DOScale(Vector3.zero, 0.3f).From().SetEase(Ease.OutBack));
            _closeSequence.Append(_cachedView.ContentDock.DOSizeDelta(Vector3.zero, 0.3f).SetEase(Ease.Linear));
        }

        public void ShowTip(string tip)
        {
            if (_isOpen)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlGameBottomTip>();
            }
            DictionaryTools.SetContentText(_cachedView.ContentText, tip);
            SocialGUIManager.Instance.OpenUI<UICtrlGameBottomTip>();
        }

        public void CloseTip()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGameBottomTip>();
        }
        #endregion
    }
}