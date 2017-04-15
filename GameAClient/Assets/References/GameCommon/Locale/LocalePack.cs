/********************************************************************
** Filename : LocalePack  
** Author : ake
** Date : 6/27/2016 3:34:36 PM
** Summary : LocalePack  
***********************************************************************/


using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace SoyEngine
{
	public class LocalePack:BaseBinaryPack
	{
		public Dictionary<string, string> LocaleDic;



		public LocalePack(BaseBinaryPack basePack)
		{
			_path = basePack.Path;
			IsBuildInPack = basePack.IsBuildInPack;
		}

		public static LocalePack UpgradeFromBaseBinaryPack(BaseBinaryPack basePack)
		{
			LocalePack res = new LocalePack(basePack);
			Stream tmpStream;
			if (res.IsBuildInPack)
			{
				var textAsset = Resources.Load<TextAsset>(basePack.Path);
				if (textAsset == null)
				{
					LogHelper.Error("UpgradeFromBaseBinaryPack called but path is invalid! {0} ", basePack.Path);
					return res;
				}
				var byteArray = textAsset.bytes;
				int md5Length = byteArray[0];
				string md5 = Encoding.ASCII.GetString(byteArray, 0, md5Length);
				res.Version = md5;
				tmpStream = new MemoryStream(byteArray);
				tmpStream.Position = HeaderLength;
				BinaryFormatter binFormat = new BinaryFormatter();
				res.LocaleDic = (Dictionary<string, string>)binFormat.Deserialize(tmpStream);
			}
			else
			{
				if (!File.Exists(res.Path))
				{
					LogHelper.Error("UpgradeFromBaseBinaryPack called but res.Path is invalid! {0}", res.Path);
					return res;
				}
				tmpStream = new FileStream(res.Path, FileMode.Open, FileAccess.Read);
				int md5Length = tmpStream.ReadByte();
				tmpStream.Read(HeaderBuffer, 0, md5Length);
				string md5 = Encoding.ASCII.GetString(HeaderBuffer, 0, md5Length);
				res.Version = md5;

				tmpStream.Position = HeaderLength;
				BinaryFormatter binFormat = new BinaryFormatter();
				res.LocaleDic = (Dictionary<string, string>)binFormat.Deserialize(tmpStream);
			}

			tmpStream.Close();
			tmpStream.Dispose();
			return res;
		}

	}
}