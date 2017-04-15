/********************************************************************
** Filename : BaseBinaryPack  
** Author : ake
** Date : 6/27/2016 3:35:47 PM
** Summary : BaseBinaryPack  
***********************************************************************/

using System.IO;
using System.Text;
using UnityEngine;

namespace SoyEngine
{
	public class BaseBinaryPack
	{
		#region global
		public static byte[] HeaderBuffer = new byte[128];

		public const int HeaderLength = 128;
		public static BaseBinaryPack ReadHeaderFromFile(string filePath)
		{
			BaseBinaryPack res = new BaseBinaryPack();
			Stream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			int md5Length = fStream.ReadByte();
			fStream.Read(HeaderBuffer, 0, md5Length);
			fStream.Close();
			fStream.Dispose();
			string md5 = Encoding.ASCII.GetString(HeaderBuffer, 0, md5Length);
			res.Version = md5;
			res.IsBuildInPack = false;
			res._path = filePath;
			return res;
		}

		public static BaseBinaryPack ReadHeaderFromResource(string path)
		{
			BaseBinaryPack res = new BaseBinaryPack();
			var textAsset = Resources.Load<TextAsset>(path);
			if (textAsset == null)
			{
				LogHelper.Error("ReadHeaderFromResource called but path is invalid! {0} ", path);
				return res;
			}
			var byteArray = textAsset.bytes;
			int md5Length = byteArray[0];
			string md5 = Encoding.ASCII.GetString(byteArray, 0, md5Length);
			res.Version = md5;
			res.IsBuildInPack = true;
			res._path = path;
			return res;
		}

		#endregion

		public string Version;
		public bool IsBuildInPack = false;
		protected string _path;

		public string Path
		{
			get
			{
				return _path;
			}
		}

		public void LinkDataToFile(string filePath)
		{
			IsBuildInPack = false;
			_path = filePath;
		}

		public void ReloadFromFile(string filePath)
		{
			BaseBinaryPack res = BaseBinaryPack.ReadHeaderFromFile(filePath);
			Version = res.Version;
			IsBuildInPack = res.IsBuildInPack;
			_path = res._path;
		}
	}
}
