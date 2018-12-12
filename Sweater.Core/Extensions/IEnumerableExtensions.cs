using System;
using System.Collections.Generic;

namespace Sweater.Core.Extensions
{
    /// <summary>
    /// This class contains a collection of extension methods for IEnumerable.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Ensure distinct items within an IEnumerable.
        /// </summary>
        /// <param name="source">The source type.</param>
        /// <param name="keySelector">The key type.</param>
        /// <typeparam name="TSource">The source data.</typeparam>
        /// <typeparam name="TKey">The keys.</typeparam>
        /// <returns>A distinct IEnumerable using the keys.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source
            , Func<TSource, TKey> keySelector
        )
        {
            var seenKeys = new HashSet<TKey>();

            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}