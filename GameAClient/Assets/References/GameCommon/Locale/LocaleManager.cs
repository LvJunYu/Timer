/********************************************************************
** Filename : LocaleManager  
** Author : ake
** Date : 6/27/2016 2:23:12 PM
** Summary : LocaleManager  
***********************************************************************/


using System.Diagnostics;
using GameA;

namespace SoyEngine
{
	public class LocaleManager
	{
		private static LocaleManager _instance;

		public static LocaleManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LocaleManager();
				}
				return _instance;
			}
		}

		private LocalePack _curAppLocalePack;
		private LocalePack _curGameLocalePack;

		public string CurAppLocaleVersion
		{
			get
			{
				if (_curAppLocalePack != null)
				{
					return _curAppLocalePack.Version;
				}
				else
				{
					return "";
				}
			}
		}

		public void Init()
		{
			UpdateCurAppLocaleDic();
		}

		public void ReloadAppLocaleData()
		{
			UpdateCurAppLocaleDic();
		}

		public void EnterGame()
		{
			UpdateCurGameLocaleDic();
		}

		public void ExitGame()
		{
			ReleaseGameLocalePack();
		}

		public static string GameLocale(string key)
		{
			if (_instance == null)
			{
				return "";
			}
			if (_instance._curGameLocalePack == null)
			{
				return "";
			}
			string value;
			if (!_instance._curGameLocalePack.LocaleDic.TryGetValue(key, out value))
			{
				LogHelper.Error("locale key {0} is invalid!",key);
				return "";
			}
			return value;
		}

		public static string GameLocale(string key, params object[] array)
		{
			string value = GameLocale(key);
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return string.Format(key, array);
		}

		#region private

		private void UpdateCurAppLocaleDic()
		{
			LocalePack pack;
			if (!LocalResourceManager.Instance.TryGetWholeLocalePack(true,SocialApp.Instance.Language, out pack))
			{
				LogHelper.Error("TryGetWholeLocalePack failed! language is {0} ", SocialApp.Instance.Language);
				return;
			}
			_curAppLocalePack = pack;
		}

		private void UpdateCurGameLocaleDic()
		{
			string gameName = "GameMaker2D";
			LocalePack pack;
			if (!LocalResourceManager.Instance.TryGetWholeLocalePack(false, SocialApp.Instance.Language, out pack,gameName))
			{
				LogHelper.Error("TryGetWholeLocalePack failed! language is {0} gameName is {1} ", SocialApp.Instance.Language, gameName);
				return;
			}
			_curGameLocalePack = pack;
		}

		private void ReleaseGameLocalePack()
		{
			if (_curGameLocalePack == null)
			{
				return;
			}
			BaseBinaryPack outValue;
			string gameName = "GameMaker2D";
			string fileName = GameResourcePathManager.Instance.GetGameLocaleDataFileName(SocialApp.Instance.Language, gameName);
			if (!LocalResourceManager.Instance.TryGetLocalePack(fileName, out outValue))
			{
				return;
			}
			if (outValue is LocalePack)
			{
				LocalePack pack = outValue as LocalePack;
				if (_curGameLocalePack != outValue)
				{
					LogHelper.Error("Error,pack duplicated!");
				}
				BaseBinaryPack resPack = BaseBinaryPack.ReadHeaderFromFile(pack.Path);
				LocalResourceManager.Instance.UpdateLocalePackDic(gameName, resPack);
				_curGameLocalePack = null;
				outValue = null;
			}
		}

		#endregion
	}
}
