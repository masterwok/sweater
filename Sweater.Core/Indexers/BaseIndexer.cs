using System.Threading.Tasks;
using Sweater.Core.Clients;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers
{
    public abstract class BaseIndexer : IIndexer
    {
        protected readonly IWebClient WebClient;

        public BaseIndexer(IWebClient webClient)
        {
            WebClient = webClient;
        }

        public abstract Task<bool> Login();
        public abstract Task<IndexerResult> Query(string queryString);
        public abstract Task<bool> Logout();
    }
}