/********************************************************************
** Filename : ResConfig  
** Author : ake
** Date : 7/12/2016 12:13:17 PM
** Summary : ResConfig  
***********************************************************************/


namespace SoyEngine
{
	public class ResConfig
	{
		public string Md5Value;
		public int Size = 1;

		public static ResConfig ParseFromString(string value)
		{
			ResConfig config = new ResConfig();
			if (!string.IsNullOrEmpty(value))
			{
				string[] splitRes = value.Split('|');
				config.Md5Value = splitRes[0];
				if (splitRes.Length > 1)
				{
					int outValue;
					if (!int.TryParse(splitRes[1],out outValue))
					{
						outValue = 1;
					}
					config.Size = outValue;
				}
			}
			return config;
		}

		public override string ToString()
		{
			return string.Format("Md5Value :{0} ,size:{1}",Md5Value,Size);
		}
	}
}