using System;
using System.Collections.Generic;
using System.Linq;
using Feedler.Extensions;
using Feedler.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Feedler.Config
{
    public class SeedingConfig
    {
        [JsonProperty("Feeds")]
        private JObject[] FeedsObjects { get; set; } = new JObject[0];

        private Feed[] _feeds;

        [JsonIgnore]
        public Feed[] Feeds
        {
            get => _feeds ?? (_feeds = ParseFeeds().ToArray());
            set => _feeds = value;
        }

        private IEnumerable<Feed> ParseFeeds()
        {
            foreach (var feed in FeedsObjects)
            {
                switch (feed.String("Type").ToLower())
                {
                    case "rss":
                    {
                        yield return new RSSFeed(feed.String("Id"), feed.String("Uri"));
                        break;
                    }
                    default:
                    {
                        throw new NotSupportedException($"{feed.Type} is not supported.");
                    }
                }
            }
        }
    }
}