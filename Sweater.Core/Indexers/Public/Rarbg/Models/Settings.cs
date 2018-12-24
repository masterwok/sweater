using System.Diagnostics.CodeAnalysis;

namespace Sweater.Core.Indexers.Public.Rarbg.Models
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public sealed class Settings
    {
        public string BaseUrl { get; set; }
        public string AppId{ get; set; }
    }
}