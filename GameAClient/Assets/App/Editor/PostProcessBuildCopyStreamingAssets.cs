
using System;
using System.IO;
using NewResourceSolution;
using NewResourceSolution.EditorTool;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class PostProcessBuildCopyStreamingAssets
{
	public const string XCodeResPathFormat = "{0}/Data/Raw";


	[PostProcessBuild(100)]
	public static void OnPostProcessBuild(BuildTarget target, string targetPath)
    {
      	if (target == BuildTarget.iOS)
		{
			if (EditorUtility.DisplayDialog("提示", "是否拷贝资源", "确定", "取消"))
			{
		    	CopyiOS(targetPath);
			}
			else
			{
				return;
			}
		}
		Debug.Log("Copy res to xcodeProject success!");
	}

	private static void CopyiOS(string targetPath)
	{
		string dstPath = string.Format(XCodeResPathFormat, targetPath);
		FileTools.CheckAndCreateFolder(dstPath);
		var dstDir = new DirectoryInfo(dstPath);
		string resPath = ABUtility.GetBuildOutputStreamingAssetsPath(BuildTarget.iOS);
		DirectoryInfo resDir = new DirectoryInfo(resPath);
		CopyDir(resDir.FullName, dstDir.FullName);
	}

	public static bool CopyDir(string srcPath, string aimPath)
	{
		try
		{
			if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
			{
				aimPath += Path.DirectorySeparatorChar;
			}

			// 判断目标目录是否存在如果不存在则新建

			if (!Directory.Exists(aimPath))
			{
				Directory.CreateDirectory(aimPath);
			}

			string[] fileList = Directory.GetFileSystemEntries(srcPath);
			foreach (string file in fileList)
			{
				if (Directory.Exists(file))
				{
					CopyDir(file, aimPath + Path.GetFileName(file));
				}
				else
				{
					File.Copy(file, aimPath + Path.GetFileName(file), true);
				}
			}
			return true;
		}

		catch (Exception e)
		{
			Debug.LogError(string.Format("CopyDir failed sor:{0},des:{1},exception:{2}.", srcPath, aimPath, e));
			return false;
		}
	}

}

