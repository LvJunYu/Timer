/********************************************************************

** Filename : AppVersionData
** Author : ake
** Date : 2016/4/18 19:14:26
** Summary : AppVersionData
***********************************************************************/


namespace SoyEngine
{
	public class AppVersionData:BaseVersionData
	{
		public int VersionId;

		public override string VersionKey
		{
			get { return VersionId.ToString(); }
		}

		public override void SetKey(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				VersionId = 0;
				return;
			}
			int tmpId;
			if (!int.TryParse(value, out tmpId))
			{
				tmpId = 0;
			}
			VersionId = tmpId;
		}
	}
}