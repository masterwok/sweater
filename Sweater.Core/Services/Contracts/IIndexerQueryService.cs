using System.Collections.Generic;
using System.Threading.Tasks;
using Sweater.Core.Models;

namespace Sweater.Core.Services.Contracts
{
    public interface IIndexerQueryService
    {
        Task<IEnumerable<string>> GetIndexerTags();

        Task<IEnumerable<IndexerResult>> Query(Query query);
    }
}