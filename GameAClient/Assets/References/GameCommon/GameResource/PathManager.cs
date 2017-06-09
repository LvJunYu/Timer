/********************************************************************

** Filename : PathManager
** Author : ake
** Date : 2016/3/14 17:08:03
** Summary : PathManager
***********************************************************************/

using System;
using GameA;
using UnityEngine;

namespace SoyEngine
{
    public class PathManager
    {
        private static string _bundleFileExtension = ".soy";
        public string GetManifestPath(string gameName)
        {
//            return string.Format("{0}/{1}/{2}", GameResourcePathManager.Instance.RootPath, gameName, BuildToolsConstDefine.ExportManifestAssetName);
            return string.Format ("{0}/{1}/{2}{3}", Application.streamingAssetsPath, gameName, BuildToolsConstDefine.ExportManifestAssetName, _bundleFileExtension);
        }

        public string GetGameLocalAssetBundlePath(string assetName,string gameName)
        {
//            return string.Format("{0}/{1}/{2}", GameResourcePathManager.Instance.RootPath, gameName, assetName);
            return string.Format ("{0}/{1}/{2}{3}", Application.streamingAssetsPath, gameName, assetName, _bundleFileExtension);
        }	

	    public string GetGameLoadlConfig(string configName,string gameName)
	    {
            return string.Format("{0}/{1}/{2}", GameResourcePathManager.Instance.RootPath, gameName, configName);
		}


		public string GetLocalePath()
		{
			//string res = string.Format("{0}/{1}/{2}", _localePath, BuildToolsConstDefine.LocalePath,GM2DApp.Instance.Language.ToString());
			return "";
		}

#if UNITY_EDITOR

	    public string GetDebugResPath(EResType type,string name)
	    {
		    switch (type)
		    {
				case EResType.Prefab:
			    {

				    break;
			    }
		    }
		    return null;
	    }


#endif

		#region private

		#endregion
	}
}
