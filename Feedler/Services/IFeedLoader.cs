using System.Threading.Tasks;
using Feedler.Models;

namespace Feedler.Services
{
    public interface IFeedLoader
    {
        Task<Item[]> LoadItemsAsync(Feed feed);
    }
}