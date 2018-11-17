using System.Collections.Generic;
using System.Linq;
using Sweater.Core.Constants;

namespace Sweater.Core.Models
{
    /// <summary>
    /// This model represents a collection of torrents found on an indexer.
    /// </summary>
    public sealed class IndexerResult
    {
        /// <summary>
        /// The unique identifier of the indexer.
        /// </summary>
        public string Indexer { get; set; }

        /// <summary>
        /// A collection of torrents found on this indexer.
        /// </summary>
        public IEnumerable<Torrent> Torrents { get; set; }

        /// <summary>
        /// The total number of torrent items in the result.
        /// </summary>
        public int Count => Torrents?.Count() ?? 0;
    }
}