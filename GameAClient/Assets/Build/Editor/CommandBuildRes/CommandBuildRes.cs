using System;
using System.IO;
using NewResourceSolution.EditorTool;
using SoyEngine;
using UnityEditor;

public class CommandBuildRes
{
//    [MenuItem("QuanTools/BuildRes")]
    public static void BuildRes()
    {
        var args = Environment.GetCommandLineArgs();
        BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        
        var inx = Array.FindIndex(args, s => s == "-bTarget");
        if (inx >= 0 && args.Length > inx+1)
        {
            var str = args[inx + 1];
            try
            {
                buildTarget = (BuildTarget) Enum.Parse(typeof(BuildTarget), str);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        string lastVersion = GetLastVersion(buildTarget);
        string resVersion = lastVersion;
        inx = Array.FindIndex(args, s => s == "-resVersion");
        if (inx >= 0  && args.Length > inx+1)
        {
            var str = args[inx + 1];
            if (!string.IsNullOrEmpty(str) && str!="0")
            {
                try
                {
                    new Version(str);
                    resVersion = str;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        if (string.IsNullOrEmpty(resVersion))
        {
            resVersion = "0.0.0.1";
        }

        if (string.IsNullOrEmpty(lastVersion))
        {
            lastVersion = "0";
        }
        
        BuildRes(buildTarget, lastVersion, resVersion);
    }
    
    public static void BuildRes(BuildTarget buildTarget, string baseVersion, string version)
    {
        BuildConfig buildConfig = AssetDatabase.LoadAssetAtPath<BuildConfig>(
            string.Format(ABConstDefine.BuildABConfigAssetPathFormat,
                ABUtility.GetPlatformFolderName(buildTarget)));
        if (null == buildConfig)
        {
            LogHelper.Error("Load buildABConfig asset failed.");
            return;
        }

        buildConfig = UnityEngine.Object.Instantiate(buildConfig);
        buildConfig.BaseResVersion = baseVersion;
        buildConfig.ResVersion = version;
        LogHelper.Info("BuildRes, BuildTarget: {0}, BaseVersion: {1}, NewVersion: {2}", buildTarget, baseVersion, version);
        AssetBundleBuilder.BuildAllAb(buildTarget, buildConfig);
    }

    public static string GetLastVersion(BuildTarget buildTarget)
    {
        var outputPath = ABUtility.GetOutputPath(buildTarget);
        DirectoryInfo root = new DirectoryInfo(outputPath);
        if (!root.Exists)
        {
            return null;
        }

        var children = root.GetDirectories();
        DateTime lastUpdateTime = DateTime.MinValue;
        string ret = null;
        foreach (var directoryInfo in children)
        {
            if (string.IsNullOrEmpty(directoryInfo.Name) || !char.IsNumber(directoryInfo.Name[0]))
            {
                continue;
            }

            if (directoryInfo.LastWriteTime > lastUpdateTime)
            {
                lastUpdateTime = directoryInfo.LastWriteTime;
                ret = directoryInfo.Name;
            }
        }

        return ret;
    }
}