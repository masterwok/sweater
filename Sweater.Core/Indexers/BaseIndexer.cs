using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sweater.Core.Clients;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers
{
    /// <summary>
    /// This abstract class defines the workflow for all indexer implementations. It's critical
    /// that all indexers defined within this API derive from this class. This is because it defines
    /// a common interface for injecting, configuring and processing queries.
    /// <p/>
    /// Workflow:
    ///  1. Login
    ///  2. Query
    ///  3. Logout
    /// <p/>
    /// Configure(..) is invoked when initializing the indexer. One strategy would be to allow the
    /// IoC container to take care of this.
    /// </summary>
    public abstract class BaseIndexer : IIndexer
    {
        /// <summary>
        /// A WebClient instance that is used to make requests to indexers.
        /// </summary>
        protected readonly IWebClient WebClient;

        /// <summary>
        /// Create a new indexer instance.
        /// </summary>
        /// <param name="webClient">A WebClient instance for making requests to indexers.</param>
        protected BaseIndexer(IWebClient webClient) => WebClient = webClient;

        /// <summary>
        /// Configure the indexer.
        /// </summary>
        /// <param name="configuration">
        /// An IConfiguration instance representing the configuration model defined within the indexer.
        /// Generally these configurations are stored within a json file (indexers.json).
        /// </param>
        /// <returns>The instance of the indexer.</returns>
        public abstract BaseIndexer Configure(IConfiguration configuration);

        /// <summary>
        /// An asynchronous method for authenticating with the indexer. This is the first method
        /// invoked in the query workflow.
        /// </summary>
        /// <returns>Whether or not the authentication succeeded.</returns>
        public abstract Task<bool> Login();

        /// <summary>
        /// Query the indexer for torrents.
        /// </summary>
        /// <param name="query">The query provided from the client.</param>
        /// <returns>An IndexerResult containing the indexer and torrent results.</returns>
        public abstract Task<IndexerResult> Query(Query query);

        /// <summary>
        /// End authenticated session with the indexer. This method is invoked last in the query
        /// workflow.
        /// </summary>
        /// <returns>Whether or not logging out of the indexer succeeded.</returns>
        public abstract Task<bool> Logout();
    }
}