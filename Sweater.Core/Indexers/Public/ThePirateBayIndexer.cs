using System.Linq.Expressions;
using System.Threading.Tasks;
using Sweater.Core.Clients;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Public
{
    public class ThePirateBayIndexer : BaseIndexer
    {
        public ThePirateBayIndexer(
            IWebClient webClient
        ) : base(webClient) => Expression.Empty();

        public override Task<bool> Login()
        {
            throw new System.NotImplementedException();
        }

        public override Task<IndexerResult> Query(string queryString)
        {
            throw new System.NotImplementedException();
        }

        public override Task<bool> Logout()
        {
            throw new System.NotImplementedException();
        }
    }
}