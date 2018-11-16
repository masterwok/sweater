namespace Sweater.Core.Models
{
    public sealed class QueryConfig
    {
        /// <summary>
        /// Whether or not query caching is enabled.
        /// </summary>
        public bool IsCacheEnabled { get; set; }

        /// <summary>
        /// How long in milliseconds to keep cached results.
        /// </summary>
        public long CacheTimeoutMs { get; set; }
    }
}