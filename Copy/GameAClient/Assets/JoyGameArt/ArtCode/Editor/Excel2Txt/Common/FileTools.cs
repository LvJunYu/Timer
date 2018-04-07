/* ==============================================================================
 * 功能描述：FileTools  
 * 创 建 者：ake
 * 创建日期：3/7/2016 3:49:52 PM
 * ==============================================================================*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Excel2Txt;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Common
{
	public class FileTools
	{

		public static string[] ReadTextFileAllLine(string filePath, Encoding encoding)
		{
			if (!File.Exists(filePath))
			{
				LogMan.Error("ReadTextFileAllLine called but filePath {0} is not exist!", filePath);
				return null;
			}
			return File.ReadAllLines(filePath, encoding);
		}

		/// <summary>
		/// 获得父级文件夹 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="isBackSlant">是否是反斜杠</param>
		/// <returns></returns>
		public static string GetParentFolderPath(string path, bool isBackSlash = false)
		{
			char slashChar;
			if (isBackSlash)
			{
				slashChar = '\\';
			}
			else
			{
				slashChar = '/';
			}
			path = path.TrimEnd(slashChar);
			int idx = path.LastIndexOf(slashChar);
			if (idx > 0)
			{
				return path.Substring(0, idx);
			}
			else
			{
				return path;
			}
		}

		public static string GetParentFolderPathSafe(string path)
		{
			string tmp = path.Replace("\\", "/");
			tmp = tmp.TrimEnd('/');
			int idx = tmp.LastIndexOf('/');
			if (idx > 0)
			{
				return tmp.Substring(0, idx);
			}
			else
			{
				return tmp;
			}
		}

		public static void WriteExcelToTxt(string txtPath, string excelPath)
		{
			if (string.IsNullOrEmpty(txtPath) || string.IsNullOrEmpty(excelPath))
			{
				LogMan.Error("WriteExcelToTxt failed! txtPath {0} excelPath {1}",txtPath,excelPath);
				return;
			}
			StringBuilder content = ReadTableToString(excelPath);
			FileInfo txtFile = new FileInfo(txtPath);
			if (txtFile.Exists)
			{
				txtFile.Delete();
			}
			string parentPath = GetParentFolderPath(txtPath, true);
			DirectoryInfo dir = new DirectoryInfo(parentPath);
			if (!dir.Exists)
			{
				dir.Create();
			}
			File.WriteAllText(txtPath, content.ToString(),Encoding.UTF8);
		}

	    public static byte[] WriteExcelToByteArray(string excelPath)
	    {
            if ( string.IsNullOrEmpty(excelPath))
            {
                LogMan.Error("WriteExcelToByteArray failed! excelPath {0}", excelPath);
                return null;
            }
            StringBuilder content = ReadTableToString(excelPath);
            //LogMan.Error("{0}\n{1}", excelPath,content.ToString());
            return Encoding.UTF8.GetBytes(content.ToString());
        }

	    public static byte[] WriteExcelToByteArrayEx(string excelPath)
	    {
	        string txtPath = excelPath.Replace(ConstDefine.TableFileSuffix, ".txt");
	        WriteExcelToTxt(txtPath, excelPath);
	        return File.ReadAllBytes(txtPath);
	    }


        public static StringBuilder ReadTableToString(string excelTablePath)
		{
   //         DataTable data = GetDataFromExcelByCom(excelTablePath);
			//if (data == null) {
			//	LogMan.Error("ReadTableToString called but {0} read failed!!!", excelTablePath);
			//	return null;
			//}
			//if (data.Rows.Count < 2)
			//{
			//	LogMan.Error("ReadTableToString called but {0} row count error! {1}!!!", excelTablePath, data.Rows.Count);
			//	return null;
			//}
			//DataRow tmpRowData;
			//StringBuilder builder = new StringBuilder();
			//for (int i = 0; i < data.Rows.Count; i++)
			//{
			//	tmpRowData = data.Rows[i];
			//	for (int j = 0; j < tmpRowData.ItemArray.Length; j++)
			//	{
			//		builder.Append(tmpRowData[j]);
			//		if (j != tmpRowData.ItemArray.Length - 1)
			//		{
			//			builder.Append('\t');
			//		}
			//	}
			//	if (i != data.Rows.Count)
			//	{
			//		builder.Append('\n');
			//	}
			//}
			//data.Dispose();
	        return null;
		}

		public static void WriteTheVersionFile(Dictionary<string, string> md5Dic, string path, bool isBackslash)
		{
			if (string.IsNullOrEmpty(path))
			{
				LogMan.Error("WriteTheVersionFile called but path is null or empty!!");
				return;
			}
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists)
			{
				fileInfo.Delete();
			}
			else
			{
				string parent = GetParentFolderPath(path, isBackslash);
				DirectoryInfo dir = new DirectoryInfo(parent);
				if (!dir.Exists)
				{
					dir.Create();
				}
			}
			string[] md5Config = new string[md5Dic.Count * 2];
			var enumerator = md5Dic.GetEnumerator();
			int curIndex = 0;
			while (enumerator.MoveNext())
			{
				md5Config[curIndex * 2] = enumerator.Current.Key;
				md5Config[curIndex * 2 + 1] = enumerator.Current.Value;
				curIndex++;
			}
			File.WriteAllLines(path, md5Config, Encoding.ASCII);
		}

		public static void UpdateVersionFile(Dictionary<string, string> md5Dic, string path, bool isBackslash)
		{
			if (string.IsNullOrEmpty(path))
			{
				LogMan.Error("WriteTheVersionFile called but path is null or empty!!");
				return;
			}
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists)
			{
				Dictionary<string, string> oldData = ParseConfigDataFromFile(path);
				var enumerator = md5Dic.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var curItem = enumerator.Current;
					oldData[curItem.Key] = curItem.Value;
				}
				WriteTheVersionFile(oldData, path, isBackslash);
			}
			else
			{
				WriteTheVersionFile(md5Dic, path, isBackslash);
			}
		}

		public static Dictionary<string, string> ParseConfigDataFromFile(string filePath)
		{
			Dictionary<string, string> res = new Dictionary<string, string>();
			if (!File.Exists(filePath))
			{
				return res;
			}
			string[] allLines = File.ReadAllLines(filePath);

			return ReadConfigDataFromStringArray(allLines, 0);
		}

		private static Dictionary<string, string> ReadConfigDataFromStringArray(string[] array, int startLineIndex)
		{
			Dictionary<string, string> res = new Dictionary<string, string>();
			if (array == null || startLineIndex < 0)
			{
				LogMan.Error("ReadConfigDataFromStringArray called but array == null || startLineIndex <0 , array {0} ,startLineIndex {1}", array, startLineIndex);
				return res;
			}

			int totalCount = (int)((array.Length - startLineIndex) / 2) * 2;
			for (int i = 0; i < totalCount; i += 2)
			{
				string key = array[i + startLineIndex].TrimEnd('\r');
				string value = array[i + 1 + startLineIndex].TrimEnd('\r');
				if (!res.ContainsKey(key))
				{
					res.Add(key, value);
				}
			}
			return res;
		}
	}

}