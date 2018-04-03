using System.Threading.Tasks;
using Feedler.Config;
using Feedler.DataAccess;
using Feedler.Services;
using Feedler.Tests.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ClearExtensions;
using Xunit;

namespace Feedler.Tests
{
    public class FeedlerTestHost: TestHost<Startup>, IAsyncLifetime
    {
        /// <summary>
        /// Context for managing database objects during tests.
        /// </summary>
        protected FeedlerContext FeedlerContext => GetRequiredService<FeedlerContext>();

        protected FeedlerConfig Config => GetRequiredService<FeedlerConfig>();

        protected readonly IFeedLoader FeedLoader = Substitute.For<IFeedLoader>();

        protected FeedlerTestHost()
        {
            // Initialize via Startup class
            base.Init();

            // Use real Redis for testing but with specific prefix
            Config.Redis.Prefix = "Feedler-Test";
        }

        protected override void AfterConfigureServices(IServiceCollection services)
        {
            // Replace db context with in-memory test instance
            services.Remove<FeedlerContext>();
            services.Remove<DbContextOptions<FeedlerContext>>();
            services.AddDbContext<FeedlerContext>(o =>
            {
                o.UseInMemoryDatabase("Test");
            });

            // Replace some of the services with substitutes
            services.ReplaceSingleton(FeedLoader);
        }

        async Task IAsyncLifetime.InitializeAsync() { }

        async Task IAsyncLifetime.DisposeAsync()
        {
            // Cleanup database after each test
            // Todo: consider tracking and removing only entities created during test
            await FeedlerContext.Database.EnsureDeletedAsync();

            // Reset substitutes
            FeedLoader.ClearSubstitute();
        }
    }
}