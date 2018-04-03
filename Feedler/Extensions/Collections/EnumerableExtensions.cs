using System;
using System.Collections.Generic;
using System.Linq;

namespace Feedler.Extensions.Collections
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source) =>
            source.SelectMany(i => i);

        public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
            source.Select((t, i) => (item: t, index: i)).First(a => predicate(a.item)).index;

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> pairs, IEqualityComparer<TKey> comparer = null)
        {
            return pairs.ToDictionary(p => p.Key, p => p.Value, comparer);
        }
    }
}