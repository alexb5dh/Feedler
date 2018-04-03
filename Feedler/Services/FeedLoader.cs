using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Feedler.Models;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;

namespace Feedler.Services
{
    public sealed class FeedLoader: IFeedLoader
    {
        /// <summary>
        /// Loads all items for specified feed.
        /// </summary>
        public Task<Item[]> LoadItemsAsync(Feed feed)
        {
            switch (feed)
            {
                case RSSFeed rssFeed:
                {
                    return LoadItemsAsync(rssFeed);
                }
                default:
                {
                    throw new NotSupportedException($"{feed.GetType().Name} is not supported.");
                }
            }
        }

        private async Task<Item[]> LoadItemsAsync(RSSFeed feed)
        {
            using (var xmlReader = XmlReader.Create(feed.Uri))
            {
                var items = new List<Item>();

                var feedReader = new RssFeedReader(xmlReader);
                while(await feedReader.Read())
                {
                    if (feedReader.ElementType != SyndicationElementType.Item) continue;

                    var feedItem = await feedReader.ReadItem();
                    items.Add(new Item
                    {
                        Title = feedItem.Title,
                        Link = feedItem.Links.FirstOrDefault()?.Uri?.ToString(),
                        Description = feedItem.Description,
                        Author = feedItem.Contributors.FirstOrDefault()?.Email,
                        PublishedAt = feedItem.Published.DateTime
                    });
                }

                return items.ToArray();
            }
        }
    }
}