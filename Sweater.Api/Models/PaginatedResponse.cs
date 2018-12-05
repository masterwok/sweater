using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sweater.Api.Models
{
    /// <summary>
    /// A generic paginated response.
    /// </summary>
    /// <typeparam name="T">The type of items contained within the response.</typeparam>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// The paged items.
        /// </summary>
        public IList<T> Items { get; set; }

        /// <summary>
        /// The requested page index.
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// The requested page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items before pagination was applied.
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// The total number of pages for the provided page size.
        /// </summary>
        public int PageCount => (TotalItemCount + PageSize - 1) / PageSize;
    }
}