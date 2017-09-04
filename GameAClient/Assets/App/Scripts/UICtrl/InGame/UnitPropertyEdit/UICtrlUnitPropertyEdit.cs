using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlUnitPropertyEdit : UICtrlInGameAnimationBase<UIViewUnitPropertyEdit>
    {
        #region 常量与字段

        private UnitEditData _originData;
        
        #endregion
        
        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void CreateSequences()
        {
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            _openSequence.Append(_cachedView.ContentDock.DOSizeDelta(new Vector2(500f, 100f), 0.5f).From());
            _closeSequence.Append(_cachedView.ContentDock.DOSizeDelta(new Vector2(500f, 100f), 0.5f));
            _openSequence.OnComplete(OnOpenAnimationComplete).SetAutoKill(false).Pause().OnUpdate(OnOpenAnimationUpdate);
            _closeSequence.OnComplete(OnCloseAnimationComplete).SetAutoKill(false).Pause()
                .PrependCallback(() => _cachedView.Trans.localPosition = Vector3.zero);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _originData = (UnitEditData) parameter;
        }
        
        

        #endregion
    }
}