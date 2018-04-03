using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Feedler.Extensions.Collections;

namespace Feedler.Models
{
    public sealed class Collection
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; private set; }

        private Collection() { }

        public Collection(string name, params Feed[] feeds)
        {
            Name = name;
            Feeds.AddRange(feeds);
        }

        // Todo: remove once EF many2many is fixed (see https://github.com/aspnet/EntityFrameworkCore/issues/1368)
        public ICollection<CollectionFeed> CollectionFeeds { get; private set; } = new List<CollectionFeed>();

        [NotMapped]
        public ICollection<Feed> Feeds => CollectionFeeds.Wrap(cf => cf.Feed, f => new CollectionFeed(this, f));
    }
}