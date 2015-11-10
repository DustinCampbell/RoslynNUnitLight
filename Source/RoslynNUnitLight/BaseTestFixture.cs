using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace RoslynNUnitLight
{
    public abstract class BaseTestFixture
    {
        protected abstract string LanguageName { get; }

        protected virtual ImmutableList<MetadataReference> References => ImmutableList.Create(
                MetadataReference.CreateFromAssembly(typeof(object).GetTypeInfo().Assembly),
                MetadataReference.CreateFromAssembly(typeof(Enumerable).GetTypeInfo().Assembly));
    }
}
