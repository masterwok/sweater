using System.Threading.Tasks;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Contracts
{
    public interface IIndexer
    {
        Task<bool> Login();

        Task<IndexerResult> Query(Query query);

        Task<bool> Logout();
    }
}