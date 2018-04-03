using System;
using System.Collections.Generic;
using System.Linq;
using Feedler.Models;

namespace Feedler.Tests
{
    public static class Create
    {
        public static string Name() => $"Test #{Guid.NewGuid()}";

        public static IEnumerable<T> Many<T>(Func<T> factory) =>
            Enumerable.Range(1, int.MaxValue).Select(_ => factory());

        public static Collection Collection() => new Collection(name: "Test collection");

        public static Collection Collection(params Feed[] feeds) => new Collection(name: "Test collection", feeds: feeds);

        public static RSSFeed RSSFeed() => new RSSFeed(id: $"Feed#{Guid.NewGuid()}", uri: "http://test.com/rss");

        public static Item Item()
        {
            var id = Guid.NewGuid();
            return new Item
            {
                Author = $"author@test.com",
                Description = "Test description",
                Link = $"test.com/news/{id}",
                PublishedAt = DateTime.UtcNow,
                Title = $"Item #{id}"
            };
        }
    }
}