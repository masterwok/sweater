using System;
using System.Collections.Generic;
using System.Linq;
using Sweater.Core.Constants;
using Sweater.Core.Models;

namespace Sweater.Core.Extensions
{
    /// <summary>
    /// This class contains static extension methods for the TorrentQueryResult type.
    /// </summary>
    public static class TorrentQueryResultExtensions
    {
        /// <summary>
        /// Sort the query results in descending order using the provided field.
        /// </summary>
        /// <param name="queryResults">The query results to sort.</param>
        /// <param name="sortField">The field to sort on.</param>
        /// <returns>A new IList instance of type TorrentQueryResult.</returns>
        private static IList<TorrentQueryResult> SortDescending(
            IEnumerable<TorrentQueryResult> queryResults
            , SortField sortField
        )
        {
            IOrderedEnumerable<TorrentQueryResult> orderedResults;

            switch (sortField)
            {
                case SortField.Name:
                    orderedResults = queryResults.OrderByDescending(r => r.Name);
                    break;
                case SortField.Size:
                    orderedResults = queryResults.OrderByDescending(r => r.Size);
                    break;
                case SortField.Seeders:
                    orderedResults = queryResults.OrderByDescending(r => r.Seeders);
                    break;
                case SortField.Leechers:
                    orderedResults = queryResults.OrderByDescending(r => r.Leechers);
                    break;
                case SortField.UploadedOn:
                    orderedResults = queryResults.OrderByDescending(r => r.UploadedOn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortField), sortField, "Unexpected sort field.");
            }

            return orderedResults.ToList();
        }

        /// <summary>
        /// Sort the query results in ascending order using the provided field.
        /// </summary>
        /// <param name="queryResults">The query results to sort.</param>
        /// <param name="sortField">The field to sort on.</param>
        /// <returns>A new IList instance of type TorrentQueryResult.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static IList<TorrentQueryResult> SortAscending(
            IEnumerable<TorrentQueryResult> queryResults
            , SortField sortField
        )
        {
            IOrderedEnumerable<TorrentQueryResult> orderedResults;

            switch (sortField)
            {
                case SortField.Name:
                    orderedResults = queryResults.OrderBy(r => r.Name);
                    break;
                case SortField.Size:
                    orderedResults = queryResults.OrderBy(r => r.Size);
                    break;
                case SortField.Seeders:
                    orderedResults = queryResults.OrderBy(r => r.Seeders);
                    break;
                case SortField.Leechers:
                    orderedResults = queryResults.OrderBy(r => r.Leechers);
                    break;
                case SortField.UploadedOn:
                    orderedResults = queryResults.OrderBy(r => r.UploadedOn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortField), sortField, "Unexpected sort field.");
            }

            return orderedResults.ToList();
        }


        /// <summary>
        /// Sort the query results using the provided field and order.
        /// </summary>
        /// <param name="queryResults">The query results to sort.</param>
        /// <param name="sortField">The field to sort on.</param>
        /// <param name="sortOrder">The order to sort the field in.</param>
        /// <returns></returns>
        public static IList<TorrentQueryResult> SortTorrentQueryResults(
            this IEnumerable<TorrentQueryResult> queryResults
            , SortField sortField
            , SortOrder sortOrder
        ) => sortOrder == SortOrder.Ascending
            ? SortAscending(queryResults, sortField)
            : SortDescending(queryResults, sortField);
    }
}