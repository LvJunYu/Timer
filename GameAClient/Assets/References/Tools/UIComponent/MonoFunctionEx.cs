/********************************************************************

** Filename : MonoFunctionEx
** Author : ake
** Date : 2016/4/13 14:43:40
** Summary : MonoFunctionEx
***********************************************************************/

using GameA.Game;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace SoyEngine
{
	public static class MonoFunctionEx
	{
		public static void SetActiveEx(this GameObject go, bool value)
		{
			if (go != null && go.activeSelf != value)
			{
				go.SetActive(value);
			}
		}

		public static void SetActiveEx(this Component com, bool value)
		{
			if (com != null && com.gameObject!=null && com.gameObject.activeSelf != value)
			{
				com.gameObject.SetActive(value);
			}
		}

		public static void SetEnableEx(this MonoBehaviour com, bool value)
		{
			if (com != null)
			{
				com.enabled = value;
			}
		}

		public static void SetAlphaEx(this Graphic com, float alphaValue)
		{
			if (com == null)
			{
				return;
			}
			Color c = com.color;
			c.a = alphaValue;
			com.color = c;
		}

        public static void Reset(this SkeletonAnimation skeletonAnimation)
        {
            if (skeletonAnimation != null && skeletonAnimation.state != null && skeletonAnimation.Skeleton != null)
            {
                skeletonAnimation.state.ClearTracks();
                skeletonAnimation.Skeleton.SetToSetupPose();
                skeletonAnimation.Update(ConstDefineGM2D.FixedDeltaTime);
            }
        }
	}
}
