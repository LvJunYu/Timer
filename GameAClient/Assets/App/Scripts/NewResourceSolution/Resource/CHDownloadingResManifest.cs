using System;
using System.Collections;
using System.Collections.Generic;
using GameA;
using SoyEngine;
using UnityEngine;
using EMessengerType = GameA.EMessengerType;
namespace NewResourceSolution
{
    public class CHDownloadingResManifest : CHResManifest
    {
        /// <summary>
        /// 同时并发的www请求数
        /// </summary>
        private static int s_maxConcurrentDownloadNum = 2;

        /// <summary>
        /// 对单个文件的最大重试次数，任意文件的下载重试次数大于此数则整体下载失败
        /// </summary>
        private static int s_maxRetryCntForSingleFile = 4;

        private long _needsDownloadTotalByte;
        private int _needsDownloadTotalCnt;

        private long _downloadDoneByte;

        /// <summary>
        /// 总共花费的时间
        /// </summary>
        private long _totalTime;
        /// <summary>
        /// 除去重试的时间，所有成功下载的过程所花费的时间总和
        /// </summary>
        private long _downloadTotalTime;

        /// <summary>
        /// 下载过程中出错
        /// </summary>
        private bool _downloadFailed;
        /// <summary>
        /// 序列化过程中出错
        /// </summary>
        private bool _serializeFailed;

        private Exception _ioException;

        private readonly Queue<BundleDownloader> _waitQueue = new Queue<BundleDownloader>();
        private readonly List<BundleDownloader> _downloadingList = new List<BundleDownloader>();
        private readonly Queue<BundleDownloader> _waitDecompressQueue = new Queue<BundleDownloader>();
        private readonly List<CHResBundle> _needDeleteBundles = new List<CHResBundle>();

        [Newtonsoft.Json.JsonIgnore]
        public long NeedsDownloadTotalByte
        {
            get { return _needsDownloadTotalByte; }
        }

        [Newtonsoft.Json.JsonIgnore]
        public long NeedsDownloadTotalCnt
        {
            get { return _needsDownloadTotalCnt; }
        }

        [Newtonsoft.Json.JsonIgnore]
        public bool DownloadFailed
        {
            get { return _downloadFailed; }
        }
        [Newtonsoft.Json.JsonIgnore]
        public bool SerializeFailed
        {
            get { return _serializeFailed; }
        }
        [Newtonsoft.Json.JsonIgnore]
        public long TotalTime
        {
            get { return _totalTime; }
        }
        [Newtonsoft.Json.JsonIgnore]
        public long DownloadTotalTime
        {
            get { return _downloadTotalTime; }
        }
        public CHDownloadingResManifest() {}
        public CHDownloadingResManifest (Version version) : base(version) {}
        public CHDownloadingResManifest (CHRuntimeResManifest runtimeManifest) : base(runtimeManifest.Ver)
        {
            _bundles = runtimeManifest.Bundles;
            _fileLocation = runtimeManifest.FileLocation;
            _adamBundleNameList = runtimeManifest.AdamBundleNameList;
        }
        /// <summary>
        /// 混合下载manifest和(包内/本地/临时)manifest (现在仅为混合包内和persistent)
        /// </summary>
        /// <param name="existingManifest">Existing manifest.</param>
        public IEnumerator MergeExistingManifest (CHRuntimeResManifest existingManifest)
        {
            for (int i = 0; i < _bundles.Count; i++)
            {
                var bundle = _bundles[i];
				CHResBundle bundleInExistingManifest = existingManifest.GetBundleByBundleName(bundle.AssetBundleName);
                if (null != bundleInExistingManifest && EFileLocation.Server != bundleInExistingManifest.FileLocation)
                {
                    if (String.CompareOrdinal(_bundles[i].CompressedMd5, bundleInExistingManifest.CompressedMd5) == 0)
                    {
                        EFileIntegrity integrity = bundleInExistingManifest.CheckFileIntegrity (bundleInExistingManifest.FileLocation);
                        if (EFileIntegrity.Integral == integrity)
                        {
                            _bundles[i].FileLocation = bundleInExistingManifest.FileLocation;
							LogHelper.Info("Bundle <{0}> found in existing manifest, fileLocation is {1}.", _bundles[i].AssetBundleName, _bundles[i].FileLocation);
                        }
                        else
                        {
                            _bundles[i].FileLocation = EFileLocation.Server;
                            if (EFileIntegrity.Md5Dismatch == integrity)
                            {
                                yield return null;
                            }
                        }
                    }
                    else
                    {
                        _bundles[i].FileLocation = EFileLocation.Server;
                    }
                }
            }
        }
        
