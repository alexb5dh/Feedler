using System;
using System.Linq;
using Feedler.Models;
using Newtonsoft.Json;

namespace Feedler.ViewModels
{
    public sealed class ViewCollection
    {
        /// <summary>
        /// Collection id.
        /// </summary>
        [JsonProperty]
        public Guid Id { get; set; }

        /// <summary>
        /// Collection name.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Feeds added to collection.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ViewFeed[] Feeds { get; set; }

        public static ViewCollection Create(Collection collection) => new ViewCollection
        {
            Id = collection.Id,
            Name = collection.Name,
            Feeds = collection.Feeds.Select(ViewFeed.Create).ToArray()
        };
    }
}