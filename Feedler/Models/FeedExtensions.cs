using System;
using System.Collections.Generic;
using System.Linq;

namespace Feedler.Models
{
    public static class FeedExtensions
    {
        public static bool ContainsId(this IEnumerable<Feed> feeds, string id) =>
            feeds.Select(f => f.Id).Contains(id, StringComparer.OrdinalIgnoreCase);

        public static Feed GetById(this IEnumerable<Feed> feeds, string id) =>
            feeds.FirstOrDefault(f => string.Equals(f.Id, id, StringComparison.OrdinalIgnoreCase));
    }
}
