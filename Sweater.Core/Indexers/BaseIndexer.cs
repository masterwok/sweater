using System.Collections.Generic;
using System.Threading.Tasks;
using Sweater.Core.Clients.Contracts;
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
        /// A unique indexer tag.
        /// </summary>
        public abstract string Tag { get; }

        /// <summary>
        /// An IHttpClient instance that is used to make requests to indexers.
        /// </summary>
        protected readonly IHttpClient HttpClient;

        /// <summary>
        /// Create a new indexer instance.
        /// </summary>
        /// <param name="httpClient">An IHttpClient instance for making requests to indexers.</param>
        protected BaseIndexer(IHttpClient httpClient) => HttpClient = httpClient;

        /// <summary>
        /// An asynchronous method for authenticating with the indexer. This is the first method
        /// invoked in the query workflow.
        /// </summary>
        /// <returns>Whether or not the authentication succeeded.</returns>
        public abstract Task Login();

        /// <summary>
        /// Query the indexer for torrents.
        /// </summary>
        /// <param name="query">The query provided from the client.</param>
        /// <returns>An enumerable collection of torrent results.</returns>
        public abstract Task<IEnumerable<Torrent>> Query(Query query);

        /// <summary>
        /// End authenticated session with the indexer. This method is invoked last in the query
        /// workflow.
        /// </summary>
        public abstract Task Logout();
    }
}