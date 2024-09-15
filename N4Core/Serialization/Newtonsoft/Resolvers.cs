#nullable disable

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace N4Core.Serialization.Newtonsoft
{
    public class JsonIgnorePropertiesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);
            if (jsonProperty.PropertyName.Equals("fileData") || jsonProperty.PropertyName.Equals("fileContent") || jsonProperty.PropertyName.Equals("filePath"))
            {
                jsonProperty.ShouldSerialize = _ => false;
            }
            return jsonProperty;
        }
    }
}
