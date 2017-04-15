/********************************************************************
** Filename : PlatformTools  
** Author : ake
** Date : 3/15/2017 3:01:57 PM
** Summary : PlatformTools  
***********************************************************************/


using UnityEngine;

namespace GameA.Game
{
	public class PlatformTools
	{
		public static bool IsRunInMacEditor()
		{
			return Application.platform == RuntimePlatform.OSXEditor;
		}
	}
}
