using System;
using System.Collections.Generic;
using System.Linq;

namespace Sweater.Core.Utils
{
    /// <summary>
    /// This static utility class contains common methods used in paging through results.
    /// </summary>
    public static class PagingUtil
    {
        /// <summary>
        /// Get an enumerable containing all pages for the provided range.
        /// </summary>
        /// <param name="lastPageIndex">The last page index scraped from the page.</param>
        /// <param name="maxPages">The maximum number of pages for scraping.</param>
        /// <param name="isZeroIndexed">Whether or not the indexers page indices are zero-indexed.</param>
        /// <returns></returns>
        public static IEnumerable<int> GetPageRange(
            int lastPageIndex
            , int maxPages
            , bool isZeroIndexed = false
        )
        {
            var remainingPageCount = Math.Min(maxPages, lastPageIndex) - 1;

            var rangeStart = isZeroIndexed
                ? 1
                : 2;

            return remainingPageCount <= 0
                ? null
                : Enumerable
                    .Range(rangeStart, remainingPageCount)
                    .ToList();
        }
    }
}