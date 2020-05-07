using System.Collections.Generic;

namespace Sweater.Core.Indexers.Public.ThePirateBay.Models
{
    public sealed class QueryResponse
    {
        public IList<QueryResponseItem> Items { get; set; }
    }
}