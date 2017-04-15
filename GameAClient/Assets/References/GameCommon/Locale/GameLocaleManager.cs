/********************************************************************

** Filename : GameLocaleManager
** Author : ake
** Date : 2016/3/22 16:22:42
** Summary : GameLocaleManager
***********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SoyEngine
{
	public class GameLocaleManager:MonoBehaviour
	{
		private Dictionary<string, string> locales; 
		void Awake()
		{
			InitData();
		}


		private void InitData()
		{
			locales = new Dictionary<string, string>();
			string localePath = GameResourceManager.Instance.PathMan.GetLocalePath();
			DirectoryInfo dir = new DirectoryInfo(localePath);
			if (!dir.Exists)
			{
				LogHelper.Error("Locale path {0} doesn't exist!", localePath);
				return;
			}
			var files = dir.GetFiles("*.txt", SearchOption.AllDirectories);
			for (int j = 0; j < files.Length; j++)
			{
				if (File.Exists(files[j].FullName))
				{
					var lines = File.ReadAllLines(files[j].FullName);

					for (int i = 0; i + 1 < lines.Length; i += 2)
					{
						var key = lines[i].Trim();
						var content = lines[i + 1];
						if (locales.ContainsKey(key))
						{
							LogHelper.Warning("本地化(locale)代号重复'{0}'，对应的文字为：'{1}'，来自文件{2}.",
								key, content, files[j].Name);
						}
						locales[key] = DecodeText(content);
					}
				}
			}
			var enumerator = locales.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Debug.Log(enumerator.Current.Value);
			}
		}



		#region private

		private static string DecodeText(string str)
		{
			if (str.IndexOf("\\") != -1)
			{
				str = str.Replace("\\n", "\n");
				str = str.Replace("\\\\", "\\");
			}
			return  str;
		}

		#endregion
	}
}