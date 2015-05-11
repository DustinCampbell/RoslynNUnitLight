using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RoslynNUnitLight
{
    public class FixProviderTestContext
    {
        public virtual AnalyzerTestContext AnalyzerTestContext { get; }

        public virtual ImmutableList<string> Expected { get; }

        public virtual bool ReformatExpected { get; }

        public FixProviderTestContext(AnalyzerTestContext analyzerTestContext, IEnumerable<string> expected,
            bool reformatExpected = true)
        {
            if (analyzerTestContext == null)
            {
                throw new ArgumentNullException(nameof(analyzerTestContext));
            }
            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            AnalyzerTestContext = analyzerTestContext;
            Expected = ImmutableList.CreateRange(expected);
            ReformatExpected = reformatExpected;
        }

        public virtual FixProviderTestContext WithExpected(IEnumerable<string> expected)
        {
            return new FixProviderTestContext(AnalyzerTestContext, expected, ReformatExpected);
        }
    }
}