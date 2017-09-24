using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using SoyEngine;
using UnityEditor;
using UnityEngine;

namespace NewResourceSolution.EditorTool
{
    public static class AssetBundleBuilder
    {
        private static int _errorCnt;

        [MenuItem("JoyTools/TestMakeServerVersionConfig")]
        private static void TestMakeServerVersionConfig()
        {
            ServerVersionConfig config = new ServerVersionConfig();
            config.LatestAppVersion = new Version(1, 2, 3);
            config.MinimumAppVersion = new Version(0, 1, 0);
            config.LatestResVersion = new Version(0, 1, 0);
            config.LatestResManifestPath = "127.0.0.1:7888/Joyyou/Manifest";
            string str = JsonConvert.SerializeObject(config, Formatting.Indented,
                new VersionJsonConverter());
            BuildABConfig buildAbConfig =
                AssetDatabase.LoadAssetAtPath<BuildABConfig>(ABConstDefine.BuildABConfigAssetPath);
            string path = string.Format(StringFormat.ThreeLevelPath, Application.dataPath, buildAbConfig.OutputPath,
                "ServerVersionConfig");
            FileTools.WriteStringToFile(str, path);
        }

        [MenuItem("JoyTools/AssetBundleTools/CheckAssetDumplicate")]
        private static void CheckAssetDumplicate()
        {
            // load config
            BuildABConfig buildAbConfig =
                AssetDatabase.LoadAssetAtPath<BuildABConfig>(ABConstDefine.BuildABConfigAssetPath);
            if (null == buildAbConfig)
            {
                LogHelper.Error("Load buildABConfig asset failed.");
                return;
            }
            Version resVersion = new Version(buildAbConfig.Version);
            CHBuildingResManifest manifest = new CHBuildingResManifest(resVersion);

//			LogHelper.Info("Set asset bundle names...");
            SetAssetBundleNames(buildAbConfig, manifest);
//			LogHelper.Info("Set bundle name complete.");

            LogHelper.Info("CheckAssetDumplicate done");
        }

