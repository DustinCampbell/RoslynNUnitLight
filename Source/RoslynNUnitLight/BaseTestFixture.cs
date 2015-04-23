using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RoslynNUnitLight
{
    public abstract class BaseTestFixture
    {
        protected abstract string LanguageName { get; }

        protected bool TryGetDocumentAndSpan(string markupCode, out Document document, out TextSpan span)
        {
            string code;
            if (!MarkupCode.TryGetCodeAndSpan(markupCode, out code, out span))
            {
                document = null;
                return false;
            }

            document = GetDocument(code);
            return true;
        }

        protected Document GetDocument(string code) => new AdhocWorkspace()
                .AddProject("TestProject", LanguageName)
                .AddMetadataReference(MetadataReference.CreateFromAssembly(typeof(object).GetTypeInfo().Assembly))
                .AddMetadataReference(MetadataReference.CreateFromAssembly(typeof(Enumerable).GetTypeInfo().Assembly))
                .AddDocument("TestDocument", code);
    }
}
