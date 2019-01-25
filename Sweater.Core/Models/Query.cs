using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sweater.Core.Constants;

namespace Sweater.Core.Models
{
    /// <summary>
    /// This model is accepted by the IndexerController query actions and is then
    /// passed down to indexers which then convert the query into indexer specific queries.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class Query
    {
        /// <summary>
        /// What indexers to use for the query.
        /// </summary>
        public Indexer[] Indexers { get; set; } = new Indexer[0];

        /// <summary>
        /// The query string (i.e. what the user is trying to find)
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// What torrent attribute to sort by.
        /// </summary>
        public SortField SortBy { get; set; }

        /// <summary>
        /// What direction to order sorted attribute by.
        /// </summary>
        public SortOrder OrderBy { get; set; }

        public override string ToString() => $"Indexer Tag = {IndexersString}, " +
                                             $"Query String = {QueryString}";

        /// <summary>
        /// Create a comma-delimited string of indexers specified in this query.
        /// </summary>
        private string IndexersString => string.Join(
            ", "
            , Indexers.Select(i => i.ToString())
        );
    }
}