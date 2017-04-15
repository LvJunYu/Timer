/********************************************************************

** Filename : UITextInject
** Author : ake
** Date : 2016/3/23 12:27:30
** Summary : UITextInject
***********************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoyEngine
{
	public partial class UITextInject:BaseInjectHandler
	{
		public override bool InjectDependsAsset(InjectItem item)
		{
			Debug.LogErrorFormat("InjectDependsAsset called! {0}",item.DefaultAssetName);
			if (!base.InjectDependsAsset(item))
			{
				LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0}", item.ToString());
				return false;
			}
			Text text = item.InjectObject as Text;
			if (text == null)
			{
				LogHelper.Error("item.InjectObject as Text is null! item is {0}", item.ToString());
				return false;
			}
			Font font = AppResourceManager.Instance.GetFont(item.DefaultAssetName);
			Debug.LogErrorFormat("InjectDependsAsset called! font == null", font == null);
			//if (!GameResourceManager.Instance.TryGetFontAssetByName(item.DefaultAssetName, out font))
			if (font == null)
			{
				LogHelper.Error("InjectDependsAsset called but get font {1} failed,item is {0}", item.ToString(), item.DefaultAssetName);
				return false;
			}
			text.font = font;
			return true;
		}
	}
}
