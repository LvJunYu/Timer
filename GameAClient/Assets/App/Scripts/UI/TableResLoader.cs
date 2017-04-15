/********************************************************************
** Filename : TableResLoader  
** Author : ake
** Date : 2/28/2017 2:38:01 PM
** Summary : TableResLoader  
***********************************************************************/


using System.IO;
using SoyEngine;
using GameA;

public class TableResLoader
{
	private string _gameName;
	private string _rootPath;

	public TableResLoader(string gameName)
	{
		_gameName = gameName;
		_rootPath = string.Format(GameResourcePathManager.ResourcesPath, SoyPath.Instance.RootPath, GameResourcePathManager.GetPlatformFolderName());
	}

	public T GetConfigAssetData<T>(string configName) where T : BaseTableAsset
	{
		string path = GetGameLoadlConfig(configName, _gameName);
		if (File.Exists(path))
		{
			return GameResourceManager.LoadAssetBundleMainAsset<T>(path) as T;
		}
		return null;
	}

	private string GetGameLoadlConfig(string configName, string gameName)
	{
		return string.Format("{0}/{1}/{2}", _rootPath, gameName, configName);
	}
}
