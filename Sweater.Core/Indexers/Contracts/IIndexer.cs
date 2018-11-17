using System.Collections.Generic;
using System.Threading.Tasks;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Contracts
{
    public interface IIndexer
    {
        string Tag { get; }

        Task Login();

        Task<IEnumerable<Torrent>> Query(Query query);

        Task Logout();
    }
}