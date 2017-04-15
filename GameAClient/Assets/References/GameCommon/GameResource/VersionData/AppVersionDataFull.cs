/********************************************************************
** Filename : AppVersionDataFull  
** Author : ake
** Date : 8/30/2016 10:23:02 AM
** Summary : AppVersionDataFull  
***********************************************************************/


using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SoyEngine
{
	public class AppVersionDataFull:AppVersionData
	{
		private Dictionary<string, GameLocalFileVersionPack> _buildInResVersionDic;

		public AppVersionDataFull()
		{
			_buildInResVersionDic = new Dictionary<string, GameLocalFileVersionPack>();
		}

		public AppVersionDataFull(AppVersionData data)
		{
			VersionId = data.VersionId;
			Md5Dic = data.Md5Dic;
			_buildInResVersionDic = new Dictionary<string, GameLocalFileVersionPack>();
		}

		public void AddGameFileVersionPack(string gameName, GameLocalFileVersionPack localFilePack)
		{
			if (!_buildInResVersionDic.ContainsKey(gameName))
			{
				_buildInResVersionDic.Add(gameName,localFilePack);
			}
		}

		public bool ContainGameFileVersionPack(string gameName)
		{
			return _buildInResVersionDic.ContainsKey(gameName);
		}

		public GameLocalFileVersionPack GetFileVersionPack(string gameName)
		{
			GameLocalFileVersionPack res = null;
			if (!_buildInResVersionDic.TryGetValue(gameName, out res))
			{
				return null;
			}
			return res;
		}

		public IEnumerator InitGamePackageLocaleFileVersionData()
		{		
			for (int i = 0; i < 1; i++)
			{
                string gameName = "GameMaker2D";
				if (_buildInResVersionDic.ContainsKey(gameName))
				{
					continue;
				}
				string resMd5, configMd5;
				string resVersionName = GameResourcePathManager.Instance.GetGameResourceVersionFileName(gameName);
				string configVersionName = GameResourcePathManager.Instance.GetGameConfigVersionFileName(gameName);

				if (!TryGetMd5Value(resVersionName, out resMd5) || !TryGetMd5Value(configVersionName, out configMd5))
				{
					continue;
				}
				string resVersionWholeName =GameResourcePathManager.GetFileWholeNameWithMd5(resVersionName, resMd5);
				string configVersionWholeName = GameResourcePathManager.GetFileWholeNameWithMd5(configVersionName, configMd5);

				string tmpPath;
				tmpPath = GameResourcePathManager.Instance.GetWebGameResUrl(resVersionWholeName, gameName, true);
				string tmpTxt;
				if (Application.platform == RuntimePlatform.Android)
				{
					WWW w = new WWW(tmpPath);
					yield return w;
					tmpTxt = w.text;
				}
				else
				{
					tmpTxt = File.ReadAllText(tmpPath);
				}

				GameVersionData resourceVersion = new GameVersionData();
				resourceVersion.Md5Dic = ResourceVersionTools.ParseConfigDataFromString(tmpTxt);
				resourceVersion.SetKey(resMd5);

				tmpPath = GameResourcePathManager.Instance.GetWebGameConfigUrl(configVersionWholeName, gameName, true);
				if (Application.platform == RuntimePlatform.Android)
				{
					WWW w = new WWW(tmpPath);
					yield return w;
					tmpTxt = w.text;
				}
				else
				{
					tmpTxt = File.ReadAllText(tmpPath);
				}

				GameVersionData configVersion = new GameVersionData();
				configVersion.Md5Dic = ResourceVersionTools.ParseConfigDataFromString(tmpTxt);
				configVersion.SetKey(configMd5);

				GameLocalFileVersionPack data = new GameLocalFileVersionPack();
				data.ConfigVersion = configVersion;
				data.ResourceVersion = resourceVersion;
				_buildInResVersionDic.Add(gameName,data);
			}
		}
	}
}