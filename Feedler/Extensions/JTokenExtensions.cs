using Newtonsoft.Json.Linq;

namespace Feedler.Extensions
{
    public static class JTokenExtensions
    {
        public static string String(this JToken jToken, string property) =>
            jToken[property].ToObject<string>();
    }
}
