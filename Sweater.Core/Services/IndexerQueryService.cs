using System.Collections.Generic;
using System.Threading.Tasks;
using Sweater.Core.Models;

namespace Sweater.Core.Services.Contracts
{
    public class IndexerQueryService : IIndexerQueryService
    {
        public Task LoadIndexers()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IndexerResult>> Query(string queryString)
        {
            throw new System.NotImplementedException();
        }
    }
}