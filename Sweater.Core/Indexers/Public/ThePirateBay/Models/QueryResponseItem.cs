using Newtonsoft.Json;
using Sweater.Core.Converters;

namespace Sweater.Core.Indexers.Public.ThePirateBay.Models
{
    public class QueryResponseItem
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("info_hash")]
        public string InfoHash { get; set; }

        [JsonProperty("leechers")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Leechers { get; set; }

        [JsonProperty("seeders")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Seeders { get; set; }

        [JsonProperty("num_files")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long NumFiles { get; set; }

        [JsonProperty("size")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Size { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("added")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Added { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("category")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Category { get; set; }

        [JsonProperty("imdb")]
        public string Imdb { get; set; }
    }
}