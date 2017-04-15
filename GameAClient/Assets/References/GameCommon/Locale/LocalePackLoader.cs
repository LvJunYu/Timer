/********************************************************************
** Filename : LocalePackLoader  
** Author : ake
** Date : 6/28/2016 4:02:01 PM
** Summary : LocalePackLoader  
***********************************************************************/


using System.Collections;
using System.IO;
using UnityEngine;

namespace SoyEngine
{
	public class LocalePackLoader
	{

		public IEnumerator LoadPack(string fileName)
		{
			string md5;
			if (!LocalResourceManager.Instance.CurAppVersion.TryGetMd5Value(fileName, out md5))
			{
				LogHelper.Error("LocalResourceManager.Instance.CurAppVersion.TryGetMd5Value failed! fileName is {0}",fileName);
				yield break;
			}
			string url = GameResourcePathManager.Instance.GetLocalePackUrl(string.Format("{0}.{1}", fileName, md5));
			if (string.IsNullOrEmpty(url))
			{
				LogHelper.Error("LoadPack called but url is invalid! {0} ",url);
				yield break; 
			}
			string localFilePath = Path.Combine(GameResourcePathManager.Instance.GetLocaleDataFileLocalFilePath(), fileName);
			WWW web = new WWW(url);

			yield return web;
			if (!web.isDone || !string.IsNullOrEmpty(web.error))
			{
				LogHelper.Error(web.error);
				yield break;
			}
			FileTools.CreateDirectorys(localFilePath);
			if (File.Exists(localFilePath))
			{
				File.Delete(localFilePath);
			}
			File.WriteAllBytes(localFilePath,web.bytes);
			UpdateLocalVersion(fileName, localFilePath);
			yield return null;
		}

		private void UpdateLocalVersion(string fileName,string path)
		{
			BaseBinaryPack outValue;

			if (!LocalResourceManager.Instance.TryGetLocalePack(fileName, out outValue))
			{  
				outValue = new BaseBinaryPack();
			}
			outValue.ReloadFromFile(path);
			LocalResourceManager.Instance.UpdateLocalePackDic(fileName, outValue);
		}
	}
}
