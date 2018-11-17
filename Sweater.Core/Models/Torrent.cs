using Sweater.Core.Extensions;

namespace Sweater.Core.Models
{
    /// <summary>
    /// This model represents a single torrent entry.
    /// </summary>
    public sealed class Torrent
    {
        public string InfoHash => MagnetUri.ParseInfoHash();
        public string MagnetUri { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public int Seeders { get; set; }
        public int Leechers { get; set; }
        public string UploadedOn { get; set; }
    }
}