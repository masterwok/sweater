using System.Collections.Generic;
using Sweater.Core.Indexers.Contracts;

namespace Sweater.Core.Models
{
    /// <summary>
    /// This model represents a collection of torrents found on an indexer.
    /// </summary>
    public sealed class IndexerResult
    {
        /// <summary>
        /// The indexer where this result was found.
        /// </summary>
        public IIndexer Indexer { get; set; }

        /// <summary>
        /// A collection of torrents found on this indexer.
        /// </summary>
        public IEnumerable<Torrent> Torrents { get; set; }
    }
}