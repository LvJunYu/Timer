/********************************************************************

** Filename : GameLocalFileVersionPack
** Author : ake
** Date : 2016/4/19 17:03:40
** Summary : GameLocalFileVersionPack
***********************************************************************/

using System;

namespace SoyEngine
{
	public class GameLocalFileVersionPack
	{
		public GameVersionData ResourceVersion;
		public GameVersionData ConfigVersion;

        public EGameUpdateCheckResult CheckGameUpdateState(string resourceMd5, string configMd5)
		{
			if (string.IsNullOrEmpty(ResourceVersion.Md5) && string.IsNullOrEmpty(ConfigVersion.Md5))
			{
                return EGameUpdateCheckResult.NeedDownload;
			}
			if (string.Compare(resourceMd5, ResourceVersion.Md5, StringComparison.Ordinal) == 0 &&
			    string.Compare(configMd5, ConfigVersion.Md5, StringComparison.Ordinal) == 0)
			{
                return EGameUpdateCheckResult.CanPlay;
			}
			else
			{
                return EGameUpdateCheckResult.NeedUpdate;
			}
		}

		public bool TryGetResourceMd5(string fileName, out string md5)
		{
			return ResourceVersion.TryGetMd5Value(fileName, out md5);
		}

		public bool TryGetConfigMd5(string fileName, out string md5)
		{
			return ConfigVersion.TryGetMd5Value(fileName, out md5);
		}

		public override string ToString()
		{
			return string.Format("ResVersion {0},ConfigVersion {1}", ResourceVersion, ConfigVersion);
		}
	}
}