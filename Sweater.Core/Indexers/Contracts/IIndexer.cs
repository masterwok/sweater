using System.Threading.Tasks;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Contracts
{
    public interface IIndexer
    {
        Task<bool> Login();

        Task<IndexerResult> Query(string queryString);

        Task<bool> Logout();
    }
}