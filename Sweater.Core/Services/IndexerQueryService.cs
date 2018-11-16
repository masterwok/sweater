using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sweater.Core.Clients;
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
        // TODO: Is there a better way to register these?
        private static readonly Dictionary<string, Func<IWebClient, Func<string, IConfigurationSection>, IIndexer>> IndexerFactory
            = new Dictionary<string, Func<IWebClient, Func<string, IConfigurationSection>, IIndexer>>
            {
                {
                    ThePirateBayIndexer.Tag, (webClient, configSection) => new ThePirateBayIndexer(webClient, configSection)
                }
            };

        private readonly Func<string, IConfigurationSection> _readConfigSection;
        private readonly Func<IWebClient> _createWebClient;
        private readonly QueryConfig _queryConfig;

        /// <summary>
        /// Create a new IndexerQueryService instance.
        /// </summary>
        /// <param name="readConfigSection">
        /// Func used to read indexer configuration by section name.
        /// </param>
        /// <param name="createWebClient">A IWebClient factory method (used for mocking).</param>
        /// <param name="queryConfig">Query service configuration.</param>
        public IndexerQueryService(
            Func<string, IConfigurationSection> readConfigSection
            , Func<IWebClient> createWebClient
            , QueryConfig queryConfig
        )
        {
            _createWebClient = createWebClient;
            _readConfigSection = readConfigSection;
            _queryConfig = queryConfig;
        }

        public Task<IEnumerable<string>> GetIndexerTags() => Task
            .FromResult(IndexerFactory.Keys.AsEnumerable());

        public async Task<IEnumerable<IndexerResult>> Query(Query query)
        {
            var indexer = GetIndexerByTagOrThrow(query.IndexerTag);

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
            ? IndexerFactory[tag](_createWebClient.Invoke(), _readConfigSection)
            : throw new KeyNotFoundException($"Torrent indexer tag not registered: {tag}");
    }
}