using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using Sweater.Api.Models;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;

namespace Sweater.Api.Test.Controllers
{
    [TestFixture]
    public class IndexerController
    {
        private Mock<IIndexerQueryService> _queryService;
        private Api.Controllers.IndexerController _indexerController;

        private static readonly Fixture Fixture = new Fixture();

        private static readonly List<string> Tags = new List<string>
        {
            Fixture.Create<string>(),
            Fixture.Create<string>(),
            Fixture.Create<string>()
        };

        private static readonly Query Query = new Query
        {
            Indexers = Fixture.Create<Indexer[]>(),
            QueryString = Fixture.Create<string>(),
            PageIndex = Fixture.Create<int>(),
            PageSize = Fixture.Create<int>()
        };

        private static IEnumerable<Torrent> GetTorrents() => new List<Torrent>
        {
            Fixture.Create<Torrent>(),
            Fixture.Create<Torrent>(),
            Fixture.Create<Torrent>()
        };

        private static readonly List<IndexerResult> IndexerResults = new List<IndexerResult>
        {
            new IndexerResult {Indexer = Indexer.Rarbg.ToString(), Torrents = GetTorrents()},
            new IndexerResult {Indexer = Indexer.LeetX.ToString(), Torrents = GetTorrents()},
            new IndexerResult {Indexer = Indexer.ThePirateBay.ToString(), Torrents = GetTorrents()}
        };

        private static readonly IList<TorrentQueryResult> QueryResults
            = IndexerResults.FlattenIndexerResults();

        [SetUp]
        public void Setup()
        {
            _queryService = new Mock<IIndexerQueryService>();

            _queryService
                .Setup(s => s.GetIndexerTags())
                .ReturnsAsync(Tags);

            _queryService
                .Setup(s => s.Query(It.IsAny<Query>()))
                .ReturnsAsync(QueryResults);

            _indexerController = new Api.Controllers.IndexerController(_queryService.Object);
        }

        [Test]
        public async Task Tags_Returns_Values_From_Query_Service()
        {
            var tags = await _indexerController.Tags();

            Assert.AreEqual(tags, Tags);
        }

        [Test]
        public async Task Query_Invokes_QueryService_Query_With_Query_Parameter()
        {
            await _indexerController.Query(Query);

            _queryService.Verify(s => s.Query(Query));
        }

        [Test]
        public async Task Query_Returns_Correct_Page_Index_In_Result()
        {
            var pageIndex = Fixture.Create<int>();
            var pageSize = Fixture.Create<int>();

            var result = await _indexerController.Query(Query);

            Assert.AreEqual(pageIndex, result.PageIndex);
        }

        [Test]
        public async Task Query_Returns_Correct_Page_Size_In_Result()
        {
            var pageIndex = Fixture.Create<int>();
            var pageSize = Fixture.Create<int>();

            var result = await _indexerController.Query(Query);

            Assert.AreEqual(pageSize, result.PageSize);
        }

        [Test]
        public async Task Query_Returns_Correct_TotalItemCount_In_Result()
        {
            var result = await _indexerController.Query(Query);

            Assert.AreEqual(IndexerResults.Count * GetTorrents().Count(), result.TotalItemCount);
        }

        [Test]
        public async Task Query_Returns_Expected_Item_Count_By_Page_Size_In_Result()
        {
            var result = await _indexerController.Query(Query);

            Assert.AreEqual(2, result.Items.Count);
        }
    }
}