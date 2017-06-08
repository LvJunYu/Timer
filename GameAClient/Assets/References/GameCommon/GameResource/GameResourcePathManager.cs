/********************************************************************

** Filename : GameResourcePathManager
** Author : ake
** Date : 2016/4/6 15:23:42
** Summary : GameResourcePathManager
***********************************************************************/

using System;
using System.IO;
using GameA.Game;
using UnityEngine;
using SoyEngine;

namespace SoyEngine
{
	public class GameResourcePathManager
	{

		private static GameResourcePathManager _instance;

		private string _webServerRoot;
		private string _webServerResourceRoot;
		private string _rootPath;

		public const string ResourcesPath = "{0}/AppGame/{1}";
		public const string GameResourceVersionFileNameFormat = "{0}_Version";
		public const string GameConfigVersionFileNameFormat = "{0}Config_Version";
		public const string GameConfigFolderNameFormat = "{0}_Config";

		public const string AppLocaleDataFileName = "AppLocaleData_{0}";
		public const string GameLocaleDataFileName = "{0}LocaleData_{1}";
		public const string LocaleFileFolderName = "Locale";

		public const string AppResourceConfig = "AppResourceConfig";

        public const string BehaviorTreeDataPath = "BehaviorTree";

		private static string _streamAssetRoot = null;


		public static GameResourcePathManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GameResourcePathManager();
				}
				return _instance;
			}
		}

		public static string MobileStreamAssetPath
		{
			get
			{
				if (!Application.isEditor &&
				    (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
				{
					if (string.IsNullOrEmpty(_streamAssetRoot))
					{
						_streamAssetRoot = Application.streamingAssetsPath + "/" + GetPlatformFolderName();
					}
					return _streamAssetRoot;
				}
				else
				{
					return null;
				}

			}
		}

        public string WebServerRoot
        {
            get { return _webServerRoot; }
	        set
	        {
		        _webServerRoot = value;
				_webServerResourceRoot = string.Format("{0}/{1}", _webServerRoot, GetPlatformFolderName());
			}
        }


	    public string RootPath
	    {
	        get { return _rootPath; }
	    }

		public GameResourcePathManager()
		{
			InitRootPath();
		}

		#region private

		private void InitRootPath()
		{
			_rootPath = string.Format(ResourcesPath, SoyPath.Instance.RootPath, GetPlatformFolderName());
		}

		#endregion


		public string GetAppResourceVersionUrl(int versionValue)
		{
			return string.Format("{0}/{1}", _webServerResourceRoot,
				string.Format("{0}.{1}",AppResourceConfig, versionValue));
		}

		public string GetPackageAppResourceVersionPath(int versionValue)
		{
			string path = MobileStreamAssetPath;
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			return string.Format("{0}/{1}", MobileStreamAssetPath,
				string.Format("{0}.{1}", AppResourceConfig, versionValue));
		}

		public string GetLocalePackUrl(string fileName)
		{
			return string.Format("{0}/{1}/{2}", _webServerRoot, LocaleFileFolderName, fileName);
		}

		public string GetWebGameResUrl(string wholeName,string gameName,bool inPackage = false)
		{
			if (inPackage)
			{
				return string.Format("{0}/{1}/{2}", MobileStreamAssetPath, gameName, wholeName);
			}
			else
			{
				return string.Format("{0}/{1}/{2}", _webServerResourceRoot, gameName,wholeName);
			}
		}

		public string GetWebGameConfigUrl(string wholeName, string gameName,bool inPackage = false)
		{
			if (inPackage)
			{
				return string.Format("{0}/{1}/{2}", MobileStreamAssetPath,string.Format(GameConfigFolderNameFormat,gameName), wholeName);
			}
			else
			{
				return string.Format("{0}/{1}/{2}", _webServerResourceRoot,string.Format(GameConfigFolderNameFormat,gameName), wholeName);
			}
		}
		 
		public string GetAppResourceVersionLocalFilePath()
		{
			return string.Format("{0}/{1}", _rootPath, AppResourceConfig);
		}

		public string GetLocaleDataFileLocalFilePath()
		{
			return string.Format("{0}/{1}", _rootPath, LocaleFileFolderName);
		}

		public string GetLocaleDataFileName(ELanguage l)
		{
			return string.Format(AppLocaleDataFileName,l);
		}

		public string GetGameLocaleDataFileName(ELanguage l,string gameName)
		{
			return string.Format(GameLocaleDataFileName, gameName, l);
		}

		public string GetBuildInLocaleDataFilePath(string fileName)
		{
			return string.Format("{0}/{1}", LocaleFileFolderName, fileName);
		}

		public string GetGameResourceVersionFileName(string gameName)
		{
			return string.Format(GameResourceVersionFileNameFormat, gameName);
		}
		public string GetGameConfigVersionFileName(string gameName)
		{
			return string.Format(GameConfigVersionFileNameFormat, gameName);
		}

		public string GetLocalFileWholePath(string gameName,string fileName)
		{
			return string.Format("{0}/{1}/{2}", _rootPath, gameName, fileName);
		}

		public static string GetPlatformFolderName()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return BuildToolsConstDefine.Android;
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return BuildToolsConstDefine.Ios;
			}
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return BuildToolsConstDefine.StandaloneWindows;
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                return BuildToolsConstDefine.StandaloneOSX;
            }
			else
			{
				return BuildToolsConstDefine.StandaloneWindows;
			}
		}

		public static string GetFileWholeNameWithMd5(string fileName, string md5)
		{
			return string.Format("{0}.{1}", fileName, md5);
		}
	}

}