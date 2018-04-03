using Feedler.Models;
using Newtonsoft.Json;

namespace Feedler.ViewModels
{
    public class ViewFeed
    {
        /// <summary>
        /// Feed id.
        /// </summary>
        [JsonProperty]
        public string Id { get; set; }

        /// <summary>
        /// Feed type.
        /// </summary>
        [JsonProperty]
        public string Type { get; set; }

        public static ViewFeed Create(Feed feed) => new ViewFeed
        {
            Id = feed.Id,
            Type = feed.GetType().Name
        };
    }
}
