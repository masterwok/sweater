using System;

namespace Sweater.Core.Models
{
    /**
     * This model represents a single torrent entry.
     */
    public sealed class Torrent
    {
        public string Name { get; set; }
        public string InfoHash { get; set; }
        public DateTime UploadedOn { get; set; }
        public int Seeders { get; set; }
        public int Leechers { get; set; }
    }
}