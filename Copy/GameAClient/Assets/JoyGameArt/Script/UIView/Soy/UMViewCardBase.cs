/********************************************************************
** Filename : UMViewCardBase
** Author : Dong
** Date : 2015/5/4 17:49:52
** Summary : UMViewCardBase
***********************************************************************/

using UnityEngine.UI;

namespace GameA
{
    public class UMViewCardBase : UMViewResManagedBase
    {
		/// <summary>
		/// 图片按钮
		/// </summary>
        public Button Card;
		/// <summary>
		/// 父物体的位置
		/// </summary>
        public RawImage Icon;
		/// <summary>
		/// 名字
		/// </summary>
        public Text Name;
    }
}
