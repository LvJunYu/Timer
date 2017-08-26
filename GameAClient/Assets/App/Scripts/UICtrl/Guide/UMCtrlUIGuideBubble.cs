using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlUIGuideBubble: UMCtrlBase<UMViewUIGuideBubble>
    {
        #region 常量与字段

        #endregion
        #region 属性

        public RectTransform UITran
        {
            get { return _cachedView.Trans; }
        }
        #endregion

        #region 方法

        public void Show()
        {
            _cachedView.SetActiveEx(true);
        }

        public void Hide()
        {
            _cachedView.SetActiveEx(false);
        }

        public void Set(RectTransform targeRectTransform, EDirectionType arrowDirection, string content,
            bool mask = false)
        {
            
        }
        #endregion
    }
}