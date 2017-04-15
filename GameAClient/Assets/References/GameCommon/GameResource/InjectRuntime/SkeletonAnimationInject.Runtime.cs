/********************************************************************
** Filename : SkeletonAnimationInject  
** Author : ake
** Date : 5/31/2016 6:38:06 PM
** Summary : SkeletonAnimationInject  
***********************************************************************/


using Spine.Unity;

namespace SoyEngine
{
	public partial class SkeletonAnimationInject:BaseInjectHandler
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
				LogHelper.Error("InjectDependsAsset called but base.InjectDependsAsset(item) is false! item is {0},item.AssetNameArray.Length != 1", item.ToString());
				return false;
			}
			SkeletonAnimation skeleton = item.InjectObject as SkeletonAnimation;
			if (skeleton == null)
			{
				LogHelper.Error("item.InjectObject as SkeletonAnimation is null! item is {0}", item.ToString());
				return false;
			}
			SkeletonDataAsset data = null;
			if (!GameResourceManager.Instance.TryGetSpineDataByName(item.AssetNameArray[0],out data,false))
			{
				LogHelper.Error("GameResourceManager.Instance.TryGetSpineDataByName(item.AssetNameArray[0],out data) is false item.AssetNameArray[0] is {0}", item.AssetNameArray[0]);
				return false;
			}
			skeleton.skeletonDataAsset = data;
			return true;
		}
	}
}