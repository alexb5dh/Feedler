using System;
using System.Linq;
using System.Reflection;
using Feedler.Models;
using Microsoft.EntityFrameworkCore;

namespace Feedler.DataAccess
{
    public sealed class FeedlerContext: DbContext
    {
        public DbSet<Collection> Collections { get; private set; }

        public DbSet<Feed> Feeds { get; private set; }

        public FeedlerContext(DbContextOptions<FeedlerContext> options): base(options)
        {
            GC.KeepAlive(this);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Feed-Collection M2M relationship
            modelBuilder.Entity<CollectionFeed>()
                .HasKey("CollectionId", "FeedId");
            modelBuilder.Entity<CollectionFeed>()
                .HasOne(cf => cf.Collection)
                .WithMany("CollectionFeeds")
                .HasForeignKey("CollectionId");
            modelBuilder.Entity<CollectionFeed>()
                .HasOne(cf => cf.Feed)
                .WithMany()
                .HasForeignKey("FeedId");

            // Feeds as TPH
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(Feed).IsAssignableFrom(t)))
            {
                modelBuilder.Entity(type);
            }
        }
    }
}