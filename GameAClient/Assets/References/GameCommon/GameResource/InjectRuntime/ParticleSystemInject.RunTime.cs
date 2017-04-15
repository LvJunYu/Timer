/********************************************************************
** Filename : ParticleSystemInject  
** Author : ake
** Date : 7/7/2016 2:27:25 PM
** Summary : ParticleSystemInject  
***********************************************************************/


using UnityEngine;

namespace SoyEngine
{
	public partial class ParticleSystemInject:BaseInjectHandler
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
			ParticleSystem assetObject = item.InjectObject as ParticleSystem;
			if (assetObject == null)
			{
				LogHelper.Error("item.InjectObject as ParticleSystem is null! item is {0}", item.ToString());
				return false;
			}
			Renderer re = assetObject.GetComponent<Renderer>();
			if (re != null && re.sharedMaterial != null)
			{
				if (Application.isEditor)
				{
					re.sharedMaterial.shader = Shader.Find(re.sharedMaterial.shader.name);
				}
				Texture t;
				if (GameResourceManager.Instance.TryGetTextureByName(item.DefaultAssetName, out t))
				{
					re.sharedMaterial.mainTexture = t;
					return true;
				}
			}
			return false;
		}
	}
}
