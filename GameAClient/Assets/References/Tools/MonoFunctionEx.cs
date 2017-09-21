/********************************************************************

** Filename : MonoFunctionEx
** Author : ake
** Date : 2016/4/13 14:43:40
** Summary : MonoFunctionEx
***********************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace SoyEngine
{
	public static partial class MonoFunctionEx
	{
		public static void SetActiveStateEx(this UnityNativeParticleItem com, bool value)
		{
			if (com != null)
			{
				com.SetActiveState(value);
			}
		}
	}
}
