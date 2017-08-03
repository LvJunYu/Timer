using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SoyEngine;
using NewResourceSolution;

namespace NewResourceSolution.EditorTool
{
	public static class AssetBundleBuilder
	{
		private static int errorCnt;

		[MenuItem("JoyTools/TestMakeServerVersionConfig")]
		private static void TestMakeServerVersionConfig ()
		{
			ServerVersionConfig config = new ServerVersionConfig();
			config.LatestAppVersion = new System.Version(1, 2, 3);
			config.MinimumAppVersion = new System.Version(0, 1, 0);
			config.LatestResVersion = new System.Version(0, 1, 0);
            config.LatestResManifestPath = "127.0.0.1:7888/Joyyou/Manifest";
			string str = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented, new VersionJsonConverter());
            BuildABConfig buildABConfig = AssetDatabase.LoadAssetAtPath<BuildABConfig>(ABConstDefine.BuildABConfigAssetPath);
            string path = string.Format(StringFormat.ThreeLevelPath, Application.dataPath, buildABConfig.OutputPath, "ServerVersionConfig");
			FileTools.WriteStringToFile(str, path);
		}

		[MenuItem("JoyTools/AssetBundleTools/CheckAssetDumplicate")]
		private static void CheckAssetDumplicate()
		{
			// load config
			BuildABConfig buildABConfig = AssetDatabase.LoadAssetAtPath<BuildABConfig>(ABConstDefine.BuildABConfigAssetPath);
			if (null == buildABConfig)
			{
				LogHelper.Error("Load buildABConfig asset failed.");
				return;
			}
			System.Version resVersion = new System.Version(buildABConfig.Version);
			CHBuildingResManifest manifest = new CHBuildingResManifest(resVersion);

//			LogHelper.Info("Set asset bundle names...");
			SetAssetBundleNames(buildABConfig, manifest);
//			LogHelper.Info("Set bundle name complete.");
		}
			
        [MenuItem("JoyTools/AssetBundleTools/BuildAllAB_Windows64")]
        private static void BuildAllAB_Windows ()
        {
            BuildAllAB (BuildTarget.StandaloneWindows64);
        }
        [MenuItem("JoyTools/AssetBundleTools/BuildAllAB_OSX64")]
        private static void BuildAllAB_OSX64 ()
        {
            BuildAllAB (BuildTarget.StandaloneOSXIntel64);
        }
        [MenuItem("JoyTools/AssetBundleTools/BuildAllAB_Android")]
        private static void BuildAllAB_Android ()
        {
            BuildAllAB (BuildTarget.Android);
        }
        [MenuItem("JoyTools/AssetBundleTools/BuildAllAB_iOS")]
        private static void BuildAllAB_iOS ()
        {
            BuildAllAB (BuildTarget.iOS);
        }

        private static void BuildAllAB (BuildTarget buildTarget)
		{
			errorCnt = 0;
			// load config
			BuildABConfig buildABConfig = AssetDatabase.LoadAssetAtPath<BuildABConfig>(ABConstDefine.BuildABConfigAssetPath);
			if (null == buildABConfig)
			{
				LogHelper.Error("Load buildABConfig asset failed.");
				return;
			}
			System.Version resVersion = new System.Version(buildABConfig.Version);
			CHBuildingResManifest manifest = new CHBuildingResManifest(resVersion);

			LogHelper.Info("Set asset bundle names...");
			SetAssetBundleNames(buildABConfig, manifest);
			LogHelper.Info("Set bundle name complete.");

			string outputPathUnCompressed = ABUtility.GetUnCompressedOutputPath(buildABConfig, buildTarget);
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
				errorCnt++;
			}
				
