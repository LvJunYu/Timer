using System;
using System.Collections.Generic;
using System.IO;
using GameA;
using NewResourceSolution;
using NewResourceSolution.EditorTool;
using SoyEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using FileTools = NewResourceSolution.FileTools;

public class CommandBuildPackage
{
//    [MenuItem("QuanTools/TestBuildPackage")]
//    public static void TestBuildPackage()
//    {
//        BuildPackage(BuildTarget.StandaloneWindows, EEnvironment.Development, "1", "/Users/quan/Downloads/Temp/JoyGame");
//    }
    
    public static void BuildPackage()
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
                LogHelper.Error("BuildTarget param error");
                return;
            }
        }
        else
        {
            LogHelper.Error("BuildTarget param missing");
            return;
        }

        EEnvironment eEnvironment = EEnvironment.Development;
        inx = Array.FindIndex(args, s => s == "-bEnv");
        if (inx >= 0 && args.Length > inx + 1)
        {
            var str = args[inx + 1];
            try
            {
                eEnvironment = (EEnvironment) Enum.Parse(typeof(EEnvironment), str);
            }
            catch (Exception)
            {
                LogHelper.Error("eEnvironment param error");
                return;
            }
        }
        else
        {
            LogHelper.Error("eEnvironment param missing");
            return;
        }


        string appVersion = null;
        inx = Array.FindIndex(args, s => s == "-appVersion");
        if (inx >= 0  && args.Length > inx+1)
        {
            var str = args[inx + 1];
            if (!string.IsNullOrEmpty(str) && str!="0")
            {
                try
                {
                    new Version(str);
                    appVersion = str;
                }
                catch (Exception)
                {
                    LogHelper.Error("appVersion param error");
                    return;
                }
            }
        }
        else
        {
            LogHelper.Error("appVersion param missing");
            return;
        }

        string outputPath = null;
        inx = Array.FindIndex(args, s => s == "-outputPath");
        if (inx >= 0  && args.Length > inx+1)
        {
            var str = args[inx + 1];
            if (!string.IsNullOrEmpty(str))
            {
                outputPath = str;
                if (Directory.Exists(outputPath))
                {
                    Directory.Delete(outputPath, true);
                }

                FileTools.CheckAndCreateFolder(outputPath);
            }
            else
            {
                LogHelper.Error("outputPath param error");
                return;
            }
        }
        else
        {
            LogHelper.Error("outputPath param missing");
            return;
        }

        
        BuildPackage(buildTarget, eEnvironment, appVersion, outputPath);
    }
    
    public static void BuildPackage(BuildTarget buildTarget, EEnvironment eEnvironment, string appVersion, string outputPath)
    {
        LogHelper.Info("BuildPackage, BuildTarget: {0}, Environment: {1}, AppVersion: {2}, OutputPath: {3}",
            buildTarget, eEnvironment, appVersion, outputPath);
        SetScene(eEnvironment, appVersion);
        var result = BuildPipeline.BuildPlayer(GetBuildScenes(), outputPath + "/JoyGame.exe", buildTarget, BuildOptions.None);
        if (string.IsNullOrEmpty(result))
        {
            Console.WriteLine("Build Success");
        }
        else
        {
            Console.WriteLine(result);
            Console.WriteLine("Build Failed");
            EditorApplication.Exit(1);
        }
    }

    static void SetScene(EEnvironment eEnvironment, string appVersion)
    {
        var scene = EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
        GameObject go = GameObject.Find("Main");
        var socialApp = go.GetComponent<SocialApp>();
        socialApp.Env = eEnvironment;
        if (eEnvironment == EEnvironment.Production)
        {
            socialApp.Channel = PublishChannel.EType.QQGame;
            socialApp.LogLevel = LogHelper.ELogLevel.Info;
        }
        else
        {
            socialApp.Channel = PublishChannel.EType.None;
            socialApp.LogLevel = LogHelper.ELogLevel.Debug;
        }
        var runtimeConfig = go.GetComponent<RuntimeConfig>();
        runtimeConfig.Version = appVersion;
        PlayerSettings.bundleVersion = appVersion;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
    
    //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach(EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if(e==null)
                continue;
            if(e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }
}