/********************************************************************

** Filename : GameResourceLoader
** Author : ake
** Date : 2016/4/18 18:15:20
** Summary : GameResourceLoader
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SevenZip;
using UnityEngine;

namespace SoyEngine
{

	public enum EGameResourceLoadingResult
	{
		None,
		Loading,
		Success,
		WaitToDecompress,
		DownloadFailed,
		DecompressFailed,
	}

	public class GameResourceLoader:MonoBehaviour
	{

		public struct LoadingItem
		{
			public WWW Content;
			public string Url;
			public EWebDownItemType ItemType;
			public EGameResourceLoadingResult Result;
			public string ResourceName;
			public bool IsCompressedFile;
			public string LoadFailedDetail;
			public string Md5;
			public bool IsBuildInRes;
			public byte[] BuildInResBytes;

			public string ContentText
			{
				get
				{
					if (IsBuildInRes && Application.platform == RuntimePlatform.IPhonePlayer)
					{
						if (BuildInResBytes == null)
						{
							return "";
						}
						return Encoding.ASCII.GetString(BuildInResBytes);
					}	
					return Content.text;
				}
			}

			public byte[] ContentBytes
			{
				get
				{
					if (IsBuildInRes && Application.platform == RuntimePlatform.IPhonePlayer)
					{
						return BuildInResBytes;
					}
					return Content.bytes;
				}
			}
		}

		public enum EWebDownItemType
		{
			Resource,
			Config,
		}

		private string _gameName;
		private Action _finish;
		private Action<float> _process;
		private Action _failed;

		private GameLocalFileVersionPack _curFileVersionPack;
		private LoadingItem _curLoadingItem;
		private bool _doLoadItemProcess = false;

		public const float UpdateResourceVersionRatio = 0.05f;
		public const float UpdateResourceRatio = 0.6f;
		public const float WriteResourceVersionRatio = 0.05f;

		public const float UpdateConfigVersionRatio = 0.05f;
		public const float UpdateConfigRatio = 0.1f;

		public const float WriteConfigVersionRatio = 0.05f;
		public const float UpdateLocaleRatio = 0.1f;

		private bool _loadfailed = false;
		private float _startLoadTime;


		public void SetLoadingProperties(string gameName, Action finish, Action<float> process, Action failed)
		{
			_startLoadTime = Time.realtimeSinceStartup;
			_gameName = gameName;
			_finish = finish;
			_process = process;
			_failed = failed;
			StartCoroutine("StartUpdateGame");
		}

		public void Clear()
		{
			_gameName = null;
			_finish = null;
			_process = null;
			_failed = null;

			ReleaseCurLoading();
			_curFileVersionPack = null;
		}

		void Update()
		{
			if (_doLoadItemProcess)
			{
				OnItemProcess(_curLoadingItem.Content.progress);
			}
		}


		#region

		private float _startTime;

		private float _depressTotalTime;

		private IEnumerator StartUpdateGame()
		{
			float curProcess = 0;
			DoProcess(curProcess);
			var data = LocalResourceManager.Instance.GetGameLocalFileVersionPack(_gameName);
			if (data == null || data.ResourceVersion== null || data.ConfigVersion == null)
			{
				DoFailedCallback();
				yield break;
			}
			_curFileVersionPack = data;
			yield return null;
			string resourceVersionMd5, configVersionMd5;
			if (!LocalResourceManager.Instance.TryGetLatestGameConfigVersionFileMd5(_gameName, out configVersionMd5) ||
			    !LocalResourceManager.Instance.TryGetLatestGameResourceVersionFileMd5(_gameName, out resourceVersionMd5))
			{
				DoFinishCallback();
				yield break;
			}
			_startTime = Time.realtimeSinceStartup;
			GameLocalFileVersionPack buildInPack = LocalResourceManager.Instance.GetBuildInPackageFileVersionPack(_gameName);

			//检查资源有变化 更新资源
			if (string.Compare(_curFileVersionPack.ResourceVersion.Md5, resourceVersionMd5, StringComparison.Ordinal) != 0)
			{
				string resourceVersionFileName = GameResourcePathManager.Instance.GetGameResourceVersionFileName(_gameName);
				SetLoadItem(resourceVersionFileName, resourceVersionMd5,false,EWebDownItemType.Resource);
				yield return LoadCurItemFromWeb();
				if (_curLoadingItem.Result != EGameResourceLoadingResult.Success)
				{
					LogHelper.Error("Update resourceVersion failed! fileName {0}  md5 {1}\n,detail is {2} _curLoadingItem.Result is {3}  whole url is {4}", 
						resourceVersionFileName, resourceVersionMd5,_curLoadingItem.LoadFailedDetail,
                        _curLoadingItem.Result,_curLoadingItem.Url);
					DoFailedCallback();
					yield break;
				}
				curProcess = UpdateResourceVersionRatio;
				DoProcess(curProcess);
				Dictionary<string, ResConfig> resourcesVersionOnWeb =
					ResourceVersionTools.ParseConfigDataFromString(_curLoadingItem.ContentText);

				List<ResourceCheckRes> changeList = ResourceVersionTools.CompareLocalResourceVersionFromWeb(_curFileVersionPack.ResourceVersion.Md5Dic,
					resourcesVersionOnWeb);
				ResourceVersionTools.UpdateChangeListLoadFrom(changeList, buildInPack, true);
				_curPartStartProcess = curProcess;
				
				int totalSize = GetTotalSize(changeList);
				for (int i = 0; i < changeList.Count; i++)
				{
					var item = changeList[i];
					_curItemProportion = item.Size * 1f / totalSize * UpdateResourceRatio;
					_doLoadItemProcess = true;
					switch (item.CheckResType)
					{
						case EResourceCheckResType.Change:
							{
								yield return LoadAndDecompressFile(item.ResourceName, item.NewMd5Value, item.LoadFromPackageRes);
								break;
							}
						case EResourceCheckResType.Delete:
							{
								DeleteLocalResource(item.ResourceName);
								continue;
							}
						default:
							{
								break;
							}
					}
					_doLoadItemProcess = false;

					_curPartStartProcess = _curPartStartProcess + _curItemProportion;
					DoProcess(_curPartStartProcess);
				}
				curProcess = UpdateResourceVersionRatio + UpdateResourceRatio;
				DoProcess(curProcess);

				//更新数据 并写入本地
				data.ResourceVersion.Md5 = resourceVersionMd5;
				data.ResourceVersion.Md5Dic = resourcesVersionOnWeb;
				ResourceVersionTools.WriteVersionData(data.ResourceVersion, GameResourcePathManager.Instance.GetLocalFileWholePath(_gameName, resourceVersionFileName), false);
			}
			curProcess = UpdateResourceVersionRatio + UpdateResourceRatio + WriteResourceVersionRatio;
			DoProcess(curProcess);

			yield return null;

			if (string.Compare(_curFileVersionPack.ConfigVersion.Md5, configVersionMd5, StringComparison.Ordinal) != 0)
			{
				string configVersionFileName = GameResourcePathManager.Instance.GetGameConfigVersionFileName(_gameName);
				SetLoadItem(configVersionFileName, configVersionMd5, false, EWebDownItemType.Config);
				yield return LoadCurItemFromWeb();
				if (_curLoadingItem.Result != EGameResourceLoadingResult.Success)
				{
					LogHelper.Error("Update configVersionFile failed! fileName {0}  md5 {1}\n,detail is ",
						configVersionFileName, configVersionMd5, _curLoadingItem.LoadFailedDetail);
					DoFailedCallback();
					yield break;
				}
				curProcess = UpdateResourceVersionRatio + UpdateResourceRatio + WriteResourceVersionRatio + UpdateConfigVersionRatio;
				DoProcess(curProcess);
				Dictionary<string, ResConfig> configVersionOnWeb =
					ResourceVersionTools.ParseConfigDataFromString(_curLoadingItem.ContentText);

				List<ResourceCheckRes> changeList = ResourceVersionTools.CompareLocalResourceVersionFromWeb(_curFileVersionPack.ConfigVersion.Md5Dic,
					configVersionOnWeb);
				ResourceVersionTools.UpdateChangeListLoadFrom(changeList, buildInPack,false);

				float tmpValue = curProcess;
                for (int i = 0; i < changeList.Count; i++)
				{
					var item = changeList[i];
					switch (item.CheckResType)
					{
						case EResourceCheckResType.Change:
							{
								yield return LoadAndWriteToLocal(item.ResourceName,item.NewMd5Value, item.LoadFromPackageRes);
								break;
							}
						case EResourceCheckResType.Delete:
							{
								DeleteLocalResource(item.ResourceName);
								break;
							}
						default:
							{
								break;
							}
					}
                    float itemValue = (i + 1) * UpdateConfigRatio / changeList.Count;

                    curProcess = tmpValue + itemValue;
                    DoProcess(curProcess);
				}
				curProcess = UpdateResourceVersionRatio + UpdateResourceRatio + WriteResourceVersionRatio + UpdateConfigVersionRatio +
				             UpdateConfigRatio;

				DoProcess(curProcess);

				//更新数据 并写入本地
				data.ConfigVersion.Md5 = configVersionMd5;
				data.ConfigVersion.Md5Dic = configVersionOnWeb;
				ResourceVersionTools.WriteVersionData(data.ConfigVersion, GameResourcePathManager.Instance.GetLocalFileWholePath(_gameName, configVersionFileName), false);
			}
			curProcess = UpdateResourceVersionRatio + UpdateResourceRatio + WriteResourceVersionRatio + UpdateConfigVersionRatio +
			 UpdateConfigRatio + WriteConfigVersionRatio;
			DoProcess(curProcess);
			{
				LocalePackLoader loader = new LocalePackLoader();
				float tmpValue = curProcess;
				var updateLocaleList = LocalResourceManager.Instance.GetNeedToUpdateLocaleDataList(_gameName);
				if (updateLocaleList.Count != 0)
				{
					for (int i = 0; i < updateLocaleList.Count; i++)
					{
						var fileName = updateLocaleList[i];
						yield return loader.LoadPack(fileName);

						float itemValue = (i + 1) * UpdateLocaleRatio / updateLocaleList.Count;

						curProcess = tmpValue + itemValue;
						DoProcess(curProcess);
					}
				}
			}
			LogHelper.Debug("Total load time = {0} ",(Time.realtimeSinceStartup - _startTime));
			LogHelper.Debug("Total depress time = {0} " ,_depressTotalTime);
			curProcess = 1;
			DoProcess(curProcess);
			DoFinishCallback();
		}



		private void DoFinishCallback()
		{
			LogHelper.Debug("DoFinishCallback called total time is {0}",Time.realtimeSinceStartup-_startLoadTime);
			if (_finish != null)
			{
				_finish();
			}
			OnFree();
		}

		private void DoFailedCallback()
		{
			if (_failed != null)
			{
				_failed();
			}
			OnFree();
		}


		private void DoProcess(float value)
		{
			if (_process != null)
			{
				float clampedValue = Mathf.Clamp(value, 0, 1);
				_process(clampedValue);
			} 
		}

		private void OnFree()
		{
			Clear();
			//LocalResourceManager.Instance.OnLoaderFree(this);
		}

		private void SetLoadItem(string fileName, string md5,bool isCompressedFile, EWebDownItemType itemType,bool isBuildRes =false)
		{
			ReleaseCurLoading();
			_curLoadingItem.IsCompressedFile = isCompressedFile;
			_curLoadingItem.ResourceName = fileName;
			_curLoadingItem.ItemType = itemType;
			_curLoadingItem.Md5 = md5;
			_curLoadingItem.IsBuildInRes = isBuildRes;
		}

		private void ReleaseCurLoading()
		{
			if (_curLoadingItem.Content != null)
			{
                _curLoadingItem.Content.Dispose();
			}
			_curLoadingItem.ResourceName = "";
			_curLoadingItem.IsCompressedFile = false;
			_curLoadingItem.Result = EGameResourceLoadingResult.None;
			_curLoadingItem.LoadFailedDetail = "";
			_curLoadingItem.IsBuildInRes = false;
			_curLoadingItem.BuildInResBytes = null;
			_curLoadingItem.Url = null;
		}

		private IEnumerator LoadCurItemFromWeb(bool showProcess = false)
		{
			string wholeName = GameResourcePathManager.GetFileWholeNameWithMd5(_curLoadingItem.ResourceName, _curLoadingItem.Md5);
	
			string wholeUrl;
			bool loadFailed = false;
			string errorDetail = "";

			if (_curLoadingItem.ItemType == EWebDownItemType.Resource)
			{
				wholeUrl = GameResourcePathManager.Instance.GetWebGameResUrl(wholeName, _gameName, _curLoadingItem.IsBuildInRes);
			}
			else
			{
				wholeUrl = GameResourcePathManager.Instance.GetWebGameConfigUrl(wholeName, _gameName, _curLoadingItem.IsBuildInRes);
			}
			_curLoadingItem.Url = wholeUrl;
			if (_curLoadingItem.IsBuildInRes && Application.platform != RuntimePlatform.Android)
			{
				//wholeUrl = string.Format("file://{0}", wholeUrl);
				_doLoadItemProcess = false;
				if (File.Exists(wholeUrl))
				{
					_curLoadingItem.BuildInResBytes = File.ReadAllBytes(wholeUrl);
				}
				else
				{
					loadFailed = true;
					errorDetail = string.Format("file {0} doesn't exist!", wholeUrl);
				}
			}
			else
			{
				_curLoadingItem.Content = new WWW(wholeUrl);
				if (showProcess)
				{
					yield return _curLoadingItem.Content;
					_doLoadItemProcess = false;
				}
				else
				{
					yield return _curLoadingItem.Content;
				}
				if (!_curLoadingItem.Content.isDone || !string.IsNullOrEmpty(_curLoadingItem.Content.error))
				{
					loadFailed = true;
					errorDetail = _curLoadingItem.Content.error;
				}
			}


			if (!loadFailed)
			{
				if (_curLoadingItem.IsCompressedFile)
				{
					_curLoadingItem.Result = EGameResourceLoadingResult.WaitToDecompress;
				}
				else
				{
					_curLoadingItem.Result = EGameResourceLoadingResult.Success;
				}
			}
			else
			{
				_curLoadingItem.Result = EGameResourceLoadingResult.DownloadFailed;
				_curLoadingItem.LoadFailedDetail = errorDetail;
			}
		}

		private IEnumerator LoadAndDecompressFile(string fileName,string newMd5,bool isBuildInRes)
		{
			SetLoadItem(fileName, newMd5,true,EWebDownItemType.Resource, isBuildInRes);
			yield return LoadCurItemFromWeb(true);
			if (_curLoadingItem.Result != EGameResourceLoadingResult.WaitToDecompress)
			{
				LogHelper.Error("loadfile {0} failed ,whole url is {1} error code is {2} load result is {3}",
					_curLoadingItem.ResourceName, _curLoadingItem.Url, _curLoadingItem.LoadFailedDetail, _curLoadingItem.Result);
				yield break;
			}
			string filePath = GameResourcePathManager.Instance.GetLocalFileWholePath(_gameName,_curLoadingItem.ResourceName);

			FileTools.MakeSureFileCanCreate(filePath, false);
			FileStream writeStream = new FileStream(filePath, FileMode.CreateNew);
			float startTime = Time.realtimeSinceStartup;
			SevenZipTool.DecompressFileLZMAFromStream(_curLoadingItem.ContentBytes, writeStream, null);
			_depressTotalTime += (Time.realtimeSinceStartup - startTime);
			_curLoadingItem.Result = EGameResourceLoadingResult.Success;
			//LogHelper.Info("Update resource {0} success {1}! url is {2}", fileName, filePath, _curLoadingItem.Content.url);
		}

		private IEnumerator LoadAndWriteToLocal(string fileName,string newMd5,bool isBuildInRes)
		{
			SetLoadItem(fileName, newMd5, false, EWebDownItemType.Config, isBuildInRes);
			yield return LoadCurItemFromWeb();
			if (_curLoadingItem.Result != EGameResourceLoadingResult.Success)
			{
				LogHelper.Error("loadfile {0} failed ,whole url is {1} error code is {2} load result is {3}",
					_curLoadingItem.ResourceName, _curLoadingItem.Url, _curLoadingItem.LoadFailedDetail, _curLoadingItem.Result);
				yield break;
			}
			string filePath = GameResourcePathManager.Instance.GetLocalFileWholePath(_gameName,_curLoadingItem.ResourceName);

			FileTools.MakeSureFileCanCreate(filePath, false);
			File.WriteAllBytes(filePath, _curLoadingItem.ContentBytes);
			//LogHelper.Info("Update Config {0} success!", fileName);
		}

		private void DeleteLocalResource(string fileName)
		{
			string wholePath = GameResourcePathManager.Instance.GetLocalFileWholePath(_gameName,fileName);
			if (File.Exists(wholePath))
			{
				File.Delete(wholePath);
			}
		}

		private int GetTotalSize(List<ResourceCheckRes> list)
		{
			int res = 0;
			if (list == null || list.Count == 0)
			{
				return 1;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if(list[i].CheckResType == EResourceCheckResType.Delete)
					continue;
				res += list[i].Size;
			}
			if (res == 0)
			{
				LogHelper.Error("Error ::Total size is 0 ! set to 1!");
				res = 1;
			}
			return res;
		}

		private float _curPartStartProcess;
		private float _curItemProportion;

		private void OnItemProcess(float value)
		{
			float curValue = _curPartStartProcess + _curItemProportion* value *0.9f;
			DoProcess(curValue);
		}

		#endregion
	}
}