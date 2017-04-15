/********************************************************************

** Filename : ResourceCheckRes
** Author : ake
** Date : 2016/4/6 16:17:52
** Summary : ResourceCheckRes
***********************************************************************/

using System;

namespace SoyEngine
{
	public struct ResourceCheckRes
	{
		public EResourceCheckResType CheckResType;
		public string ResourceName;
		public string NewMd5Value;
		public int Size;
		public bool LoadFromPackageRes;

		public override string ToString()
		{
			return string.Format("EResourceCheckResType :{0} ResourceName : {1}  NewMd5Value :{2}", 
				CheckResType, ResourceName,NewMd5Value);
		}
	}

	public enum EResourceCheckResType
	{
		Equal,
		Change,
		Delete,
	}
}