/********************************************************************
** Filename : UMCtrlBase
** Author : Dong
** Date : 2015/4/30 16:08:41
** Summary : UMCtrlBase
***********************************************************************/

using SoyEngine;
using UnityEngine.UI;
using UnityEngine;


namespace GameA
{
    public class UMCtrlBase<T> : UMCtrlGenericBase<T> where T : UMViewBase
    {
        #region 变量

        #endregion

        #region 属性

        #endregion

        #region 方法

        public bool Init(RectTransform parent,Vector3 localpos = new Vector3())
        {
            return base.Init(parent, localpos, SocialGUIManager.Instance.UIRoot);
        }
        
        #endregion
    }
}