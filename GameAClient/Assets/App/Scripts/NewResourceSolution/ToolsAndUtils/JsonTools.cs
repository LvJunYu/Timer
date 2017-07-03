using Newtonsoft.Json;

namespace NewResourceSolution
{
    public static class JsonTools
    {
        public static VersionJsonConverter s_VersionJsonConverter = new VersionJsonConverter ();
        public static T DeserializeObject<T> (string value)
        {
            return JsonConvert.DeserializeObject<T> (value, s_VersionJsonConverter);
        }

        public static string SerializeObject (object value, bool indented)
        {
            return JsonConvert.SerializeObject (value, indented ? Formatting.Indented : Formatting.None, s_VersionJsonConverter);
        }

        /// <summary>
        /// 序列化一个object，只序列化propertyNames包含的属性/字段名称（该限制也作用于object所包含的物体）
        /// </summary>
        /// <returns>The onject with limited properties.</returns>
        /// <param name="value">Value.</param>
        /// <param name="indented">If set to <c>true</c> indented.</param>
        /// <param name="propertyNames">Property names.</param>
        public static string SerializeOnjectWithLimitedProperties (object value, bool indented, string[] propertyNames)
        {
            if (null == propertyNames || propertyNames.Length == 0)
                return SerializeObject (value, indented);
            JsonSerializerSettings jsetting = new JsonSerializerSettings ();
            jsetting.Formatting = indented ? Formatting.Indented : Formatting.None;
            jsetting.Converters.Add (s_VersionJsonConverter);
            jsetting.ContractResolver = new LimitedPropsContractResolver (propertyNames);
            return JsonConvert.SerializeObject (value, jsetting);
        }

        /// <summary>
        /// 序列化一个object，不序列化propertyNames包含的属性/字段名称（该限制也作用于object所包含的物体）
        /// </summary>
        /// <returns>The onject with excluded properties.</returns>
        /// <param name="value">Value.</param>
        /// <param name="indented">If set to <c>true</c> indented.</param>
        /// <param name="propertyNames">Property names.</param>
        public static string SerializeOnjectWithExcludedProperties (object value, bool indented, string[] propertyNames)
        {
            if (null == propertyNames || propertyNames.Length == 0)
                return SerializeObject (value, indented);
            JsonSerializerSettings jsetting = new JsonSerializerSettings ();
            jsetting.Formatting = indented ? Formatting.Indented : Formatting.None;
            jsetting.Converters.Add (s_VersionJsonConverter);
            jsetting.ContractResolver = new ExcludePropsContractResolver (propertyNames);
            return JsonConvert.SerializeObject (value, jsetting);
        }
    }
}