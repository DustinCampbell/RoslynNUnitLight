using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace RoslynNUnitLight
{
    public abstract class AnalysisTestFixture
    {
        protected abstract string DiagnosticId { get; }

        protected abstract DiagnosticAnalyzer CreateAnalyzer();

        protected abstract CodeFixProvider CreateFixProvider();

        public void AssertDiagnostics(AnalyzerTestContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            RunDiagnostics(context);
        }

        private ImmutableArray<Diagnostic> RunDiagnostics(AnalyzerTestContext context)
        {
            DocumentWithSpans documentWithSpans = TestHelpers2.GetDocumentAndSpansFromMarkup(context.MarkupCode,
                context.LanguageName, context.References, context.FileName);

            ImmutableArray<Diagnostic> diagnostics = GetDiagnosticsForDocument(documentWithSpans.Document);
            IList<TextSpan> spans = documentWithSpans.TextSpans;
            Assert.That(diagnostics.Length, Is.EqualTo(spans.Count));

            for (int index = 0; index < diagnostics.Length; index++)
            {
                var diagnostic = diagnostics[index];
                var span = spans[index];

                Assert.That(diagnostic.Id, Is.EqualTo(DiagnosticId));
                Assert.That(diagnostic.Location.IsInSource, Is.True);
                Assert.That(diagnostic.Location.SourceSpan, Is.EqualTo(span));
            }

            return diagnostics;
        }

        private ImmutableArray<Diagnostic> GetDiagnosticsForDocument(Document document)
        {
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create(CreateAnalyzer());
            Compilation compilation = document.Project.GetCompilationAsync().Result;
            CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(analyzers,
                cancellationToken: CancellationToken.None);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(CancellationToken.None);
            ValidateCompilerDiagnostics(compilerDiagnostics);

            SyntaxTree tree = document.GetSyntaxTreeAsync().Result;

            ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();
            foreach (Diagnostic analyzerDiagnostic in compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result)
            {
                Location location = analyzerDiagnostic.Location;
                if (location.IsInSource && location.SourceTree == tree)
                {
                    builder.Add(analyzerDiagnostic);
                }
            }

            return builder.ToImmutable();
        }

        protected virtual void ValidateCompilerDiagnostics(ImmutableArray<Diagnostic> compilerDiagnostics)
        {
            bool hasErrors = compilerDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);
            Assert.That(hasErrors, Is.False);
        }

        public void AssertDiagnosticsWithCodeFixes(FixProviderTestContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ImmutableArray<Diagnostic> diagnostics = RunDiagnostics(context.AnalyzerTestContext);

            IList<string> expectedCode = TestHelpers2.RemoveMarkupFrom(context.Expected,
                context.AnalyzerTestContext.LanguageName, context.ReformatExpected,
                context.AnalyzerTestContext.References, context.AnalyzerTestContext.FileName);
            context = context.WithExpected(expectedCode);

            CodeFixProvider fixProvider = CreateFixProvider();
            foreach (var diagnostic in diagnostics)
            {
                RunCodeFixes(context, diagnostic, fixProvider);
            }
        }

        private void RunCodeFixes(FixProviderTestContext context, Diagnostic diagnostic, CodeFixProvider fixProvider)
        {
            for (int index = 0; index < context.Expected.Count; index++)
            {
                Document document =
                    TestHelpers2.GetDocumentAndSpansFromMarkup(context.AnalyzerTestContext.MarkupCode,
                        context.AnalyzerTestContext.LanguageName, context.AnalyzerTestContext.References,
                        context.AnalyzerTestContext.FileName).Document;

                ImmutableArray<CodeAction> codeFixes = GetCodeFixesForDiagnostic(diagnostic, document, fixProvider);
                Assert.That(codeFixes.Length, Is.EqualTo(context.Expected.Count));

                Verify.CodeAction(codeFixes[index], document, context.Expected[index]);
            }
        }

        private ImmutableArray<CodeAction> GetCodeFixesForDiagnostic(Diagnostic diagnostic, Document document,
            CodeFixProvider fixProvider)
        {
            ImmutableArray<CodeAction>.Builder builder = ImmutableArray.CreateBuilder<CodeAction>();
            Action<CodeAction, ImmutableArray<Diagnostic>> registerCodeFix = (a, _) => builder.Add(a);

            var context = new CodeFixContext(document, diagnostic, registerCodeFix, CancellationToken.None);
            fixProvider.RegisterCodeFixesAsync(context).Wait();

            return builder.ToImmutable();
        }
    }
}