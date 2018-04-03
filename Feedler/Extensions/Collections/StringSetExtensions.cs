using System.Collections.Generic;

namespace Feedler.Extensions.Collections
{
    public static class StringSetExtensions
    {
        public static StringSet ToStringSet(this IEnumerable<string> source)
        {
            var set = new StringSet();
            set.AddRange(source);
            return set;
        }
    }
}