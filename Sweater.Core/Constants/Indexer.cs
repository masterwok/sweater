using Sweater.Core.Attributes;
using Sweater.Core.Indexers.Public.Kat;
using Sweater.Core.Indexers.Public.LeetX;
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
        [Type(typeof(ThePirateBay))] ThePirateBay,
        [Type(typeof(LeetX))] LeetX,
        [Type(typeof(Rarbg))] Rarbg,
        [Type(typeof(Kat))] Kat,
        [Type(typeof(Zooqle))] Zooqle
    }
}