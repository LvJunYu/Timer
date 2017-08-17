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
        public static IEnumerator CheckVerInternal (CHRuntimeResManifest persistentManifest, CHRuntimeResManifest buildInManifest)
        {
            bool manifestUpdated = false;
            Messenger.Broadcast(EMessengerType.OnResourcesCheckStart);
			Messenger<string>.Broadcast(EMessengerType.OnVersionUpdateStateChange, "正在检查资源...");
            // 读取远端版本
            // todo 按程序版本拼地址
			string serverVersionConfigPath = StringUtil.Format (
				StringFormat.TwoLevelPath,
                GameA.SocialApp.GetAppServerAddress().GameResoureRoot, ResDefine.ServerVersionConfigFileName
            );
            ServerVersionConfig serverVersionConfig = null;

			bool updateFinish = false;
			do
			{
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
	                    // 要求用户下载最新版本
	                    LogHelper.Info ("MinimumAppVersion: {0}", serverVersionConfig.MinimumAppVersion);
//						CommonDialogMessageData dialogData = new CommonDialogMessageData(
//							null,
//							LocalizationManager.GetText(ELocalText.AppVersionExpired),
//							false,
//							// ok btn
//							() =>
//							{
//								UnityTools.OpenURL(serverVersionConfig.LatestAppDownloadPath);
//							},
//							null,
//							null,
//							LocalizationManager.GetText(ELocalText.GotoDownloadLink),
//							null,
//							null,
//							false
//						);
//						Messenger<CommonDialogMessageData>.Broadcast(EMessengerType.ShowCommonDialog, dialogData);
						yield return new UnityEngine.WaitForSeconds(float.MaxValue);
	                }
	                if (applicationVersion < serverVersionConfig.LatestAppVersion)
	                {
						bool waitForUserInput = true;
	                    // 询问用户是否下载最新app
//						CommonDialogMessageData dialogData = new CommonDialogMessageData(
//							null,
//							LocalizationManager.GetText(ELocalText.NewAppVersionReleased),
//							false,
//							// ok btn
//							() =>
//							{
//								UnityTools.OpenURL(serverVersionConfig.LatestAppDownloadPath);
//								waitForUserInput = false;
//							},
//							() =>
//							{
//								waitForUserInput = false;
//							},
//							null,
//							LocalizationManager.GetText(ELocalText.GotoDownloadLink),
//							LocalizationManager.GetText(ELocalText.SkipUpdate),
//							null
//						);
//						Messenger<CommonDialogMessageData>.Broadcast(EMessengerType.ShowCommonDialog, dialogData);
						while (waitForUserInput) yield return null;
	                }
	                // 如果还没有解压资源
	                if (null == persistentManifest)
	                {
	                    CHDownloadingResManifest downloadingResManifest = null;
	                    if (buildInManifest.Ver == serverVersionConfig.LatestResVersion)
	                    {
	                        downloadingResManifest = new CHDownloadingResManifest (buildInManifest);
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
	                            downloadingResManifest = new CHDownloadingResManifest (buildInManifest);
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
	                        yield return downloadingResManifest.MergeExistingManifest (buildInManifest);
	                    }
	                    downloadingResManifest.CalculateFilesNeedsDownload();
	                    if (downloadingResManifest.NeedsDownloadTotalCnt == 0)
	                    {
                            Messenger<string>.Broadcast(EMessengerType.OnVersionUpdateStateChange, "正在解压资源...");
	                        yield return downloadingResManifest.DecompressOrCopyToPersistant ();
	                        manifestUpdated = true;
							updateFinish = true;
	                    }
	                    else
	                    {
                            Messenger<string>.Broadcast(EMessengerType.OnVersionUpdateStateChange, "正在下载资源...");
	                        // 进入下载流程
	                        LogHelper.Info ("Begin download, cnt: {0}, size: {1}", downloadingResManifest.NeedsDownloadTotalCnt, downloadingResManifest.NeedsDownloadTotalByte);
	                        yield return downloadingResManifest.DownloadServerAssetBundles ();
	                        if (downloadingResManifest.DownloadFailed)
	                        {
	                            LogHelper.Error ("Download failed");
								bool waitForRetry = true;
	                            // 提示用户下载必要的文件出错，等待用户确认重试
//								CommonDialogMessageData dialogData = new CommonDialogMessageData(
//									null,
//									LocalizationManager.GetText(ELocalText.DownloadResError),
//									false,
//									// ok btn
//									() =>
//									{
//										waitForRetry = false; 
//									},
//									null,
//									null,
//									LocalizationManager.GetText(ELocalText.OK),
//									null,
//									null
//								);
//								Messenger<CommonDialogMessageData>.Broadcast(EMessengerType.ShowCommonDialog, dialogData);
								while (waitForRetry) yield return null;
	                        }
	                        else if (downloadingResManifest.SerializeFailed)
	                        {
	                            LogHelper.Error ("Serialize failed");
								bool waitForRetry = true;
								// 提示用户序列化资源出错，等待用户确认重试
//								CommonDialogMessageData dialogData = new CommonDialogMessageData(
//									null,
//									LocalizationManager.GetText(ELocalText.SerializeResFailed),
//									false,
//									// retry btn
//									() =>
//									{
//										waitForRetry = false;
//									},
//									null,
//									null,
//									LocalizationManager.GetText(ELocalText.Retry),
//									null,
//									null
//								);
//								Messenger<CommonDialogMessageData>.Broadcast(EMessengerType.ShowCommonDialog, dialogData);
								while (waitForRetry) yield return null;
	                        }
	                        else
	                        {
	                            LogHelper.Info ("Download done, totalTime: {0}, downloadTime: {1}", downloadingResManifest.TotalTime, downloadingResManifest.DownloadTotalTime);
                                Messenger<string>.Broadcast(EMessengerType.OnVersionUpdateStateChange, "正在解压资源...");
	                            yield return downloadingResManifest.DecompressOrCopyToPersistant ();
								FileTools.DeleteFolder(StringUtil.Format(StringFormat.TwoLevelPath, ResPath.PersistentDataPath, ResPath.TempCache));
	                            manifestUpdated = true;
								updateFinish = true;
	                        }
	                    }
	                }
	                else
	                {
	                    if (persistentManifest.Ver < serverVersionConfig.LatestResVersion)
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
		                        if (persistentManifest.Ver < buildInManifest.Ver)
		                        {
			                        latestResManifest = new CHDownloadingResManifest(buildInManifest);
			                        yield return latestResManifest.MergeExistingManifest(persistentManifest);
			                        latestResManifest.CalculateFilesNeedsDownload ();
			                        if (latestResManifest.NeedsDownloadTotalCnt != 0)
			                        {
				                        // 提示用户需要下载必须的资源，请检查网络再重试
				                        updateFinish = true;
			                        }
			                        else
			                        {
				                        yield return latestResManifest.DecompressOrCopyToPersistant ();
				                        updateFinish = true;
			                        }
		                        }
		                        else
		                        {
									updateFinish = true;
		                        }
	                        }
	                        else
	                        {
	                            yield return latestResManifest.MergeTemporaryCacheFiles ();
	                            yield return latestResManifest.MergeExistingManifest (persistentManifest);
		                        yield return latestResManifest.MergeExistingManifest (buildInManifest);
	                            latestResManifest.CalculateFilesNeedsDownload ();
	                            if (latestResManifest.NeedsDownloadTotalCnt != 0)
	                            {
									int downloadConfirm = 0;
//									CommonDialogMessageData dialogData = new CommonDialogMessageData(
//										null,
//										StringUtil.Format(
//											LocalizationManager.GetText(ELocalText.ResNeedsUpdate),
//											latestResManifest.NeedsDownloadTotalCnt,
//											latestResManifest.NeedsDownloadTotalByte
//										),
//										false,
//										// ok btn
//										() =>
//										{
//											downloadConfirm = 1;
//										},
//										() =>
//										{
//											downloadConfirm = -1;
//										},
//										null,
//										LocalizationManager.GetText(ELocalText.OK),
//										LocalizationManager.GetText(ELocalText.Cancel),
//										null
//									);
//									Messenger<CommonDialogMessageData>.Broadcast(EMessengerType.ShowCommonDialog, dialogData);
									while (0 == downloadConfirm)
									{
										yield return null;
									}
									if (1 == downloadConfirm)
	                                {
                                        Messenger<string>.Broadcast(EMessengerType.OnVersionUpdateStateChange, "正在下载资源...");
	                                    yield return latestResManifest.DownloadServerAssetBundles ();
	                                    if (latestResManifest.DownloadFailed)
	                                    {
	                                        LogHelper.Error ("Download failed");
											updateFinish = true;
	                                    }
	                                    else if (latestResManifest.SerializeFailed)
	                                    {
	                                        LogHelper.Error ("Serialize failed");
											updateFinish = true;
	                                    }
	                                    else
	                                    {
	                                        LogHelper.Info ("Download done, totalTime: {0}, downloadTime: {1}", latestResManifest.TotalTime, latestResManifest.DownloadTotalTime);
                                            Messenger<string>.Broadcast(EMessengerType.OnVersionUpdateStateChange, "正在解压资源...");
	                                        yield return latestResManifest.DecompressOrCopyToPersistant ();
											FileTools.DeleteFolder(StringUtil.Format(StringFormat.TwoLevelPath, ResPath.PersistentDataPath, ResPath.TempCache));
	                                        manifestUpdated = true;
		                                    updateFinish = true;
	                                    }
	                                }
	                            }
	                            else
	                            {
		                            yield return latestResManifest.DecompressOrCopyToPersistant ();
		                            manifestUpdated = true;
		                            updateFinish = true;
	                            }
	                        }
	                    }
	                    else
	                    {
	                        // 清空临时资源
							FileTools.DeleteFolder(StringUtil.Format(StringFormat.TwoLevelPath, ResPath.PersistentDataPath, ResPath.TempCache));
							manifestUpdated = true;
							updateFinish = true;
	                    }
	                }
	            }
	            else
	            {
	                LogHelper.Info("Connect server failed.");
					// 如果还没有解压资源
					if (null == persistentManifest)
					{
						var downloadingResManifest = new CHDownloadingResManifest (buildInManifest);
						downloadingResManifest.CalculateFilesNeedsDownload();
						if (downloadingResManifest.NeedsDownloadTotalCnt == 0)
						{
							yield return downloadingResManifest.DecompressOrCopyToPersistant();
							manifestUpdated = true;
							updateFinish = true;
						}
						else
						{
							bool waitForRetry = true;
							// 提示用户必须联网下载资源，等待用户确认重试
//							CommonDialogMessageData dialogData = new CommonDialogMessageData(
//								null,
//								StringUtil.Format(
//									LocalizationManager.GetText(ELocalText.TryRecheckRes),
//									downloadingResManifest.NeedsDownloadTotalByte
//								),
//								false,
//								// retry btn
//								() =>
//								{
//									waitForRetry = false;
//								},
//								null,
//								null,
//								LocalizationManager.GetText(ELocalText.Retry),
//								null,
//								null
//							);
//							Messenger<CommonDialogMessageData>.Broadcast(EMessengerType.ShowCommonDialog, dialogData);
							while (waitForRetry) yield return null;
						}
					}
					else
					{
						if (persistentManifest.Ver < buildInManifest.Ver)
	                    {
		                    CHDownloadingResManifest latestResManifest = null;
		                    latestResManifest = new CHDownloadingResManifest(buildInManifest);
		                    yield return latestResManifest.MergeExistingManifest(persistentManifest);
		                    latestResManifest.CalculateFilesNeedsDownload ();
		                    if (latestResManifest.NeedsDownloadTotalCnt != 0)
		                    {
			                    // 提示用户需要下载必须的资源，请检查网络再重试
			                    updateFinish = true;
		                    }
		                    else
		                    {
			                    yield return latestResManifest.DecompressOrCopyToPersistant ();
			                    updateFinish = true;
		                    }
	                    }
	                    else
	                    {
							updateFinish = true;
	                    }
					}
	            }
				if (!updateFinish)
				{
                    Messenger<string>.Broadcast(EMessengerType.OnVersionUpdateStateChange, "正在检查资源...");
					yield return new UnityEngine.WaitForSeconds(1);
				}
			}
			while (!updateFinish);
            LogHelper.Info ("Res check finish");
            if (manifestUpdated)
            {
                ResourcesManager.Instance.Init ();
            }
			ResourcesManager.Instance.UnloadScenary(0);
            Messenger.Broadcast(EMessengerType.OnResourcesCheckFinish);
            SocialApp.Instance.LoginAfterUpdateResComplete ();
        }
    }
}