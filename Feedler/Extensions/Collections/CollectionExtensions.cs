using System.Collections.Generic;
using System.Linq;

namespace Feedler.Extensions.Collections
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items) collection.Add(item);
        }

        public static void AddRange<T>(this ICollection<T> collection, params T[] items) =>
            collection.AddRange(items.AsEnumerable());
    }
}