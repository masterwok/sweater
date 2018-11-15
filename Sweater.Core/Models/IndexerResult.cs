using System.Collections.Generic;
using Sweater.Core.Indexers.Contracts;

namespace Sweater.Core.Models
{
    /**
     * This model represents a collection of torrents found on an indexer.
     */
    public sealed class IndexerResult
    {
        /***
         * The indexer where this result was found.
         */
        public IIndexer Indexer { get; set; }

        /**
         * A collection of torrents found on this indexer.
         */
        public IEnumerable<Torrent> Torrents { get; set; }
    }
}