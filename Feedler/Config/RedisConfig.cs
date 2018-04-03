using System;
using Newtonsoft.Json;

namespace Feedler.Config
{
    public sealed class RedisConfig
    {
        [JsonProperty(Required = Required.Always)]
        public string Endpoint { get; set; }

        [JsonProperty]
        public string Prefix { get; set; } = null;

        [JsonProperty(Required = Required.Always)]
        public TimeSpan TTL { get; set; }
    }
}