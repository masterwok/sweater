using System.Threading.Tasks;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Contracts
{
    public interface IIndexer
    {
        Task Login();

        Task<IndexerResult> Query(Query query);

        Task Logout();
    }
}