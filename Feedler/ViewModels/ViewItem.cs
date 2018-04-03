using System;
using Feedler.Models;
using Newtonsoft.Json;

namespace Feedler.ViewModels
{
    public class ViewItem
    {
        [JsonProperty]
        public string Title { get; set; }

        [JsonProperty]
        public string Link { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public string Author { get; set; }

        [JsonProperty]
        public DateTime PublishedAt { get; set; }

        public static ViewItem Create(Item item) => new ViewItem
        {
            Title = item.Title,
            Link = item.Link,
            Description = item.Description,
            Author = item.Author,
            PublishedAt = item.PublishedAt
        };
    }
}
