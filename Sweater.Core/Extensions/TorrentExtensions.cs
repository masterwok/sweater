using System.Collections.Generic;
using System.Linq;
using Sweater.Core.Models;

namespace Sweater.Core.Extensions
{
    /// <summary>
    /// This class contains a collection of extension methods for IndexerResult.
    /// </summary>
    public static class TorrentExtensions
    {
        /// <summary>
        /// Convert an enumerable of IndexerResult instances to a list of TorrentQueryResult
        /// instances.
        /// </summary>
        /// <param name="indexerResults">The results to flatten.</param>
        /// <returns>A flattened list of TorrentQueryResult instances.</returns>
        public static IList<TorrentQueryResult> FlattenIndexerResults(
            this IEnumerable<IndexerResult> indexerResults
        ) => indexerResults
            ?.SelectMany(r => r.Torrents
                .Select(t => new TorrentQueryResult
                {
                    Indexer = r.Indexer.ToString(),
                    MagnetUri = t.MagnetUri,
                    Name = t.Name,
                    Size = t.Size,
                    Seeders = t.Seeders,
                    Leechers = t.Leechers,
                    UploadedOn = t.UploadedOn
                }))
            .DistinctBy(t => t.MagnetUri)
            .OrderByDescending(t => t.Seeders)
            .ToList();
    }
}