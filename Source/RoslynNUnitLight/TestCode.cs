using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace RoslynNUnitLight
{
    public class TestCode
    {
        public string OriginalText { get; }
        public string Code { get; }
        public int? Position { get; }
        private readonly Dictionary<string, List<TextSpan>> spans;

        private TestCode(string originalText, string code, int? position, Dictionary<string, List<TextSpan>> spans)
        {
            this.OriginalText = originalText;
            this.Code = code;
            this.Position = position;
            this.spans = spans;
        }

        private struct CharStream
        {
            private readonly char[] array;
            private int index;

            public CharStream(char[] array)
            {
                this.array = array;
                this.index = 0;
            }

            public bool HasNext => index < array.Length;

            public char Next() => array[index++];

            public char? Peek(int offset = 0) =>
                index + offset < array.Length ? array[index + offset] : (char?)null;

            public void Skip(int count)
            {
                index += count;

                if (index > array.Length)
                {
                    index = array.Length;
                }
            }
        }

        public static TestCode Parse(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var code = new StringBuilder(capacity: text.Length);
            var stream = new CharStream(text.ToCharArray());
            var codeIndex = 0;
            int? position = null;

            while (stream.HasNext)
            {
                var ch = stream.Next();
                switch (ch)
                {
                    case '$': // $$
                        if (stream.Peek() == '$' && stream.Peek(1) != '$')
                        {
                            if (position != null)
                            {
                                throw new ArgumentException("$$ can only appear once.", nameof(text));
                            }

                            position = codeIndex;
                            stream.Skip(1);
                            continue;
                        }

                        // We found a $, but it isn't the $$ we're looking for. Add everything
                        // up to the next non-$ character and continue the outer loop.

                        code.Append(ch);
                        while (stream.Peek() == '$')
                        {
                            code.Append(stream.Next());
                        }

                        continue;
                }

                code.Append(ch);
                codeIndex++;
            }

            return new TestCode(text, code.ToString(), position, spans: null);
        }
    }
}
