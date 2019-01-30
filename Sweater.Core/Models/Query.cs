using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sweater.Core.Attributes;
using Sweater.Core.Constants;

namespace Sweater.Core.Models
{
    /// <summary>
    /// This model is accepted by the IndexerController query actions and is then
    /// passed down to indexers which then convert the query into indexer specific queries.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class Query
    {
        public Indexer[] Indexers { get; set; } = new Indexer[0];

        public SortField SortField { get; set; } = SortField.Seeders;

        public SortOrder SortOrder { get; set; } = SortOrder.Descending;

        public string QueryString { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public override string ToString() => $"Indexer Tag = {IndexersString}, " +
                                             $"Query String = {QueryString}";

        private string IndexersString => string.Join(
            ", "
            , Indexers.Select(i => i.ToString())
        );
    }
}