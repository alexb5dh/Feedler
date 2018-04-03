using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Feedler.Extensions.MVC
{
    public static class ConfigurationExtensions
    {
        public static JToken ToJson(this IConfigurationSection section)
        {
            if (!section.GetChildren().Any()) // Value
                return new JValue(section.Value);

            return ToJson(section as IConfiguration);
        }

        public static JToken ToJson(this IConfiguration configuration)
        {
            var children = configuration.GetChildren().ToArray();
            var keys = children.Select(c => c.Key).ToArray();

            int _;
            if (keys.All(k => int.TryParse(k, out _))) // Array
            {
                var items = children.Select(ToJson).Cast<object>().ToArray();
                return JArray.FromObject(items);
            }

            else // Object
            {
                var properties = children.ToDictionary(c => c.Key, c => c.ToJson());
                return JObject.FromObject(properties);
            }
        }
    }
}