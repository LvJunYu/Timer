using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameA;
using Newtonsoft.Json;
using SoyEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NewResourceSolution
{
    public class CHResBundle : IEquatable<CHResBundle>
    {
        [JsonProperty(PropertyName = "GI")] public int GroupId;
        [JsonProperty(PropertyName = "AN")] public string[] AssetNames;
        [JsonProperty(PropertyName = "BN")] public string AssetBundleName;
        [JsonProperty(PropertyName = "R5")] public string RawMd5;
        [JsonProperty(PropertyName = "C5")] public string CompressedMd5;

        /// <summary>
        /// 对于CompressType是LZMA的bundle，除了persistentData中的manifest文件，
        /// 其余manifest文件中的bundle的size表示的是压缩后的大小，persistentData中
        /// 的manifest中的bundle的size表示的解压后的文件大小，也是bundle缓存在内存中的
        /// 预估所占空间
        /// 对于CompressType是NoCompress的bundle，该值的意义始终是未压缩的大小
        /// </summary>
        [JsonProperty(PropertyName = "SZ")] public long Size;

        /// <summary>
        /// 以怎样的压缩格式进行打包发布的
        /// </summary>
        [JsonProperty(PropertyName = "CT")] public EAssetBundleCompressType CompressType;

        [JsonProperty(PropertyName = "FL")] public EFileLocation FileLocation;

        private string _filePathPersistent;
        private string _filePathStreamingAsset;
        private string _filePathTemporaryCache;
        private string _filePathServer;

        #region used by runtime manifest

        [JsonIgnore] public AssetBundle CachedBundle;
        [JsonIgnore] public Dictionary<string, Object> AssetDic = new Dictionary<string, Object>();
        [JsonIgnore] public int ScenaryMask;
        [JsonIgnore] public bool IsLocaleRes;
        [JsonIgnore] public string LocaleName;

        #endregion

//		#region used when build bundle
//		[Newtonsoft.Json.JsonIgnore]
//		public bool IsAdamRes;
//		#endregion

        /// <summary>
        /// 获得文件应该在的位置
        /// </summary>
        /// <returns>The file path.</returns>
        /// <param name="location">Location.</param>
        /// <param name="resVersion"></param>
        public string GetFilePath(EFileLocation location, Version resVersion = null)
        {
            var md5Str = CompressType == EAssetBundleCompressType.NoCompress ? RawMd5 : CompressedMd5;
            if (EFileLocation.Persistent == location)
            {
                if (null == _filePathPersistent)
                {
                    _filePathPersistent = StringUtil.Format(StringFormat.ThreeLevelPath, ResPath.PersistentDataPath,
                        ResPath.PersistentBundleRoot, AssetBundleName);
                }
                return _filePathPersistent;
            }
            if (EFileLocation.StreamingAsset == location)
            {
                if (null == _filePathStreamingAsset)
                {
                    _filePathStreamingAsset = StringUtil.Format(StringFormat.TwoLevelPathWithExtention,
                        ResPath.StreamingAssetsPath, AssetBundleName, md5Str);
                }
                return _filePathStreamingAsset;
            }
            if (EFileLocation.TemporaryCache == location)
            {
                if (null == _filePathTemporaryCache)
                {
                    _filePathTemporaryCache = StringUtil.Format(StringFormat.ThreeLevelPath, ResPath.PersistentDataPath,
                        ResPath.TempCache, AssetBundleName);
                }
                return _filePathTemporaryCache;
            }
            if (EFileLocation.Server == location && null != resVersion)
            {
                if (null == _filePathServer)
                {
                    _filePathServer = StringUtil.Format(StringFormat.ThreeLevelPathWithExtention,
                        SocialApp.GetAppServerAddress().GameResoureRoot, resVersion, AssetBundleName, md5Str);
                }
                return _filePathServer;
            }
            return string.Empty;
        }

        /// <summary>
        /// 检查文件完整性，如果检测的是StreamingAsset中的文件，需要根据是否是亚当资源的判断来做特殊处理
        /// </summary>
        /// <returns><c>true</c> if this instance is persistant file ready; otherwise, <c>false</c>.</returns>
        public EFileIntegrity CheckFileIntegrity(EFileLocation location, bool isAdamBundle)
        {
            if (EFileLocation.Server == location)
                return EFileIntegrity.NotExist;
            FileInfo fi = new FileInfo(GetFilePath(location));
            if (EFileLocation.StreamingAsset == location)
            {
                if (fi.Exists)
                {
                    string md5;
                    FileTools.GetFileMd5(fi.FullName, out md5);
                    string compareMd5 = isAdamBundle || (EAssetBundleCompressType.NoCompress == CompressType)
                        ? RawMd5
                        : CompressedMd5;
                    if (String.CompareOrdinal(compareMd5, md5) == 0)
                    {
                        return EFileIntegrity.Integral;
                    }
                    return EFileIntegrity.Md5Dismatch;
                }
                return EFileIntegrity.NotExist;
            }
            if (EFileLocation.Persistent == location ||
                EFileLocation.TemporaryCache == location)
            {
//                LogHelper.Info ("Check {0} in temporary", AssetBundleName);
                if (fi.Exists)
                {
//                    LogHelper.Info ("File exist");
                    string md5;
                    FileTools.GetFileMd5(fi.FullName, out md5);
//                    LogHelper.Info ("File md5 is {0}", md5);
//                    LogHelper.Info ("Raw md5 is {0}", RawMd5);
                    if (String.CompareOrdinal(RawMd5, md5) == 0)
                    {
                        return EFileIntegrity.Integral;
                    }
                    return EFileIntegrity.Md5Dismatch;
                }
                return EFileIntegrity.NotExist;
            }
            return EFileIntegrity.NotExist;
        }

        /// <summary>
        /// 解压或拷贝（非压缩状态）到永久存储
        /// </summary>
        /// <returns>The or copy to persistant.</returns>
        public IEnumerator DecompressOrCopyToPersistant(bool isAdamBundle)
        {
            if (EFileLocation.Server == FileLocation ||
                EFileLocation.Persistent == FileLocation)
            {
                yield break;
            }
            string sourceFilePath = GetFilePath(FileLocation);
            string destFilePath = GetFilePath(EFileLocation.Persistent);
            if (String.IsNullOrEmpty(sourceFilePath) || string.IsNullOrEmpty(destFilePath))
                yield break;
            // 以下几种情况使用拷贝，否则使用解压：是临时文件夹内的文件、是非压缩的文件、是streamingAssets中的文件并且是亚当资源
            if (EFileLocation.TemporaryCache == FileLocation ||
                EAssetBundleCompressType.NoCompress == CompressType ||
                (EFileLocation.StreamingAsset == FileLocation && isAdamBundle)
            )
            {
                ThreadAction copyFile = new ThreadAction(
                    () =>
                    {
                        FileTools.CopyFileSync(
                            sourceFilePath,
                            destFilePath
                        );
                    }
                );
                yield return new WaitUntil(() => copyFile.IsDone);
                if (!string.IsNullOrEmpty(copyFile.Error))
                {
                    LogHelper.Error("Copy file error: {0}", copyFile.Error);
                }
//                LogHelper.Info ("Copy file done");
            }
            else if (EFileLocation.StreamingAsset == FileLocation)
            {
                long uncompressFileSize = 0;
                ThreadAction decompressFile = new ThreadAction(
                    () =>
                    {
                        uncompressFileSize = CompressTools.DecompressFileLZMA(
                            sourceFilePath,
                            destFilePath
                        );
                    }
                );
                while (!decompressFile.IsDone)
                {
                    yield return null;
                }
                if (!string.IsNullOrEmpty(decompressFile.Error))
                {
                    LogHelper.Error("Decompress file error: {0}", decompressFile.Error);
                }
                else if (0 != uncompressFileSize)
                {
                    Size = uncompressFileSize;
                }
//                LogHelper.Info ("Decompress file done");
            }
            FileLocation = EFileLocation.Persistent;
        }

        public bool Cache(int scenary, bool logWhenError)
        {
            if (null == CachedBundle)
            {
//                LogHelper.Info ("CAche {0}", AssetBundleName);
                CachedBundle = AssetBundle.LoadFromFile(GetFilePath(FileLocation));
                if (null == CachedBundle)
                {
                    if (logWhenError)
                    {
                        LogHelper.Error("Load assetbundle {0} failed.", AssetBundleName);
                    }
                    return false;
                }
                AssetDic.Clear();

//                UnityEngine.Object[] allAssets = CachedBundle.LoadAllAssets();
//                for (int j = 0; j < allAssets.Length; j++)
//                {
//                    if (AssetNames.Contains(allAssets[j].name))
//                    {
//                        AssetDic [allAssets[j].name] =  allAssets[j];
//                    }
//                }

                for (int j = 0; j < AssetNames.Length; j++)
                {
                    Object asset = CachedBundle.LoadAsset(AssetNames[j]);
                    if (null == asset)
                    {
                        if (logWhenError)
                        {
                            LogHelper.Error("Load asset {0} from asset bundle {1} failed.",
                                AssetNames[j],
                                AssetBundleName);
                        }
                        return false;
                    }
                    AssetDic.Add(AssetNames[j], asset);
                }
            }
            ScenaryMask |= 1 << scenary;
            return true;
        }

        public bool UncacheMask(int mask)
        {
            if ((ScenaryMask & mask) == 0)
                return false;
            ScenaryMask &= ~mask;
            if (0 == ScenaryMask)
            {
                UncacheAll();
            }
            return true;
        }

        public bool Uncache(int scenary)
        {
            if ((ScenaryMask & (1 << scenary)) == 0)
                return false;
            ScenaryMask &= ~(1 << scenary);
            if (0 == ScenaryMask)
            {
                UncacheAll();
            }
            return true;
        }

        public void UncacheAll(bool force = true)
        {
            ScenaryMask = 0;
            AssetDic.Clear();
            if (null != CachedBundle)
            {
                CachedBundle.Unload(force);
                CachedBundle = null;
            }
            LogHelper.Info("UncacheAll {0}", AssetBundleName);
        }


        #region compare

        public static bool operator ==(CHResBundle a, CHResBundle b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            return a.Equals(b);
//            return string.Compare(a.AssetName, b.AssetName) == 0 && string.Compare(a.Md5, b.Md5) == 0;
        }

        public static bool operator !=(CHResBundle a, CHResBundle b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }
            return !a.Equals(b);
//            return string.Compare(a.AssetName, b.AssetName) != 0 || string.Compare(a.Md5, b.Md5) != 0;
        }

        public override bool Equals(System.Object obj)
        {
            return Equals(obj as CHResBundle);
        }

        public bool Equals(CHResBundle that)
        {
            if (that == null) return false;
            return String.CompareOrdinal(AssetBundleName, that.AssetBundleName) == 0 &&
                   String.CompareOrdinal(RawMd5, that.RawMd5) == 0;
        }

        public override int GetHashCode()
        {
            return RawMd5.GetHashCode();
        }

        #endregion
    }

    public class BundleDownloader
    {
        private static long s_webRequestTimeoutInMilliSeconds = 3000;
        private static string s_webRequestTimeoutErrorInfo = "{0} www time out.";
        private static string s_sizeNotMatchErrorInfo = "{0} download size not match.";
        private static string s_webRequestMd5CheckFailedErrorInfo = "{0} www md5 check failed.";
        private static string s_temporaryFileMd5CheckFailedErrorInfo = "{0} file md5 check failed.";

        public enum EDownloaderState
        {
            None,
            Waiting,
            Downloading,
            DownloadDone,
            Serializing,
            SerializeDone
        }

        public Version ResVersion;
        public CHResBundle Bundle;
        public EDownloaderState State;
        public WWW WebRequest;
        public ThreadAction SerializeAction;

        public string Error;
        public int TotalErrorCnt;

        /// <summary>
        /// 总共花费的时间，出错重试也会累计
        /// </summary>
        public long DownloadTotalTime;

        /// <summary>
        /// 本次下载的开始时间
        /// </summary>
        public long CurrentDownloadBeginTime;

        /// <summary>
        /// 下载成功的那次下载所花费的时间
        /// </summary>
        public long DownloadTime;

        public bool IsDownloadDone
        {
            get
            {
                if (EDownloaderState.DownloadDone == State)
                    return true;
                if (EDownloaderState.Downloading != State)
                    return false;
                DownloadTime = DateTimeUtil.GetNowTicks() / 10000 - CurrentDownloadBeginTime;
                if (!WebRequest.isDone && DownloadTime < s_webRequestTimeoutInMilliSeconds)
                    return false;
                DownloadTotalTime += DownloadTime;
                if (DownloadTime >= s_webRequestTimeoutInMilliSeconds)
                {
                    Error = StringUtil.Format(s_webRequestTimeoutErrorInfo, Bundle.AssetBundleName);
                    TotalErrorCnt++;
                }
                else if (!string.IsNullOrEmpty(WebRequest.error))
                {
                    Error = StringUtil.Format("{0}, bundle name: {1}", WebRequest.error, Bundle.AssetBundleName);
                    TotalErrorCnt++;
                }
                else if (WebRequest.bytes.Length != Bundle.Size)
                {
                    Error = StringUtil.Format(s_sizeNotMatchErrorInfo, Bundle.AssetBundleName);
                    TotalErrorCnt++;
                }
                else if (FileTools.GetBytesMd5(WebRequest.bytes) != Bundle.CompressedMd5)
                {
                    Error = StringUtil.Format(s_webRequestMd5CheckFailedErrorInfo, Bundle.AssetBundleName);
                    TotalErrorCnt++;
                }
                State = EDownloaderState.DownloadDone;
                return true;
            }
        }

        public bool IsDecompressDone
        {
            get
            {
                if (EDownloaderState.SerializeDone == State)
                    return true;
                if (EDownloaderState.Serializing != State)
                    return false;
                if (!SerializeAction.IsDone)
                    return false;
                if (!string.IsNullOrEmpty(SerializeAction.Error))
                {
                    Error = StringUtil.Format("{0}, bundle name: {1}", SerializeAction.Error, Bundle.AssetBundleName);
                }
                else if (EFileIntegrity.Integral != Bundle.CheckFileIntegrity(EFileLocation.TemporaryCache, false))
                {
                    Error = StringUtil.Format(s_temporaryFileMd5CheckFailedErrorInfo, Bundle.AssetBundleName);
                }

                return true;
            }
        }

        public BundleDownloader(CHResBundle bundle, Version resVersion)
        {
            Bundle = bundle;
            ResVersion = resVersion;
            State = EDownloaderState.Waiting;
        }

        public void BeginDownload()
        {
            State = EDownloaderState.Downloading;
            CurrentDownloadBeginTime = DateTimeUtil.GetNowTicks() / 10000;
//            LogHelper.Info ("Begin to download {0}", Bundle.GetFilePath(EFileLocation.Server, ResVersion));
            WebRequest = new WWW(Bundle.GetFilePath(EFileLocation.Server, ResVersion));
        }

        public void BeginDecompressAndSave()
        {
            string destPath = Bundle.GetFilePath(EFileLocation.TemporaryCache);
            Byte[] bytes = WebRequest.bytes;
            if (EAssetBundleCompressType.NoCompress == Bundle.CompressType)
            {
                State = EDownloaderState.Serializing;
                SerializeAction = new ThreadAction(
                    () => { FileTools.WriteBytesToFile(bytes, destPath); },
                    true
                );
            }
            else if (EAssetBundleCompressType.LZMA == Bundle.CompressType)
            {
                State = EDownloaderState.Serializing;
                SerializeAction = new ThreadAction(
                    () => { CompressTools.DecompressBytesLZMA(bytes, destPath); },
                    true
                );
            }
        }

        public void ReWaitDownload()
        {
            State = EDownloaderState.Waiting;
            if (null != WebRequest) WebRequest.Dispose();
        }
    }
}