/********************************************************************
 ** Filename : UISocialCtrlBase.cs
 ** Author : quansiwei
 ** Date : 2015/5/7 0:05
 ** Summary : UISocialCtrlBase.cs
 ***********************************************************************/


using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public abstract class UISocialContentCtrlBase<T> : UISocialCtrlBase<T>, IUISocialContentCtrl where T : UIViewBase
    {
        #region 变量

        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void OnOpen(object parameter)
        {
            ScrollRect sr = GetBoundsScrollRect();
            if (sr == null)
            {
                LogHelper.Warning("UISocialContentCtrlBase Without BoundsScrollRect, UICtrl Name is {0}", GetType().Name);
                return;
            }
            var rectTransform = _cachedView.Trans;
            
            if (rectTransform == null)
            {
                LogHelper.Warning("UISocialContentCtrlBase Without RectTransform, UICtrl Name is {0}", GetType().Name);
                return;
            }
            
            int top = 0;
            int bottom = 0;
            
            if (this is IUIWithTitle)
            {
                top = SocialUIConfig.TitleHeight;
            }
            else
            {
                top = SocialUIConfig.SystemStatusBarHeight;
            }
            if (this is IUIWithTaskBar)
            {
                bottom = SocialUIConfig.TaskBarHeight;
            }
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
            base.OnOpen(parameter);
        }

        /// <summary>
        ///     获取内容最外围的滚动视图
        /// </summary>
        /// <returns>The bounds scroll rect.</returns>
        public virtual ScrollRect GetBoundsScrollRect()
        {
            return _view.Trans.GetComponent<ScrollRect>();
        }

        public void ScrollToTop()
        {
            ScrollRect sr = GetBoundsScrollRect();
            if (sr == null)
            {
                LogHelper.Warning("UISocialContentCtrlBase Without BoundsScrollRect, UICtrl Name is {0}", GetType().Name);
                return;
            }
            sr.normalizedPosition = new Vector2(0, 1);
        }

        public void LockScroll ()
        {
            ScrollRect sr = GetBoundsScrollRect ();
            if (sr == null) {
                LogHelper.Warning ("UISocialContentCtrlBase Without BoundsScrollRect, UICtrl Name is {0}", GetType ().Name);
                return;
            }
            sr.vertical = false;
        }

        public void UnLockScroll ()
        {
            ScrollRect sr = GetBoundsScrollRect ();
            if (sr == null) {
                LogHelper.Warning ("UISocialContentCtrlBase Without BoundsScrollRect, UICtrl Name is {0}", GetType ().Name);
                return;
            }
            sr.vertical = true;
        }

        #endregion
    }
}