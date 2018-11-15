using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace Sweater.Core.Clients
{
    public class WebClientWrapper : IWebClient
    {
        private readonly WebClient _webClient = new WebClient();

        public object GetLifetimeService()
        {
            return _webClient.GetLifetimeService();
        }

        public object InitializeLifetimeService()
        {
            return _webClient.InitializeLifetimeService();
        }

        public void Dispose()
        {
            _webClient.Dispose();
        }

        public IContainer Container => _webClient.Container;

        public ISite Site
        {
            get => _webClient.Site;
            set => _webClient.Site = value;
        }

        public event EventHandler Disposed
        {
            add => _webClient.Disposed += value;
            remove => _webClient.Disposed -= value;
        }

        public void CancelAsync()
        {
            _webClient.CancelAsync();
        }

        public byte[] DownloadData(string address)
        {
            return _webClient.DownloadData(address);
        }

        public byte[] DownloadData(Uri address)
        {
            return _webClient.DownloadData(address);
        }

        public void DownloadDataAsync(Uri address)
        {
            _webClient.DownloadDataAsync(address);
        }

        public void DownloadDataAsync(Uri address, object userToken)
        {
            _webClient.DownloadDataAsync(address, userToken);
        }

        public Task<byte[]> DownloadDataTaskAsync(string address)
        {
            return _webClient.DownloadDataTaskAsync(address);
        }

        public Task<byte[]> DownloadDataTaskAsync(Uri address)
        {
            return _webClient.DownloadDataTaskAsync(address);
        }

        public void DownloadFile(string address, string fileName)
        {
            _webClient.DownloadFile(address, fileName);
        }

        public void DownloadFile(Uri address, string fileName)
        {
            _webClient.DownloadFile(address, fileName);
        }

        public void DownloadFileAsync(Uri address, string fileName)
        {
            _webClient.DownloadFileAsync(address, fileName);
        }

        public void DownloadFileAsync(Uri address, string fileName, object userToken)
        {
            _webClient.DownloadFileAsync(address, fileName, userToken);
        }

        public Task DownloadFileTaskAsync(string address, string fileName)
        {
            return _webClient.DownloadFileTaskAsync(address, fileName);
        }

        public Task DownloadFileTaskAsync(Uri address, string fileName)
        {
            return _webClient.DownloadFileTaskAsync(address, fileName);
        }

        public string DownloadString(string address)
        {
            return _webClient.DownloadString(address);
        }

        public string DownloadString(Uri address)
        {
            return _webClient.DownloadString(address);
        }

        public void DownloadStringAsync(Uri address)
        {
            _webClient.DownloadStringAsync(address);
        }

        public void DownloadStringAsync(Uri address, object userToken)
        {
            _webClient.DownloadStringAsync(address, userToken);
        }

        public Task<string> DownloadStringTaskAsync(string address)
        {
            return _webClient.DownloadStringTaskAsync(address);
        }

        public Task<string> DownloadStringTaskAsync(Uri address)
        {
            return _webClient.DownloadStringTaskAsync(address);
        }

        public Stream OpenRead(string address)
        {
            return _webClient.OpenRead(address);
        }

        public Stream OpenRead(Uri address)
        {
            return _webClient.OpenRead(address);
        }

        public void OpenReadAsync(Uri address)
        {
            _webClient.OpenReadAsync(address);
        }

        public void OpenReadAsync(Uri address, object userToken)
        {
            _webClient.OpenReadAsync(address, userToken);
        }

        public Task<Stream> OpenReadTaskAsync(string address)
        {
            return _webClient.OpenReadTaskAsync(address);
        }

        public Task<Stream> OpenReadTaskAsync(Uri address)
        {
            return _webClient.OpenReadTaskAsync(address);
        }

        public Stream OpenWrite(string address)
        {
            return _webClient.OpenWrite(address);
        }

        public Stream OpenWrite(Uri address)
        {
            return _webClient.OpenWrite(address);
        }

        public Stream OpenWrite(string address, string method)
        {
            return _webClient.OpenWrite(address, method);
        }

        public Stream OpenWrite(Uri address, string method)
        {
            return _webClient.OpenWrite(address, method);
        }

        public void OpenWriteAsync(Uri address)
        {
            _webClient.OpenWriteAsync(address);
        }

        public void OpenWriteAsync(Uri address, string method)
        {
            _webClient.OpenWriteAsync(address, method);
        }

        public void OpenWriteAsync(Uri address, string method, object userToken)
        {
            _webClient.OpenWriteAsync(address, method, userToken);
        }

        public Task<Stream> OpenWriteTaskAsync(string address)
        {
            return _webClient.OpenWriteTaskAsync(address);
        }

        public Task<Stream> OpenWriteTaskAsync(Uri address)
        {
            return _webClient.OpenWriteTaskAsync(address);
        }

        public Task<Stream> OpenWriteTaskAsync(string address, string method)
        {
            return _webClient.OpenWriteTaskAsync(address, method);
        }

        public Task<Stream> OpenWriteTaskAsync(Uri address, string method)
        {
            return _webClient.OpenWriteTaskAsync(address, method);
        }

        public byte[] UploadData(string address, byte[] data)
        {
            return _webClient.UploadData(address, data);
        }

        public byte[] UploadData(Uri address, byte[] data)
        {
            return _webClient.UploadData(address, data);
        }

        public byte[] UploadData(string address, string method, byte[] data)
        {
            return _webClient.UploadData(address, method, data);
        }

        public byte[] UploadData(Uri address, string method, byte[] data)
        {
            return _webClient.UploadData(address, method, data);
        }

        public void UploadDataAsync(Uri address, byte[] data)
        {
            _webClient.UploadDataAsync(address, data);
        }

        public void UploadDataAsync(Uri address, string method, byte[] data)
        {
            _webClient.UploadDataAsync(address, method, data);
        }

        public void UploadDataAsync(Uri address, string method, byte[] data, object userToken)
        {
            _webClient.UploadDataAsync(address, method, data, userToken);
        }

        public Task<byte[]> UploadDataTaskAsync(string address, byte[] data)
        {
            return _webClient.UploadDataTaskAsync(address, data);
        }

        public Task<byte[]> UploadDataTaskAsync(Uri address, byte[] data)
        {
            return _webClient.UploadDataTaskAsync(address, data);
        }

        public Task<byte[]> UploadDataTaskAsync(string address, string method, byte[] data)
        {
            return _webClient.UploadDataTaskAsync(address, method, data);
        }

        public Task<byte[]> UploadDataTaskAsync(Uri address, string method, byte[] data)
        {
            return _webClient.UploadDataTaskAsync(address, method, data);
        }

        public byte[] UploadFile(string address, string fileName)
        {
            return _webClient.UploadFile(address, fileName);
        }

        public byte[] UploadFile(Uri address, string fileName)
        {
            return _webClient.UploadFile(address, fileName);
        }

        public byte[] UploadFile(string address, string method, string fileName)
        {
            return _webClient.UploadFile(address, method, fileName);
        }

        public byte[] UploadFile(Uri address, string method, string fileName)
        {
            return _webClient.UploadFile(address, method, fileName);
        }

        public Task<byte[]> UploadFileTaskAsync(string address, string fileName)
        {
            return _webClient.UploadFileTaskAsync(address, fileName);
        }

        public Task<byte[]> UploadFileTaskAsync(Uri address, string fileName)
        {
            return _webClient.UploadFileTaskAsync(address, fileName);
        }

        public Task<byte[]> UploadFileTaskAsync(string address, string method, string fileName)
        {
            return _webClient.UploadFileTaskAsync(address, method, fileName);
        }

        public Task<byte[]> UploadFileTaskAsync(Uri address, string method, string fileName)
        {
            return _webClient.UploadFileTaskAsync(address, method, fileName);
        }

        public void UploadFileAsync(Uri address, string fileName)
        {
            _webClient.UploadFileAsync(address, fileName);
        }

        public void UploadFileAsync(Uri address, string method, string fileName)
        {
            _webClient.UploadFileAsync(address, method, fileName);
        }

        public void UploadFileAsync(Uri address, string method, string fileName, object userToken)
        {
            _webClient.UploadFileAsync(address, method, fileName, userToken);
        }

        public string UploadString(string address, string data)
        {
            return _webClient.UploadString(address, data);
        }

        public string UploadString(Uri address, string data)
        {
            return _webClient.UploadString(address, data);
        }

        public string UploadString(string address, string method, string data)
        {
            return _webClient.UploadString(address, method, data);
        }

        public string UploadString(Uri address, string method, string data)
        {
            return _webClient.UploadString(address, method, data);
        }

        public void UploadStringAsync(Uri address, string data)
        {
            _webClient.UploadStringAsync(address, data);
        }

        public void UploadStringAsync(Uri address, string method, string data)
        {
            _webClient.UploadStringAsync(address, method, data);
        }

        public void UploadStringAsync(Uri address, string method, string data, object userToken)
        {
            _webClient.UploadStringAsync(address, method, data, userToken);
        }

        public Task<string> UploadStringTaskAsync(string address, string data)
        {
            return _webClient.UploadStringTaskAsync(address, data);
        }

        public Task<string> UploadStringTaskAsync(Uri address, string data)
        {
            return _webClient.UploadStringTaskAsync(address, data);
        }

        public Task<string> UploadStringTaskAsync(string address, string method, string data)
        {
            return _webClient.UploadStringTaskAsync(address, method, data);
        }

        public Task<string> UploadStringTaskAsync(Uri address, string method, string data)
        {
            return _webClient.UploadStringTaskAsync(address, method, data);
        }

        public byte[] UploadValues(string address, NameValueCollection data)
        {
            return _webClient.UploadValues(address, data);
        }

        public byte[] UploadValues(Uri address, NameValueCollection data)
        {
            return _webClient.UploadValues(address, data);
        }

        public byte[] UploadValues(string address, string method, NameValueCollection data)
        {
            return _webClient.UploadValues(address, method, data);
        }

        public byte[] UploadValues(Uri address, string method, NameValueCollection data)
        {
            return _webClient.UploadValues(address, method, data);
        }

        public void UploadValuesAsync(Uri address, NameValueCollection data)
        {
            _webClient.UploadValuesAsync(address, data);
        }

        public void UploadValuesAsync(Uri address, string method, NameValueCollection data)
        {
            _webClient.UploadValuesAsync(address, method, data);
        }

        public void UploadValuesAsync(Uri address, string method, NameValueCollection data,
            object userToken)
        {
            _webClient.UploadValuesAsync(address, method, data, userToken);
        }

        public Task<byte[]> UploadValuesTaskAsync(string address, NameValueCollection data)
        {
            return _webClient.UploadValuesTaskAsync(address, data);
        }

        public Task<byte[]> UploadValuesTaskAsync(string address, string method,
            NameValueCollection data)
        {
            return _webClient.UploadValuesTaskAsync(address, method, data);
        }

        public Task<byte[]> UploadValuesTaskAsync(Uri address, NameValueCollection data)
        {
            return _webClient.UploadValuesTaskAsync(address, data);
        }

        public Task<byte[]> UploadValuesTaskAsync(Uri address, string method,
            NameValueCollection data)
        {
            return _webClient.UploadValuesTaskAsync(address, method, data);
        }

        public string BaseAddress
        {
            get => _webClient.BaseAddress;
            set => _webClient.BaseAddress = value;
        }

        public RequestCachePolicy CachePolicy
        {
            get => _webClient.CachePolicy;
            set => _webClient.CachePolicy = value;
        }

        public ICredentials Credentials
        {
            get => _webClient.Credentials;
            set => _webClient.Credentials = value;
        }

        public Encoding Encoding
        {
            get => _webClient.Encoding;
            set => _webClient.Encoding = value;
        }

        public WebHeaderCollection Headers
        {
            get => _webClient.Headers;
            set => _webClient.Headers = value;
        }

        public bool IsBusy => _webClient.IsBusy;

        public IWebProxy Proxy
        {
            get => _webClient.Proxy;
            set => _webClient.Proxy = value;
        }

        public NameValueCollection QueryString
        {
            get => _webClient.QueryString;
            set => _webClient.QueryString = value;
        }

        public WebHeaderCollection ResponseHeaders => _webClient.ResponseHeaders;

        public bool UseDefaultCredentials
        {
            get => _webClient.UseDefaultCredentials;
            set => _webClient.UseDefaultCredentials = value;
        }

        public event DownloadDataCompletedEventHandler DownloadDataCompleted
        {
            add => _webClient.DownloadDataCompleted += value;
            remove => _webClient.DownloadDataCompleted -= value;
        }

        public event AsyncCompletedEventHandler DownloadFileCompleted
        {
            add => _webClient.DownloadFileCompleted += value;
            remove => _webClient.DownloadFileCompleted -= value;
        }

        public event DownloadProgressChangedEventHandler DownloadProgressChanged
        {
            add => _webClient.DownloadProgressChanged += value;
            remove => _webClient.DownloadProgressChanged -= value;
        }

        public event DownloadStringCompletedEventHandler DownloadStringCompleted
        {
            add => _webClient.DownloadStringCompleted += value;
            remove => _webClient.DownloadStringCompleted -= value;
        }

        public event OpenReadCompletedEventHandler OpenReadCompleted
        {
            add => _webClient.OpenReadCompleted += value;
            remove => _webClient.OpenReadCompleted -= value;
        }

        public event OpenWriteCompletedEventHandler OpenWriteCompleted
        {
            add => _webClient.OpenWriteCompleted += value;
            remove => _webClient.OpenWriteCompleted -= value;
        }

        public event UploadDataCompletedEventHandler UploadDataCompleted
        {
            add => _webClient.UploadDataCompleted += value;
            remove => _webClient.UploadDataCompleted -= value;
        }

        public event UploadFileCompletedEventHandler UploadFileCompleted
        {
            add => _webClient.UploadFileCompleted += value;
            remove => _webClient.UploadFileCompleted -= value;
        }

        public event UploadProgressChangedEventHandler UploadProgressChanged
        {
            add => _webClient.UploadProgressChanged += value;
            remove => _webClient.UploadProgressChanged -= value;
        }

        public event UploadStringCompletedEventHandler UploadStringCompleted
        {
            add => _webClient.UploadStringCompleted += value;
            remove => _webClient.UploadStringCompleted -= value;
        }

        public event UploadValuesCompletedEventHandler UploadValuesCompleted
        {
            add => _webClient.UploadValuesCompleted += value;
            remove => _webClient.UploadValuesCompleted -= value;
        }

    }
}