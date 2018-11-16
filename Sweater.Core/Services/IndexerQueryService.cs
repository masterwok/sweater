using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sweater.Core.Clients;
using Sweater.Core.Indexers;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Indexers.Public;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;

namespace Sweater.Core.Services
{
    /// <summary>
    /// An IIndexerQueryService implementation. This service should be injected as transient so a
    /// new instance is created for each request. Failure to do so will result
    /// </summary>
    public class IndexerQueryService : IIndexerQueryService
    {
        private static readonly Dictionary<string, Func<IWebClient, IIndexer>> IndexerFactory =
            new Dictionary<string, Func<IWebClient, IIndexer>>
            {
                {ThePirateBayIndexer.Tag, webClient => new ThePirateBayIndexer(webClient)}
            };

        private IList<IIndexer> _indexers = new List<IIndexer>();

        private readonly Func<IWebClient> _createWebClient;

        /// <summary>
        /// Create a new IndexerQueryService instance.
        /// </summary>
        /// <param name="createWebClient">A IWebClient factory method (used for mocking).</param>
        public IndexerQueryService(Func<IWebClient> createWebClient)
        {
            _createWebClient = createWebClient;
        }

        public Task<IEnumerable<string>> GetIndexerTags() => Task
            .FromResult(IndexerFactory.Keys.AsEnumerable());

        public async Task<IEnumerable<IndexerResult>> Query(Query query)
        {
            // TODO: Actually query..
            return new List<IndexerResult>
            {
                new IndexerResult
                {
                    Indexer = null,
                    Torrents = new List<Torrent>()
                    {
                        new Torrent {Name = "Hackers 1995", UploadedOn = DateTime.Now},
                        new Torrent {Name = "Hackers", UploadedOn = DateTime.Now}
                    }
                }
            };
        }

        public async Task<IndexerResult> QueryByIndexer(
            string tag
            , string queryString
        )
        {
            var indexer = GetIndexerByTagOrThrow(tag);

            var loginSuccessful = await indexer.Login();
            var result = await indexer.Query(queryString);
            var logoutSuccessful = await indexer.Logout();

            return result;
        }


        private IIndexer GetIndexerByTagOrThrow(string tag) => IndexerFactory.ContainsKey(tag)
            ? IndexerFactory[tag](_createWebClient.Invoke())
            : throw new KeyNotFoundException($"Torrent indexer tag not registered: {tag}");
    }
}