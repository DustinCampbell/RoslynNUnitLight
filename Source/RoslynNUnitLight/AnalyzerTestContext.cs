using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace RoslynNUnitLight
{
    public class AnalyzerTestContext
    {
        protected const string DefaultFileName = "TestDocument";

        protected static readonly ImmutableList<MetadataReference> DefaultReferences =
            ImmutableList.Create(MetadataReference.CreateFromAssembly(typeof (object).GetTypeInfo().Assembly),
                MetadataReference.CreateFromAssembly(typeof (Enumerable).GetTypeInfo().Assembly));

        public virtual string MarkupCode { get; }

        public virtual string LanguageName { get; }

        public virtual string FileName { get; }

        public virtual ImmutableList<MetadataReference> References { get; }

        protected AnalyzerTestContext(string markupCode, string languageName, string fileName,
            ImmutableList<MetadataReference> references)
        {
            if (markupCode == null)
            {
                throw new ArgumentNullException(nameof(markupCode));
            }
            if (languageName == null)
            {
                throw new ArgumentNullException(nameof(languageName));
            }

            MarkupCode = markupCode;
            LanguageName = languageName;
            FileName = fileName;
            References = references;
        }

        public AnalyzerTestContext(string markupCode, string languageName)
            : this(markupCode, languageName, DefaultFileName, DefaultReferences)
        {
        }

        public virtual AnalyzerTestContext WithFileName(string fileName)
        {
            return new AnalyzerTestContext(MarkupCode, LanguageName, fileName, References);
        }

        public virtual AnalyzerTestContext WithReferences(IEnumerable<MetadataReference> references)
        {
            ImmutableList<MetadataReference> referenceList = ImmutableList.CreateRange(references);
            return new AnalyzerTestContext(MarkupCode, LanguageName, FileName, referenceList);
        }
    }
}