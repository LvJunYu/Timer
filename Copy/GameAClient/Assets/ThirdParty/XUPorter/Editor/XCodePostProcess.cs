using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;

public static class XCodePostProcess
{

#if UNITY_EDITOR
	[PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{
		if (target != BuildTarget.iOS) {
			return;
		}

		// Create a new project object from build target
		XCProject project = new XCProject( pathToBuiltProject );

		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
		string[] files = Directory.GetFiles( Application.dataPath, "*.projmods", SearchOption.AllDirectories );
		foreach( string file in files ) {
			UnityEngine.Debug.Log("ProjMod File: "+file);
			project.ApplyMod( file );
		}

		//TODO implement generic settings as a module option
		project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Developer", "Release");
//        EditCode(pathToBuiltProject);
		// Finally save the xcode project
		project.Save();

	}


    private static void EditCode(string filePath)
    {
        EditUnityViewControllerBaseiOS(filePath);
    }

    private static void EditUnityViewControllerBaseiOS(string filePath)
    {
        XCClass unityViewControllerBaseiOS_H = new XCClass(filePath + "/Classes/UI/UnityViewControllerBaseiOS.h");
        if(unityViewControllerBaseiOS_H.FullText.StartsWith("//JoyModified"))
        {
            return;
        }
        unityViewControllerBaseiOS_H.Insert(0,"//JoyModified\n");
        unityViewControllerBaseiOS_H.WriteBelow("preferredStatusBarStyle;", "+ (void)setStatusBarHidden:(BOOL)hidden;");
        unityViewControllerBaseiOS_H.Save();
        XCClass unityViewControllerBaseiOS_MM = new XCClass(filePath + "/Classes/UI/UnityViewControllerBaseiOS.mm");
        unityViewControllerBaseiOS_MM.WriteBelow("@implementation UnityViewControllerBase", "static BOOL _statusBarHidden = true;\n" +
            "+ (void)setStatusBarHidden:(BOOL)hidden\n{\n    _statusBarHidden = hidden;\n}");
        int sInx = unityViewControllerBaseiOS_MM.FullText.IndexOf("- (BOOL)prefersStatusBarHidden");
        string endStr = "return _PrefersStatusBarHidden;";
        int eInx = unityViewControllerBaseiOS_MM.FullText.IndexOf(endStr, sInx)+endStr.Length;
        eInx = unityViewControllerBaseiOS_MM.FullText.IndexOf("}", eInx) + 1;
        unityViewControllerBaseiOS_MM.Remove(sInx, eInx - sInx);
        unityViewControllerBaseiOS_MM.Insert(sInx, "- (BOOL)prefersStatusBarHidden\n{\n    return _statusBarHidden;\n}");
        unityViewControllerBaseiOS_MM.Save();
    }

    private static void EditUnityAppController(string filePath)
    {
        //读取UnityAppController.mm文件
        XCClass unityAppController = new XCClass(filePath + "/Classes/UnityAppController.mm");

        //在指定代码后面增加一行代码
        unityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"","#import <ShareSDK/ShareSDK.h>");

        //在指定代码中替换一行
        unityAppController.Replace("return YES;","return [ShareSDK handleOpenURL:url sourceApplication:sourceApplication annotation:annotation wxDelegate:nil];");

        //在指定代码后面增加一行
        unityAppController.WriteBelow("UnityCleanup();\n}","- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url\r{\r    return [ShareSDK handleOpenURL:url wxDelegate:nil];\r}");
        unityAppController.Save();
    }

#endif

	public static void Log(string message)
	{
		UnityEngine.Debug.Log("PostProcess: "+message);
	}
}
