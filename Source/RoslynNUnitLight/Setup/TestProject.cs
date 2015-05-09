using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace RoslynNUnitLight.Setup
{
    public class TestProject
    {
        private int defaultDocumentNameOrdinal = 1;

        private readonly List<TestDocument> documents;
        private readonly List<MetadataReference> references;
        private readonly ReadOnlyCollection<TestDocument> readOnlyDocuments;
        private readonly ReadOnlyCollection<MetadataReference> readOnlyReferences;

        public TestSolution TestSolution { get; }
        public string Name { get; }
        public string LanguageName { get; }

        public ReadOnlyCollection<TestDocument> Documents => readOnlyDocuments;
        public ReadOnlyCollection<MetadataReference> References => readOnlyReferences;

        internal TestProject(TestSolution testSolution, string languageName, string name, bool includeCommonReferences)
        {
            this.TestSolution = testSolution;
            this.Name = name;
            this.LanguageName = languageName;

            this.documents = new List<TestDocument>();
            this.references = new List<MetadataReference>();
            this.readOnlyDocuments = new ReadOnlyCollection<TestDocument>(this.documents);
            this.readOnlyReferences = new ReadOnlyCollection<MetadataReference>(this.references);

            if (includeCommonReferences)
            {
                AddCommonReferences();
            }
        }

        private void AddCommonReferences()
        {
            AddReferences(
                MetadataReference.CreateFromAssembly(typeof(object).GetTypeInfo().Assembly),
                MetadataReference.CreateFromAssembly(typeof(Enumerable).GetTypeInfo().Assembly));
        }

        private string GetNextDefaultDocumentName() =>
            $"{nameof(TestDocument)}{defaultDocumentNameOrdinal++}";

        public TestDocument AddDocument(string name = null)
        {
            name = name ?? GetNextDefaultDocumentName();

            var result = new TestDocument(this, name);
            this.documents.Add(result);

            return result;
        }

        public void AddReference(MetadataReference reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            this.references.Add(reference);
        }

        public void AddReferences(IEnumerable<MetadataReference> references)
        {
            if (references == null)
            {
                throw new ArgumentNullException(nameof(references));
            }

            this.references.AddRange(references);
        }

        public void AddReferences(params MetadataReference[] references)
        {
            if (references == null)
            {
                throw new ArgumentNullException(nameof(references));
            }

            this.references.AddRange(references);
        }

        public static TestProject Create(string languageName, string name = null, bool includeCommonReferences = true)
        {
            var testSolution = TestSolution.Create(name);
            return testSolution.AddProject(languageName, name);
        }
    }
}
