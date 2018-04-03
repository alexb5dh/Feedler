using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feedler.Models;
using Microsoft.EntityFrameworkCore;

namespace Feedler.DataAccess
{
    public static class FeedlerContextExtensions
    {
        /// <summary>
        /// Loads <see cref="Collection"/> from <paramref name="db"/> including all it's feeds.
        /// </summary>
        public static Task<Collection> GetWithFeedsAsync(this DbSet<Collection> db, Guid id) =>
            db.Include(c => c.CollectionFeeds).ThenInclude(cf => cf.Feed).FirstOrDefaultAsync(c => c.Id == id);

        /// <summary>
        /// Loads all <see cref="Collection"/>s from <paramref name="db"/> including feeds for each one.
        /// </summary>
        public static Task<List<Collection>> ToListWithFeedsAsync(this DbSet<Collection> db) =>
            db.Include(c => c.CollectionFeeds).ThenInclude(cf => cf.Feed).ToListAsync();
    }
}