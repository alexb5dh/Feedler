using Newtonsoft.Json;

namespace Feedler.Config
{
    public class ConnectionStringsConfig
    {
        [JsonProperty(Required = Required.Always)]
        public string Feedler { get; set; }
    }
}