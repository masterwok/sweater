using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sweater.Core.Extensions;

namespace Sweater.Core.Models
{
    /// <summary>
    /// A torrent query result item.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class TorrentQueryResult
    {
        public string InfoHash => MagnetUri.ParseInfoHash();
        public string Indexer { get; set; }
        public string MagnetUri { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public int Seeders { get; set; }
        public int Leechers { get; set; }
        public DateTime? UploadedOn { get; set; }

        protected bool Equals(TorrentQueryResult other) => string.Equals(MagnetUri, other.MagnetUri);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((TorrentQueryResult) obj);
        }

        public override int GetHashCode() => MagnetUri != null
            ? MagnetUri.GetHashCode()
            : 0;
    }
}