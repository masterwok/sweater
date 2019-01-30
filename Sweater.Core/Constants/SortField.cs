namespace Sweater.Core.Constants
{
    /// <summary>
    /// The field to sort the query results by.
    /// </summary>
    public enum SortField
    {
        /// <summary>
        /// The name of the torrent.
        /// </summary>
        Name,

        /// <summary>
        /// The size of the torrent.
        /// </summary>
        Size,

        /// <summary>
        /// The number of seeders for the torrent.
        /// </summary>
        Seeders,

        /// <summary>
        /// The number of leechers leeching the torrent.
        /// </summary>
        Leechers,

        /// <summary>
        /// The date when the torrent was uploaded.
        /// </summary>
        UploadedOn
    }
}