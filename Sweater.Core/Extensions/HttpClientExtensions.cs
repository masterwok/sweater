using Sweater.Core.Clients;

namespace Sweater.Core.Extensions
{
    /// <summary>
    /// A collection of extension methods for HttpClient.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Set the default user-agent of an IHttpClient.
        /// </summary>
        /// <param name="httpClient">The client to set the user-agent on.</param>
        /// <param name="userAgent">The user-agent to set.</param>
        public static void SetDefaultUserAgent(
            this IHttpClient httpClient,
            string userAgent
        ) => httpClient.DefaultRequestHeaders
            .UserAgent
            .ParseAdd(userAgent);
    }
}