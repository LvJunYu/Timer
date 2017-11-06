using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using SoyEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NewResourceSolution.EditorTool
{
    public static class AssetBundleBuilder
    {
        private static int _errorCnt;

        [MenuItem("JoyTools/MakeServerVersionConfig/Windows32")]
        private static void MakeServerVersionConfigWin()
        {
            WriteServerVersionConfig(BuildTarget.StandaloneWindows);
        }

        [MenuItem("JoyTools/MakeServerVersionConfig/OSX64")]
        private static void MakeServerVersionConfigOSX64()
        {
            WriteServerVersionConfig(BuildTarget.StandaloneOSXIntel64);
        }

        [MenuItem("JoyTools/MakeServerVersionConfig/iOS")]
        private static void MakeServerVersionConfigiOS()
        {
            WriteServerVersionConfig(BuildTarget.iOS);
        }

        [MenuItem("JoyTools/MakeServerVersionConfig/Android")]
        private static void MakeServerVersionConfigAndroid()
        {
            WriteServerVersionConfig(BuildTarget.Android);
        }

        private static void WriteServerVersionConfig(BuildTarget buildTarget)
        {
            BuildConfig buildConfig = AssetDatabase.LoadAssetAtPath<BuildConfig>(
                string.Format(ABConstDefine.BuildABConfigAssetPathFormat,
                    ABUtility.GetPlatformFolderName(buildTarget)));
            var outputRootPath = ABUtility.GetOutputPath(buildTarget);
            ServerVersionConfig config = new ServerVersionConfig();
            config.LatestAppVersion = new Version(buildConfig.AppVersion);
            config.MinimumAppVersion = new Version(buildConfig.MinimumAppVersion);
            config.LatestResVersion = new Version(buildConfig.ResVersion);
            FileTools.CheckAndCreateFolder(outputRootPath);
            string str = JsonConvert.SerializeObject(config, Formatting.Indented, new VersionJsonConverter());
            string path = string.Format(StringFormat.TwoLevelPath, outputRootPath, "ServerVersionConfig");
            FileTools.WriteStringToFile(str, path);
            PlayerSettings.bundleVersion = buildConfig.AppVersion;
            var scene = EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
            var ary = Resources.FindObjectsOfTypeAll<RuntimeConfig>();
            if (ary != null && ary.Length > 0)
            {
                ary[0].Version = buildConfig.AppVersion;
                EditorSceneManager.SaveScene(scene);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            LogHelper.Info("MakeServerVersionConfig Success");
        }

        [MenuItem("JoyTools/AssetBundleTools/CheckAssetDumplicate")]
        private static void CheckAssetDumplicate()
        {
            // load config
            BuildConfig buildConfig = AssetDatabase.LoadAssetAtPath<BuildConfig>(
                string.Format(ABConstDefine.BuildABConfigAssetPathFormat, ResPath.GetPlatformFolder()));
            if (null == buildConfig)
            {
                LogHelper.Error("Load buildABConfig asset failed.");
                return;
            }
            Version resVersion = new Version(buildConfig.ResVersion);
            CHBuildingResManifest manifest = new CHBuildingResManifest(resVersion);

//			LogHelper.Info("Set asset bundle names...");
            SetAssetBundleNames(buildConfig, manifest);
//			LogHelper.Info("Set bundle name complete.");
            LogHelper.Info("CheckAssetDumplicate done");
        }

        [MenuItem("JoyTools/AssetBundleTools/BuildAllAB_Windows32")]
        private static void BuildAllAB_Windows()
        {
            BuildAllAb(BuildTarget.StandaloneWindows);
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
            BuildConfig buildConfig = AssetDatabase.LoadAssetAtPath<BuildConfig>(
                string.Format(ABConstDefine.BuildABConfigAssetPathFormat,
                    ABUtility.GetPlatformFolderName(buildTarget)));
            if (null == buildConfig)
            {
                LogHelper.Error("Load buildABConfig asset failed.");
                return;
            }
            Version resVersion = new Version(buildConfig.ResVersion);
            CHBuildingResManifest manifest = new CHBuildingResManifest(resVersion);

            LogHelper.Info("Set asset bundle names...");
            SetAssetBundleNames(buildConfig, manifest);
            LogHelper.Info("Set bundle name complete.");

            string outputPathUnCompressed = ABUtility.GetUnCompressedOutputPath(buildTarget);
            FileTools.CheckAndCreateFolder(outputPathUnCompressed);
            if (!Directory.Exists(outputPathUnCompressed))
            {
                LogHelper.Error("Create output directory failed, path: {0}", outputPathUnCompressed);
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
                BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.StrictMode,
                buildTarget
            );
//            // rename unity manifest bundle
//            FileTools.RenameFile(
//                string.Format(StringFormat.TwoLevelPath, outputPathUnCompressed, ABConstDefine.OutputPathUnCompressed),
//                string.Format(StringFormat.TwoLevelPath, outputPathUnCompressed, ResDefine.UnityManifestBundleName)
//            );
            LogHelper.Info("Build assetbundle complete.");

            LogHelper.Info("Compress assetbundles...");
            CompressAssetBundles(buildConfig, manifest, buildTarget);
            LogHelper.Info("Compress done.");


            LogHelper.Info("Copy in package assetbundldes...");
            CopyAssetBundles(buildConfig, manifest, buildTarget);
            LogHelper.Info("Copy complete.");

            LogHelper.Info("Make manifest...");
            MakeManifest(buildConfig, manifest, buildTarget);
            LogHelper.Info("Make manifest done.");

            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Done", "Build asset bundles complete", "OK");
            LogHelper.Info("Build asset bundles complete");
        }

        /// <summary>
        /// find all asset to build, set bundle names
        /// </summary>
        private static void SetAssetBundleNames(BuildConfig buildConfig, CHBuildingResManifest manifest)
        {
            EditorUtility.DisplayProgressBar("SetAssetBundleNames", String.Empty, 0);
            var allResList = buildConfig.AllResLists;
            for (int i = 0; i < allResList.Count; i++)
            {
                var resList = allResList[i];
                if (resList.IsLocaleRes)
                {
                    foreach (int j in Enum.GetValues(typeof(ELocale)))
                    {
                        if (((1 << (j - 1)) & buildConfig.IncludeLocales) != 0)
                        {
                            ELocale targetLocale = (ELocale) j;

                            string rootPath =
                                ResPathUtility.GetEditorDebugResFolderPath(resList.ResType, true, targetLocale);
                            if (AssetDatabase.IsValidFolder(rootPath))
                            {
                                LogHelper.Info("{0} rootPath: {1}", resList.ResType, rootPath);
                                if (resList.IsFolderRes)
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
                                        EditorUtility.DisplayProgressBar("SetAssetBundleNames " + resList.ResType,
                                            "" + ((ELocale) j).ToString() + "@" + childDirRelatedToUnityProject,
                                            1f * k / childDirectorys.Length);
                                        var assets = AssetDatabase.FindAssets(resList.SearchFilter,
                                            new[] {childDirRelatedToUnityProject});
                                        SetBundleNameToFolderAssets(buildConfig, manifest, resList.ResType,
                                            resList.SeparateTexture, rootPath, childDirRelatedToUnityProject, assets);
                                    }
                                }
                                else
                                {
                                    var assets = AssetDatabase.FindAssets(resList.SearchFilter, new[] {rootPath});
                                    for (int k = 0; k < assets.Length; k++)
                                    {
                                        EditorUtility.DisplayProgressBar("SetAssetBundleNames " + resList.ResType,
                                            assets[k], 1f * k / assets.Length);
                                        SetBundleNameToSingleAsset(buildConfig, manifest, resList.ResType, rootPath,
                                            assets[k]);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    string rootPath = ResPathUtility.GetEditorDebugResFolderPath(resList.ResType);
                    if (AssetDatabase.IsValidFolder(rootPath))
                    {
                        LogHelper.Info("{0} rootPath: {1}", resList.ResType, rootPath);
                        if (resList.IsFolderRes)
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
                                EditorUtility.DisplayProgressBar("SetAssetBundleNames " + resList.ResType,
                                    childDirRelatedToUnityProject, 1f * k / childDirectorys.Length);
                                var assets = AssetDatabase.FindAssets(resList.SearchFilter,
                                    new[] {childDirRelatedToUnityProject});
                                SetBundleNameToFolderAssets(buildConfig, manifest, resList.ResType,
                                    resList.SeparateTexture,
                                    rootPath, childDirRelatedToUnityProject, assets);
                                // 这段代码会造成增两打包时会重打全部图集，所以注掉，流程改为需要自动设置图片属性时手动调用菜单快捷方式
                                if (EResType.Sprite == resList.ResType)
                                {
                                    SpriteAssetTools.SetAtlasSetting(rootPath, childDirRelatedToUnityProject, assets);
                                }
                            }
                        }
                        else
                        {
                            var assets = AssetDatabase.FindAssets(resList.SearchFilter, new[] {rootPath});
                            for (int k = 0; k < assets.Length; k++)
                            {
                                EditorUtility.DisplayProgressBar("SetAssetBundleNames " + resList.ResType,
                                    assets[k], 1f * k / assets.Length);
                                SetBundleNameToSingleAsset(buildConfig, manifest, resList.ResType, rootPath, assets[k]);
                            }
                        }
                    }
                    else
                    {
                        LogHelper.Error("{0} asset rootPath invalid, path: {1}", resList.ResType, rootPath);
                        _errorCnt++;
                    }
                }
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            EditorUtility.ClearProgressBar();
        }

        private static void SetBundleNameToSingleAsset(
            BuildConfig buildConfig,
            CHBuildingResManifest manifest,
            EResType resType,
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
                bundle.ResType = resType;
                // todo 这里只区分是否随包发布的资源
                if (buildConfig.FullResPackage || buildConfig.IsGuidInPackageAsset(assetGuid))
                {
                    bundle.GroupId = ResDefine.ResGroupInPackage;
                }
                else
                {
                    bundle.GroupId = ResDefine.ResGroupNecessary;
                }
                if (buildConfig.SingleAdamResList.Contains(assetGuid))
                {
                    manifest.AdamBundleNameList.Add(bundleName);
                }
                if (!manifest.AddBundle(bundle))
                {
                    _errorCnt++;
                }
                if (ai.assetBundleName != bundleName)
                {
                    ai.assetBundleName = bundleName;
                }
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
            else
            {
                LogHelper.Error("Set asset bundle name failed, importer is null, path: {0}", path);
                _errorCnt++;
            }
        }

        private static void SetBundleNameToFolderAssets(
            BuildConfig buildConfig,
            CHBuildingResManifest manifest,
            EResType resType,
            bool separateTexture,
            string rootPath,
            string childFolderPath,
            string[] assetsGuid)
        {
            string bundleName;
            ABUtility.PathToAssetBundleName(rootPath, childFolderPath, out bundleName);
            CHResBundle bundle = new CHResBundle();
            bundle.AssetBundleName = bundleName;
            bundle.ResType = resType;
            string folderGuid = AssetDatabase.AssetPathToGUID(childFolderPath);
            // todo 这里只区分是否随包发布的资源
            if (buildConfig.FullResPackage || buildConfig.IsGuidInPackageAsset(folderGuid))
            {
                bundle.GroupId = ResDefine.ResGroupInPackage;
            }
            else
            {
                bundle.GroupId = ResDefine.ResGroupNecessary;
            }
            HashSet<string> usedTexturePathSet = null;
            if (separateTexture)
            {//把文件夹中材质中使用的同一文件夹中的纹理分离打包
                var textureGuidAry = AssetDatabase.FindAssets("t:Texture", new [] {childFolderPath});
                var texturePathSet = new HashSet<string>();
                for (int i = 0; i < textureGuidAry.Length; i++)
                {
                    texturePathSet.Add(AssetDatabase.GUIDToAssetPath(textureGuidAry[i]));
                }
                usedTexturePathSet = new HashSet<string>();
                var dict = new Dictionary<string, List<CHResBundle.MaterialTextureProperty>>();
                var matGuidAry = AssetDatabase.FindAssets("t:Material", new[] {childFolderPath});
                for (int i = 0; i < matGuidAry.Length; i++)
                {
                    var matPath = AssetDatabase.GUIDToAssetPath(matGuidAry[i]);
                    string tempBundleName;
                    string matAssetName = ABUtility.PathToAssetBundleName(rootPath, matPath, out tempBundleName);
                    var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                    Shader shader = mat.shader;
                    for (int j = 0; j < ShaderUtil.GetPropertyCount(shader); ++j)  
                    {  
                        if (ShaderUtil.GetPropertyType(shader, j) == ShaderUtil.ShaderPropertyType.TexEnv)  
                        {  
                            string propertyName = ShaderUtil.GetPropertyName(shader, j);
                            Texture tex = mat.GetTexture(propertyName);  
                            string texPath = AssetDatabase.GetAssetPath(tex.GetInstanceID());
                            if (texturePathSet.Contains(texPath))
                            {
                                string texAssetBundleName;
                                var texAssetName = ABUtility.PathToAssetBundleName(rootPath, texPath, out texAssetBundleName);
                                if (!usedTexturePathSet.Contains(texPath))
                                {
                                    AssetImporter assetImporter = AssetImporter.GetAtPath(texPath);
                                    assetImporter.assetBundleName = texAssetBundleName;
                                    usedTexturePathSet.Add(texPath);
                                    
                                    CHResBundle texBundle = new CHResBundle();
                                    texBundle.AssetBundleName = texAssetBundleName;
                                    texBundle.ResType = EResType.Texture;
                                    var texGuid = AssetDatabase.AssetPathToGUID(texPath);
                                    // todo 这里只区分是否随包发布的资源
                                    if (buildConfig.FullResPackage || buildConfig.IsGuidInPackageAsset(texGuid))
                                    {
                                        texBundle.GroupId = ResDefine.ResGroupInPackage;
                                    }
                                    else
                                    {
                                        texBundle.GroupId = ResDefine.ResGroupNecessary;
                                    }
                                    texBundle.AssetNames = new [] {texAssetName};
                                    if (buildConfig.SingleAdamResList.Contains(texGuid))
                                    {
                                        manifest.AdamBundleNameList.Add(texAssetBundleName);
                                    }
                                    if (!manifest.AddBundle(texBundle))
                                    {
                                        _errorCnt++;
                                    }
                                }
                                List<CHResBundle.MaterialTextureProperty> materialTextureProperties;
                                if (!dict.TryGetValue(matAssetName, out materialTextureProperties))
                                {
                                    materialTextureProperties = new List<CHResBundle.MaterialTextureProperty>();
                                    dict.Add(matAssetName, materialTextureProperties);
                                }
                                materialTextureProperties.Add(new CHResBundle.MaterialTextureProperty()
                                {
                                    PropertyName = propertyName,
                                    TextureName = texAssetName
                                });
                            }
                        }
                    }
                }
                var mdAry = new CHResBundle.MaterialDependency[dict.Count];
                int inx = 0;
                foreach (var entry in dict)
                {
                    mdAry[inx++] = new CHResBundle.MaterialDependency()
                    {
                        MaterialName = entry.Key,
                        TextureProperties = entry.Value.ToArray()
                    };
                }
                bundle.MaterialDependencies = mdAry;
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
            

            HashSet<string> assetNameSet = new HashSet<string>();

            for (int i = 0; i < assetsGuid.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetsGuid[i]);
                if (usedTexturePathSet != null && usedTexturePathSet.Contains(path))
                {
                    continue;
                }
                AssetImporter ai = AssetImporter.GetAtPath(path);
                if (null != ai)
                {
                    string temp;
                    string assetName = ABUtility.PathToAssetBundleName(rootPath, path, out temp);
                    if (!assetNameSet.Contains(assetName))
                        assetNameSet.Add(assetName);

                    if (ai.assetBundleName != bundleName)
                    {
                        ai.assetBundleName = bundleName;
                    }
                    EditorUtility.UnloadUnusedAssetsImmediate();
                }
                else
                {
                    LogHelper.Error("Set asset bundle name failed, importer is null, path: {0}", path);
                    _errorCnt++;
                }
            }
            bundle.AssetNames = assetNameSet.ToArray();
            if (buildConfig.FolderAdamResList.Contains(folderGuid))
            {
                manifest.AdamBundleNameList.Add(bundleName);
            }
            if (!manifest.AddBundle(bundle))
            {
                _errorCnt++;
            }
        }

        private static void CompressAssetBundles(BuildConfig buildConfig, CHBuildingResManifest manifest,
            BuildTarget buildTarget)
        {
            string outputPathUnCompressed = ABUtility.GetUnCompressedOutputPath(buildTarget);
            string outputPathCompressed = string.Format(StringFormat.TwoLevelPath, ABUtility.GetOutputPath(buildTarget),
                buildConfig.ResVersion);
            string baseResPathCompressed = string.Format(StringFormat.TwoLevelPath,
                ABUtility.GetOutputPath(buildTarget), buildConfig.BaseResVersion);
            string lastBuildManifestPath = string.Format(StringFormat.TwoLevelPath, baseResPathCompressed,
                ResDefine.CHResManifestFileName);
            FileTools.CheckAndCreateFolder(outputPathCompressed);
            string baseBuildManifestStr;
            CHRuntimeResManifest baseBuildManifest = null;
            if (FileTools.TryReadFileToString(lastBuildManifestPath, out baseBuildManifestStr))
            {
                baseBuildManifest = JsonTools.DeserializeObject<CHRuntimeResManifest>(baseBuildManifestStr);
                baseBuildManifest.MapBundles();
            }
            EditorUtility.DisplayProgressBar("building res", "compress res", 0);

            int maxThreadNum = Mathf.Max(1, Environment.ProcessorCount - 1);
            int[] currentThreadNum = {0};
            // 之前的压缩文件，需要删除的列表
            bool outputToBasePath = buildConfig.BaseResVersion == buildConfig.ResVersion;
            HashSet<CHResBundle> invalidCompressFileSet = new HashSet<CHResBundle>();
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
                        if (null != baseBuildManifest)
                        {
                            CHResBundle oldBundle = baseBuildManifest.GetBundleByBundleName(bundle.AssetBundleName);
                            if (null != oldBundle)
                            {
                                // todo 判读该文件的压缩需求是否还和前次保持一致
                                if (String.CompareOrdinal(bundle.RawMd5, oldBundle.RawMd5) == 0)
                                {
                                    string oldCompressFilePath = string.Format(
                                        StringFormat.TwoLevelPathWithExtention,
                                        baseResPathCompressed,
                                        oldBundle.AssetBundleName,
                                        oldBundle.CompressedMd5
                                    );
                                    if (File.Exists(oldCompressFilePath))
                                    {
                                        bundle.Size = oldBundle.Size;
                                        bundle.CompressedMd5 = oldBundle.CompressedMd5;
                                        bundle.CompressType = EAssetBundleCompressType.LZMA;
                                        if (outputToBasePath)
                                        {
                                            lock (invalidCompressFileSet)
                                            {
                                                invalidCompressFileSet.Add(oldBundle);
                                            }
                                        }
                                        else
                                        {
                                            string fileFullName = string.Format(StringFormat.TwoLevelPathWithExtention,
                                                outputPathCompressed,
                                                bundle.AssetBundleName, bundle.CompressedMd5);
                                            FileTools.CopyFileSync(oldCompressFilePath, fileFullName);
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                        string compressedAssetBundlePath = string.Format(StringFormat.TwoLevelPath,
                            outputPathCompressed, bundle.AssetBundleName);
                        try
                        {
                            CompressTools.CompressFileLZMA(unCompressedAssetBundlePath, compressedAssetBundlePath);
                        }
                        catch (Exception e)
                        {
                            LogHelper.Error("CompressAssetBundles Compress Error, FilePath: {0}, Exception: {1}",
                                compressedAssetBundlePath, e);
                        }
                        if (File.Exists(compressedAssetBundlePath))
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
            if (null != baseBuildManifest && outputToBasePath)
            {
                int deleteFileCnt = 0;
                for (int i = 0; i < baseBuildManifest.Bundles.Count; i++)
                {
                    if (invalidCompressFileSet.Contains(baseBuildManifest.Bundles[i]))
                        continue;
                    string compressedFilePath = string.Format(StringFormat.TwoLevelPathWithExtention,
                        baseResPathCompressed,
                        baseBuildManifest.Bundles[i].AssetBundleName,
                        baseBuildManifest.Bundles[i].CompressedMd5);
                    if (FileTools.DeleteFile(compressedFilePath))
                    {
                        deleteFileCnt++;
                    }
                }
                LogHelper.Info("{0} obsolete file deleted.", deleteFileCnt);
            }
            EditorUtility.ClearProgressBar();
        }

        private static void CopyAssetBundles(BuildConfig buildConfig, CHBuildingResManifest manifest,
            BuildTarget buildTarget)
        {
            var buildStreamingAssets = ABUtility.GetBuildOutputStreamingAssetsPath(buildTarget);
            // clear streaming assets path
            if (Directory.Exists(buildStreamingAssets))
            {
                if (!FileUtil.DeleteFileOrDirectory(buildStreamingAssets))
                {
                    LogHelper.Error("Clear streaming assets path failed.");
                }
            }
            FileTools.CheckAndCreateFolder(buildStreamingAssets);
            string outputPathCompressed = string.Format(StringFormat.TwoLevelPath, ABUtility.GetOutputPath(buildTarget),
                buildConfig.ResVersion);
            string outputPathUncompressed = ABUtility.GetUnCompressedOutputPath(buildTarget);
            var allBundles = manifest.Bundles;
            for (int i = 0; i < allBundles.Count; i++)
            {
                var bundle = allBundles[i];
                bool isAdamBundle = manifest.AdamBundleNameList.Contains(bundle.AssetBundleName);
                bool isUnityManifestBundle =
                    String.CompareOrdinal(bundle.AssetBundleName, ResDefine.UnityManifestBundleName) == 0;
                if (bundle.GroupId == ResDefine.ResGroupInPackage || isAdamBundle || isUnityManifestBundle)
                {
                    string abPath;
                    string destPath;
                    if (!NeedStreamingAssetsCompress(isAdamBundle, isUnityManifestBundle, buildTarget))
                    {
                        abPath = string.Format(StringFormat.TwoLevelPath, outputPathUncompressed,
                            bundle.AssetBundleName);
                        bundle.CompressType = EAssetBundleCompressType.NoCompress;
                        destPath = string.Format(StringFormat.TwoLevelPathWithExtention, buildStreamingAssets,
                            bundle.AssetBundleName, bundle.RawMd5);
                    }
                    else
                    {
                        abPath = string.Format(StringFormat.TwoLevelPathWithExtention, outputPathCompressed,
                            bundle.AssetBundleName, bundle.CompressedMd5);
                        destPath = string.Format(StringFormat.TwoLevelPathWithExtention, buildStreamingAssets,
                            bundle.AssetBundleName, bundle.CompressedMd5);
                    }

                    FileTools.CopyFileSync(abPath, destPath);
                    bundle.FileLocation = EFileLocation.StreamingAsset;
                }
                else
                {
                    bundle.FileLocation = EFileLocation.Server;
                }
            }
        }

        private static void MakeManifest(BuildConfig buildConfig, CHBuildingResManifest manifest,
            BuildTarget buildTarget)
        {
            string outputPathCompressed = string.Format(StringFormat.TwoLevelPath, ABUtility.GetOutputPath(buildTarget),
                buildConfig.ResVersion);
            string manifestInStreamingAssetsFolder =
                string.Format(StringFormat.TwoLevelPath, ABUtility.GetBuildOutputStreamingAssetsPath(buildTarget),
                    ResDefine.CHResManifestFileName);
            string manifestInCompressedAbFolder = string.Format(StringFormat.TwoLevelPath, outputPathCompressed,
                ResDefine.CHResManifestFileName);
            string str = JsonTools.SerializeObject(manifest, buildConfig.Debug);
            FileTools.WriteStringToFile(str, manifestInStreamingAssetsFolder);
            string manifestInStreamingAssetsFolderCopy = string.Format("{0}_buildin", manifestInCompressedAbFolder);
            FileTools.WriteStringToFile(str, manifestInStreamingAssetsFolderCopy);
            manifest.SwitchToResServerManifest();
            str = JsonTools.SerializeOnjectWithExcludedProperties(manifest, buildConfig.Debug, new[] {"FL"});
            FileTools.WriteStringToFile(str, manifestInCompressedAbFolder);
        }

        private static bool NeedStreamingAssetsCompress(bool isAdam, bool isUnityManifestBundle,
            BuildTarget buildTarget)
        {
            if (buildTarget != BuildTarget.Android)
            {
                return false;
            }
            if (isAdam)
            {
                return false;
            }
            if (isUnityManifestBundle)
            {
                return false;
            }
            return true;
        }
    }
}