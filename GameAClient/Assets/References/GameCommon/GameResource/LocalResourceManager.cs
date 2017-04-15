/********************************************************************

** Filename : LocalResourceManager
** Author : ake
** Date : 2016/4/18 18:14:18
** Summary : LocalResourceManager
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameA;
using UnityEngine;

namespace SoyEngine
{
	public class LocalResourceManager : MonoBehaviour
	{

		#region static  const
		private static LocalResourceManager _instance;

		public static LocalResourceManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = CreateInstance();
				}
				return _instance;
			}
		}

		private static LocalResourceManager CreateInstance()
		{
			GameObject go = new GameObject("LocalResourceManager");
			LocalResourceManager res = go.AddComponent<LocalResourceManager>();
			return res;
		}

		public const int MaxLoaderCount = 1;
		public const string AppVersionKey = "SoyAppVersion";

		#endregion

        #region field
		private Stack<GameResourceLoaderEx> _idleLoaders;
		private Dictionary<string, GameLocalFileVersionPack> _curGameLocalFileData;
		private AppVersionDataFull _curAppVersion;
		private AppVersionDataFull _packageAppVersion = null;
        private EAppResVersionCheckState _appResVersionCheckState;
        private int _appResVersionChecking;

		private Dictionary<string, BaseBinaryPack> _localePackDic;

		
		#endregion field

		#region property

		public AppVersionDataFull CurAppVersion
        {
            get { return _curAppVersion; }
        }

        public EAppResVersionCheckState AppResVersionCheckState
        {
            get { return _appResVersionCheckState; }
        }

		public bool CheckAppVersionComplete
		{
			get
			{
				return _appResVersionCheckState == EAppResVersionCheckState.Checked;
			}
		}
        #endregion property

		#region function

		public void Init()
		{
			_idleLoaders = new Stack<GameResourceLoaderEx>();
			for (int i = 0; i < MaxLoaderCount; i++)
			{
				var loader = gameObject.AddComponent<GameResourceLoaderEx>();
				loader.InitLoader();
				_idleLoaders.Push(loader);
			}
			_curGameLocalFileData = new Dictionary<string, GameLocalFileVersionPack>();
            _appResVersionCheckState = EAppResVersionCheckState.UnCheck;
			ClearOverdueRes();
			InitLocalData();
		}

		public void CheckAppVersion(int versionValue)
		{
			_appResVersionChecking = versionValue;
            if(_appResVersionCheckState != EAppResVersionCheckState.Checking)
            {
				StartCoroutine("StartCheckAppVersion", versionValue);
				_appResVersionCheckState = EAppResVersionCheckState.Checking;
            }
		}

		public EGameUpdateCheckResult CheckGameLocalFile(string gameType)
		{
			GameLocalFileVersionPack data;
			if (_curGameLocalFileData.TryGetValue(gameType, out data))
			{
				string resourceVersionFileName = GameResourcePathManager.Instance.GetGameResourceVersionFileName(gameType);
				string configVersionFileName = GameResourcePathManager.Instance.GetGameConfigVersionFileName(gameType);
				string resourceMd5;
				string configMd5;
				if (!_curAppVersion.TryGetMd5Value(resourceVersionFileName, out resourceMd5) ||
				    !_curAppVersion.TryGetMd5Value(configVersionFileName, out configMd5)) 
				{
					LogHelper.Error("Local App Version error,doesn't have file {0} or {1},", resourceVersionFileName, configVersionFileName);
					return EGameUpdateCheckResult.Error;
				}

				var localeUpdateList = GetNeedToUpdateLocaleDataList(gameType);
				if (localeUpdateList.Count > 0)
				{
					return EGameUpdateCheckResult.NeedUpdate;
				}
				return data.CheckGameUpdateState(resourceMd5, configMd5);
			}
			LogHelper.Error("Invalid gameType {0}",gameType);
			return EGameUpdateCheckResult.Error;
		}

		public void DoUpdateGame(string gameType, Action finishCallback, Action<float> processCallback = null,Action failedCallback = null)
		{
			EGameUpdateCheckResult result = CheckGameLocalFile(gameType);
			if (result != EGameUpdateCheckResult.NeedUpdate && result != EGameUpdateCheckResult.NeedDownload)
			{
				LogHelper.Warning("DoUpdateGame failed,check result is unexpected!{0}", result);
				if (finishCallback != null)
				{
					finishCallback();
					return;
				}
				return;
			}
			if (_idleLoaders.Count == 0)
			{
				if (failedCallback != null)
				{
					failedCallback();
				}
				LogHelper.Error("Cur download count has reached the upper limit !");
				return;
			}
			GameResourceLoaderEx loader = _idleLoaders.Pop();
            loader.SetLoadingProperties(gameType, finishCallback, processCallback, failedCallback);

        }

		public GameLocalFileVersionPack GetGameLocalFileVersionPack(string gameTypeName)
		{
			GameLocalFileVersionPack data;
			if (!_curGameLocalFileData.TryGetValue(gameTypeName, out data))
			{
				LogHelper.Error("GetGameLocalFileVersionPack failed,gameTypeName is {0}",gameTypeName);
				return null;
			}
			return data;
		}

		public bool TryGetLatestGameResourceVersionFileMd5(string gameTypeName,out string md5)
		{
			string fileName = GameResourcePathManager.Instance.GetGameResourceVersionFileName(gameTypeName);
			return _curAppVersion.TryGetMd5Value(fileName, out md5);
		}

		public bool TryGetLatestGameConfigVersionFileMd5(string gameTypeName, out string md5)
		{
			string fileName = GameResourcePathManager.Instance.GetGameConfigVersionFileName(gameTypeName);
			return _curAppVersion.TryGetMd5Value(fileName, out md5);
		}

		public void OnLoaderFree(GameResourceLoaderEx loader)
		{
			_idleLoaders.Push(loader);
		}

		public bool TryGetWholeLocalePack(bool isAppLocale, ELanguage l,out LocalePack localePack,string gameName = "")
		{
			string loacleFilename;
			if (isAppLocale)
			{
				loacleFilename = GameResourcePathManager.Instance.GetLocaleDataFileName(l);
			}
			else
			{
				loacleFilename = GameResourcePathManager.Instance.GetGameLocaleDataFileName(l, gameName);
			}
			if (!_localePackDic.ContainsKey(loacleFilename))
			{
				localePack = null;
				LogHelper.Error("TryGetLocalePack called but l is invalid! {0}",l);
				return false;
			}
			var basePack = _localePackDic[loacleFilename];
			localePack = LocalePack.UpgradeFromBaseBinaryPack(basePack);
			if (localePack == null)
			{
				LogHelper.Error("UpgradeFromBaseBinaryPack failed!");
				return false;
			}
			_localePackDic[loacleFilename] = localePack;
			return true;
		}

		public bool TryGetLocalePackMd5(string fileName, out string md5)
		{
			BaseBinaryPack outValue;
			if (!_localePackDic.TryGetValue(fileName, out outValue))
			{
				md5 = "";
				return false;
			}
			md5 = outValue.Version;
			return true;
		}

		public bool TryGetLocalePack(string fileName, out BaseBinaryPack outValue)
		{
			return _localePackDic.TryGetValue(fileName, out outValue);
		}

		public void UpdateLocalePackDic(string fileName, BaseBinaryPack value)
		{
			_localePackDic[fileName] = value;
		}

		public List<string> GetNeedToUpdateLocaleDataList(string gameName)
		{
			List<string> updateList = new List<string>();
			string tmpFileName;
			string localeMd5;
			string targetMd5;
			for (ELanguage i = 0; i < ELanguage.Max; i++)
			{
				tmpFileName = GameResourcePathManager.Instance.GetGameLocaleDataFileName(i, gameName);
				if (!LocalResourceManager.Instance.CurAppVersion.TryGetMd5Value(tmpFileName, out targetMd5))
				{
					LogHelper.Error("LocalResourceManager.Instance.CurAppVersion.TryGetMd5Value failed! fileName is {0} ", tmpFileName);
					continue;
				}
				if (!LocalResourceManager.Instance.TryGetLocalePackMd5(tmpFileName, out localeMd5))
				{
					localeMd5 = "";
				}
				if (String.CompareOrdinal(localeMd5, targetMd5) != 0)
				{
					updateList.Add(tmpFileName);
				}
			}
			return updateList;
		}

		public List<ResourceCheckRes> GetLocaleChangeList(string gameName)
		{
			List<ResourceCheckRes> updateList = new List<ResourceCheckRes>();
			string tmpFileName;
			string localeMd5;
			string targetMd5;
			for (ELanguage i = 0; i < ELanguage.Max; i++)
			{
				tmpFileName = GameResourcePathManager.Instance.GetGameLocaleDataFileName(i, gameName);
				if (!LocalResourceManager.Instance.CurAppVersion.TryGetMd5Value(tmpFileName, out targetMd5))
				{
					LogHelper.Error("LocalResourceManager.Instance.CurAppVersion.TryGetMd5Value failed! fileName is {0} ", tmpFileName);
					continue;
				}
				if (!LocalResourceManager.Instance.TryGetLocalePackMd5(tmpFileName, out localeMd5))
				{
					localeMd5 = "";
				}
				if (String.CompareOrdinal(localeMd5, targetMd5) != 0)
				{
					updateList.Add(new ResourceCheckRes()
					{
						CheckResType = EResourceCheckResType.Change,
						LoadFromPackageRes = false,
						NewMd5Value = targetMd5,
						ResourceName = tmpFileName,
					});
				}
			}
			return updateList;
		}

		public GameLocalFileVersionPack GetBuildInPackageFileVersionPack(string gameName)
		{
			if (_packageAppVersion == null)
			{
				return null;
			}
			return _packageAppVersion.GetFileVersionPack(gameName);
		}

        public float GetNeedDownloadSizeMB(string gameType)
        {
            GameLocalFileVersionPack data;
			GameLocalFileVersionPack buildInPack = LocalResourceManager.Instance.GetBuildInPackageFileVersionPack(gameType);
			if (_curGameLocalFileData.TryGetValue(gameType, out data))
            {
                GameLocalFileVersionPack updateTarget = _curAppVersion.GetFileVersionPack(gameType);
                float totalUpdateCount = 0;
                if (updateTarget != null)
                {
                    List<ResourceCheckRes> resChangeList = ResourceVersionTools.CompareLocalResourceVersionFromWeb(data.ResourceVersion.Md5Dic,
                        updateTarget.ResourceVersion.Md5Dic);
					ResourceVersionTools.UpdateChangeListLoadFrom(resChangeList, buildInPack, true);
					List<ResourceCheckRes> configChangeList = ResourceVersionTools.CompareLocalResourceVersionFromWeb(data.ConfigVersion.Md5Dic,
                        updateTarget.ConfigVersion.Md5Dic);
					ResourceVersionTools.UpdateChangeListLoadFrom(configChangeList, buildInPack, false);

					for (int i = 0; i < resChangeList.Count; i++)
					{
						var item = resChangeList[i];
						if (!item.LoadFromPackageRes)
						{
							totalUpdateCount += item.Size;
						}
                    }
                    for (int i = 0; i < configChangeList.Count; i++)
                    {
						var item = configChangeList[i];
	                    if (!item.LoadFromPackageRes)
	                    {
							totalUpdateCount += item.Size;
						}
					}
                    return totalUpdateCount = totalUpdateCount / 1024 / 1024;
                }
                else
                {
	                LogHelper.Error("_curAppVersion.GetFileVersionPack(gameType) return  null! gameType is {0}",gameType);
                }
            }
			else
			{
				LogHelper.Error("_curGameLocalFileData.TryGetValue(gameType, out data) return false, gameType is {0}",gameType);
			}
            return 0;
        }
		#region  private 

		private void InitLocalData()
		{
			_localePackDic = new Dictionary<string, BaseBinaryPack>();
			DirectoryInfo localeDir = new DirectoryInfo(GameResourcePathManager.Instance.GetLocaleDataFileLocalFilePath());
			if (localeDir.Exists)
			{
				var files = localeDir.GetFiles("*");
				for (int i = 0; i < files.Length; i++)
				{
					var curFile = files[i];
					_localePackDic.Add(curFile.Name, BaseBinaryPack.ReadHeaderFromFile(curFile.FullName));
				}
			}
			else
			{
				localeDir.Create();
			}
			for (ELanguage i = 0; i < ELanguage.Max; i++)
			{
				string appLocaleFileName = GameResourcePathManager.Instance.GetLocaleDataFileName(i);
				if (_localePackDic.ContainsKey(appLocaleFileName))
				{
					continue;
				}
				string path = GameResourcePathManager.Instance.GetBuildInLocaleDataFilePath(appLocaleFileName);
				_localePackDic.Add(appLocaleFileName,BaseBinaryPack.ReadHeaderFromResource(path));
			}
		}

		private IEnumerator StartCheckAppVersion(int versionValue)
		{
			LogHelper.Debug("StartCheckAppVersion value :{0}",versionValue);
			//MatrixManager.Instance.AllMatrixList;
			string localFilePath = GameResourcePathManager.Instance.GetAppResourceVersionLocalFilePath();
			var localAppVersion = ResourceVersionTools.ParseLocalAppVersionDataFromFile(localFilePath);

            string gameResWebRoot = GameResourcePathManager.Instance.WebServerRoot;
			//if (localeData.VersionId != versionValue)
			{
				string appVersionUrl = GameResourcePathManager.Instance.GetAppResourceVersionUrl(versionValue);
				LogHelper.Debug("StartCheckAppVersion appConfig url is {0}", appVersionUrl);
				WWW versionFileWWW = new WWW(appVersionUrl);
				yield return versionFileWWW;
                LogHelper.Info("AppResourceVersion Url: " + appVersionUrl);
                LogHelper.Info(versionFileWWW.text);
				if (versionFileWWW.isDone && string.IsNullOrEmpty(versionFileWWW.error))
				{
					Dictionary<string, ResConfig> newVersionData = ResourceVersionTools.ParseConfigDataFromString(versionFileWWW.text);
					AppVersionData webData = new AppVersionData();
					webData.Md5Dic = newVersionData;
					webData.VersionId = versionValue;
					if (ResourceVersionTools.WriteVersionData(webData, localFilePath, false))
					{
						localAppVersion = webData;
					}
					else
					{
						LogHelper.Error("Write new AppVersionData failed,use local version file, cur versionId is {0},new versionId is {1}\n" +
						                "url {2}  \n"+ 
                            "detail is {3} fff", localAppVersion.VersionId,versionValue, versionFileWWW.url, "WriteVersionDataError");
					}
				}
				else
				{
					LogHelper.Error("Update AppVersionData failed,use local version file, cur versionId is {0},new versionId is {1}\n" +
                                    "url {2}  \n" +
                        "detail is {3}", localAppVersion.VersionId,versionValue, versionFileWWW.url, versionFileWWW.error);
				}
			}
			_curAppVersion = new AppVersionDataFull(localAppVersion);

            _curGameLocalFileData.Clear();
			LogHelper.Debug("Init build in pack res，gameList.count = {0}", 1);
            for(int i=0; i<1; i++)
            {
				string gameName = "GameMaker2D";
				//索引本地文件配置
				{
					string tmpFileName = GameResourcePathManager.Instance.GetGameResourceVersionFileName(gameName);
					GameVersionData resourceVersion =
						ResourceVersionTools.ParseGameVersionDataFromFile(
							GameResourcePathManager.Instance.GetLocalFileWholePath(gameName, tmpFileName));

					tmpFileName = GameResourcePathManager.Instance.GetGameConfigVersionFileName(gameName);
					GameVersionData configVersion = ResourceVersionTools.ParseGameVersionDataFromFile(
						GameResourcePathManager.Instance.GetLocalFileWholePath(gameName, tmpFileName));

					GameLocalFileVersionPack data = new GameLocalFileVersionPack();
					data.ConfigVersion = configVersion;
					data.ResourceVersion = resourceVersion;
                    LogHelper.Debug("Init build in pack res，game name {0}, configVersion is {1},ResourceVersion is {2}", gameName, configVersion.Md5, resourceVersion.Md5);
					if (!_curGameLocalFileData.ContainsKey(gameName))
					{
						_curGameLocalFileData.Add(gameName, data);
					}
				}
				{
					if (_curAppVersion.ContainGameFileVersionPack(gameName))
					{
						continue;
					}
					string resourceVersionMd5, configVersionMd5;
					string resourceVersionFileName = GameResourcePathManager.Instance.GetGameResourceVersionFileName(gameName);
					string configVersionFileName = GameResourcePathManager.Instance.GetGameConfigVersionFileName(gameName);
					if (!_curAppVersion.TryGetMd5Value(configVersionFileName, out configVersionMd5) ||
					    !_curAppVersion.TryGetMd5Value(resourceVersionFileName, out resourceVersionMd5))
					{
						continue;
					}
					string tmpWholeName, tmpWholeUrl;
					tmpWholeName = GameResourcePathManager.GetFileWholeNameWithMd5(resourceVersionFileName, resourceVersionMd5);
					tmpWholeUrl = GameResourcePathManager.Instance.GetWebGameResUrl(tmpWholeName, gameName);
					WWW w1 = new WWW(tmpWholeUrl);
					yield return w1;
					if (!w1.isDone || !string.IsNullOrEmpty(w1.error))
					{
						continue;
					}
					tmpWholeName = GameResourcePathManager.GetFileWholeNameWithMd5(configVersionFileName, configVersionMd5);
					tmpWholeUrl = GameResourcePathManager.Instance.GetWebGameConfigUrl(tmpWholeName, gameName);
					WWW w2= new WWW(tmpWholeUrl);
					yield return w2;
					if (!w2.isDone || !string.IsNullOrEmpty(w2.error))
					{
						continue;
					}
					Dictionary<string, ResConfig> resourcesVersionOnWeb =
						ResourceVersionTools.ParseConfigDataFromString(w1.text);
					Dictionary<string, ResConfig> configVersionOnWeb =
						ResourceVersionTools.ParseConfigDataFromString(w2.text);
					GameLocalFileVersionPack pack = new GameLocalFileVersionPack();
					pack.ResourceVersion = new GameVersionData();
					pack.ResourceVersion.Md5 = resourceVersionMd5;
					pack.ResourceVersion.Md5Dic = resourcesVersionOnWeb;

					pack.ConfigVersion = new GameVersionData();
					pack.ConfigVersion.Md5 = configVersionMd5;
					pack.ConfigVersion.Md5Dic = configVersionOnWeb;
					_curAppVersion.AddGameFileVersionPack(gameName, pack);
					LogHelper.Debug("_curAppVersion.AddGameFileVersionPack(gameName, pack); gameName is {0} 。",gameName);
				}

			}

			yield return UpdatePackageConfigData(SocialApp.Instance.PackageAppResourceVersion, versionValue);

            if (versionValue == _appResVersionChecking && gameResWebRoot == GameResourcePathManager.Instance.WebServerRoot)
            {
                _appResVersionCheckState = EAppResVersionCheckState.Checked;
	            if (CheckAppLocaleNeedToUpdate())
	            {
		            StartCoroutine("UpdateAppLocale");
	            }
                Messenger.Broadcast(GameA.EMessengerType.CheckAppVersionComplete);
            }
            else
            {
                StartCoroutine("StartCheckAppVersion", _appResVersionChecking);
            }
		}

		private IEnumerator UpdatePackageConfigData(int packVersion,int requestVersionId)
		{
			if (requestVersionId == 0)
			{
				yield break;
			}
			if (_packageAppVersion != null)
			{
				yield break;
			}
			if (!Application.isEditor &&
			    (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
			{
				string path = GameResourcePathManager.Instance.GetPackageAppResourceVersionPath(packVersion);
				string allTxt = "";
				if (Application.platform == RuntimePlatform.Android)
				{
					WWW w = new WWW(path);
					yield return w;
					allTxt = w.text;
				}
				else
				{
					if (File.Exists(path))
					{
						allTxt = File.ReadAllText(path);
					}
				}
				LogHelper.Debug("UpdatePackageConfigData failed, path {0}, content is :\n{1}", path, allTxt);
				if (string.IsNullOrEmpty(allTxt))
				{
					LogHelper.Error("UpdatePackageConfigData failed path is {0}",path);
					yield break;
				}
				Dictionary<string, ResConfig> newVersionData = ResourceVersionTools.ParseConfigDataFromString(allTxt);
				AppVersionDataFull data = new AppVersionDataFull();
				data.Md5Dic = newVersionData;
				data.VersionId = packVersion;
				_packageAppVersion = data;
				yield return _packageAppVersion.InitGamePackageLocaleFileVersionData();
			}

		}

		private bool CheckAppLocaleNeedToUpdate()
		{
			string appLocaleFileName = GameResourcePathManager.Instance.GetLocaleDataFileName(SocialApp.Instance.Language);
			var curValue = LocaleManager.Instance.CurAppLocaleVersion;
			string targetMD5;
			if (!_curAppVersion.TryGetMd5Value(appLocaleFileName,out targetMD5))
			{
				return false;
			}
			return String.CompareOrdinal(targetMD5, curValue) != 0;
		}

		private IEnumerator UpdateAppLocale()
		{
			string appLocaleFileName = GameResourcePathManager.Instance.GetLocaleDataFileName(SocialApp.Instance.Language);
			string md5;
			if (!_curAppVersion.TryGetMd5Value(appLocaleFileName,out md5))
			{
				LogHelper.Error("_curAppVersion.TryGetMd5Value failed!! fileName is {0}", appLocaleFileName);
				yield break; 
			}
			BaseBinaryPack pack;
			if (!_localePackDic.TryGetValue(appLocaleFileName, out pack))
			{
				LogHelper.Error("_localePackDic.TryGetValue failed!! fileName is {0}", appLocaleFileName);
				yield break;
			}

			string wholeFileName = string.Format("{0}.{1}", appLocaleFileName, md5);
			string url = GameResourcePathManager.Instance.GetLocalePackUrl(wholeFileName);
			WWW ww = new WWW(url);
			yield return ww;
			if (!ww.isDone || !string.IsNullOrEmpty(ww.error))
			{
				LogHelper.Error("Load failed! url is {0} error is {1}", url,ww.error);
				yield break; 
			}
			string path = Path.Combine(GameResourcePathManager.Instance.GetLocaleDataFileLocalFilePath(), appLocaleFileName);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			File.WriteAllBytes(path,ww.bytes);
			pack.LinkDataToFile(path);

			LocaleManager.Instance.ReloadAppLocaleData();
		}

		private void ClearOverdueRes()
		{
			string appVersionValue = "0";
			if (PlayerPrefs.HasKey(AppVersionKey))
			{
                appVersionValue = PlayerPrefs.GetString(AppVersionKey);
			}
			if (SocialApp.Instance.AppVersion != appVersionValue)
			{
                PlayerPrefs.SetString(AppVersionKey, SocialApp.Instance.AppVersion);
				if (Directory.Exists(GameResourcePathManager.Instance.RootPath))
				{
					FileTools.DeleteFolder(GameResourcePathManager.Instance.RootPath);
				}
			}
		}

		#endregion
		#endregion

        public enum EAppResVersionCheckState
        {
            None,
            UnCheck,
            Checking,
            Checked,
        }

	}

    public enum EGameUpdateCheckResult
    {
        CanPlay,
        NeedUpdate,
        NeedDownload,
        Error,
    }

	//public enum EGame
}
