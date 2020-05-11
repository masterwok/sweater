namespace Sweater.Core.Models
{
    /// <summary>
    /// The application query configuration for IIndexerQueryService implementations.
    /// This configuration is read from appsettings.json
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class QueryConfig
    {
        /// <summary>
        /// Whether or not query caching is enabled.
        /// </summary>
        public bool IsCacheEnabled { get; set; }

        /// <summary>
        /// How long in milliseconds to keep cached results.
        /// </summary>
        public long CacheTimeSpanMs { get; set; }
        
        /// <summary>
        /// The time in milliseconds to wait before cancelling a query.
        /// </summary>
        public long QueryTimeoutMs { get; set; }
    }
}