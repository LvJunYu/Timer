using System;
using System.Collections;
using System.Collections.Generic;
using GameA;
using SoyEngine;
using UnityEngine;
using EMessengerType = GameA.EMessengerType;

namespace NewResourceSolution
{
    public static class VersionUpdater
    {
        public static bool IsDevelopment()
        {
            if (Application.isEditor)
            {
                return true;
            }
            var env = SocialApp.Instance.Env;
            if (env == EEnvironment.Local
                || env == EEnvironment.Development)
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 版本检测与更新主协程，逻辑见流程图“动态资源解决方案流程图”
        /// </summary>
        /// <returns>The ver internal.</returns>
        public static IEnumerator CheckVerInternal(CHRuntimeResManifest persistentManifest,
            CHRuntimeResManifest buildInManifest)
        {
            bool manifestUpdated = false;
            Messenger.Broadcast(EMessengerType.OnResourcesCheckStart);
            Messenger<string>.Broadcast(EMessengerType.OnVersionUpdateStateChange, "正在检查资源（不消耗流量）...");
            // 读取远端版本
            // todo 按程序版本拼地址
            string serverVersionConfigPath = StringUtil.Format(
                StringFormat.ThreeLevelPath,
                SocialApp.GetAppServerAddress().GameResoureRoot,
                ResPath.GetPlatformFolder(),
                ResDefine.ServerVersionConfigFileName) + "?t="+DateTimeUtil.GetNowTicks();
            ServerVersionConfig serverVersionConfig = null;

            while (true)
            {
                yield return UnityTools.GetObjectFromServer<ServerVersionConfig>(
                    serverVersionConfigPath,
                    (obj) => { serverVersionConfig = obj; },
                    ResDefine.GetServerVersionConfigTimeout
                );
                if (serverVersionConfig == null && !IsDevelopment())
                {
                    var waitForUserInput = true;
                    SocialGUIManager.ShowPopupDialog("连接资源服务器失败，请检查网络后重试", null,
                        new KeyValuePair<string, Action>("重试", () =>
                        {
                            waitForUserInput = false;
                        })
                    );
                    while (waitForUserInput) yield return null;
                }
                else
                {
                    break;
                }
            }
            
            if (null != serverVersionConfig)
            {
                Version applicationVersion = new Version(RuntimeConfig.Instance.Version);
                LogHelper.Info("applicationVersion: {0}", applicationVersion);
                if (applicationVersion < serverVersionConfig.MinimumAppVersion)
                {
                    // 要求用户下载最新版本
                    LogHelper.Info("MinimumAppVersion: {0}", serverVersionConfig.MinimumAppVersion);
                    SocialGUIManager.ShowPopupDialog("请更新最新版本进入游戏", null,
                        new KeyValuePair<string, Action>("更新", () =>
                        {
                            Application.OpenURL(serverVersionConfig.LatestAppDownloadPath);
                        })
                    );
                    yield return new WaitForSeconds(float.MaxValue);
                }
                if (applicationVersion < serverVersionConfig.LatestAppVersion)
                {
                    bool waitForUserInput = true;
                    // 提示用户更新
                    LogHelper.Info("LatestAppVersion: {0}", serverVersionConfig.LatestAppVersion);
                    SocialGUIManager.ShowPopupDialog("游戏已更新，当前版本现在还能使用，是否更新最新版本", null,
                        new KeyValuePair<string, Action>("继续", () =>
                        {
                            waitForUserInput = false;
                        }),
                        new KeyValuePair<string, Action>("更新", () =>
                        {
                            Application.OpenURL(serverVersionConfig.LatestAppDownloadPath);
                        })
                    );
                    while (waitForUserInput) yield return null;
                }
            }
            CHRuntimeResManifest newestLocalResManifest;
            if (persistentManifest != null)
            {
                if (persistentManifest.Ver < buildInManifest.Ver)
                {
                    var latestResManifest = new CHDownloadingResManifest(buildInManifest);
                    yield return latestResManifest.MergePersistentToStreamingManifest(persistentManifest);
                    latestResManifest.MapBundles();
                    newestLocalResManifest = latestResManifest;
                    manifestUpdated = true;
                }
                else
                {
                    newestLocalResManifest = persistentManifest;
                }
            }
            else
            {
                newestLocalResManifest = buildInManifest;
                manifestUpdated = true;
            }
            
            CHDownloadingResManifest newestDownloadingResManifest = null;
            if (null != serverVersionConfig)
            {
                if (newestLocalResManifest.Ver < serverVersionConfig.LatestResVersion)
                {
                    CHDownloadingResManifest serverResManifest = null;
                    string latestResManifestPath = string.Format(StringFormat.FourLevelPath,
                        SocialApp.GetAppServerAddress().GameResoureRoot,
                        ResPath.GetPlatformFolder(),
                        serverVersionConfig.LatestResVersion,
                        ResDefine.CHResManifestFileName);
                    while (true)
                    {
                        var waitForUserInput = true;
                        yield return UnityTools.GetObjectFromServer<CHDownloadingResManifest>(
                            latestResManifestPath,
                            (obj) => { serverResManifest = obj; },
                            ResDefine.GetServerManifestTimeout
                        );
                        if (null == serverResManifest)
                        {
                            LogHelper.Error("Download latest resManifest failed, path: {0}",  
                                latestResManifestPath);
                            SocialGUIManager.ShowPopupDialog("最新资源获取失败", null,
                                new KeyValuePair<string, Action>("重试", () =>
                                {
                                    waitForUserInput = false;
                                })
                            );
                            while (waitForUserInput) yield return null;
                        }
                        else
                        {
                            serverResManifest.FileLocation = EFileLocation.Server;
                            break;
                        }
                    }
                    yield return serverResManifest.MergeExistingManifest(newestLocalResManifest);
                    newestDownloadingResManifest = serverResManifest;
                    manifestUpdated = true;
                }
            }
            if (newestDownloadingResManifest == null)
            {
                if (newestLocalResManifest is CHDownloadingResManifest)
                {
                    newestDownloadingResManifest = newestLocalResManifest as CHDownloadingResManifest;
                }
                else
                {
                    newestDownloadingResManifest = new CHDownloadingResManifest(newestLocalResManifest);
                }
            }
            yield return newestDownloadingResManifest.MergeTemporaryCacheFiles();
            newestDownloadingResManifest.CalculateFilesNeedsDownload();
            if (newestDownloadingResManifest.NeedsDownloadTotalCnt != 0)
            {
                manifestUpdated = true;
                {
                    bool waitForRetry = true;
                    // 提示用户下载资源
                    SocialGUIManager.ShowPopupDialog(string.Format("需要下载更新资源 {0:F1}M，是否立即开始",
                            1f*newestDownloadingResManifest.NeedsDownloadTotalByte/1024/1024), null,
                        new KeyValuePair<string, Action>("下载", () =>
                        {
                            waitForRetry = false;
                        })
                    );
                    while (waitForRetry) yield return null;
                }
                Messenger<string>.Broadcast(EMessengerType.OnVersionUpdateStateChange, "正在下载资源...");
                // 进入下载流程
                LogHelper.Info("Begin download, cnt: {0}, size: {1}",
                    newestDownloadingResManifest.NeedsDownloadTotalCnt,
                    newestDownloadingResManifest.NeedsDownloadTotalByte);
                while (true)
                {
                    yield return newestDownloadingResManifest.DownloadServerAssetBundles();
                    if (newestDownloadingResManifest.DownloadFailed)
                    {
                        LogHelper.Error("Download failed");
                        bool waitForRetry = true;
                        // 提示用户下载必要的文件出错，等待用户确认重试
                        SocialGUIManager.ShowPopupDialog("资源下载出错", null,
                            new KeyValuePair<string, Action>("重试", () =>
                            {
                                waitForRetry = false;
                            })
                        );
                        while (waitForRetry) yield return null;
                    }
                    else if (newestDownloadingResManifest.SerializeFailed)
                    {
                        LogHelper.Error("Serialize failed");
                        // 提示用户下载必要的文件出错，等待用户确认重试
                        SocialGUIManager.ShowPopupDialog("资源保存出错，请检查剩余存储空间", null,
                            new KeyValuePair<string, Action>("退出", Application.Quit)
                        );
                        while (true) yield return null;
                    }
                    else
                    {
                        break;
                    }
                    yield return newestDownloadingResManifest.MergeTemporaryCacheFiles();
                    newestDownloadingResManifest.CalculateFilesNeedsDownload();
                }
            }
            if (manifestUpdated)
            {
                yield return newestDownloadingResManifest.DecompressOrCopyToPersistant();
                newestDownloadingResManifest.DeleteUnusedBundle();
            }
                
            LogHelper.Info("Res check finish");
            if (manifestUpdated)
            {
                JoyResManager.Instance.Init();
            }
            Messenger.Broadcast(EMessengerType.OnResourcesCheckFinish);
            SocialApp.Instance.LoginAfterUpdateResComplete();
        }
    }
}