/********************************************************************

** Filename : BaseVersionData
** Author : ake
** Date : 2016/4/19 16:48:03
** Summary : BaseVersionData
***********************************************************************/

using System;
using System.Collections.Generic;

namespace SoyEngine
{
	public abstract class BaseVersionData
	{
		public abstract string VersionKey { get; }
		public Dictionary<string, ResConfig> Md5Dic;

		public override string ToString()
		{
			return string.Format("{2}  :: VersionKey = {0},Md5Dic.Count = {1} ", VersionKey,
				(Md5Dic == null ? "Invalid" : Md5Dic.Count.ToString()),this.GetType());
		}

		public abstract void SetKey(string value);

		public bool TryGetMd5Value(string fileName, out string md5)
		{
			ResConfig outValue;
			if (Md5Dic.TryGetValue(fileName, out outValue))
			{
				md5 = outValue.Md5Value;
				return true;
			}
			md5 = "";
			return false;
		}
	}
}