        [MenuItem("JoyTools/AssetBundleTools/BuildAllAB_Windows64")]
        private static void BuildAllAB_Windows()
        {
            BuildAllAb(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("JoyTools/AssetBundleTools/BuildAllAB_OSX64")]
        private static void BuildAllAB_OSX64()
        {
            BuildAllAb(BuildTarget.StandaloneOSXIntel64);
        }

        [MenuItem("JoyTools/AssetBundleTools/BuildAllAB_Android")]
        private static void BuildAllAB_Android()
        {
            BuildAllAb(BuildTarget.Android);
        }

        [MenuItem("JoyTools/AssetBundleTools/BuildAllAB_iOS")]
        private static void BuildAllAB_iOS()
        {
            BuildAllAb(BuildTarget.iOS);
        }

        private static void BuildAllAb(BuildTarget buildTarget)
        {
            _errorCnt = 0;
            // load config
            BuildABConfig buildAbConfig =
                AssetDatabase.LoadAssetAtPath<BuildABConfig>(ABConstDefine.BuildABConfigAssetPath);
            if (null == buildAbConfig)
            {
                LogHelper.Error("Load buildABConfig asset failed.");
                return;
            }
            Version resVersion = new Version(buildAbConfig.Version);
            CHBuildingResManifest manifest = new CHBuildingResManifest(resVersion);

            LogHelper.Info("Set asset bundle names...");
            SetAssetBundleNames(buildAbConfig, manifest);
            LogHelper.Info("Set bundle name complete.");

            string outputPathUnCompressed = ABUtility.GetUnCompressedOutputPath(buildAbConfig, buildTarget);
            DirectoryInfo outputDir = new DirectoryInfo(outputPathUnCompressed);
            if (outputDir.Exists)
            {
//				if (!EditorUtility.DisplayDialog
//					(
//						"Confirm clear path",
//						string.Format("Output path exist, sure to clear \"{0}\"", outputDir.FullName),
//						"OK",
//						"Cancel"
//					))
//				{
//					LogHelper.Info("Build progress terminated by user.");
//					return;
//				}
                outputDir.Delete(true);
            }
            outputDir.Create();
            if (!Directory.Exists(outputDir.FullName))
            {
                LogHelper.Error("Create output directory failed, path: {0}", outputDir);
                _errorCnt++;
            }

            // if error occurs, stop process
            if (_errorCnt > 0)
            {
                EditorUtility.DisplayDialog
                (
                    "Error occurs",
                    string.Format("We found {0} error(s) when prepare build assetbundle.", _errorCnt),
                    "OK"
                );
                return;
            }
            LogHelper.Info("Begin build assetbundle...");
            BuildPipeline.BuildAssetBundles(
                outputPathUnCompressed,
                BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.StrictMode,
                buildTarget
            );
            // rename unity manifest bundle
            FileTools.RenameFile(
                string.Format(StringFormat.TwoLevelPath, outputPathUnCompressed, ABConstDefine.OutputPathUnCompressed),
                string.Format(StringFormat.TwoLevelPath, outputPathUnCompressed, ResDefine.UnityManifestBundleName)
            );
            LogHelper.Info("Build assetbundle complete.");

            LogHelper.Info("Compress assetbundles...");
            CompressAssetBundles(buildAbConfig, manifest, buildTarget);
            LogHelper.Info("Compress done.");


            LogHelper.Info("Copy in package assetbundldes...");
            CopyAssetBundles(buildAbConfig, manifest, buildTarget);
            LogHelper.Info("Copy complete.");

            LogHelper.Info("Make manifest...");
            MakeManifest(buildAbConfig, manifest, buildTarget);
            LogHelper.Info("Make manifest done.");
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
			EditorUtility.DisplayDialog ("Done", "Build asset bundles complete", "OK");
            LogHelper.Info("Build asset bundles complete");
        }

        /// <summary>
        /// find all asset to build, set bundle names
        /// </summary>
        private static void SetAssetBundleNames(BuildABConfig buildAbConfig, CHBuildingResManifest manifest)
        {
            var allResList = buildAbConfig.AllResLists;
            for (int i = 0; i < allResList.Count; i++)
            {
                if (allResList[i].IsLocaleRes)
                {
                    foreach (int j in Enum.GetValues(typeof(ELocale)))
                    {
                        if (((1 << (j - 1)) & buildAbConfig.IncludeLocales) != 0)
                        {
                            ELocale targetLocale = (ELocale) j;

                            string rootPath =
                                ResPathUtility.GetEditorDebugResFolderPath(allResList[i].ResType, true, targetLocale);
                            if (AssetDatabase.IsValidFolder(rootPath))
                            {
                                LogHelper.Info("{0} rootPath: {1}", allResList[i].ResType, rootPath);
                                if (allResList[i].IsFolderRes)
                                {
                                    DirectoryInfo rootDirectoryInfo = new DirectoryInfo(rootPath);
                                    var childDirectorys = rootDirectoryInfo.GetDirectories();
                                    for (int k = 0; k < childDirectorys.Length; k++)
                                    {
                                        var parts = childDirectorys[k].ToString().Split(new[] {ResPath.Assets},
                                            StringSplitOptions.None);
                                        string childDirRelatedToUnityProject =
                                            string.Format(
                                                "{0}{1}",
                                                ResPath.Assets,
                                                parts[parts.Length - 1]
                                            );
                                        var assets = AssetDatabase.FindAssets(allResList[i].SearchFilter,
                                            new[] {childDirRelatedToUnityProject});
                                        SetBundleNameToFolderAssets(buildAbConfig, manifest, rootPath,
                                            childDirRelatedToUnityProject, assets);
                                    }
                                }
                                else
                                {
                                    var assets = AssetDatabase.FindAssets(allResList[i].SearchFilter, new[] {rootPath});
                                    for (int k = 0; k < assets.Length; k++)
                                    {
                                        SetBundleNameToSingleAsset(buildAbConfig, manifest, rootPath, assets[k]);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    string rootPath = ResPathUtility.GetEditorDebugResFolderPath(allResList[i].ResType);
                    if (AssetDatabase.IsValidFolder(rootPath))
                    {
                        LogHelper.Info("{0} rootPath: {1}", allResList[i].ResType, rootPath);
                        if (allResList[i].IsFolderRes)
                        {
                            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(rootPath);
                            var childDirectorys = rootDirectoryInfo.GetDirectories();
                            for (int k = 0; k < childDirectorys.Length; k++)
                            {
//                                LogHelper.Info ("Childe dir: {0}", childDirectorys[k]);
                                var parts = childDirectorys[k].ToString().Split(new[] {ResPath.Assets},
                                    StringSplitOptions.None);
                                string childDirRelatedToUnityProject =
                                    string.Format(
                                        "{0}{1}",
                                        ResPath.Assets,
                                        parts[parts.Length - 1]
                                    );
                                var assets = AssetDatabase.FindAssets(allResList[i].SearchFilter,
                                    new[] {childDirRelatedToUnityProject});
                                SetBundleNameToFolderAssets(buildAbConfig, manifest, rootPath,
                                    childDirRelatedToUnityProject, assets);
                                // 这段代码会造成增两打包时会重打全部图集，所以注掉，流程改为需要自动设置图片属性时手动调用菜单快捷方式
                                if (EResType.Sprite == allResList[i].ResType)
                                {
                                    SpriteAssetTools.SetAtlasSetting(rootPath, childDirRelatedToUnityProject, assets);
                                }
                            }
                        }
                        else
                        {
                            var assets = AssetDatabase.FindAssets(allResList[i].SearchFilter, new[] {rootPath});
                            for (int k = 0; k < assets.Length; k++)
                            {
                                SetBundleNameToSingleAsset(buildAbConfig, manifest, rootPath, assets[k]);
                            }
                        }
                    }
                    else
                    {
                        LogHelper.Error("{0} asset rootPath invalid, path: {1}", allResList[i].ResType, rootPath);
                        _errorCnt++;
                    }
                }
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        private static void SetBundleNameToSingleAsset(
            BuildABConfig buildAbConfig,
            CHBuildingResManifest manifest,
            string rootPath,
            string assetGuid)
        {
            string path = AssetDatabase.GUIDToAssetPath(assetGuid);
            AssetImporter ai = AssetImporter.GetAtPath(path);
            if (null != ai)
            {
                CHResBundle bundle = new CHResBundle();
                string bundleName;
                bundle.AssetNames = new[] {ABUtility.PathToAssetBundleName(rootPath, path, out bundleName)};
                bundle.AssetBundleName = bundleName;
                // todo 这里只区分是否随包发布的资源
                if (buildAbConfig.FullResPackage || buildAbConfig.IsGuidInPackageAsset(assetGuid))
                {
                    bundle.GroupId = ResDefine.ResGroupInPackage;
                }
                else
                {
                    bundle.GroupId = ResDefine.ResGroupNecessary;
                }
                if (buildAbConfig.SingleAdamResList.Contains(assetGuid))
                {
                    manifest.AdamBundleNameList.Add(bundleName);
                }
                if (!manifest.AddBundle(bundle))
                {
                    _errorCnt++;
                }
                ai.assetBundleName = bundleName;
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
            else
            {
                LogHelper.Error("Set asset bundle name failed, importer is null, path: {0}", path);
                _errorCnt++;
            }
        }

        private static void SetBundleNameToFolderAssets(
            BuildABConfig buildAbConfig,
            CHBuildingResManifest manifest,
            string rootPath,
            string childFolderPath,
            string[] assetsGuid)
        {
            string bundleName;
            ABUtility.PathToAssetBundleName(rootPath, childFolderPath, out bundleName);
            CHResBundle bundle = new CHResBundle();
            bundle.AssetBundleName = bundleName;
            string folderGuid = AssetDatabase.AssetPathToGUID(childFolderPath);
            // todo 这里只区分是否随包发布的资源
            if (buildAbConfig.FullResPackage || buildAbConfig.IsGuidInPackageAsset(folderGuid))
            {
                bundle.GroupId = ResDefine.ResGroupInPackage;
            }
            else
            {
                bundle.GroupId = ResDefine.ResGroupNecessary;
            }

            List<string> assetNameList = new List<string>();

            for (int i = 0; i < assetsGuid.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetsGuid[i]);
                AssetImporter ai = AssetImporter.GetAtPath(path);
                if (null != ai)
                {
                    string temp;
                    string assetName = ABUtility.PathToAssetBundleName(rootPath, path, out temp);
                    if (!assetNameList.Contains(assetName))
                        assetNameList.Add(assetName);
                    ai.assetBundleName = bundleName;
                    EditorUtility.UnloadUnusedAssetsImmediate();
                }
                else
                {
                    LogHelper.Error("Set asset bundle name failed, importer is null, path: {0}", path);
                    _errorCnt++;
                }
            }
            bundle.AssetNames = assetNameList.ToArray();
            if (buildAbConfig.FolderAdamResList.Contains(folderGuid))
            {
                manifest.AdamBundleNameList.Add(bundleName);
            }
            if (!manifest.AddBundle(bundle))
            {
                _errorCnt++;
            }
        }

        private static void CompressAssetBundles(BuildABConfig buildAbConfig, CHBuildingResManifest manifest,
            BuildTarget buildTarget)
        {
            string outputPathUnCompressed = ABUtility.GetUnCompressedOutputPath(buildAbConfig, buildTarget);
            string outputPathCompressed = ABUtility.GetOutputPath(buildAbConfig, buildTarget);

            string lastBuildManifestPath = string.Format(StringFormat.TwoLevelPath, outputPathCompressed,
                ResDefine.CHResManifestFileName);
            string lastBuildManifestStr;
            CHRuntimeResManifest lastBuildManifest = null;
            if (FileTools.TryReadFileToString(lastBuildManifestPath, out lastBuildManifestStr))
            {
                lastBuildManifest = JsonTools.DeserializeObject<CHRuntimeResManifest>(lastBuildManifestStr);
                lastBuildManifest.MapBundles();
            }
            EditorUtility.DisplayProgressBar("building res", "compress res", 0);

            int maxThreadNum = Mathf.Max(1, Environment.ProcessorCount - 1);
            int[] currentThreadNum = {0};
            // 之前的压缩文件，需要删除的列表
            List<CHResBundle> remainingOldCompressedBundleList = new List<CHResBundle>();
            var allBundles = manifest.Bundles;
            for (int i = 0; i < allBundles.Count; i++)
            {
                var bundle = allBundles[i];
                while (currentThreadNum[0] >= maxThreadNum)
                {
                    Thread.Sleep(10);
                }
                EditorUtility.DisplayProgressBar("building res", "compress res", 1f * i / allBundles.Count);
                Interlocked.Increment(ref currentThreadNum[0]);
                ThreadPool.QueueUserWorkItem(obj =>
                {
                    while (true)
                    {
                        string unCompressedAssetBundlePath = string.Format(StringFormat.TwoLevelPath,
                            outputPathUnCompressed,
                            bundle.AssetBundleName);
                        string rawMd5 = null;
                        try
                        {
                            FileTools.GetFileMd5(unCompressedAssetBundlePath, out rawMd5);
                        }
                        catch (Exception e)
                        {
                            LogHelper.Error("CompressAssetBundles Md5 Error, FilePath: {0}, Exception: {1}",
                                unCompressedAssetBundlePath, e);
                        }
                        bundle.RawMd5 = rawMd5;
    
                        // 判断是否可以用以前的压缩文件
                        if (null != lastBuildManifest)
                        {
                            CHResBundle oldBundle = lastBuildManifest.GetBundleByBundleName(bundle.AssetBundleName);
                            if (null != oldBundle)
                            {
                                // todo 判读该文件的压缩需求是否还和前次保持一致
                                if (String.CompareOrdinal(bundle.RawMd5, oldBundle.RawMd5) == 0)
                                {
                                    FileInfo oldCompressedFileInfo =
                                        new FileInfo(string.Format(
                                            StringFormat.TwoLevelPathWithExtention,
                                            outputPathCompressed,
                                            oldBundle.AssetBundleName,
                                            oldBundle.CompressedMd5
                                        ));
                                    if (oldCompressedFileInfo.Exists)
                                    {
                                        bundle.Size = oldBundle.Size;
                                        bundle.CompressedMd5 = oldBundle.CompressedMd5;
                                        bundle.CompressType = EAssetBundleCompressType.LZMA;
                                        remainingOldCompressedBundleList.Add(oldBundle);
                                        break;
                                    }
                                }
                            }
                        }
    
                        string compressedAssetBundlePath = string.Format(StringFormat.TwoLevelPath, outputPathCompressed,
                            bundle.AssetBundleName);
                        try
                        {
                            CompressTools.CompressFileLZMA(unCompressedAssetBundlePath, compressedAssetBundlePath);
                        }
                        catch (Exception e)
                        {
                            LogHelper.Error("CompressAssetBundles Compress Error, FilePath: {0}, Exception: {1}",
                                compressedAssetBundlePath, e);
                        }
                        FileInfo fi = new FileInfo(compressedAssetBundlePath);
                        if (fi.Exists)
                        {
                            string compressedMd5;
                            bundle.Size = FileTools.GetFileMd5(compressedAssetBundlePath, out compressedMd5);
                            bundle.CompressedMd5 = compressedMd5;
                            bundle.CompressType = EAssetBundleCompressType.LZMA;
                            string fileFullName = string.Format(StringFormat.TwoLevelPathWithExtention,
                                outputPathCompressed,
                                bundle.AssetBundleName, compressedMd5);
                            try
                            {
                                FileTools.RenameFile(compressedAssetBundlePath, fileFullName);
                            }
                            catch (Exception e)
                            {
                                LogHelper.Error("CompressAssetBundles Rename Error, FilePath: {0}, Exception: {1}",
                                    compressedAssetBundlePath, e);
                            }
                        }
                        else
                        {
                            LogHelper.Error("Compress assetbundle failed, bundle name is {0}.", bundle.AssetBundleName);
                        }
                        break;
                    }
                    Interlocked.Decrement(ref currentThreadNum[0]);
                }, null);
            }
            while (currentThreadNum[0] > 0)
            {
                Thread.Sleep(10);
            }
            // 清理存留的上次打包文件
            if (null != lastBuildManifest)
            {
                int deleteFileCnt = 0;
                for (int i = 0; i < lastBuildManifest.Bundles.Count; i++)
                {
                    if (remainingOldCompressedBundleList.Contains(lastBuildManifest.Bundles[i]))
                        continue;
                    string compressedFilePath = string.Format(StringFormat.TwoLevelPathWithExtention,
                        outputPathCompressed,
                        lastBuildManifest.Bundles[i].AssetBundleName,
                        lastBuildManifest.Bundles[i].CompressedMd5);
                    if (FileTools.DeleteFile(compressedFilePath))
                    {
                        deleteFileCnt++;
                    }
                }
                LogHelper.Info("{0} obsolete file deleted.", deleteFileCnt);
            }
            EditorUtility.ClearProgressBar();
        }

        private static void CopyAssetBundles(BuildABConfig buildAbConfig, CHBuildingResManifest manifest,
            BuildTarget buildTarget)
        {
            // clear streaming assets path
            if (AssetDatabase.IsValidFolder(ResPathUtility.GetStreamingAssetsPath()))
            {
                if (!FileUtil.DeleteFileOrDirectory(ResPathUtility.GetStreamingAssetsPath()))
                {
                    LogHelper.Error("Clear streaming assets path failed.");
                }
            }
            AssetDatabase.CreateFolder(ResPath.Assets, ResPath.StreamingAssets);
//            string outputPathCompressed = ABUtility.GetOutputPath(buildAbConfig, buildTarget);
            string outputPathUncompressed = ABUtility.GetUnCompressedOutputPath(buildAbConfig, buildTarget);
            var allBundles = manifest.Bundles;
            for (int i = 0; i < allBundles.Count; i++)
            {
                if (allBundles[i].GroupId == ResDefine.ResGroupInPackage)
                {
                    var abPath = string.Format(StringFormat.TwoLevelPath, outputPathUncompressed,
                        allBundles[i].AssetBundleName);
                    allBundles[i].CompressType = EAssetBundleCompressType.NoCompress;
                    var destPath = string.Format(StringFormat.TwoLevelPathWithExtention, ResPath.StreamingAssetsPath,
                        allBundles[i].AssetBundleName, allBundles[i].RawMd5);
                    FileTools.CopyFileSync(abPath, destPath);
                    allBundles[i].FileLocation = EFileLocation.StreamingAsset;
                }
                else
                {
                    allBundles[i].FileLocation = EFileLocation.Server;
                }
            }
        }

        private static void MakeManifest(BuildABConfig buildAbConfig, CHBuildingResManifest manifest,
            BuildTarget buildTarget)
        {
            string outputPathCompressed = ABUtility.GetOutputPath(buildAbConfig, buildTarget);
            string manifestInStreamingAssetsFolder = string.Format(StringFormat.TwoLevelPath,
                ResPath.StreamingAssetsPath, ResDefine.CHResManifestFileName);
            string manifestInCompressedAbFolder = string.Format(StringFormat.TwoLevelPath, outputPathCompressed,
                ResDefine.CHResManifestFileName);
            string str = JsonTools.SerializeObject(manifest, buildAbConfig.Debug);
            FileTools.WriteStringToFile(str, manifestInStreamingAssetsFolder);
            string manifestInStreamingAssetsFolderCopy = string.Format("{0}_buildin", manifestInCompressedAbFolder);
            FileTools.WriteStringToFile(str, manifestInStreamingAssetsFolderCopy);
            manifest.SwitchToResServerManifest();
            str = JsonTools.SerializeOnjectWithExcludedProperties(manifest, buildAbConfig.Debug, new[] {"FL"});
            FileTools.WriteStringToFile(str, manifestInCompressedAbFolder);
        }
    }
}