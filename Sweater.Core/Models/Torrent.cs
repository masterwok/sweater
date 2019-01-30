using System.Diagnostics.CodeAnalysis;
using Sweater.Core.Extensions;

namespace Sweater.Core.Models
{
    /// <summary>
    /// This model represents a single torrent entry.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class Torrent
    {
        public string InfoHash => MagnetUri.ParseInfoHash();
        public string MagnetUri { get; set; }

        public string Name { get; set; }

        // TODO: Size needs to be parsed to bytes.
        public long Size { get; set; }
        public int Seeders { get; set; }

        public int Leechers { get; set; }

        // TODO: UploadedOn needs to be parsed to datetime.
        public string UploadedOn { get; set; }
    }
}