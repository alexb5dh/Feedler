using System;
using System.Collections.Generic;

namespace Feedler.Extensions.Collections
{
    public class StringSet: HashSet<string>
    {
        public StringSet(): base(StringComparer.OrdinalIgnoreCase) { }

        public StringSet(IEnumerable<string> collection): base(collection, StringComparer.OrdinalIgnoreCase) { }
    }
}