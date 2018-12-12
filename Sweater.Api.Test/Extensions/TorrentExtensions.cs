using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using NUnit.Framework;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Models;

namespace Sweater.Api.Test.Extensions
{
    [TestFixture]
    public class TorrentExtensions
    {
        private static readonly Fixture Fixture = new Fixture();

        private static IEnumerable<Torrent> GetTorrents() => new List<Torrent>()
        {
            Fixture.Create<Torrent>(),
            Fixture.Create<Torrent>(),
            Fixture.Create<Torrent>()
        };

        private static readonly List<IndexerResult> IndexerResults = new List<IndexerResult>
        {
            new IndexerResult {Indexer = Indexer.Rarbg.ToString(), Torrents = GetTorrents()},
            new IndexerResult {Indexer = Indexer.LeetX.ToString(), Torrents = GetTorrents()},
            new IndexerResult { Indexer = Indexer.ThePirateBay.ToString(), Torrents = GetTorrents() }
        };

        [Test]
        public void FlattenIndexerResults_Should_Have_Entries_For_Each_Indexer()
        {
            var flattenedResults = IndexerResults.FlattenIndexerResults();

            var groups = flattenedResults
                .GroupBy(g => new {Indexer = g.Indexer.ToString()})
                .ToList();

            Assert.AreEqual(IndexerResults.Count, groups.Count);
        }

        [Test]
        public void FlattenIndexerResults_Should_Have_Torrents_For_Each_Indexer()
        {
            var flattenedResults = IndexerResults.FlattenIndexerResults();

            var groups = flattenedResults
                .GroupBy(g => new {Indexer = g.Indexer.ToString()})
                .ToList();

            var torrents = groups
                .SelectMany(t => t)
                .ToList();

            foreach (var indexerResult in IndexerResults)
            {
                Assert.AreEqual(
                    GetTorrents().Count()
                    , torrents.Count(t => t.Indexer == indexerResult.Indexer.ToString())
                );
            }
        }
    }
}