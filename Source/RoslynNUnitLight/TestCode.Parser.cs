using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace RoslynNUnitLight
{
    public partial class TestCode
    {
        private class Parser
        {
            private readonly char[] array;
            private int index;
            private readonly StringBuilder code;
            private int currentPosition;
            private int? position;

            public string Code => this.code.ToString();
            public int? Position => this.position;
            public Dictionary<string, List<TextSpan>> SpanMap { get; }

            public Parser(char[] array)
            {
                this.array = array;
                this.index = 0;
                this.code = new StringBuilder(array.Length);
                this.currentPosition = 0;
                this.position = null;
                this.SpanMap = new Dictionary<string, List<TextSpan>>();
            }

            private bool HasNext => index < array.Length;

            private char Next() => array[index++];

            private char? Peek(int offset = 0) =>
                index + offset < array.Length ? array[index + offset] : (char?)null;

            private void Skip(int count)
            {
                index += count;

                if (index > array.Length)
                {
                    index = array.Length;
                }
            }

            private void AddSpan(string name, TextSpan span)
            {
                List<TextSpan> spans;
                if (!SpanMap.TryGetValue(name, out spans))
                {
                    spans = new List<TextSpan>();
                    SpanMap.Add(name, spans);
                }

                spans.Add(span);
            }

            private void AddSpan(TextSpan span)
            {
                AddSpan(string.Empty, span);
            }

            public void Parse()
            {
                Stack<int> spanStack = null;

                while (HasNext)
                {
                    var ch = Next();
                    switch (ch)
                    {
                        case '$': // $$
                            if (Peek() == '$' && Peek(1) != '$')
                            {
                                if (Position != null)
                                {
                                    throw new InvalidOperationException("$$ can only appear once.");
                                }

                                this.position = this.currentPosition;
                                Skip(1);
                                continue;
                            }

                            // We found a $, but it isn't the $$ we're looking for. Add everything
                            // up to the next non-$ character and continue the outer loop.

                            code.Append(ch);
                            while (Peek() == '$')
                            {
                                code.Append(Next());
                            }

                            continue;

                        case '[': // [|
                            if (Peek() == '|')
                            {
                                if (spanStack == null)
                                {
                                    spanStack = new Stack<int>();
                                }

                                spanStack.Push(this.currentPosition);
                                Skip(1);
                                continue;
                            }

                            break;

                        case '|': // |]
                            if (Peek() == ']')
                            {
                                if (spanStack == null || spanStack.Count == 0)
                                {
                                    throw new InvalidOperationException("Found |] without matching [|");
                                }

                                var start = spanStack.Pop();
                                AddSpan(TextSpan.FromBounds(start, this.currentPosition));
                                Skip(1);
                                continue;
                            }

                            break;
                    }

                    code.Append(ch);
                    this.currentPosition++;
                }

                if (spanStack?.Count > 0)
                {
                    throw new InvalidOperationException("Found [| without matching |]");
                }
            }
        }
    }
}
