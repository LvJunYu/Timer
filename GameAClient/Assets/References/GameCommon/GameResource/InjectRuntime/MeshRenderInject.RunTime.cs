/********************************************************************
** Filename : MeshRenderInject  
** Author : ake
** Date : 8/8/2016 11:52:09 AM
** Summary : MeshRenderInject  
***********************************************************************/


using UnityEngine;

namespace SoyEngine
{
	public partial class MeshRenderInject:BaseInjectHandler
	{
		public override bool InjectDependsAsset(InjectItem item)
		{
			if (!base.InjectDependsAsset(item))
			{
				LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0}", item.ToString());
				return false;
			}
			if (item.AssetNameArray.Length != 1)
			{
				LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0},item.AssetNameArray.Length != 3", item.ToString());
				return false;
			}
			MeshRenderer assetObject = item.InjectObject as MeshRenderer;
			if (assetObject == null)
			{
				LogHelper.Error("item.InjectObject as MeshRenderer is null! item is {0}", item.ToString());
				return false;
			}
			if (assetObject.sharedMaterial != null)
			{
				Texture t;
				if (GameResourceManager.Instance.TryGetTextureByName(item.DefaultAssetName, out t))
				{
					assetObject.sharedMaterial.mainTexture = t;

					{
						if (Application.isEditor && Application.platform == RuntimePlatform.OSXEditor)
						{
							assetObject.sharedMaterial.shader = Shader.Find(assetObject.sharedMaterial.shader.name);
						}
					}
					return true;
				}
			}
			return false;
		}
	}
}
