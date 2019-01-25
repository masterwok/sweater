namespace Sweater.Core.Models
{
    /// <summary>
    /// This enumeration represents all fields that can be sorted during a query.
    /// </summary>
    public enum SortField
    {
        /// <summary>
        /// The number of seeders associated with a torrent.
        /// </summary>
        Seeders,

        /// <summary>
        /// The number of leechers associated with a torrent.
        /// </summary>
        Leechers,

        /// <summary>
        /// When the torrent was uploaded.
        /// </summary>
        UploadedOn,

        /// <summary>
        /// The total size of all files contained within the torrent.
        /// </summary>
        Size
    }
}