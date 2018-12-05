using Sweater.Core.Models;

namespace Sweater.Api.Models
{
    public class PaginatedQuery
    {
        public Query Query { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}