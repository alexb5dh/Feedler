using System.Linq;
using System.Threading.Tasks;
using Feedler.DataAccess;
using Feedler.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feedler.Controllers
{
    /// <summary>
    /// Controller for viewing feeds added to the application.
    /// </summary>
    [Route("feeds")]
    public sealed class FeedsController: ControllerBase
    {
        private readonly FeedlerContext _context;

        public FeedsController(FeedlerContext context)
        {
            _context = context;
        }

        // Todo: sorting & pagination
        /// <summary>
        /// Fetches all feeds present in the application.
        /// </summary>
        [HttpGet]
        public async Task<ViewFeed[]> GetFeeds()
        {
            var feeds = await _context.Feeds.ToListAsync();
            return feeds.Select(ViewFeed.Create).ToArray();
        }

        /// <summary>
        /// Fetches feed specified by id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ViewFeed> GetFeed(string id)
        {
            var feed = await _context.Feeds.FindOrThrowAsync(id);
            return ViewFeed.Create(feed);
        }
    }
}