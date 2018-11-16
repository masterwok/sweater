using Sweater.Core.Constants;

namespace Sweater.Core.Models
{
    /// <summary>
    /// This model is accepted by the IndexerController query actions and is then
    /// passed down to indexers which then convert the query into indexer specific queries.
    /// </summary>
    public class Query
    {
        public Indexer Indexer { get; set; }
        public string QueryString { get; set; }

        public override string ToString() => $"Indexer Tag = {Indexer}, " +
                                             $"Query String = {QueryString}";
    }
}