/********************************************************************
** Filename : XCodePostProcessCopyRes  
** Author : ake
** Date : 8/26/2016 4:52:28 PM
** Summary : XCodePostProcessCopyRes  
***********************************************************************/


using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class XCodePostProcessCopyRes
{
	public const string CopyResSubPath = "{0}/Data/Raw/Ios";
	public const string ResSubPath = "{0}/GameDesigner/AppGame/Resources/Ios";


	[PostProcessBuild(100)]
	public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
	{
		if (target != BuildTarget.iOS)
		{
			return;
		}
		string dstPath = string.Format(CopyResSubPath, pathToBuiltProject);
		DirectoryInfo dstDir = new DirectoryInfo(dstPath);
		if (!dstDir.Exists)
		{
			dstDir.Create();
		}
		DirectoryInfo projectInfo = new DirectoryInfo(Application.dataPath).Parent.Parent.Parent.Parent;
		string resPath = string.Format(ResSubPath, ReplaceBackSlash(projectInfo.FullName));
		DirectoryInfo resDir = new DirectoryInfo(resPath);
		CopyDir(resDir.FullName, dstDir.FullName);
		Debug.Log("Copy res to xcodeProject success!");
	}


	private static string ReplaceBackSlash(string str)
	{
		return str.Replace('\\', '/');
	}


	public static bool CopyDir(string srcPath, string aimPath)
	{
		try
		{
			if (aimPath[aimPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
			{
				aimPath += System.IO.Path.DirectorySeparatorChar;
			}

			// 判断目标目录是否存在如果不存在则新建

			if (!System.IO.Directory.Exists(aimPath))
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
			Debug.LogError(string.Format("CopyDir failed sor:{0},des:{1},exception:{2}.", srcPath, aimPath, e.ToString()));
			return false;
		}
	}

}

