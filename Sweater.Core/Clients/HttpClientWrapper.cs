using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Sweater.Core.Clients.Contracts;

namespace Sweater.Core.Clients
{
    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public void CancelPendingRequests()
        {
            _httpClient.CancelPendingRequests();
        }

        public Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return _httpClient.DeleteAsync(requestUri);
        }

        public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken)
        {
            return _httpClient.DeleteAsync(requestUri, cancellationToken);
        }

        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
        {
            return _httpClient.DeleteAsync(requestUri);
        }

        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            return _httpClient.DeleteAsync(requestUri, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _httpClient.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption)
        {
            return _httpClient.GetAsync(requestUri, completionOption);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return _httpClient.GetAsync(requestUri, completionOption, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
        {
            return _httpClient.GetAsync(requestUri, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            return _httpClient.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
        {
            return _httpClient.GetAsync(requestUri, completionOption);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return _httpClient.GetAsync(requestUri, completionOption, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            return _httpClient.GetAsync(requestUri, cancellationToken);
        }

        public Task<byte[]> GetByteArrayAsync(string requestUri)
        {
            return _httpClient.GetByteArrayAsync(requestUri);
        }

        public Task<byte[]> GetByteArrayAsync(Uri requestUri)
        {
            return _httpClient.GetByteArrayAsync(requestUri);
        }

        public Task<Stream> GetStreamAsync(string requestUri)
        {
            return _httpClient.GetStreamAsync(requestUri);
        }

        public Task<Stream> GetStreamAsync(Uri requestUri)
        {
            return _httpClient.GetStreamAsync(requestUri);
        }

        public Task<string> GetStringAsync(string requestUri)
        {
            return _httpClient.GetStringAsync(requestUri);
        }

        public Task<string> GetStringAsync(Uri requestUri)
        {
            return _httpClient.GetStringAsync(requestUri);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return _httpClient.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return _httpClient.PostAsync(requestUri, content, cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
        {
            return _httpClient.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return _httpClient.PostAsync(requestUri, content, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            return _httpClient.PutAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return _httpClient.PutAsync(requestUri, content, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
        {
            return _httpClient.PutAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return _httpClient.PutAsync(requestUri, content, cancellationToken);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _httpClient.SendAsync(request);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return _httpClient.SendAsync(request, completionOption);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return _httpClient.SendAsync(request, completionOption, cancellationToken);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _httpClient.SendAsync(request, cancellationToken);
        }

        public Uri BaseAddress
        {
            get => _httpClient.BaseAddress;
            set => _httpClient.BaseAddress = value;
        }

        public HttpRequestHeaders DefaultRequestHeaders => _httpClient.DefaultRequestHeaders;

        public long MaxResponseContentBufferSize
        {
            get => _httpClient.MaxResponseContentBufferSize;
            set => _httpClient.MaxResponseContentBufferSize = value;
        }

        public TimeSpan Timeout
        {
            get => _httpClient.Timeout;
            set => _httpClient.Timeout = value;
        }
    }
}