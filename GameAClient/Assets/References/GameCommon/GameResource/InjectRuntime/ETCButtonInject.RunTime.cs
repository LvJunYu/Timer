/********************************************************************

** Filename : ETCButtonInject
** Author : ake
** Date : 2016/4/15 20:49:53
** Summary : ETCButtonInject
***********************************************************************/

using System;

namespace SoyEngine
{
	public partial class ETCButtonInject:BaseInjectHandler
	{
		public override bool InjectDependsAsset(InjectItem item)
		{
			if (!base.InjectDependsAsset(item))
			{
				LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0}", item.ToString());
				return false;
			}
			if (item.AssetNameArray.Length != 2)
			{
				LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0},item.AssetNameArray.Length != 2", item.ToString());
				return false;
			}
			ETCButton button = item.InjectObject as ETCButton;
			if (button == null)
			{
				LogHelper.Error("item.InjectObject as Button is null! item is {0}", item.ToString());
				return false;
			}
			button.normalSprite = GameResourceManager.Instance.GetSpriteByName(item.AssetNameArray[0]);
			button.pressedSprite = GameResourceManager.Instance.GetSpriteByName(item.AssetNameArray[1]);
			return true;
		}
	}
}
