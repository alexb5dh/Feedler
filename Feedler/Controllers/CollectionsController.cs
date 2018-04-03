using System;
using System.Linq;
using System.Threading.Tasks;
using Feedler.DataAccess;
using Feedler.Models;
using Feedler.Services;
using Feedler.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Feedler.Controllers
{
    /// <summary>
    /// Controller for managing feed collections and fetching collection news.
    /// </summary>
    [Route("collections")]
    public sealed partial class CollectionsController: ControllerBase
    {
        private readonly ILogger<CollectionsController> _logger;
        private readonly FeedlerContext _context;
        private readonly IFeedCache _feedCache;

        public CollectionsController(ILogger<CollectionsController> logger,
            FeedlerContext context, IFeedCache feedCache)
        {
            _logger = logger;
            _context = context;
            _feedCache = feedCache;
        }

        // Todo: sorting & pagination
        /// <summary>
        /// Fetches all collections present in the application.
        /// </summary>
        [HttpGet]
        public async Task<ViewCollection[]> GetCollectionsAsync()
        {
            var collections = await _context.Collections.ToListWithFeedsAsync();
            return collections.Select(ViewCollection.Create).ToArray();
        }

        /// <summary>
        /// Fetches collection specified by id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ViewCollection> GetCollectionAsync(Guid id)
        {
            var collection = await _context.Collections.GetWithFeedsAsync(id);
            return ViewCollection.Create(collection);
        }

        /// <summary>
        /// Creates new collection.
        /// </summary>
        [HttpPost]
        public async Task<ViewCollection> CreateCollectionAsync([FromBody] CreateCollection createCollection)
        {
            var collection = createCollection.GetModel();

            _context.Collections.Add(collection);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created collection #{collection.Id}.");

            return ViewCollection.Create(collection);
        }

        /// <summary>
        /// Creates new collection.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task RemoveCollectionAsync(Guid id)
        {
            // Todo: consider removing with single query
            var collection = await _context.Collections.FindOrThrowAsync(id);

            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Removed collection #{id}.");
        }

        /// <summary>
        /// Adds feed to collection.
        /// </summary>
        [HttpPost("{id}/feed/{feedId}")]
        public async Task<ViewCollection> AddFeedToCollection(Guid id, string feedId)
        {
            var collection = await _context.Collections.GetWithFeedsAsync(id);

            if (!collection.Feeds.ContainsId(feedId))
            {
                var feed = await _context.Feeds.FindOrThrowAsync(feedId);
                collection.Feeds.Add(feed);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Added feed \"{feed.Id}\" to collection #{collection.Id}.");
            }

            return ViewCollection.Create(collection);
        }

        /// <summary>
        /// Removes feed from collection.
        /// </summary>
        [HttpDelete("{id}/feed/{feedId}")]
        public async Task<ViewCollection> RemoveFeedFromCollectionAsync(Guid id, string feedId)
        {
            var collection = await _context.Collections.GetWithFeedsAsync(id);

            var feed = collection.Feeds.FindById(feedId);
            if (feed != null)
            {
                collection.Feeds.Remove(feed);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Removed feed \"{feed.Id}\" from collection #{collection.Id}.");
            }

            return ViewCollection.Create(collection);
        }
    }
}