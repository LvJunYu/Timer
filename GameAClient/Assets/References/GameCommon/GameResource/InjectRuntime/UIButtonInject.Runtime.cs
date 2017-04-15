/********************************************************************

** Filename : UIButtonInject
** Author : ake
** Date : 2016/3/23 16:52:02
** Summary : UIButtonInject
***********************************************************************/

using System;
using UnityEngine.UI;

namespace SoyEngine
{
	public partial class UIButtonInject:BaseInjectHandler
	{
		public override bool InjectDependsAsset(InjectItem item)
		{
			if (!base.InjectDependsAsset(item))
			{
				LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0}", item.ToString());
				return false;
			}
			if (item.AssetNameArray.Length != 3)
			{
				LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0},item.AssetNameArray.Length != 3", item.ToString());
				return false;
			}
			Button button = item.InjectObject as Button;
			if (button == null)
			{
				LogHelper.Error("item.InjectObject as Button is null! item is {0}", item.ToString());
				return false;
			}
			SpriteState state = new SpriteState();
			state.highlightedSprite = GameResourceManager.Instance.GetSpriteByName(item.AssetNameArray[0]);
			state.pressedSprite = GameResourceManager.Instance.GetSpriteByName(item.AssetNameArray[1]);
			state.disabledSprite = GameResourceManager.Instance.GetSpriteByName(item.AssetNameArray[2]);

			button.spriteState = state;
			return true;
		}
	}
}