        /// <summary>
        /// 现在仅为混合包内和persistent
        /// </summary>
        /// <param name="existingManifest">Existing manifest.</param>
        public IEnumerator MergePersistentToStreamingManifest (CHRuntimeResManifest existingManifest)
        {
            var lastTime = Time.realtimeSinceStartup;
            var persistentManifestDict = new Dictionary<string, CHResBundle>(existingManifest.BundleName2BundleDict);
            for (int i = 0; i < _bundles.Count; i++)
            {
                if (Time.realtimeSinceStartup - lastTime > 0.2f)
                {
                    yield return null;
                    lastTime = Time.realtimeSinceStartup;
                }
                var bundle = _bundles[i];
				CHResBundle bundleInExistingManifest = existingManifest.GetBundleByBundleName(bundle.AssetBundleName);
                if (null == bundleInExistingManifest)
                {
                    continue;
                }
                if (EFileLocation.Persistent != bundleInExistingManifest.FileLocation)
                {//不在磁盘的文件不处理
                    continue;
                }
                if (bundle.NeedWriteToPersistent())
                {//不需要写到本地 就跳过 等待删除
                    continue;
                }
                //需要写到本地 比较MD5
                if (String.CompareOrdinal(_bundles[i].RawMd5, bundleInExistingManifest.RawMd5) == 0)
                {
                    EFileIntegrity integrity = bundleInExistingManifest.CheckFileIntegrity (bundleInExistingManifest.FileLocation);
                    if (EFileIntegrity.Integral == integrity)
                    {
                        _bundles[i].FileLocation = EFileLocation.Persistent;
                        LogHelper.Debug("Bundle <{0}> found in existing manifest, fileLocation is {1}.", _bundles[i].AssetBundleName, _bundles[i].FileLocation);
                    }
                }
                persistentManifestDict.Remove(bundleInExistingManifest.AssetBundleName);
            }
            _needDeleteBundles.Clear();
            using (var itor = persistentManifestDict.GetEnumerator())
            {//删除已经不使用的本地资源
                while (itor.MoveNext())
                {
                    if (itor.Current.Value.FileLocation == EFileLocation.Persistent)
                    {
                        _needDeleteBundles.Add(itor.Current.Value);
                    }
                }
            }
        }

		public IEnumerator MergeTemporaryCacheFiles ()
        {
            // check if the temporary folder exist
			System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(StringUtil.Format(StringFormat.TwoLevelPath, ResPath.PersistentDataPath, ResPath.TempCache));
            if (!di.Exists)
            {
                yield break;
            }
            for (int i = 0; i < _bundles.Count; i++)
            {
                if (EFileLocation.Server != _bundles [i].FileLocation)
                {
                    // 之前已经和其他manifest合并并确认有此文件了
                    continue;
                }
                EFileIntegrity integrity = _bundles [i].CheckFileIntegrity (EFileLocation.TemporaryCache);
                if (EFileIntegrity.Integral == integrity)
                {
                    _bundles [i].FileLocation = EFileLocation.TemporaryCache;
                    LogHelper.Info ("Find temporary file of {0}", _bundles [i].AssetBundleName);
                }
                else if (EFileIntegrity.Md5Dismatch == integrity)
                {
                    yield return null;
                }
            }
        }


