using System.Collections.Generic;
using System.Threading.Tasks;
using Feedler.Models;

namespace Feedler.Services
{
    public interface IFeedCache
    {
        Task SetFeedAsync(string id, Item[] items);

        Task SetFeedsAsync(IEnumerable<KeyValuePair<string, Item[]>> pairs);

        Task<IDictionary<string, Item[]>> GetFeedsAsync(ISet<string> ids);
    }
}