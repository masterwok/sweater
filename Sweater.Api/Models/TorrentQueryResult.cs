using System.Diagnostics.CodeAnalysis;
using Sweater.Core.Extensions;

namespace Sweater.Api.Models
{
    /// <summary>
    /// A torrent query result item.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class TorrentQueryResult
    {
        public string Indexer { get; set; }
        public string InfoHash => MagnetUri.ParseInfoHash();
        public string MagnetUri { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public int Seeders { get; set; }
        public int Leechers { get; set; }
        public string UploadedOn { get; set; }
    }
}