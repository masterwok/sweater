using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Contracts
{
    public interface IIndexer
    {
        string Tag { get; }

        Task<IEnumerable<Torrent>> Query(Query query, CancellationToken cancellationToken);

    }
}