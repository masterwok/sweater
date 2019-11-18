using Sweater.Core.Attributes;
using Sweater.Core.Indexers.Public.Kat;
using Sweater.Core.Indexers.Public.LeetX;
using Sweater.Core.Indexers.Public.LimeTorrents;
using Sweater.Core.Indexers.Public.Nyaa;
using Sweater.Core.Indexers.Public.Rarbg;
using Sweater.Core.Indexers.Public.ThePirateBay;
using Sweater.Core.Indexers.Public.Zooqle;

namespace Sweater.Core.Constants
{
    /// <summary>
    /// An enumeration of implemented indexers.
    /// </summary>
    public enum Indexer
    {
        [Type(typeof(ThePirateBay))] ThePirateBay = 1,
        [Type(typeof(LeetX))] LeetX = 2,
        [Type(typeof(Rarbg))] Rarbg = 3,
        [Type(typeof(Zooqle))] Zooqle = 4,
        [Type(typeof(Nyaa))] Nyaa = 5,
        [Type(typeof(LimeTorrents))] LimeTorrents = 6,
        [Type(typeof(Kat))] Kat = 7
    }
}