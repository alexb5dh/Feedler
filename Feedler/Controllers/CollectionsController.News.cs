﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Feedler.DataAccess;
using Feedler.Extensions.Collections;
using Feedler.Services;
using Feedler.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Feedler.Controllers
{
    public sealed partial class CollectionsController
    {
        // Todo: pagination & filtering
        /// <summary>
        /// Fetches all news from all feeds present in the collection.
        /// News for each feed may be loaded from periodically flushed cache.
        /// </summary>
        [HttpGet("{id}/news")]
        public async Task<ViewItem[]> GetCollectionNewsAsync(Guid id,
            [FromServices] IFeedLoader feedLoader)
        {
            var collection = await _context.Collections.GetWithFeedsAsync(id);
            var feedsById = collection.Feeds.ToDictionary(f => f.Id);

            // Load in parallel
            var newsByFeed = await _feedCache.GetFeedsAsync(feedsById.Keys.ToHashSet());
            var news = await Task.WhenAll(feedsById.Values.Select(async feed =>
            {
                var feedNews = newsByFeed.GetValueOrDefault(feed.Id);
                if (feedNews == null)
                {
                    feedNews = await feedLoader.LoadItemsAsync(feed);
                    await _feedCache.SetFeedAsync(feed.Id, feedNews);
                }

                return feedNews;
            }));

            return news.SelectMany(ns => ns).Select(ViewItem.Create).ToArray();
        }
    }
}
