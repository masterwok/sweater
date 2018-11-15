using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sweater.Core.Clients;
using Sweater.Core.Indexers;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;

namespace Sweater.Core.Services
{
    public class IndexerQueryService : IIndexerQueryService
    {
        private IList<BaseIndexer> _indexers = new List<BaseIndexer>();

        private readonly Func<IWebClient> _createWebClient;

        /// <summary>
        /// Create a new IndexerQueryService instance.
        /// </summary>
        /// <param name="createWebClient">A IWebClient factory method (used for mocking).</param>
        public IndexerQueryService(
            Func<IWebClient> createWebClient
        )
        {
            _createWebClient = createWebClient;
        }

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