        /// <summary>
        /// 检查包内和临时文件的完整性，文件损坏则重新下载
        /// </summary>
        public IEnumerator CheckStreamingAndTempFileIntegrity ()
        {
            for (int i = 0; i < _bundles.Count; i++)
            {
                if (EFileLocation.Server != _bundles[i].FileLocation)
                {
                    EFileIntegrity integrity = _bundles [i].CheckFileIntegrity (EFileLocation.StreamingAsset);
                    if (EFileIntegrity.Integral == integrity)
                    {
                        _bundles [i].FileLocation = EFileLocation.Server;
                    }
                    else if (EFileIntegrity.Md5Dismatch == integrity)
                    {
                        yield return null;
                    }
                }
            }
        }
        /// <summary>
        /// 计算需要下载文件的数量和大小
        /// </summary>
        public void CalculateFilesNeedsDownload ()
        {
            _needsDownloadTotalCnt = 0;
            _needsDownloadTotalByte = 0;
            for (int i = 0; i < _bundles.Count; i++)
            {
                // 只有位置在服务器上，并且分组是‘随包’或‘必须’的资源才需要下载
                if (EFileLocation.Server == _bundles[i].FileLocation &&
                    (ResDefine.ResGroupInPackage == _bundles[i].GroupId ||
                     ResDefine.ResGroupNecessary == _bundles[i].GroupId))
                {
                    _needsDownloadTotalCnt++;
                    _needsDownloadTotalByte += _bundles[i].Size;
                }
            }
        }

        /// <summary>
        /// 下载并且解压服务器上的文件
        /// </summary>
        /// <returns>The server asset bundles.</returns>
        public IEnumerator DownloadServerAssetBundles ()
        {
            long beginTime = DateTimeUtil.GetNowTicks() / 10000;
            _downloadFailed = false;
            _serializeFailed = false;
            _downloadTotalTime = 0;
			_downloadDoneByte = 0;
            FileTools.CheckAndCreateFolder(StringUtil.Format(StringFormat.TwoLevelPath, ResPath.PersistentDataPath, ResPath.TempCache));

            _waitQueue.Clear ();
            _downloadingList.Clear ();
            _waitDecompressQueue.Clear ();
            for (int i = 0; i < _bundles.Count; i++)
            {
                if (EFileLocation.Server == _bundles [i].FileLocation)
                {
                    _waitQueue.Enqueue (new BundleDownloader(_bundles [i], _version));
                }
            }
            BundleDownloader currentSerializeDownloader = null;
            while (true)
            {
                if (_waitQueue.Count == 0 &&
                    _downloadingList.Count == 0 &&
                    _waitDecompressQueue.Count == 0) {
                    break;
                }

                while (_downloadingList.Count < s_maxConcurrentDownloadNum && _waitQueue.Count > 0)
                {
                    var downloader = _waitQueue.Dequeue ();
                    downloader.BeginDownload ();
                    _downloadingList.Add (downloader);
                    LogHelper.Info ("Begin download bundle: {0}", downloader.WebRequest.url);
                }

                for (int i = _downloadingList.Count - 1; i >= 0; i--)
                {
                    if (_downloadingList [i].IsDownloadDone)
                    {
                        if (!string.IsNullOrEmpty (_downloadingList [i].Error))
                        {
                            LogHelper.Error ("Error when download assetBundle: {0}", _downloadingList[i].Error);
                            if (_downloadingList [i].TotalErrorCnt >= s_maxRetryCntForSingleFile)
                            {
                                _downloadFailed = true;
                                _downloadingList.RemoveAt (i);
//                                yield break;
                            }
                            else
                            {
                                _downloadingList [i].ReWaitDownload ();
                                _waitQueue.Enqueue (_downloadingList [i]);
                                _downloadingList.RemoveAt (i);
                            }
                        }
                        else
                        {
                            _downloadTotalTime += _downloadingList [i].DownloadTime;
                            _waitDecompressQueue.Enqueue (_downloadingList [i]);
                            _downloadingList.RemoveAt (i);
                        }
                    }
                }

                if (null == currentSerializeDownloader || currentSerializeDownloader.IsDecompressDone)
                {
                    if (null != currentSerializeDownloader)
                    {
                        // 序列化过程中出错了，有可能是磁盘空间不足，有可能是压缩文件本身错误，不管哪种情况都无法重试继续
                        if (!string.IsNullOrEmpty (currentSerializeDownloader.Error))
                        {
                            LogHelper.Error ("Error when serialize assetBundle: {0}", currentSerializeDownloader.Error);
                            _serializeFailed = true;
//                            yield break;
                        }
                        else
                        {
                            _downloadDoneByte += currentSerializeDownloader.Bundle.Size;
                            Messenger<long, long>.Broadcast(EMessengerType.OnResourcesUpdateProgressUpdate, _downloadDoneByte, _needsDownloadTotalByte);
                            LogHelper.Info ("Serialize {0} done.", currentSerializeDownloader.Bundle.AssetBundleName);
                        }
                    }
                    if (_waitDecompressQueue.Count > 0)
                    {
                        currentSerializeDownloader = _waitDecompressQueue.Dequeue ();
                        currentSerializeDownloader.BeginDecompressAndSave ();
                    }
                }
                yield return null;
            }
            if (null != currentSerializeDownloader)
            {
                yield return new WaitUntil (() => currentSerializeDownloader.IsDecompressDone);
            }
            if (!_downloadFailed && !_serializeFailed)
            {
                for (int i = 0; i < _bundles.Count; i++)
                {
                    if (EFileLocation.Server == _bundles [i].FileLocation)
                    {
                        _bundles [i].FileLocation = EFileLocation.TemporaryCache;
                    }
                }
                Messenger<long, long>.Broadcast(EMessengerType.OnResourcesUpdateProgressUpdate, _needsDownloadTotalByte, _needsDownloadTotalByte);
            }
            _totalTime = DateTimeUtil.GetNowTicks() / 10000 - beginTime;
        }

