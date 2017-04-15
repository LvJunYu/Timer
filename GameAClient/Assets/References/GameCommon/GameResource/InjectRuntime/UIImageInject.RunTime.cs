/********************************************************************

** Filename : UIImageInjectRunTime
** Author : ake
** Date : 2016/3/23 12:24:41
** Summary : UIImageInjectRunTime
***********************************************************************/

using System;
using UnityEngine.UI;

namespace SoyEngine
{
	public partial class UIImageInject:BaseInjectHandler
	{
		public override bool InjectDependsAsset(InjectItem item)
		{
			if (!base.InjectDependsAsset(item))
			{
				LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0}", item.ToString());
				return false;
			}
			Image render = item.InjectObject as Image;
			if (render == null)
			{
				LogHelper.Error("item.InjectObject as Image is null! item is {0}", item.ToString());
				return false;
			} 
			render.sprite = GameResourceManager.Instance.GetSpriteByName(item.DefaultAssetName);
			return true;
		}
	}
}