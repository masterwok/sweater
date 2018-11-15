using System.Threading.Tasks;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Models;

namespace Sweater.Core.Indexers.Public
{
    public class ThePirateBayIndexer : IIndexer
    {
        public Task<bool> Login()
        {
            throw new System.NotImplementedException();
        }

        public Task<IndexerResult> Query(string queryString)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Logout()
        {
            throw new System.NotImplementedException();
        }
    }
}