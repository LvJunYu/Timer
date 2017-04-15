/********************************************************************
** Filename : ResourceUnloadTools  
** Author : ake
** Date : 12/6/2016 2:33:09 PM
** Summary : ResourceUnloadTools  
***********************************************************************/


using Spine.Unity;
using UnityEngine;

namespace SoyEngine
{
	public static class ResourceUnloadTools
	{
		public static void UnloadSpineData(SkeletonDataAsset asset)
		{
			if (asset == null)
			{
				return;
			}
			if (asset.atlasAssets != null)
			{
				for (int i = 0; i < asset.atlasAssets.Length; i++)
				{
					var atlas = asset.atlasAssets[i];
					if (atlas != null)
					{
						if (atlas.materials != null)
						{
							for (int j = 0; j < atlas.materials.Length; j++)
							{
								var mat = atlas.materials[j];
								if (mat != null)
								{
									DestroyImmediate(mat);
								}
							}
						}
					}
					DestroyImmediate(atlas);
				}
			}
			DestroyImmediate(asset);
		}


		public static void DestroyImmediate(Object obj)
		{
			if (obj == null)
			{
				return;
			}
			Object.DestroyImmediate(obj, true);
		}
	}
}