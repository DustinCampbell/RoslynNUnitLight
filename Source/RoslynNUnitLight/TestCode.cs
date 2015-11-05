using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;

namespace RoslynNUnitLight
{
    public partial class TestCode
    {
        public string OriginalText { get; }
        public string Code { get; }
        public int? Position { get; }
        private readonly Dictionary<string, List<TextSpan>> spanMap;

        private static readonly IReadOnlyList<TextSpan> EmptySpans = new TextSpan[0];

        private TestCode(string originalText, string code, int? position, Dictionary<string, List<TextSpan>> spanMap)
        {
            this.OriginalText = originalText;
            this.Code = code;
            this.Position = position;
            this.spanMap = spanMap;
        }

        public IReadOnlyList<TextSpan> GetSpans(string name = null)
        {
            name = name ?? string.Empty;

            List<TextSpan> spans;
            if (this.spanMap.TryGetValue(name, out spans))
            {
                return spans;
            }

            return EmptySpans;
        }

        public static TestCode Parse(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var parser = new Parser(text.ToCharArray());
            parser.Parse();

            return new TestCode(text, parser.Code, parser.Position, parser.SpanMap);
        }
    }
}
