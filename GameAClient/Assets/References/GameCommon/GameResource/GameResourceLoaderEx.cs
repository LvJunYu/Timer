/********************************************************************
** Filename : GameResourceLoaderEx  
** Author : ake
** Date : 9/18/2016 10:36:15 AM
** Summary : GameResourceLoaderEx  
***********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using GameA.Game;
using SevenZip;
using UnityEngine;

namespace SoyEngine
{
	public class GameResourceLoaderEx: MonoBehaviour
	{
		public enum EWebDownItemType
		{
			Resource,
			Config,
			Locale,
		}

		public enum EUpdateGameState
		{
			None,
			LoadingResVersion,
			LoadingConfigVersion,
			LoadingRes,
			LoadingConfig,
			LoadingLocaleFile,
			LoadComplete,
			LoadFailed,
		}

		public const int MaxLoadProcessCount = 6;
		public const int MaxDepressProcessCount = 2;

		public const float LoadingResVersionProportion = 0.05f;
		public const float LoadingConfigVersionProportion = 0.05f;
		public const float LoadingResProportion = 0.8f;
		public const float LoadingConfigProportion = 0.09f;
		public const float LoadingLocaleFileProportion = 0.01f;

		public const float DownloadResProportion = 0.5f;
		public const float DepressResProportion = 0.5f;

		private string _gameName;
		private Action _finish;
		private Action<float> _process;
		private Action _failed;

		private List<LoadingItem> _itemLoadList;

		private List<ResourceCheckRes> _changeResList;
		private List<ResourceCheckRes> _changeConfigList;
		private int _hasLoadCount = 0;
		private Stack<LoadingItem> _prepareToDepressStack;
		private List<LoadingItem> _addToStackBuffer;
		private List<LoadingItem> _writeToLocalList; 
		private GameLocalFileVersionPack _curWebPack;
		private GameLocalFileVersionPack _curLocalPack;


		private List<Thread> _depressThreadList;
		private List<ResourceCheckRes> _updateLocalePackList;
		private Dictionary<EUpdateGameState, float> _time; 

		private float _startLoadingTime;
		private float _curProcess;

		private int _totalUpdateResSize;
		private int _hasDownloadSize;
		private int _hasDepressSize;
		private int _totalDepressCount;
		private int _hasDepressCount = 0;


		private EUpdateGameState _curState;


		private void ClearLoader()
		{
			_updateLocalePackList = null;

			ClearDepressThread();
			_curWebPack = null;
			_curLocalPack = null;

			_prepareToDepressStack = null;
			_addToStackBuffer = null;
			_writeToLocalList = null;
			_hasLoadCount = 0;
			_changeConfigList = null;
			_changeResList = null;

			_finish = null;
			_process = null;
			_failed = null;
			_curProcess = 0;
			_totalDepressCount = 0;
			_hasDepressCount = 0;
			GC.Collect();
		}

		public bool IsRunning
		{
			get { return _curState > EUpdateGameState.None && _curState<EUpdateGameState.LoadComplete; }
		}


		public void InitLoader()
		{
			_itemLoadList = new List<LoadingItem>();
		}


		public void SetLoadingProperties(string gameName, Action finish, Action<float> process, Action failed)
		{
			_startLoadingTime = Time.realtimeSinceStartup;
			_gameName = gameName;
			_finish = finish;
			_process = process;
			_failed = failed;
			_curState = EUpdateGameState.None;
			EnterUpdateGame();
		}


		private void Update()
		{
			if(!IsRunning)
			{
				return;
			}
			switch (_curState)
			{
				case EUpdateGameState.LoadingRes:
				{
					UpdateLoadingRes();
					break;
				}
				case EUpdateGameState.LoadingConfig:
				{
					UpdateLoadingConfig();
					break;
				}
				case EUpdateGameState.LoadingResVersion:
				{
					UpdateLoadingResVersion();
					break;
				}
				case EUpdateGameState.LoadingConfigVersion:
				{
					UpdateLoadingConfigVersion();
					break;
				}
				case EUpdateGameState.LoadingLocaleFile:
				{
					UpdateLoadingLoaleFile();
					break;
				}
			}
		}



		#region init


		private void EnterUpdateGame()
		{
			_curProcess = 0;
			_curLocalPack = LocalResourceManager.Instance.GetGameLocalFileVersionPack(_gameName);
			if (_curLocalPack == null || _curLocalPack.ResourceVersion == null || _curLocalPack.ConfigVersion == null)
			{
				OnUpdateFaild("Local file invalid!");
				return;
			}
			var curAppVersion = LocalResourceManager.Instance.CurAppVersion;
			if (curAppVersion == null)
			{
				OnUpdateFaild("LocalResourceManager.Instance.CurAppVersion is null!");
				return;
			}
			_time = new Dictionary<EUpdateGameState, float>();
			var newGamePack = curAppVersion.GetFileVersionPack(_gameName);
			if (newGamePack == null || newGamePack.ResourceVersion == null || newGamePack.ConfigVersion == null)
			{
				EnterLoadingResVersionState();
				return;
			}
			_curWebPack = newGamePack;
			EnterLoadingResState();
		}



		#endregion



		#region  state

		private void EnterLoadingResState()
		{
			_curProcess = LoadingResVersionProportion + LoadingConfigVersionProportion;
			DoUpdateProcess(_curProcess);
			_hasDownloadSize = 0;
			_hasDepressSize = 0;
			_time[EUpdateGameState.LoadingRes] = Time.realtimeSinceStartup;
			if (!InitChangeList())
			{
				return;
			}
			_curState = EUpdateGameState.LoadingRes;
			_hasLoadCount = 0;
			_prepareToDepressStack = new Stack<LoadingItem>();
			_addToStackBuffer = new List<LoadingItem>();

			_isDepressing = true;
			_depressThreadList = new List<Thread>();

			for (int i = 0; i < MaxDepressProcessCount; i++)
			{
				Thread t = new Thread(new ParameterizedThreadStart(Depress));
				t.Start(i+2);
			}
		}

		private void EnterLoadingConfigState()
		{
			_time[EUpdateGameState.LoadingConfig] = Time.realtimeSinceStartup;
			_curState = EUpdateGameState.LoadingConfig;
			_hasLoadCount = 0;
			_writeToLocalList = new List<LoadingItem>();
		}



		private bool _isDepressing = false;
		private void Depress(object par)
		{
			int index = (int) par;
			while (_isDepressing)
			{	
				{
                    if(_prepareToDepressStack.Count == 0)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
					LoadingItem cur = null;
					lock (_prepareToDepressStack)
					{
						if (_prepareToDepressStack.Count > 0)
						{
							cur = _prepareToDepressStack.Pop();
						}	
					}
					if (cur != null)
					{
						string filePath = GameResourcePathManager.Instance.GetLocalFileWholePath(_gameName, cur.ResourceName);

						FileTools.MakeSureFileCanCreate(filePath, false);
						FileStream writeStream = new FileStream(filePath, FileMode.CreateNew);
//						SevenZipTool.DecompressFileLZMAFromStream(cur.ContentBytes, writeStream, null);
						_hasDepressSize += cur.HasDownloadSize;
                        Interlocked.Increment(ref _hasDepressCount);
					}
				}
			}
		}

		private void EnterLoadingResVersionState()
		{
			_curProcess = 0;
			DoUpdateProcess(_curProcess);
			_time[EUpdateGameState.LoadingResVersion] = Time.realtimeSinceStartup;
			_curWebPack = new GameLocalFileVersionPack();
			string resourceVersionMd5;
			if (!LocalResourceManager.Instance.TryGetLatestGameResourceVersionFileMd5(_gameName, out resourceVersionMd5))
			{
				OnUpdateFaild("LocalResourceManager.Instance.TryGetLatestGameResourceVersionFileMd5 failed! gameName is "+_gameName);
				return;
			}
			_curState = EUpdateGameState.LoadingResVersion;
			string resourceVersionFileName = GameResourcePathManager.Instance.GetGameResourceVersionFileName(_gameName);
			LoadingItem loadItem = LoadingItem.CreateLoadItem(_gameName, resourceVersionFileName, resourceVersionMd5,EWebDownItemType.Resource,1);
			_itemLoadList.Add(loadItem);
		}

		private void EnterLoadingConfigVersionState()
		{
			_time[EUpdateGameState.LoadingConfigVersion] = Time.realtimeSinceStartup;
			string configVersionMd5;
			if (!LocalResourceManager.Instance.TryGetLatestGameConfigVersionFileMd5(_gameName, out configVersionMd5))
			{
				OnUpdateFaild("LocalResourceManager.Instance.TryGetLatestGameConfigVersionFileMd5 failed! gameName is " + _gameName);
				return;
			}
			_curState = EUpdateGameState.LoadingConfigVersion;
			string fileName = GameResourcePathManager.Instance.GetGameConfigVersionFileName(_gameName);
			LoadingItem loadItem = LoadingItem.CreateLoadItem(_gameName, fileName, configVersionMd5, EWebDownItemType.Config,1);
			_itemLoadList.Add(loadItem);
		}

		private void EnterLoadingLoaleFileState()
		{
			_time[EUpdateGameState.LoadingLocaleFile] = Time.realtimeSinceStartup;
			_curState = EUpdateGameState.LoadingLocaleFile;
			_updateLocalePackList = LocalResourceManager.Instance.GetLocaleChangeList(_gameName);
			_hasLoadCount = 0;
		}
		private void UpdateLoadingResVersion()
		{
			if (_itemLoadList.Count !=1)
			{
				OnUpdateFaild("UpdateLoadingResVersion called but _itemLoadList.Count !=1");
				return;
			}
			var item = _itemLoadList[0];
			item.Update();
			if (item.HasLoadComplete)
			{
				Dictionary<string, ResConfig> data = ResourceVersionTools.ParseConfigDataFromString(item.ContentText);
				var version = new GameVersionData();
				version.SetKey(item.Md5);
				version.Md5Dic = data;
				_curWebPack.ResourceVersion = version;
				_itemLoadList.Clear();
				UpdateDeltaTime(EUpdateGameState.LoadingResVersion);
				_curProcess = LoadingResVersionProportion;
				DoUpdateProcess(_curProcess);
				EnterLoadingConfigVersionState();
				return;
			}
			else if (item.HasLoadFailed)
			{
				OnUpdateFaild(item.LoadFailedDetail);
				return;
			}
		}

		private void UpdateLoadingConfigVersion()
		{
			if (_itemLoadList.Count != 1)
			{
				OnUpdateFaild("UpdateLoadingConfigVersion called but _itemLoadList.Count !=1");
				return;
			}
			var item = _itemLoadList[0];
			item.Update();
			if (item.HasLoadComplete)
			{
				Dictionary<string, ResConfig> data = ResourceVersionTools.ParseConfigDataFromString(item.ContentText);
				var version = new GameVersionData();
				version.SetKey(item.Md5);
				version.Md5Dic = data;
				_curWebPack.ConfigVersion = version;
				_itemLoadList.Clear();
				_curProcess = LoadingResVersionProportion + LoadingConfigVersionProportion;
				DoUpdateProcess(_curProcess);
				UpdateDeltaTime(EUpdateGameState.LoadingConfigVersion);
				EnterLoadingResState();
				return;
			}
			else if (item.HasLoadFailed)
			{
				OnUpdateFaild(item.LoadFailedDetail);
				return;
			}
		}

		private void UpdateLoadingRes()
		{
			if (_changeResList.Count == _hasLoadCount && _prepareToDepressStack.Count ==0 && _addToStackBuffer.Count ==0 && _itemLoadList.Count == 0 && _totalDepressCount == _hasDepressCount)
			{
				ExitLoadingResState();
				UpdateDeltaTime(EUpdateGameState.LoadingRes);
				EnterLoadingConfigState();
			}
			else
			{
				if (_addToStackBuffer.Count > 0)
				{
					lock (_prepareToDepressStack)
					{
						for (int j = 0; j < _addToStackBuffer.Count; j++)
						{
							_prepareToDepressStack.Push(_addToStackBuffer[j]);
						}
						_addToStackBuffer.Clear();
					}
				}

				int otherDownloadSie = 0;
				for (int i = _itemLoadList.Count -1; i >=0; i--)
				{
					var item = _itemLoadList[i];
					item.Update();
					if (item.HasLoadComplete)
					{
						_hasDownloadSize += item.HasDownloadSize;
						_itemLoadList.RemoveAt(i);
						_addToStackBuffer.Add(item);
						continue;
					}
					else if (item.HasLoadFailed)
					{
						OnUpdateFaild(item.LoadFailedDetail);
						return;
					}
					otherDownloadSie += item.HasDownloadSize;
				}
				float tmpProcess = _curProcess + (DownloadResProportion*(_hasDownloadSize + otherDownloadSie)/_totalUpdateResSize +
				                   DepressResProportion*_hasDepressSize/_totalUpdateResSize) * LoadingResProportion;
				DoUpdateProcess(tmpProcess);
				for (int i = _hasLoadCount; i < _changeResList.Count; i++)
				{
					if (_itemLoadList.Count >= MaxLoadProcessCount)
					{
						break;
					}
					var item = _changeResList[i];
					if (item.CheckResType == EResourceCheckResType.Delete)
					{
						DeleteLocalResource(item.ResourceName);
					}
					else if (item.CheckResType == EResourceCheckResType.Change)
					{
						LoadingItem loadItem = LoadingItem.CreateLoadItem(_gameName, item.ResourceName, item.NewMd5Value,
							EWebDownItemType.Resource, item.Size,item.LoadFromPackageRes);
						_itemLoadList.Add(loadItem);
					}
					_hasLoadCount = i+1;
				}
			}
		}


		private void UpdateLoadingConfig()
		{
			if (_changeConfigList.Count == _hasLoadCount && _itemLoadList.Count == 0)
			{
				ExitLoadingConfigState();
				UpdateDeltaTime(EUpdateGameState.LoadingConfig);
				EnterLoadingLoaleFileState();
			}
			else
			{
				for (int i = _itemLoadList.Count - 1; i >= 0; i--)
				{
					var item = _itemLoadList[i];
					item.Update();
					if (item.HasLoadComplete)
					{
						_itemLoadList.RemoveAt(i);
						_writeToLocalList.Add(item);
					}
					else if (item.HasLoadFailed)
					{
						OnUpdateFaild(item.LoadFailedDetail);
						return;
					}
				}
				for (int i = _hasLoadCount; i < _changeConfigList.Count; i++)
				{
					if (_itemLoadList.Count >= MaxLoadProcessCount)
					{
						break;
					}
					var item = _changeConfigList[i];
					if (item.CheckResType == EResourceCheckResType.Delete)
					{
						DeleteLocalResource(item.ResourceName);
					}
					else if (item.CheckResType == EResourceCheckResType.Change)
					{
						LoadingItem loadItem = LoadingItem.CreateLoadItem(_gameName, item.ResourceName, item.NewMd5Value,EWebDownItemType.Config,item.Size, item.LoadFromPackageRes);
						_itemLoadList.Add(loadItem);
					}
					_hasLoadCount = i + 1;
				}
			}
		}

		private void ExitLoadingResState()
		{
			ClearDepressThread();
			_itemLoadList.Clear();
			
			GC.Collect();
			_curLocalPack.ResourceVersion = new GameVersionData(_curWebPack.ResourceVersion);
			ResourceVersionTools.WriteVersionData(_curLocalPack.ResourceVersion, GameResourcePathManager.Instance.GetLocalFileWholePath(_gameName, GameResourcePathManager.Instance.GetGameResourceVersionFileName(_gameName)), false);

			_curProcess = LoadingResVersionProportion + LoadingConfigVersionProportion + LoadingResProportion;
			DoUpdateProcess(_curProcess);
		}

		private void ExitLoadingConfigState()
		{
			for (int i = 0; i < _writeToLocalList.Count; i++)
			{
				var item = _writeToLocalList[i];
				string filePath = GameResourcePathManager.Instance.GetLocalFileWholePath(_gameName, item.ResourceName);

				FileTools.MakeSureFileCanCreate(filePath, false);
				File.WriteAllBytes(filePath, item.ContentBytes);
			}
			_itemLoadList.Clear();
			GC.Collect();
			_curLocalPack.ConfigVersion = new GameVersionData(_curWebPack.ConfigVersion);
			ResourceVersionTools.WriteVersionData(_curLocalPack.ConfigVersion, GameResourcePathManager.Instance.GetLocalFileWholePath(_gameName, GameResourcePathManager.Instance.GetGameConfigVersionFileName(_gameName)), false);

			_curProcess = LoadingResVersionProportion + LoadingConfigVersionProportion + LoadingResProportion + LoadingConfigProportion;
			DoUpdateProcess(_curProcess);
		}

		private void UpdateLoadingLoaleFile()
		{
			if (_updateLocalePackList.Count == _hasLoadCount && _itemLoadList.Count == 0)
			{
				UpdateDeltaTime(EUpdateGameState.LoadingLocaleFile);
				OnLoadComplete();
			}
			else
			{
				for (int i = _itemLoadList.Count - 1; i >= 0; i--)
				{
					var item = _itemLoadList[i];
					item.Update();
					if (item.HasLoadComplete)
					{
						_itemLoadList.RemoveAt(i);
						OnLoadLocaleFileFinish(item);
					}
					else if (item.HasLoadFailed)
					{
						OnUpdateFaild(item.LoadFailedDetail);
						return;
					}
				}
				for (int i = _hasLoadCount; i < _updateLocalePackList.Count; i++)
				{
					if (_itemLoadList.Count >= MaxLoadProcessCount)
					{
						break;
					}
					var item = _updateLocalePackList[i];

					LoadingItem loadItem = LoadingItem.CreateLoadItem(_gameName, item.ResourceName, item.NewMd5Value, EWebDownItemType.Locale,1, item.LoadFromPackageRes);
						_itemLoadList.Add(loadItem);
					_hasLoadCount = i + 1;
				}
			}
		}


		private void OnLoadLocaleFileFinish(LoadingItem item)
		{
			string localFilePath = Path.Combine(GameResourcePathManager.Instance.GetLocaleDataFileLocalFilePath(), item.ResourceName);
			FileTools.CreateDirectorys(localFilePath);
			if (File.Exists(localFilePath))
			{
				File.Delete(localFilePath);
			}
			File.WriteAllBytes(localFilePath, item.ContentBytes);
			UpdateLocalVersion(item.ResourceName, localFilePath);
		}

		private void DeleteLocalResource(string fileName)
		{
			string wholePath = GameResourcePathManager.Instance.GetLocalFileWholePath(_gameName, fileName);
			if (File.Exists(wholePath))
			{
				File.Delete(wholePath);
			}
		}

		private bool InitChangeList()
		{
			if (_curWebPack == null || _curWebPack.ResourceVersion == null || _curWebPack.ConfigVersion == null)
			{
				OnUpdateFaild(string.Format("_curWebPack is invalid! "));
				return false;
			}
			GameLocalFileVersionPack buildInPack = LocalResourceManager.Instance.GetBuildInPackageFileVersionPack(_gameName);
			if (String.CompareOrdinal(_curWebPack.ResourceVersion.Md5, _curLocalPack.ResourceVersion.Md5) == 0)
			{
				_changeResList = new List<ResourceCheckRes>();
			}
			else
			{
				_changeResList = ResourceVersionTools.CompareLocalResourceVersionFromWeb(_curLocalPack.ResourceVersion.Md5Dic,
					_curWebPack.ResourceVersion.Md5Dic);
				ResourceVersionTools.UpdateChangeListLoadFrom(_changeResList, buildInPack, true);
			}
			for (int i = 0; i < _changeResList.Count; i++)
			{
				var item = _changeResList[i];
				if (item.CheckResType == EResourceCheckResType.Change)
				{
					_totalUpdateResSize += item.Size;
					_totalDepressCount ++;
				}
			}
			if (_totalUpdateResSize <= 0)
			{
				_totalUpdateResSize = 1;
			}
			if (String.CompareOrdinal(_curWebPack.ConfigVersion.Md5, _curLocalPack.ConfigVersion.Md5) == 0)
			{
				_changeConfigList = new List<ResourceCheckRes>();
			}
			else
			{
				_changeConfigList = ResourceVersionTools.CompareLocalResourceVersionFromWeb(_curLocalPack.ConfigVersion.Md5Dic,
					_curWebPack.ConfigVersion.Md5Dic);
				ResourceVersionTools.UpdateChangeListLoadFrom(_changeConfigList, buildInPack, false);
			}
			return true;
		}

		private void UpdateLocalVersion(string fileName, string path)
		{
			BaseBinaryPack outValue;

			if (!LocalResourceManager.Instance.TryGetLocalePack(fileName, out outValue))
			{
				outValue = new BaseBinaryPack();
			}
			outValue.ReloadFromFile(path);
			LocalResourceManager.Instance.UpdateLocalePackDic(fileName, outValue);
		}

		private void OnLoadComplete()
		{			
			_itemLoadList.Clear();
			_curState = EUpdateGameState.LoadComplete;
			DoUpdateProcess(1);
			//LogHelper.Debug("Total load time {0}.",Time.realtimeSinceStartup - _startLoadingTime);
			PrintDuringTime();
			var callback = _finish;
			ClearLoader();
			if (callback != null)
			{
				callback();
			}
			LocalResourceManager.Instance.OnLoaderFree(this);
		}

		private void OnUpdateFaild(string msg)
		{
			_curState = EUpdateGameState.LoadFailed;
			LogHelper.Error("UpdateGameRes failed ,detail is {0}", msg);
			
			var callback = _failed;
			ClearLoader();
			if (callback != null)
			{
				callback();
			}
		}

		private float _lastUpdateValue;
		private void DoUpdateProcess(float value)
		{
			if (_process != null)
			{
				_process(value);
			}
			_lastUpdateValue = value;
		}

		private void ClearDepressThread()
		{
			if (_depressThreadList != null)
			{
				_isDepressing = false;
				for (int i = 0; i < _depressThreadList.Count; i++)
				{
					var item = _depressThreadList[i];
					item.Abort();
				}
				_depressThreadList.Clear();
				_depressThreadList = null;
			}
		}

		private void UpdateDeltaTime(EUpdateGameState state)
		{
			float outValue;
			if (!_time.TryGetValue(state,out outValue))
			{
				return;
			}
			_time[state] = Time.realtimeSinceStartup - outValue;
		}

		private void PrintDuringTime()
		{
			StringBuilder timeShow = new StringBuilder();
			var enumerator = _time.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var item = enumerator.Current;
				timeShow.AppendLine(item.Key + "_" + item.Value);
			}
			LogHelper.Debug("Total load time {0}\n.total time {1}", Time.realtimeSinceStartup - _startLoadingTime, timeShow);
		}

		private void OnDestroy()
		{
			ClearDepressThread();
		}


		#endregion

	}
}

