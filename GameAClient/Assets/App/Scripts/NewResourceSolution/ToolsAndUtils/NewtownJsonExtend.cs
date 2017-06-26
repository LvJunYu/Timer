using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace NewResourceSolution
{
	public class VersionJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(Version));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return new Version(reader.Value as string);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var version = value as Version;
			writer.WriteValue(version.ToString());
		}
	}

    public class LimitedPropsContractResolver : DefaultContractResolver
    {
        private string[] _limitedProps = null;

        public LimitedPropsContractResolver (string[] limitedProps)
        {
            _limitedProps = limitedProps;
        }
        protected override IList<JsonProperty> CreateProperties (
            Type type,
            MemberSerialization memberSerialization)
        {
            IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);
            return list.Where(p => _limitedProps.Contains(p.PropertyName)).ToList();
        }
    }

    public class ExcludePropsContractResolver : DefaultContractResolver
    {
        private string[] _excludeProps = null;

        public ExcludePropsContractResolver (string[] excludeProps)
        {
            _excludeProps = excludeProps;
        }
        protected override IList<JsonProperty> CreateProperties (
            Type type,
            MemberSerialization memberSerialization)
        {
            IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);
            return list.Where(p => !_excludeProps.Contains(p.PropertyName)).ToList();
        }
    }
}