			// if error occurs, stop process
			if (errorCnt > 0)
			{
				EditorUtility.DisplayDialog
				(
					"Error occurs",
					string.Format("We found {0} error(s) when prepare build assetbundle.", errorCnt),
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
			CompressAssetBundles(buildABConfig, manifest, buildTarget);
			LogHelper.Info("Compress done.");


			LogHelper.Info("Copy in package assetbundldes...");
			CopyAssetBundles(buildABConfig, manifest, buildTarget);
			LogHelper.Info("Copy complete.");

			LogHelper.Info("Make manifest...");
			MakeManifest (buildABConfig, manifest, buildTarget);
			LogHelper.Info("Make manifest done.");

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
//			EditorUtility.DisplayDialog ("Done", "Build asset bundles complete", "OK");
			LogHelper.Info("Build asset bundles complete");
		}

		/// <summary>
		/// find all asset to build, set bundle names
		/// </summary>
		private static void SetAssetBundleNames (BuildABConfig buildABConfig, CHBuildingResManifest manifest)
		{
            var allResList = buildABConfig.AllResLists;
            for (int i = 0; i < allResList.Count; i++)
            {
                if (allResList [i].IsLocaleRes)
                {
                    foreach (int j in System.Enum.GetValues(typeof(ELocale)))             
                    {
                        if (((1 << (j - 1)) & buildABConfig.IncludeLocales) != 0)
                        {
                            ELocale targetLocale = (ELocale)j;

                            string rootPath = ResPathUtility.GetEditorDebugResFolderPath(allResList[i].ResType, true, targetLocale);
                            if (AssetDatabase.IsValidFolder(rootPath))
                            {
                                LogHelper.Info("{0} rootPath: {1}", allResList[i].ResType, rootPath);
                                if (allResList [i].IsFolderRes)
                                {
                                    DirectoryInfo rootDirectoryInfo = new DirectoryInfo (rootPath);
                                    var childDirectorys = rootDirectoryInfo.GetDirectories ();
                                    for (int k = 0; k < childDirectorys.Length; k++)
                                    {
                                        var parts = childDirectorys [k].ToString ().Split (new[] {ResPath.Assets}, System.StringSplitOptions.None);
                                        string childDirRelatedToUnityProject = 
                                            string.Format (
                                                "{0}{1}",
                                                ResPath.Assets,
                                                parts [parts.Length - 1]
                                            );
                                        var assets = AssetDatabase.FindAssets(allResList[i].SearchFilter, new[] {childDirRelatedToUnityProject});
                                        SetBundleNameToFolderAssets (buildABConfig, manifest, rootPath, childDirRelatedToUnityProject, assets);
                                    }
                                }
                                else
                                {
                                    var assets = AssetDatabase.FindAssets(allResList[i].SearchFilter, new[] {rootPath});
                                    for (int k = 0; k < assets.Length; k++)
                                    {
                                        SetBundleNameToSingleAsset (buildABConfig, manifest, rootPath, assets[k]);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    string rootPath = ResPathUtility.GetEditorDebugResFolderPath(allResList[i].ResType, false);
                    if (AssetDatabase.IsValidFolder(rootPath))
                    {
                        LogHelper.Info("{0} rootPath: {1}", allResList[i].ResType, rootPath);
                        if (allResList [i].IsFolderRes)
                        {
                            DirectoryInfo rootDirectoryInfo = new DirectoryInfo (rootPath);
                            var childDirectorys = rootDirectoryInfo.GetDirectories ();
                            for (int k = 0; k < childDirectorys.Length; k++)
                            {
//                                LogHelper.Info ("Childe dir: {0}", childDirectorys[k]);
                                var parts = childDirectorys [k].ToString ().Split (new[] {ResPath.Assets}, System.StringSplitOptions.None);
                                string childDirRelatedToUnityProject = 
                                    string.Format (
                                        "{0}{1}",
                                        ResPath.Assets,
                                        parts [parts.Length - 1]
                                    );
                                var assets = AssetDatabase.FindAssets(allResList[i].SearchFilter, new[] {childDirRelatedToUnityProject});
                                SetBundleNameToFolderAssets (buildABConfig, manifest, rootPath, childDirRelatedToUnityProject, assets);
                            }
                        }
                        else
                        {
                            var assets = AssetDatabase.FindAssets(allResList[i].SearchFilter, new[] {rootPath});
                            for (int k = 0; k < assets.Length; k++)
                            {
                                SetBundleNameToSingleAsset (buildABConfig, manifest, rootPath, assets[k]);
                            }
                        }
                    }
                    else
                    {
                        LogHelper.Error("{0} asset rootPath invalid, path: {1}", allResList[i].ResType, rootPath);
                        errorCnt++;
                    }
                }
            }
			AssetDatabase.RemoveUnusedAssetBundleNames();
		}

        private static void SetBundleNameToSingleAsset (
            BuildABConfig buildABConfig,
            CHBuildingResManifest manifest,
            string rootPath,
            string assetGuid)
        {
            string path = AssetDatabase.GUIDToAssetPath(assetGuid);
            AssetImporter ai = AssetImporter.GetAtPath(path);
            if (null != ai)
            {
                CHResBundle bundle = new CHResBundle ();
                string bundleName;
                bundle.AssetNames = new string[] { ABUtility.PathToAssetBundleName (rootPath, path, out bundleName) };
                bundle.AssetBundleName = bundleName;
                // todo 这里只区分是否随包发布的资源
                if (buildABConfig.FullResPackage || buildABConfig.IsGuidInPackageAsset (assetGuid))
                {
                    bundle.GroupId = ResDefine.ResGroupInPackage;
                }
                else
                {
                    bundle.GroupId = ResDefine.ResGroupNecessary;
                }
                if (buildABConfig.SingleAdamResList.Contains (assetGuid))
                {
                    manifest.AdamBundleNameList.Add (bundleName);
                }
                if (!manifest.AddBundle (bundle))
                {
                    errorCnt++;
                }
                ai.assetBundleName = bundleName;
                EditorUtility.UnloadUnusedAssetsImmediate ();
            }
            else
            {
                LogHelper.Error ("Set asset bundle name failed, importer is null, path: {0}", path);
                errorCnt++;
            }
        }

        private static void SetBundleNameToFolderAssets (
            BuildABConfig buildABConfig,
            CHBuildingResManifest manifest,
            string rootPath,
            string childFolderPath,
            string[] assetsGuid)
        {
            string bundleName;
            ABUtility.PathToAssetBundleName (rootPath, childFolderPath, out bundleName);
            CHResBundle bundle = new CHResBundle ();
            bundle.AssetBundleName = bundleName;
			string folderGuid = AssetDatabase.AssetPathToGUID(childFolderPath);
            // todo 这里只区分是否随包发布的资源
            if (buildABConfig.FullResPackage || buildABConfig.IsGuidInPackageAsset (folderGuid))
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
                    string assetName = ABUtility.PathToAssetBundleName (rootPath, path, out temp);
                    if (!assetNameList.Contains(assetName))
                        assetNameList.Add (assetName);
                    ai.assetBundleName = bundleName;
                    EditorUtility.UnloadUnusedAssetsImmediate ();
                }
                else
                {
                    LogHelper.Error ("Set asset bundle name failed, importer is null, path: {0}", path);
                    errorCnt++;
                }
            }
            bundle.AssetNames = assetNameList.ToArray ();
            if (buildABConfig.FolderAdamResList.Contains (folderGuid))
            {
                manifest.AdamBundleNameList.Add (bundleName);
            }
            if (!manifest.AddBundle (bundle))
            {
                errorCnt++;
            }
        }

		private static void CompressAssetBundles (BuildABConfig buildABConfig, CHBuildingResManifest manifest, BuildTarget buildTarget)
		{
			string outputPathUnCompressed = ABUtility.GetUnCompressedOutputPath(buildABConfig, buildTarget);
			string outputPathCompressed = ABUtility.GetOutputPath(buildABConfig, buildTarget);

            string lastBuildManifestPath = string.Format (StringFormat.TwoLevelPath, outputPathCompressed, ResDefine.CHResManifestFileName);
            string lastBuildManifestStr;
            CHRuntimeResManifest lastBuildManifest = null;
            if (FileTools.TryReadFileToString (lastBuildManifestPath, out lastBuildManifestStr))
            {
                lastBuildManifest = JsonTools.DeserializeObject<CHRuntimeResManifest> (lastBuildManifestStr);
                lastBuildManifest.MapBundles ();
            }
            // 之前的压缩文件，需要删除的列表
            List<CHResBundle> remainingOldCompressedBundleList = new List<CHResBundle> ();
			var allBundles = manifest.Bundles;
			for (int i = 0; i < allBundles.Count; i++)
			{
                bool isUnityManifestBundle = string.Compare (allBundles [i].AssetBundleName, ResDefine.UnityManifestBundleName) == 0;
				string unCompressedAssetBundlePath = string.Format(StringFormat.TwoLevelPath, outputPathUnCompressed, allBundles[i].AssetBundleName);
                string rawMd5;
                long rawSize = FileTools.GetFileMd5(unCompressedAssetBundlePath, out rawMd5);
                allBundles [i].RawMd5 = rawMd5;

                // 判断是否可以用以前的压缩文件
                if (null != lastBuildManifest)
                {
                    CHResBundle oldBundle = lastBuildManifest.GetBundleByBundleName (allBundles [i].AssetBundleName);
                    if (null != oldBundle)
                    {
                        // todo 判读该文件的压缩需求是否还和前次保持一致
                        if (string.Compare (allBundles [i].RawMd5, oldBundle.RawMd5) == 0)
                        {
                            FileInfo oldCompressedFileInfo = 
                                new FileInfo (string.Format (
                                    StringFormat.TwoLevelPathWithExtention,
                                    outputPathCompressed,
                                    oldBundle.AssetBundleName,
                                    oldBundle.CompressedMd5
                                ));
                            if (oldCompressedFileInfo.Exists)
                            {
                                allBundles [i].Size = oldBundle.Size;
                                allBundles [i].CompressedMd5 = oldBundle.CompressedMd5;
                                allBundles [i].CompressType = isUnityManifestBundle ? EAssetBundleCompressType.NoCompress : EAssetBundleCompressType.LZMA;
                                remainingOldCompressedBundleList.Add (oldBundle);
                                continue;
                            }
                        }
                    }
                }

				string compressedAssetBundlePath = string.Format(StringFormat.TwoLevelPath, outputPathCompressed, allBundles[i].AssetBundleName);
                if (isUnityManifestBundle)
				{
                    allBundles[i].Size = rawSize;
                    allBundles[i].CompressedMd5 = rawMd5;
                    allBundles [i].CompressType = EAssetBundleCompressType.NoCompress;
					compressedAssetBundlePath = string.Format(StringFormat.TwoLevelPathWithExtention, outputPathCompressed, allBundles[i].AssetBundleName, rawMd5);
					FileTools.CopyFileSync(unCompressedAssetBundlePath, compressedAssetBundlePath);
					continue;
				}
                CompressTools.CompressFileLZMA(unCompressedAssetBundlePath, compressedAssetBundlePath);
				FileInfo fi = new FileInfo(compressedAssetBundlePath);
				if (fi.Exists)
				{
                    string compressedMd5;
					allBundles[i].Size = FileTools.GetFileMd5(compressedAssetBundlePath, out compressedMd5);
					allBundles[i].CompressedMd5 = compressedMd5;
					allBundles[i].CompressType = EAssetBundleCompressType.LZMA;
					string fileFullName = string.Format(StringFormat.TwoLevelPathWithExtention, outputPathCompressed, allBundles[i].AssetBundleName, compressedMd5);
					FileTools.RenameFile(compressedAssetBundlePath, fileFullName);
				}
				else
				{
					LogHelper.Error("Compress assetbundle failed, bundle name is {0}.", allBundles[i].AssetBundleName);
				}
			}
            // 清理存留的上次打包文件
            if (null != lastBuildManifest)
            {
                int deleteFileCnt = 0;
                for (int i = 0; i < lastBuildManifest.Bundles.Count; i++)
                {
                    if (remainingOldCompressedBundleList.Contains (lastBuildManifest.Bundles [i]))
                        continue;
                    string compressedFilePath = string.Format (StringFormat.TwoLevelPathWithExtention,
                                                          outputPathCompressed,
                                                          lastBuildManifest.Bundles [i].AssetBundleName,
                                                          lastBuildManifest.Bundles [i].CompressedMd5);
                    if (FileTools.DeleteFile (compressedFilePath))
                    {
                        deleteFileCnt++;
                    }
                }
                LogHelper.Info ("{0} obsolete file deleted.", deleteFileCnt);
            }
		}

		private static void CopyAssetBundles(BuildABConfig buildABConfig, CHBuildingResManifest manifest, BuildTarget buildTarget)
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
			string outputPathCompressed = ABUtility.GetOutputPath(buildABConfig, buildTarget);
			string outputPathUncompressed = ABUtility.GetUnCompressedOutputPath(buildABConfig, buildTarget);
			var allBundles = manifest.Bundles;
			for (int i = 0; i < allBundles.Count; i++)
			{
                bool isAdamBundle = manifest.AdamBundleNameList.Contains (allBundles [i].AssetBundleName);
                if (allBundles[i].GroupId == ResDefine.ResGroupInPackage || isAdamBundle)
				{
					string abPath = string.Format(StringFormat.TwoLevelPathWithExtention, outputPathCompressed, allBundles[i].AssetBundleName, allBundles[i].CompressedMd5);
					string destPath = string.Format(StringFormat.TwoLevelPathWithExtention, ResPath.StreamingAssetsPath, allBundles[i].AssetBundleName, allBundles[i].CompressedMd5);
                    if (isAdamBundle)
                    {
                        abPath = string.Format(StringFormat.TwoLevelPath, outputPathUncompressed, allBundles[i].AssetBundleName);
                    }

					FileTools.CopyFileSync(abPath, destPath);
					allBundles[i].FileLocation = EFileLocation.StreamingAsset;
				}
				else
				{
					allBundles[i].FileLocation = EFileLocation.Server;
				}
			}
		}

		private static void MakeManifest(BuildABConfig buildABConfig, CHBuildingResManifest manifest, BuildTarget buildTarget)
		{
			string outputPathCompressed = ABUtility.GetOutputPath(buildABConfig, buildTarget);
			string manifestInStreamingAssetsFolder = string.Format(StringFormat.TwoLevelPath, ResPath.StreamingAssetsPath, ResDefine.CHResManifestFileName);
			string manifestInCompressedABFolder = string.Format(StringFormat.TwoLevelPath, outputPathCompressed, ResDefine.CHResManifestFileName);
			string str = JsonTools.SerializeObject(manifest, buildABConfig.Debug);
			FileTools.WriteStringToFile(str, manifestInStreamingAssetsFolder);
            string manifestInStreamingAssetsFolderCopy = string.Format ("{0}_buildin", manifestInCompressedABFolder);
            FileTools.WriteStringToFile(str, manifestInStreamingAssetsFolderCopy);
            manifest.SwitchToResServerManifest ();
            str = JsonTools.SerializeOnjectWithExcludedProperties(manifest, buildABConfig.Debug, new string[] {"FL"});
			FileTools.WriteStringToFile(str, manifestInCompressedABFolder);
		}
	}
}