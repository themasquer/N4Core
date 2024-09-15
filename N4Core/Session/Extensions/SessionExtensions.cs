#nullable disable

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace N4Core.Session.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value) where T : class
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                string serializedValue = JsonConvert.SerializeObject(value);
                byte[] bytes = Encoding.UTF8.GetBytes(serializedValue);
                session.Set(key, bytes);
            }
        }

        public static T Get<T>(this ISession session, string key) where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            byte[] bytes;
            session.TryGetValue(key, out bytes);
            if (bytes is null)
                return null;
            string deserializedValue = Encoding.UTF8.GetString(bytes);
            T value = JsonConvert.DeserializeObject<T>(deserializedValue);
            return value;
        }
    }
}
