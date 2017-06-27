using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;

namespace NewResourceSolution
{
    public class CHDownloadingResManifest : CHResManifest
    {
        /// <summary>
        /// 同时并发的www请求数
        /// </summary>
        private static int s_maxcConcurrentDownloadNum = 2;
        /// <summary>
        /// 对单个文件的最大重试次数，任意文件的下载重试次数大于此数则整体下载失败
        /// </summary>
        private static int s_maxRetryCntForSingleFile = 4;

        private long _needsDownloadTotalByte;
        private int _needsDownloadTotalCnt;

        private long _downloadFinishByte;
        private int _downloadFinishCnt;

        /// <summary>
        /// 总共花费的时间
        /// </summary>
        private long _totalTime;
        /// <summary>
        /// 除去重试的时间，所有成功下载的过程所花费的时间总和
        /// </summary>
        private long _downloadTotalTime;

        private int _totalErrorCnt;
        /// <summary>
        /// 下载过程中出错
        /// </summary>
        private bool _downloadFailed;
        /// <summary>
        /// 序列化过程中出错
        /// </summary>
        private bool _serializeFailed;

        private Queue<BundleDownloader> _waitQueue = new Queue<BundleDownloader>();
        private List<BundleDownloader> _downloadingList = new List<BundleDownloader>();
        private Queue<BundleDownloader> _waitDecompressQueue = new Queue<BundleDownloader>();

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
        }
        /// <summary>
        /// 混合下载manifest和(包内/本地/临时)manifest
        /// </summary>
        /// <param name="existingManifest">Existing manifest.</param>
        public IEnumerator MergeExistingManifest (CHRuntimeResManifest existingManifest)
        {
            for (int i = 0; i < _bundles.Count; i++)
            {
                if (EFileLocation.Server != _bundles[i].FileLocation)
                {
                    // 之前已经和其他manifest合并并确认有此文件了
                    continue;
                }
				CHResBundle bundleInExistingManifest = existingManifest.GetBundleByBundleName(_bundles[i].AssetBundleName);
                if (EFileLocation.Server != bundleInExistingManifest.FileLocation)
                {
                    if (null != bundleInExistingManifest &&
                        string.Compare(_bundles[i].CompressedMd5, bundleInExistingManifest.CompressedMd5) == 0)
                    {
                        bool isAdamBundle = existingManifest.AdamBundleNameList.Contains (bundleInExistingManifest.AssetBundleName);
                        EFileIntegrity integrity = bundleInExistingManifest.CheckFileIntegrity (bundleInExistingManifest.FileLocation, isAdamBundle);
                        if (EFileIntegrity.Integral == integrity)
                        {
                            _bundles[i].FileLocation = bundleInExistingManifest.FileLocation;
                            if (isAdamBundle)
                            {
                                _adamBundleNameList.Add (_bundles[i].AssetBundleName);
                            }
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
                EFileIntegrity integrity = _bundles [i].CheckFileIntegrity (EFileLocation.TemporaryCache, false);
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
                    bool isadamBundle = _adamBundleNameList.Contains (_bundles [i].AssetBundleName);
                    EFileIntegrity integrity = _bundles [i].CheckFileIntegrity (EFileLocation.StreamingAsset, isadamBundle);
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
            _totalErrorCnt = 0;
            _downloadTotalTime = 0;
            _downloadFinishByte = 0;
            _downloadFinishCnt = 0;
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

                while (_downloadingList.Count < s_maxcConcurrentDownloadNum && _waitQueue.Count > 0)
                {
                    var downloader = _waitQueue.Dequeue ();
                    downloader.BeginDownload ();
                    _downloadingList.Add (downloader);
                    LogHelper.Info ("Begin download bundle: {0}", downloader.Bundle.AssetBundleName);
                }

                for (int i = _downloadingList.Count - 1; i >= 0; i--)
                {
                    if (_downloadingList [i].IsDownloadDone)
                    {
                        if (!string.IsNullOrEmpty (_downloadingList [i].Error))
                        {
                            LogHelper.Error ("Error when download assetBundle: {0}", _downloadingList[i].Error);
                            _totalErrorCnt++;
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
                            _downloadFinishCnt++;
                            _downloadFinishByte += currentSerializeDownloader.Bundle.Size;
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
                yield return new UnityEngine.WaitUntil (() => currentSerializeDownloader.IsDecompressDone);
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

            for (int i = 0; i < _bundles.Count; i++)
            {
                yield return _bundles [i].DecompressOrCopyToPersistant (_adamBundleNameList.Contains(_bundles[i].AssetBundleName));
            }
            // 写入manifest文件
			UnityTools.TrySaveObjectToLocal<CHResManifest> (this, ResDefine.CHResManifestFileName);
        }
    }
}