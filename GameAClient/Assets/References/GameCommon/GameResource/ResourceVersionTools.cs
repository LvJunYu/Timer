/********************************************************************

** Filename : ResourceVersionTools
** Author : ake
** Date : 2016/4/6 11:48:03
** Summary : ResourceVersionTools
***********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SoyEngine
{
	public class ResourceVersionTools
	{

		public static void UpdateChangeListLoadFrom(List<ResourceCheckRes> list, GameLocalFileVersionPack pack, bool isRes)
		{
			if (list == null || pack == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				var item = list[i];
				string packMd5;
				if (isRes)
				{
					if (!pack.TryGetResourceMd5(item.ResourceName, out packMd5))
					{
						packMd5 = null;
					}
				}
				else
				{
					if (!pack.TryGetConfigMd5(item.ResourceName, out packMd5))
					{
						packMd5 = null;
					}
				}
				if (packMd5 == null)
				{
					continue;
				}
				if (packMd5 == item.NewMd5Value)
				{
					item.LoadFromPackageRes = true;
					list[i] = item;
				}
			}
		}

		public static Dictionary<string, ResConfig> ParseConfigDataFromString(string data)
		{
			var array = data.Split('\n');
			return ReadConfigDataFromStringArray(array, 0);
		}

		public static Dictionary<string, ResConfig> ParseConfigDataFromFile(string filePath)
		{
			Dictionary<string, ResConfig> res = new Dictionary<string, ResConfig>();
			if (!File.Exists(filePath))
			{
				return res;
			}
			string[] allLines = File.ReadAllLines(filePath);
			
			return ReadConfigDataFromStringArray(allLines,0);
		}

		public static AppVersionData ParseLocalAppVersionDataFromFile(string filePath)
		{
			AppVersionData data = new AppVersionData();
			if (!File.Exists(filePath))
			{
				data.Md5Dic = new Dictionary<string, ResConfig>();
				data.VersionId = 0;
				return data;
			}

			ParseVersionDataFromFile(filePath, data);
			return data;
		}

		public static GameVersionData ParseGameVersionDataFromFile(string filePath)
		{
			GameVersionData data = new GameVersionData();
			if (!File.Exists(filePath))
			{
				data.Md5Dic = new Dictionary<string, ResConfig>();
				data.Md5 = null;
				return data;
			}
			ParseVersionDataFromFile(filePath, data);
			return data;
		}

		public static GameVersionData ParseGameVersionDataFromData(string strData)
		{
			var array = strData.Split('\n');
			GameVersionData data = new GameVersionData();
			ParseVersionDataFromFileFromArray(array, data);
			return data;
		}


		public static bool WriteVersionData(BaseVersionData data,string path,bool isBackslash)
		{
			if (data == null || data.Md5Dic == null)
			{
				LogHelper.Error("WriteVersionData called but data == null || data.Md5Dic is null!");
				return false;
			}
			if (string.IsNullOrEmpty(path))
			{
				LogHelper.Error("WriteVersionData called but path is null or empty! data is \n {0}", data.ToString());
				return false;
			}
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists)
			{
				fileInfo.Delete();
			}
			else
			{
				string parent = FileTools.GetParentFolderPath(path, isBackslash);
				DirectoryInfo dir = new DirectoryInfo(parent);
				if (!dir.Exists)
				{
					dir.Create();
				}
			}
			string[] md5Config = new string[data.Md5Dic.Count * 2 + 1];
			md5Config[0] = data.VersionKey;
			var enumerator = data.Md5Dic.GetEnumerator();
			int curIndex = 0;
			while (enumerator.MoveNext())
			{
				md5Config[curIndex * 2 + 1] = enumerator.Current.Key;
				md5Config[curIndex * 2 + 2] = enumerator.Current.Value.Md5Value;
				curIndex++;
			}
			File.WriteAllLines(path, md5Config, Encoding.ASCII);
			return true;
		}


		public static List<ResourceCheckRes> CompareLocalResourceVersionFromWeb(Dictionary<string, ResConfig> local,
			Dictionary<string, ResConfig> web)
		{
			List<ResourceCheckRes> res = new List<ResourceCheckRes>();
			if (local == null || web == null)
			{
				return res;
			}
			var el = local.GetEnumerator();
			while (el.MoveNext())
			{
				if (!web.ContainsKey(el.Current.Key))
				{
					res.Add(new ResourceCheckRes()
					{
						CheckResType = EResourceCheckResType.Delete,
						ResourceName = el.Current.Key,
						LoadFromPackageRes = false,
					});
				}
			}

			var wl = web.GetEnumerator();
			ResConfig tmpOutValue;
			while (wl.MoveNext())
			{
				var item = wl.Current;

				if (local.TryGetValue(item.Key, out tmpOutValue))
				{
					if (String.CompareOrdinal(tmpOutValue.Md5Value, item.Value.Md5Value) == 0)
					{
						continue;
					}
				}
				res.Add(new ResourceCheckRes()
				{
					CheckResType = EResourceCheckResType.Change,
					ResourceName = item.Key,
					NewMd5Value = item.Value.Md5Value,
					Size = item.Value.Size,
					LoadFromPackageRes = false,
				});
			}

			return res;
		}

		#region  private

		private static Dictionary<string, ResConfig> ReadConfigDataFromStringArray(string[] array, int startLineIndex)
		{
			Dictionary<string, ResConfig> res = new Dictionary<string, ResConfig>();
			if (array == null || startLineIndex < 0)
			{
				LogHelper.Error("ReadConfigDataFromStringArray called but array == null || startLineIndex <0 , array {0} ,startLineIndex {1}", array, startLineIndex);
				return res;
			}

			int totalCount = (int)((array.Length - startLineIndex) / 2) * 2;
			for (int i = 0; i < totalCount; i += 2)
			{
				string key = array[i + startLineIndex].TrimEnd('\r');
				string value = array[i + 1 + startLineIndex].TrimEnd('\r');
				if (!res.ContainsKey(key))
				{
					ResConfig conf = ResConfig.ParseFromString(value);
					res.Add(key, conf);
				}
			} 
			return res;
		}
		/// <summary>
		/// 不用判断参数 外层调用保证
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="data"></param>
		private static void ParseVersionDataFromFile(string filePath, BaseVersionData data)
		{
			string[] allLines = File.ReadAllLines(filePath);
			ParseVersionDataFromFileFromArray(allLines, data);
		}

		private static void ParseVersionDataFromFileFromArray(string[] allLines, BaseVersionData data)
		{
			if (allLines == null || data == null)
			{
				return;
			}
			if (allLines.Length == 0)
			{
				data.Md5Dic = new Dictionary<string, ResConfig>();
				data.SetKey(null);
				LogHelper.Error("ParseVersionDataFromFile failed version file invalid allLines.Length == 0");
				return;
			}
			data.Md5Dic = ReadConfigDataFromStringArray(allLines, 1);
			data.SetKey(allLines[0]);
		}

		#endregion



	}
}
