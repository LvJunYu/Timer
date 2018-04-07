namespace NewResourceSolution
{
	public static class StringUtil
	{
		private static readonly System.Text.StringBuilder s_sb = new System.Text.StringBuilder(500);
		public static string Format (string format, object arg)
		{
			lock(s_sb)
			{
				s_sb.Length = 0;
				s_sb.AppendFormat(format, arg);
				return s_sb.ToString();
			}
		}
		public static string Format (string format, object arg1, object arg2)
		{
			lock(s_sb)
			{
				s_sb.Length = 0;
				s_sb.AppendFormat(format, arg1, arg2);
				return s_sb.ToString();
			}
		}
		public static string Format (string format, object arg1, object arg2, object arg3)
		{
			lock(s_sb)
			{
				s_sb.Length = 0;
				s_sb.AppendFormat(format, arg1, arg2, arg3);
				return s_sb.ToString();
			}
		}
		public static string Format (string format, params object[] args)
		{
			lock(s_sb)
			{
				s_sb.Length = 0;
				s_sb.AppendFormat(format, args);
				return s_sb.ToString();
			}
		}
	}
}