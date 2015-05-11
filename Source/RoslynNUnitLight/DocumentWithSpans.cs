using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RoslynNUnitLight
{
    internal class DocumentWithSpans
    {
        public Document Document { get; }
        public IList<TextSpan> TextSpans { get; }

        public DocumentWithSpans(Document document, IList<TextSpan> textSpans)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            if (textSpans == null)
            {
                throw new ArgumentNullException(nameof(textSpans));
            }

            Document = document;
            TextSpans = textSpans;
        }
    }
}