using System.Diagnostics.CodeAnalysis;

namespace Sweater.Core.Indexers.Public.ThePirateBay.Models
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public sealed class Settings
    {
        public string BaseUrl { get; set; }
        public int MaxPages { get; set; }
        public bool IsEnabled { get; set; }
    }
}