using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;

namespace NewResourceSolution
{

    public class CHResBundle : IEquatable<CHResBundle>
    {
		[Newtonsoft.Json.JsonProperty(PropertyName = "GI")]
        public int GroupId;
		[Newtonsoft.Json.JsonProperty(PropertyName = "AN")]
        public string[] AssetNames;
		[Newtonsoft.Json.JsonProperty(PropertyName = "BN")]
        public string AssetBundleName;
		[Newtonsoft.Json.JsonProperty(PropertyName = "R5")]
        public string RawMd5;
		[Newtonsoft.Json.JsonProperty(PropertyName = "C5")]
        public string CompressedMd5;
		[Newtonsoft.Json.JsonProperty(PropertyName = "SZ")]
        public long Size;
        /// <summary>
        /// 以怎样的压缩格式进行打包发布的
        /// </summary>
		[Newtonsoft.Json.JsonProperty(PropertyName = "CT")]
        public EAssetBundleCompressType CompressType;
		[Newtonsoft.Json.JsonProperty(PropertyName = "FL")]
        public EFileLocation FileLocation;

		private string _filePathPersistent;
		private string _filePathStreamingAsset;
		private string _filePathTemporaryCache;
		private string _filePathServer;

		#region used by runtime manifest
		[Newtonsoft.Json.JsonIgnore]
		public Dictionary<string, UnityEngine.Object> AssetDic = new Dictionary<string, UnityEngine.Object>();
		[Newtonsoft.Json.JsonIgnore]
        public int ScenaryMask;
		[Newtonsoft.Json.JsonIgnore]
		public long AssetsTotalSize;
		[Newtonsoft.Json.JsonIgnore]
		public bool IsLocaleRes;
		[Newtonsoft.Json.JsonIgnore]
		public string LocaleName;
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
        public string GetFilePath(EFileLocation location, Version resVersion = null)
        {
            if (EFileLocation.Persistent == location)
            {
				if (null == _filePathPersistent)
				{
					_filePathPersistent = StringUtil.Format (StringFormat.ThreeLevelPath, ResPath.PersistentDataPath, ResPath.PersistentBundleRoot, AssetBundleName);
				}
				return _filePathPersistent;
            }
            else if (EFileLocation.StreamingAsset == location)
            {
				if (null == _filePathStreamingAsset)
				{
					_filePathStreamingAsset = StringUtil.Format (StringFormat.TwoLevelPathWithExtention, ResPath.StreamingAssetsPath, AssetBundleName, CompressedMd5);
				}
				return _filePathStreamingAsset;
            }
            else if (EFileLocation.TemporaryCache == location)
            {
				if (null == _filePathTemporaryCache)
				{
					_filePathTemporaryCache = StringUtil.Format (StringFormat.ThreeLevelPath, ResPath.PersistentDataPath, ResPath.TempCache, AssetBundleName);
				}
				return _filePathTemporaryCache;
            }
            else if (EFileLocation.Server == location && null != resVersion)
            {
				if (null == _filePathServer)
				{
					_filePathServer = StringUtil.Format (StringFormat.ThreeLevelPathWithExtention, RuntimeConfig.Instance.ResourceServerAddress, resVersion, AssetBundleName, CompressedMd5);
				}
				return _filePathServer;
            }
            return string.Empty;
        }

        /// <summary>
        /// 检查文件完整性，如果检测的是StreamingAsset中的文件，需要根据是否是亚当资源的判断来做特殊处理
        /// </summary>
        /// <returns><c>true</c> if this instance is persistant file ready; otherwise, <c>false</c>.</returns>
        public EFileIntegrity CheckFileIntegrity (EFileLocation location, bool isAdamBundle)
        {
            if (EFileLocation.Server == location)
                return EFileIntegrity.NotExist;
            System.IO.FileInfo fi = new System.IO.FileInfo (GetFilePath (location));
            if (EFileLocation.StreamingAsset == location)
            {
                if (fi.Exists)
                {
                    string md5;
                    FileTools.GetFileMd5 (fi.FullName, out md5);
                    string compareMd5 = isAdamBundle || (EAssetBundleCompressType.NoCompress == CompressType) ? RawMd5 : CompressedMd5;
                    if (string.Compare (
                        compareMd5,
                        md5) == 0)
                    {
                        return EFileIntegrity.Integral;
                    }
                    else
                    {
                        return EFileIntegrity.Md5Dismatch;
                    }
                }
                else
                {
                    return EFileIntegrity.NotExist;
                }
            }
            else if (EFileLocation.Persistent == location ||
                EFileLocation.TemporaryCache == location)
            {
//                LogHelper.Info ("Check {0} in temporary", AssetBundleName);
                if (fi.Exists)
                {
//                    LogHelper.Info ("File exist");
                    string md5;
                    FileTools.GetFileMd5 (fi.FullName, out md5);
//                    LogHelper.Info ("File md5 is {0}", md5);
//                    LogHelper.Info ("Raw md5 is {0}", RawMd5);
                    if (string.Compare(RawMd5, md5) == 0)
                    {
                        return EFileIntegrity.Integral;
                    }
                    else
                    {
                        return EFileIntegrity.Md5Dismatch;
                    }
                }
                else
                {
                    return EFileIntegrity.NotExist;
                }
            }
            return EFileIntegrity.NotExist;
        }

