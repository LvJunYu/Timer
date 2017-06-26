using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Diagnostics;
using Debug=UnityEngine.Debug;

namespace NewResourceSolution.EditorTool
{
    public class ToolUtility
    {
		/// <summary>
		/// osx下执行shell脚本
		/// </summary>
		/// <param name="shellPath">Shell path.</param>
        public static void RunBash(string shellPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo ();
            psi.FileName = "/bin/bash";
            psi.Arguments = shellPath;

            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            Process p = new Process ();
            p.StartInfo = psi;
            try
            {
                p.OutputDataReceived += (o, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Debug.Log (e.Data);
                    }
                };
                p.Start();
                p.BeginOutputReadLine();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally {
                // 好像并没有用
                AssetDatabase.Refresh ();
            }
        }

		/// <summary>
		/// 编辑器下递归创建文件夹
		/// </summary>
		/// <returns><c>true</c>, if create folder was recursived, <c>false</c> otherwise.</returns>
		/// <param name="path">Assets下的相对路径.</param>
		public static void RecursiveCreateFolder (string path, string parent = null)
		{
			// todo 改成用directoryInfo创建
			if (string.IsNullOrEmpty(parent) )
			{
				parent = ResPath.Assets;
			}
			if (string.IsNullOrEmpty(path)) return;
			path.Replace('\\', '/');
			int firstSlashIndex = path.IndexOf('/');
			if (firstSlashIndex >= 0)
			{
				string root = path.Substring(0, firstSlashIndex);
				string child = path.Substring(firstSlashIndex + 1, path.Length - firstSlashIndex - 1);
				if (!string.IsNullOrEmpty(root))
				{
					string folderToCreate = string.Format(StringFormat.TwoLevelPath, parent, root);
					if (!AssetDatabase.IsValidFolder(folderToCreate))
					{
						AssetDatabase.CreateFolder(parent, root);
					}
					if (!string.IsNullOrEmpty(child))
					{
						RecursiveCreateFolder(child, folderToCreate);
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(child))
					{
						RecursiveCreateFolder(child, parent);
					}
				}
			}
			else
			{
				string folderToCreate = string.Format(StringFormat.TwoLevelPath, parent, path);
				if (!AssetDatabase.IsValidFolder(folderToCreate))
				{
					AssetDatabase.CreateFolder(parent, path);
				}
			}
		}

		#region editor gui tools
		private static Color LightRed = new Color(1f, .2f, .2f, 1f);
		private static Color LightGreen = new Color(.2f, 1f, .2f, 1f);
		private static Color LightYellow = new Color(1f, 1f, .2f, 1f);
		public static void SetGUIColorDefault ()
		{
			GUI.color = Color.white;
		}
		public static void SetGUIColorLightGreen ()
		{
			GUI.color = LightGreen;
		}
		public static void SetGUIColorLightRed ()
		{
			GUI.color = LightRed;
		}
		public static void SetGUIColorLightYellow ()
		{
			GUI.color = LightYellow;
		}
		#endregion
    }
}