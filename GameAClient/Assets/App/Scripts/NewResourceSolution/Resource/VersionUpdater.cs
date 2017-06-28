using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using GameA;
using EMessengerType = GameA.EMessengerType;

namespace NewResourceSolution
{
    public static class VersionUpdater
    {
        /// <summary>
        /// 版本检测与更新主协程，逻辑见流程图“动态资源解决方案流程图”
        /// </summary>
        /// <returns>The ver internal.</returns>
        public static IEnumerator CheckVerInternal (CHRuntimeResManifest manifest)
        {
            bool manifestUpdated = false;
            Messenger.Broadcast(EMessengerType.OnResourcesCheckStart);
            // 读取远端版本
            // todo 按程序版本拼地址
			string serverVersionConfigPath = StringUtil.Format (
				StringFormat.TwoLevelPath,
                GameA.SocialApp.GetAppServerAddress().GameResoureRoot, ResDefine.ServerVersionConfigFileName
            );
            ServerVersionConfig serverVersionConfig = null;
            yield return UnityTools.GetObjectFromServer<ServerVersionConfig> (
                serverVersionConfigPath,
                (obj)=>{ serverVersionConfig = obj; },
                ResDefine.GetServerVersionConfigTimeout
            );
            if (null != serverVersionConfig)
            {
                Version applicationVersion = new Version (RuntimeConfig.Instance.Version);
                LogHelper.Info ("applicationVersion: {0}", applicationVersion);
                if (applicationVersion < serverVersionConfig.MinimumAppVersion)
                {
                    // todo 要求用户更新
                    LogHelper.Info ("MinimumAppVersion: {0}", serverVersionConfig.MinimumAppVersion);
                    yield break;
                }
                if (applicationVersion < serverVersionConfig.LatestAppVersion)
                {
                    // todo 询问用户是否下载

                }
                // 如果还没有解压资源
                if (EFileLocation.StreamingAsset == manifest.FileLocation)
                {
                    CHDownloadingResManifest downloadingResManifest = null;
                    if (manifest.Ver == serverVersionConfig.LatestResVersion)
                    {
                        downloadingResManifest = new CHDownloadingResManifest (manifest);
                    }
                    else
                    {
                        CHDownloadingResManifest latestResManifest = null;
                        yield return UnityTools.GetObjectFromServer<CHDownloadingResManifest> (
                            serverVersionConfig.LatestResManifestPath,
                            (obj) => {
                                latestResManifest = obj;
                            },
                            ResDefine.GetServerManifestTimeout
                        );
                        if (null == latestResManifest)
                        {
                            LogHelper.Error ("Download latest resManifest failed, path: {0}", serverVersionConfig.LatestResManifestPath);
                            downloadingResManifest = new CHDownloadingResManifest (manifest);
                        }
                        else
                        {
                            downloadingResManifest = latestResManifest;
                            downloadingResManifest.FileLocation = EFileLocation.Server;
                        }
                    }
                    yield return downloadingResManifest.MergeTemporaryCacheFiles ();
                    // 如果这个dowload用的manifest是以服务器manifest为基础的话
                    if (EFileLocation.Server == downloadingResManifest.FileLocation)
                    {
                        yield return downloadingResManifest.MergeExistingManifest (manifest);
                    }
                    downloadingResManifest.CalculateFilesNeedsDownload();
                    if (downloadingResManifest.NeedsDownloadTotalCnt == 0)
                    {
                        LogHelper.Info ("Decompress asset bundles");
                        yield return downloadingResManifest.DecompressOrCopyToPersistant ();
                        manifestUpdated = true;
                    }
                    else
                    {
                        // 进入下载流程
                        LogHelper.Info ("Begin download, cnt: {0}, size: {1}", downloadingResManifest.NeedsDownloadTotalCnt, downloadingResManifest.NeedsDownloadTotalByte);
                        yield return downloadingResManifest.DownloadServerAssetBundles ();
                        if (downloadingResManifest.DownloadFailed)
                        {
                            LogHelper.Error ("Download failed");
                            // todo 提示用户下载必要的文件出错，然后终止app
                        }
                        else if (downloadingResManifest.SerializeFailed)
                        {
                            LogHelper.Error ("Serialize failed");
                            // todo 提示用户下载必要的文件出错
                        }
                        else
                        {
                            LogHelper.Info ("Download done, totalTime: {0}, downloadTime: {1}", downloadingResManifest.TotalTime, downloadingResManifest.DownloadTotalTime);
                            yield return downloadingResManifest.DecompressOrCopyToPersistant ();
							FileTools.DeleteFolder(StringUtil.Format(StringFormat.TwoLevelPath, ResPath.PersistentDataPath, ResPath.TempCache));
                            manifestUpdated = true;
                        }
                    }
                }
                else
                {
                    if (manifest.Ver < serverVersionConfig.LatestResVersion)
                    {
                        CHDownloadingResManifest latestResManifest = null;
                        yield return UnityTools.GetObjectFromServer<CHDownloadingResManifest> (
                            serverVersionConfig.LatestResManifestPath,
                            (obj) => {
                                latestResManifest = obj;
                            },
                            ResDefine.GetServerManifestTimeout
                        );
                        if (null == latestResManifest)
                        {
                            LogHelper.Error ("Download latest resManifest failed, path: {0}",
                                           serverVersionConfig.LatestResManifestPath);
                            // 不做其他处理，自然进入游戏
                        }
                        else
                        {
                            yield return latestResManifest.MergeTemporaryCacheFiles ();
                            yield return latestResManifest.MergeExistingManifest (manifest);
                            latestResManifest.CalculateFilesNeedsDownload ();
                            if (latestResManifest.NeedsDownloadTotalCnt != 0)
                            {
                                // todo 询问用户是否更新
                                LogHelper.Info ("是否更新资源，数量：{0}，大小：{1}？是Y否N", latestResManifest.NeedsDownloadTotalCnt, latestResManifest.NeedsDownloadTotalByte);
                                while (true)
                                    if (!UnityEngine.Input.anyKey ||
                                        (!UnityEngine.Input.GetKeyDown (UnityEngine.KeyCode.Y) &&
                                     !UnityEngine.Input.GetKeyDown (UnityEngine.KeyCode.N)))
                                    {
                                        yield return null;
                                    }
                                    else
                                        break;
                                if (UnityEngine.Input.GetKeyDown (UnityEngine.KeyCode.Y))
                                {
                                    yield return latestResManifest.DownloadServerAssetBundles ();
                                    if (latestResManifest.DownloadFailed)
                                    {
                                        LogHelper.Error ("Download failed");
                                    }
                                    else if (latestResManifest.SerializeFailed)
                                    {
                                        LogHelper.Error ("Serialize failed");
                                    }
                                    else
                                    {
                                        LogHelper.Info ("Download done, totalTime: {0}, downloadTime: {1}", latestResManifest.TotalTime, latestResManifest.DownloadTotalTime);
                                        yield return latestResManifest.DecompressOrCopyToPersistant ();
										FileTools.DeleteFolder(StringUtil.Format(StringFormat.TwoLevelPath, ResPath.PersistentDataPath, ResPath.TempCache));
                                        manifestUpdated = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // 清空临时资源
						FileTools.DeleteFolder(StringUtil.Format(StringFormat.TwoLevelPath, ResPath.PersistentDataPath, ResPath.TempCache));
                    }
                }
            }
            else
            {
                LogHelper.Info("Connect server failed.");

            }

            LogHelper.Info ("Res check finish");
            if (manifestUpdated)
            {
                ResourcesManager.Instance.Init ();
            }
            Messenger.Broadcast(EMessengerType.OnResourcesCheckFinish);
            SocialApp.Instance.LoginAfterUpdateResComplete ();
        }
    }
}