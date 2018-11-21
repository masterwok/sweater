using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sweater.Core.Indexers.Public.Rarbg.Models
{
    public class QueryResponse
    {
        [JsonProperty("torrent_results")]
        public IList<TorrentResult> TorrentResults { get; set; }
    }
}