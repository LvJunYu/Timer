namespace NewResourceSolution
{
	public static class ExtensionMethods
	{
		public static T ToEnum<T>(this string value)
		{
			return (T) System.Enum.Parse(typeof(T), value, true);
		}
	}
}