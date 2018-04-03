using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Feedler.Controllers;
using Feedler.Extensions;
using Feedler.Extensions.Collections;
using Feedler.Models;
using Feedler.Tests.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace Feedler.Tests
{
    public class CollectionTests: FeedlerTestHost
    {
        [TestFor(nameof(CollectionsController.GetCollectionsAsync))]
        public async Task GetCollections_ShouldReturnAllCollections()
        {
            // Arrange
            var testCollections = Create.Many(Create.Collection).Take(3).ToArray();
            FeedlerContext.Collections.AddRange(testCollections);
            await FeedlerContext.SaveChangesAsync();

            // Act
            var response = await Client.GetAsync("collections");
            var responseJson = await response.Content.ReadAsAsync<JArray>();

            // Assert: response
            await HttpAssert.IsSuccessResponseAsync(response);

            Assert.Equal(
                expected: testCollections.Select(c => c.Id).ToHashSet(),
                actual: responseJson.Select(j => j["id"].ToObject<Guid>()).ToHashSet()
            );
        }

        [TestFor(nameof(CollectionsController.CreateCollectionAsync))]
        public async Task PostCollection_ShouldCreateCollection_AndReturnIt()
        {
            // Act
            var request = new
            {
                Name = Create.Name()
            };
            var response = await Client.PostAsJsonAsync("collections", request);

            // Assert: response
            await HttpAssert.IsSuccessResponseAsync(response);
            var responseJson = await response.Content.ReadAsAsync<JObject>();

            Assert.Equal(
                expected: new { Name = request.Name },
                actual: new { Name = responseJson["name"].ToObject<string>() }
            );

            // Assert: database
            var collectionId = responseJson["id"].ToObject<Guid>();
            var collection = await FeedlerContext.Collections.FindAsync(collectionId);

            Assert.Equal(
                expected: new { Name = request.Name },
                actual: new { Name = collection.Name }
            );
        }

        [TestFor(nameof(CollectionsController.RemoveCollectionAsync))]
        public async Task DeleteCollection_ShouldRemoveCollection()
        {
            // Arrange
            var collection = Create.Collection();
            FeedlerContext.Collections.Add(collection);
            await FeedlerContext.SaveChangesAsync();

            // Act
            var response = await Client.DeleteAsync($"collections/{collection.Id}");

            // Assert: response
            await HttpAssert.IsSuccessResponseAsync(response);

            // Assert: database
            Assert.Null(await FeedlerContext.Collections.Where(c => c.Id == collection.Id).FirstOrDefaultAsync());
        }

        [TestFor(nameof(CollectionsController.GetCollectionNewsAsync))]
        public async Task GetNewsFromCollection_ShouldFetchCollectionNews()
        {
            // Arrange
            var feeds = Create.Many(Create.RSSFeed).Cast<Feed>().Take(2).ToArray();
            FeedlerContext.Feeds.AddRange(feeds);

            var collection = Create.Collection(feeds);
            FeedlerContext.Collections.Add(collection);

            await FeedlerContext.SaveChangesAsync();

            var itemsByFeed = new Dictionary<Feed, Item[]>();
            foreach (var feed in feeds)
            {
                itemsByFeed[feed] = Create.Many(Create.Item).Take(3).ToArray();
                FeedLoader.LoadItemsAsync(feed).Returns(itemsByFeed[feed]);
            }

            // Act
            var response = await Client.GetAsync($"collections/{collection.Id}/news");

            // Assert: response
            await HttpAssert.IsSuccessResponseAsync(response);
            var responseJson = await response.Content.ReadAsAsync<JArray>();

            Assert.Equal(
                expected: itemsByFeed.Values.Flatten().Select(i => new { Title = i.Title }).ToHashSet(),
                actual: responseJson.Select(j => new { Title = j.String("title") }).ToHashSet()
            );
        }

        [TestFor(nameof(CollectionsController.GetCollectionNewsAsync))]
        public async Task GetNewsFromCollection_ShouldLoadNewsFromSource_IfRequestIsNotCached()
        {
            // Arrange
            var feed = Create.RSSFeed();
            FeedlerContext.Feeds.Add(feed);

            var collection = Create.Collection(feed);
            FeedlerContext.Collections.Add(collection);

            await FeedlerContext.SaveChangesAsync();

            var items = Create.Many(Create.Item).Take(5).ToArray();
            FeedLoader.LoadItemsAsync(feed).Returns(items);

            // Act
            var response = await Client.GetAsync($"collections/{collection.Id}/news");

            // Assert: response
            await HttpAssert.IsSuccessResponseAsync(response);

            // Assert: calls
            await FeedLoader.Received().LoadItemsAsync(feed);
        }

        [TestFor(nameof(CollectionsController.GetCollectionNewsAsync))]
        public async Task GetNewsFromCollection_ShouldLoadNewsFromCache_IfRequestIsCached()
        {
            // Arrange
            var feed = Create.RSSFeed();
            FeedlerContext.Feeds.Add(feed);

            var collection = Create.Collection(feed);
            FeedlerContext.Collections.Add(collection);

            await FeedlerContext.SaveChangesAsync();

            var items = Create.Many(Create.Item).Take(5).ToArray();
            FeedLoader.LoadItemsAsync(feed).Returns(items);

            // Act
            var response1 = await Client.GetAsync($"collections/{collection.Id}/news");
            await HttpAssert.IsSuccessResponseAsync(response1);

            FeedLoader.ClearReceivedCalls();

            var response2 = await Client.GetAsync($"collections/{collection.Id}/news");
            await HttpAssert.IsSuccessResponseAsync(response2);

            // Assert: calls
            await FeedLoader.DidNotReceive().LoadItemsAsync(feed);
        }
    }
}