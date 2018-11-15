using System.Collections.Generic;
using System.Threading.Tasks;
using Sweater.Core.Models;

namespace Sweater.Core.Services.Contracts
{
    public interface IIndexerQueryService
    {
        Task LoadIndexers();

        Task<IEnumerable<IndexerResult>> Query(string queryString);
    }
}