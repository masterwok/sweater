namespace Sweater.Core.Constants
{
    /// <summary>
    /// The field to sort the query results by.
    /// </summary>
    public enum SortField : byte
    {
        /// <summary>
        /// The name of the torrent.
        /// </summary>
        Name = 1,

        /// <summary>
        /// The size of the torrent.
        /// </summary>
        Size = 2,

        /// <summary>
        /// The number of seeders for the torrent.
        /// </summary>
        Seeders = 3,

        /// <summary>
        /// The number of leechers leeching the torrent.
        /// </summary>
        Leechers = 4,

        /// <summary>
        /// The date when the torrent was uploaded.
        /// </summary>
        UploadedOn = 5
    }
}