        /// <summary>
        /// 解压所有压缩文件并保存到永久存储
        /// </summary>
        /// <returns>The and copy to persistant.</returns>
        public IEnumerator DecompressOrCopyToPersistant ()
        {
			FileTools.CheckAndCreateFolder(StringUtil.Format(StringFormat.TwoLevelPath, ResPath.PersistentDataPath, ResPath.PersistentBundleRoot));

			// get total size of files to decompress
			long totalSizeToDecompress = 0;
			for (int i = 0; i < _bundles.Count; i++)
			{
			    var bundle = _bundles[i];
				if (!bundle.NeedWriteToPersistent())
				{//非安卓平台 未压缩资源无需拷贝到persistent
					continue;
				}
				totalSizeToDecompress += _bundles[i].Size;
			}
			long[] sizeDone = {0};
            for (int i = 0; i < _bundles.Count; i++)
            {
                var bundle = _bundles[i];
                if (!bundle.NeedWriteToPersistent())
                {
                    continue;
                }
                if (_ioException != null)
                {
                    break;
                }
                Loom.RunAsync(() =>
                {
                    if (_ioException != null)
                    {
                        return;
                    }
                    try
                    {
                        long compressedSize = bundle.Size;
                        bundle.DecompressOrCopyToPersistant();
                        Loom.QueueOnMainThread(() =>
                        {
                            sizeDone[0] += compressedSize;
                            Messenger<long, long>.Broadcast(EMessengerType.OnResourcesUpdateProgressUpdate, sizeDone[0], totalSizeToDecompress);
                        });
                    }
                    catch (Exception e)
                    {
                        Loom.QueueOnMainThread(()=>
                        {
                            if (_ioException != null)
                            {
                                return;
                            }
                            _ioException = e;
                        });
                    }
                });
            }
            while (totalSizeToDecompress - sizeDone[0] > 0 && _ioException == null)
            {
                yield return null;
            }
            if (_ioException != null)
            {
                LogHelper.Error("DecompressOrCopyToPersistant Failed, Exception: {0}", _ioException.ToString());
                SocialGUIManager.ShowPopupDialog("资源解压失败，请检查剩余存储空间后重试", null,
                    new KeyValuePair<string, Action>("确定", () =>
                {
                    Application.Quit();
                }));
                while (true)
                {
                    yield return null;
                }
            }
            // 写入manifest文件
			UnityTools.TrySaveObjectToLocal<CHResManifest> (this, ResDefine.CHResManifestFileName);
        }

        public void DeleteUnusedBundle()
        {
            for (int i = 0; i < _needDeleteBundles.Count; i++)
            {
                var bundle = _needDeleteBundles[i];
                var path = bundle.GetFilePath(EFileLocation.Persistent);
                try
                {
                    FileTools.DeleteFile(path);
                }
                catch (Exception e)
                {
                    LogHelper.Warning("DeleteUnusedBundle OneFile Failed, FilePath: {0}, Exception: {1}", path, e);
                }
            }
        }
    }
}