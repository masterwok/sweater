using System;

namespace Sweater.Core.Attributes
{
    public sealed class TypeAttribute : Attribute
    {
        private readonly Type _type;

        internal TypeAttribute(Type type) => _type = type;

        public Type Type => _type;
    }
}