		/// <summary>
		/// 解压或拷贝（非压缩状态）到永久存储
		/// </summary>
		/// <returns>The or copy to persistant.</returns>
        public IEnumerator DecompressOrCopyToPersistant (bool isAdamBundle)
        {
            if (EFileLocation.Server == FileLocation ||
                EFileLocation.Persistent == FileLocation)
            {
                yield break;
            }
            string sourceFilePath = GetFilePath (FileLocation);
            string destFilePath = GetFilePath (EFileLocation.Persistent);
            if (String.IsNullOrEmpty (sourceFilePath) || string.IsNullOrEmpty(destFilePath))
                yield break;
            // 以下几种情况使用拷贝，否则使用解压：是临时文件夹内的文件、是非压缩的文件、是streamingAssets中的文件并且是亚当资源
            if (EFileLocation.TemporaryCache == FileLocation ||
                EAssetBundleCompressType.NoCompress == CompressType ||
                (EFileLocation.StreamingAsset == FileLocation && isAdamBundle)
               )
            {
                ThreadAction copyFile = new ThreadAction (
                    () => {
                        FileTools.CopyFileSync (
                            sourceFilePath,
                            destFilePath
                        );
                    },
                    true
                );
                yield return new UnityEngine.WaitUntil (() => copyFile.IsDone);
                if (!string.IsNullOrEmpty (copyFile.Error))
                {
                    LogHelper.Error ("Copy file error: {0}", copyFile.Error);
                }
//                LogHelper.Info ("Copy file done");
            }
            else if (EFileLocation.StreamingAsset == FileLocation)
            {
                ThreadAction decompressFile = new ThreadAction (
                    () => {
                        CompressTools.DecompressFileLZMA (
                            sourceFilePath,
                            destFilePath
                        );
                    },
                    true
                );
                yield return new UnityEngine.WaitUntil (() => decompressFile.IsDone);
                if (!string.IsNullOrEmpty (decompressFile.Error))
                {
                    LogHelper.Error ("Decompress file error: {0}", decompressFile.Error);
                }
//                LogHelper.Info ("Decompress file done");
            }
            FileLocation = EFileLocation.Persistent;
        }


        #region compare
        public static bool operator == (CHResBundle a, CHResBundle b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }
            return a.Equals (b);
//            return string.Compare(a.AssetName, b.AssetName) == 0 && string.Compare(a.Md5, b.Md5) == 0;
        }
        public static bool operator != (CHResBundle a, CHResBundle b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return !object.ReferenceEquals(b, null);
            }
            return !a.Equals (b);
//            return string.Compare(a.AssetName, b.AssetName) != 0 || string.Compare(a.Md5, b.Md5) != 0;
        }
        public override bool Equals(System.Object obj)
        {
            return Equals(obj as CHResBundle);
        }
        public bool Equals(CHResBundle that)
        {
            if (that == null) return false;
			return string.Compare(this.AssetBundleName, that.AssetBundleName) == 0 && string.Compare(this.RawMd5, that.RawMd5) == 0;
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
            SerializeDone,
        }
        public Version ResVersion;
        public CHResBundle Bundle;
        public EDownloaderState State;
        public UnityEngine.WWW WebRequest;
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
                else if (!string.IsNullOrEmpty (WebRequest.error))
                {
					Error = StringUtil.Format ("{0}, bundle name: {1}", WebRequest.error, Bundle.AssetBundleName);
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
                if (!string.IsNullOrEmpty (SerializeAction.Error))
                {
					Error = StringUtil.Format ("{0}, bundle name: {1}", SerializeAction.Error, Bundle.AssetBundleName);
                }
                else if (EFileIntegrity.Integral != Bundle.CheckFileIntegrity(EFileLocation.TemporaryCache, false))
                {
					Error = StringUtil.Format(s_temporaryFileMd5CheckFailedErrorInfo, Bundle.AssetBundleName);
                }
                    
                return true;
            }
        }

        public BundleDownloader (CHResBundle bundle, Version resVersion)
        {
            Bundle = bundle;
            ResVersion = resVersion;
            State = EDownloaderState.Waiting;
        }

        public void BeginDownload ()
        {
            State = EDownloaderState.Downloading;
            CurrentDownloadBeginTime = DateTimeUtil.GetNowTicks() / 10000;
//            LogHelper.Info ("Begin to download {0}", Bundle.GetFilePath(EFileLocation.Server, ResVersion));
            WebRequest = new UnityEngine.WWW (Bundle.GetFilePath(EFileLocation.Server, ResVersion));
        }

        public void BeginDecompressAndSave ()
        {
            string destPath = Bundle.GetFilePath (EFileLocation.TemporaryCache);
            Byte[] bytes = WebRequest.bytes;
            if (EAssetBundleCompressType.NoCompress == Bundle.CompressType) {
                State = EDownloaderState.Serializing;
                SerializeAction = new ThreadAction (
                    () =>
                    {
                        FileTools.WriteBytesToFile(bytes, destPath);
                    },
                    true
                );
            } else if (EAssetBundleCompressType.LZMA == Bundle.CompressType) {
                State = EDownloaderState.Serializing;
                SerializeAction = new ThreadAction (
                    () =>
                    {
                        CompressTools.DecompressBytesLZMA(bytes, destPath);
                    },
                    true
                );
            }
        }

        public void ReWaitDownload ()
        {
            State = EDownloaderState.Waiting;
            if (null != WebRequest) WebRequest.Dispose ();
        }
    }
}