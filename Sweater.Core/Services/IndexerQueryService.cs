using System.Collections.Generic;
using System.Threading.Tasks;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;

namespace Sweater.Core.Services
{
    public class IndexerQueryService : IIndexerQueryService
    {
        private IList<IIndexer> _indexers = new List<IIndexer>();

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