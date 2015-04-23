using System.Linq;
using System.Reflection;
using System.Text;
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
            if (!TryGetCodeAndSpan(markupCode, out code, out span))
            {
                document = null;
                return false;
            }

            document = GetDocument(code);
            return true;
        }

        private bool TryGetCodeAndSpan(string markupCode, out string code, out TextSpan span)
        {
            code = null;
            span = default(TextSpan);

            var builder = new StringBuilder();

            var start = markupCode.IndexOf("[|");
            if (start < 0)
            {
                return false;
            }

            builder.Append(markupCode.Substring(0, start));

            var end = markupCode.IndexOf("|]");
            if (end < 0)
            {
                return false;
            }

            builder.Append(markupCode.Substring(start + 2, end - start - 2));
            builder.Append(markupCode.Substring(end + 2));

            code = builder.ToString();
            span = TextSpan.FromBounds(start, end - 2);

            return true;
        }

        protected Document GetDocument(string code)
        {
            return new AdhocWorkspace()
                .AddProject("TestProject", LanguageName)
                .AddMetadataReference(MetadataReference.CreateFromAssembly(typeof(object).GetTypeInfo().Assembly))
                .AddMetadataReference(MetadataReference.CreateFromAssembly(typeof(Enumerable).GetTypeInfo().Assembly))
                .AddDocument("TestDocument", code);
        }
    }
}
