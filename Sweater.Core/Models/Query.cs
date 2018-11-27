using System.Diagnostics.CodeAnalysis;
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
        [ValidEnum] public Indexer Indexer { get; set; } = Indexer.All;

        public string QueryString { get; set; }

        public override string ToString() => $"Indexer Tag = {Indexer}, " +
                                             $"Query String = {QueryString}";
    }
}