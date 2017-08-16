/********************************************************************
** Filename : UMCtrlBase
** Author : Dong
** Date : 2015/4/30 16:08:41
** Summary : UMCtrlBase
***********************************************************************/

using System;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

/*UM 是啥，初始化*/
namespace GameA
{
    public class UMCtrlBase<T> : UMCtrlGenericBase<T> where T : UMViewBase//todo
    {
        #region 变量

        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// Init表示的是初始化
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="localpos"></param>
        /// <returns></returns>
        public bool Init(RectTransform parent,Vector3 localpos = new Vector3())
        {
            return base.Init(parent, localpos, SocialGUIManager.Instance.UIRoot);
        }
        
        #endregion
    }
}
