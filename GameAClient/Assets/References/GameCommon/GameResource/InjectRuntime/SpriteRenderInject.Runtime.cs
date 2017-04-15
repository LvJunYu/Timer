/********************************************************************

** Filename : SpriteRenderInjectRunTime
** Author : ake
** Date : 2016/3/23 11:46:27
** Summary : SpriteRenderInjectRunTime
***********************************************************************/

using System;
using UnityEngine;

namespace SoyEngine
{
	public partial class SpriteRenderInject:BaseInjectHandler
	{
		public override bool InjectDependsAsset(InjectItem item)
		{
			if (!base.InjectDependsAsset(item))
			{
				LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0}", item.ToString());
				return false;
			}
			SpriteRenderer render = item.InjectObject as SpriteRenderer;
			if (render == null)
			{
				LogHelper.Error("item.InjectObject as SpriteRenderer is null! item is {0}", item.ToString());
				return false;
			}
			render.sprite = GameResourceManager.Instance.GetSpriteByName(item.DefaultAssetName);
			return true;
		}
	}
}
