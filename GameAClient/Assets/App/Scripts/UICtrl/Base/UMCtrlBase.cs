/********************************************************************
** Filename : UMCtrlBase
** Author : Dong
** Date : 2015/4/30 16:08:41
** Summary : UMCtrlBase
***********************************************************************/

using SoyEngine;
using UnityEngine;

/*UM 是啥，初始化*/
namespace GameA
{
    public class UMCtrlBase<T> : UMCtrlResManagedBase<T> where T : UMViewBase
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
        /// <param name="resScenary"></param>
        /// <param name="localpos"></param>
        /// <returns></returns>
        public bool Init(RectTransform parent, EResScenary resScenary, Vector3 localpos = new Vector3())
        {
            return base.Init(parent, resScenary, localpos, SocialGUIManager.Instance.UIRoot as ResManagedUIRoot);
        }
        
        #endregion
    }
}
