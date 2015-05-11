using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace RoslynNUnitLight
{
    internal class TestHelpers2
    {
        public static DocumentWithSpans GetDocumentAndSpansFromMarkup(string markupCode, string languageName,
            ImmutableList<MetadataReference> references, string fileName)
        {
            string code;
            IList<TextSpan> spans;
            GetCodeAndSpansFromMarkup(markupCode, out code, out spans);

            var document = GetDocument(code, languageName, references, fileName);
            return new DocumentWithSpans(document, spans);
        }

        private static void GetCodeAndSpansFromMarkup(string markupCode, out string code, out IList<TextSpan> spans)
        {
            code = null;
            spans = null;

            var codeBuilder = new StringBuilder();
            var textSpans = new List<TextSpan>();

            int offset = 0;
            var start = markupCode.IndexOf("[|", offset, StringComparison.Ordinal);
            while (start != -1)
            {
                codeBuilder.Append(markupCode.Substring(offset, start - offset));

                var end = markupCode.IndexOf("|]", start + 2, StringComparison.Ordinal);
                if (end == -1)
                {
                    throw new Exception("Missing |] in source.");
                }

                codeBuilder.Append(markupCode.Substring(start + 2, end - start - 2));

                int shift = textSpans.Count * 4;
                textSpans.Add(TextSpan.FromBounds(start - shift, end - 2 - shift));

                offset = end + 2;
                start = markupCode.IndexOf("[|", offset, StringComparison.Ordinal);
            }

            var extra = markupCode.IndexOf("|]", offset, StringComparison.Ordinal);
            if (extra != -1)
            {
                throw new Exception("Additional |] in source.");
            }

            codeBuilder.Append(markupCode.Substring(offset));

            spans = textSpans;
            code = codeBuilder.ToString();
        }

        public static Document GetDocument(string code, string languageName,
            ImmutableList<MetadataReference> references, string fileName)
        {
            return new AdhocWorkspace()
                .AddProject("TestProject", languageName)
                    .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddMetadataReferences(references)
                .AddDocument(fileName, code);
        }
        
        public static IList<string> RemoveMarkupFrom(IList<string> expected, string language, bool reformat,
            ImmutableList<MetadataReference> references, string fileName)
        {
            return expected.Select(text => 
                RemoveMarkupFrom(text, language, reformat, references, fileName)).ToList();
        }

        private static string RemoveMarkupFrom(string expected, string language, bool reformat,
            ImmutableList<MetadataReference> references, string fileName)
        {
            Document document =
                GetDocumentAndSpansFromMarkup(expected, language, references, fileName).Document;
            SyntaxNode syntaxRoot = document.GetSyntaxRootAsync().Result;

            if (reformat)
            {
                SyntaxNode formattedSyntaxRoot = Formatter.Format(syntaxRoot, document.Project.Solution.Workspace);
                return formattedSyntaxRoot.ToFullString();
            }

            return syntaxRoot.ToFullString();
        }
    }
}
