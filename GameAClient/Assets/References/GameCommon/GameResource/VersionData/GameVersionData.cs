/********************************************************************

** Filename : GameVersionData
** Author : ake
** Date : 2016/4/19 16:56:57
** Summary : GameVersionData
***********************************************************************/

using System;
using System.Collections.Generic;

namespace SoyEngine
{
	public class GameVersionData:BaseVersionData
	{
		public string Md5;

		public override string VersionKey
		{
			get { return Md5; }
		}

		public override void SetKey(string value)
		{
			Md5 = value;
		}

		public GameVersionData()
		{
			
		}

		public GameVersionData(GameVersionData copy)
		{
			if (copy == null)
			{
				return;
			}
			Md5 = copy.Md5;
			Md5Dic = new Dictionary<string, ResConfig>();
			var enumerator = copy.Md5Dic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var cur = enumerator.Current;
				Md5Dic.Add(cur.Key, cur.Value);
			}
		}
	}
}