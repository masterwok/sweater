using Sweater.Core.Attributes;
using Sweater.Core.Indexers.Public.LeetX;
using Sweater.Core.Indexers.Public.Rarbg;
using Sweater.Core.Indexers.Public.ThePirateBay;

namespace Sweater.Core.Constants
{
    /// <summary>
    /// An enumeration of implemented indexers.
    /// </summary>
    public enum Indexer
    {
        // TODO: Consider removing "All" value 
        All,
        [Type(typeof(ThePirateBay))] ThePirateBay,
        [Type(typeof(LeetX))] LeetX,
        [Type(typeof(Rarbg))] Rarbg
    }
}