using System.Collections.Generic;
using System.Threading.Tasks;
using Sweater.Api.Models;
using Sweater.Core.Models;

namespace Sweater.Core.Services.Contracts
{
    public interface IIndexerQueryService
    {
        Task<IList<string>> GetIndexerTags();

        Task<IList<TorrentQueryResult>> Query(Query query);
    }
}