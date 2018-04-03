using System.Collections.Generic;

namespace Feedler.Extensions.Collections
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) =>
            dictionary.TryGetValue(key, out var value) ? value : default(TValue);
    }
}