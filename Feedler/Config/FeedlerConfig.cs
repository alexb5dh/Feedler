using Newtonsoft.Json;

namespace Feedler.Config
{
    public sealed class FeedlerConfig
    {
        [JsonProperty(Required = Required.Always)]
        public ConnectionStringsConfig ConnectionStrings;

        [JsonProperty]
        public bool ForwardExceptions { get; set; }

        [JsonProperty(Required = Required.Always)]
        public RedisConfig Redis { get; set; }

        [JsonProperty]
        public SeedingConfig Seeding { get; set; } = new SeedingConfig();
    }
}