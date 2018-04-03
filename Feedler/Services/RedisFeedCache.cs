using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feedler.Config;
using Feedler.Extensions.Collections;
using Feedler.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Feedler.Services
{
    public sealed class RedisFeedCache: IFeedCache, IDisposable
    {
        private readonly RedisConfig _config;
        private readonly ConnectionMultiplexer _redis;

        private IDatabase Database => _redis.GetDatabase();

        private RedisKey Key(string id) => _config.Prefix == null ? id : $"{_config.Prefix}.{id}";

        private RedisValue Serialize(Item[] items) => items == null ? null : JsonConvert.SerializeObject(items);

        private Item[] Deserialize(string @string) => @string == null ? null : JsonConvert.DeserializeObject<Item[]>(@string);

        public RedisFeedCache(FeedlerConfig config)
        {
            _config = config.Redis;
            _redis = ConnectionMultiplexer.Connect(config.Redis.Endpoint);
        }

        public Task SetFeedAsync(string id, Item[] items)
        {
            return Database.StringSetAsync(Key(id), Serialize(items), _config.TTL);
        }

        public async Task SetFeedsAsync(IEnumerable<KeyValuePair<string, Item[]>> pairs)
        {
            var redisPairs = pairs.Select(p => KeyValuePair.Create(Key(p.Key), Serialize(p.Value)))
                .ToArray();

            // Todo: find simpler approach for multi-set with TTL
            var transaction = Database.CreateTransaction();
#pragma warning disable 4014
            transaction.StringSetAsync(redisPairs);
            foreach (var pair in redisPairs) transaction.KeyExpireAsync(pair.Key, _config.TTL);
#pragma warning restore 4014
            await transaction.ExecuteAsync();
        }

        public async Task<IDictionary<string, Item[]>> GetFeedsAsync(ISet<string> ids)
        {
            var redisKeys = ids.Select(Key).ToArray();

            var redisValues = await Database.StringGetAsync(redisKeys);
            return ids.Zip(redisValues, (k, v) => KeyValuePair.Create(k, Deserialize(v)))
                .ToDictionary();
        }

        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}