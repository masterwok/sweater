using System;
using System.Linq;

namespace Sweater.Core.Extensions
{
    /// <summary>
    /// This class contains a collection of extension methods for Enum types.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get an attribute of an Enum.
        /// </summary>
        /// <param name="enum">The enum value</param>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The value of the attribute on the enum.</returns>
        public static TAttribute GetAttribute<TAttribute>(
            this Enum @enum
        ) where TAttribute : Attribute
        {
            var type = @enum.GetType();
            var name = Enum.GetName(type, @enum);

            return type
                .GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}