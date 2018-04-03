using System.IO;
using System.Threading.Tasks;
using Feedler.Config;
using Feedler.DataAccess;
using Feedler.Extensions;
using Feedler.Extensions.MVC;
using Feedler.Extensions.MVC.ExceptionHandling;
using Feedler.Extensions.MVC.Validation;
using Feedler.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace Feedler
{
    public sealed class Startup
    {
        private readonly FeedlerConfig _config;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            // JSON is used as it's has wider support for configuring class-to-options mapping
            _config = configuration.ToJson().ToObject<FeedlerConfig>();
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);

            services.AddSingleton<IFeedLoader, FeedLoader>();
            services.AddSingleton<IFeedCache, RedisFeedCache>();

            services.AddMvc(o =>
            {
                o.Filters.Add(new ModelValidationFilter());
                o.Filters.Add(new ValidateActionParametersAttribute());
            });

            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new Info { Title = "Feedler", Version = "v1" });

                o.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Feedler.xml"));
            });

            services.AddDbContext<FeedlerContext>(o =>
            {
                o.UseSqlServer(_config.ConnectionStrings.Feedler);
            });
        }

        public void Configure(IApplicationBuilder app, FeedlerContext context)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>(new ExceptionHandlingMiddleware.Options
            {
                ForwardExceptions = _config.ForwardExceptions
            });

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("/swagger/v1/swagger.json", "Feedler");
            });

            if (context.Database.IsSqlServer())
            {
                _logger.LogInformation("Applying migrations.");
                var migrationTime = Timer.Time(() => context.Database.Migrate());
                _logger.LogInformation($"Applied migrations in {migrationTime}.");
            }

            SeedConfigFeedsAsync(context).Await();
        }

        // Feed sources are assumed to be added to db directly or via config.
        // This method is used for the latter.
        // Todo: move to db context class
        private async Task SeedConfigFeedsAsync(FeedlerContext context)
        {
            _logger.LogInformation("Seeding feed sources from config.");

            var seedingTime = await Timer.TimeAsync(async () =>
            {
                // Todo: consider optimizing seeding to avoid loading all entities to memory
                var addedFeeds = await context.Feeds.ToDictionaryAsync(f => f.Id);
                foreach (var feed in _config.Seeding.Feeds)
                {
                    var addedFeed = addedFeeds.GetValueOrDefault(feed.Id);
                    if (addedFeed == null)
                    {
                        context.Feeds.Add(feed);

                        _logger.LogInformation($"Added feed \"{feed.Id}\".");
                    }
                    else if (!feed.DeepEquals(addedFeed))
                    {
                        context.Feeds.Remove(addedFeed);
                        context.Feeds.Add(feed);
                        context.Entry(feed).State = EntityState.Modified;

                        _logger.LogInformation($"Updated feed \"{feed.Id}\".");
                    }
                }

                await context.SaveChangesAsync();
            });

            _logger.LogInformation($"Seeded feed sources in {seedingTime}.");
        }
    }
}