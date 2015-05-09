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
        private readonly List<MetadataReference> metadataReferences;
        private readonly ReadOnlyCollection<TestDocument> readOnlyDocuments;
        private readonly ReadOnlyCollection<MetadataReference> readOnlyMetadataReferences;

        public TestSolution TestSolution { get; }
        public ProjectId Id { get; }
        public string Name { get; }
        public string LanguageName { get; }

        public ReadOnlyCollection<TestDocument> Documents => readOnlyDocuments;
        public ReadOnlyCollection<MetadataReference> MetadataReferences => readOnlyMetadataReferences;

        internal TestProject(TestSolution testSolution, string languageName, string name, bool includeCommonReferences)
        {
            this.TestSolution = testSolution;
            this.Id = ProjectId.CreateNewId();
            this.Name = name;
            this.LanguageName = languageName;

            this.documents = new List<TestDocument>();
            this.metadataReferences = new List<MetadataReference>();
            this.readOnlyDocuments = new ReadOnlyCollection<TestDocument>(this.documents);
            this.readOnlyMetadataReferences = new ReadOnlyCollection<MetadataReference>(this.metadataReferences);

            if (includeCommonReferences)
            {
                AddCommonReferences();
            }
        }

        public static TestProject Create(string languageName, string name = null, bool includeCommonReferences = true)
        {
            var testSolution = TestSolution.Create();
            return testSolution.AddProject(languageName, name);
        }

        internal ProjectInfo ToProjectInfo() =>
            ProjectInfo.Create(
                id: this.Id,
                version: VersionStamp.Create(),
                name: this.Name,
                assemblyName: this.Name,
                language: this.LanguageName,
                documents: this.documents.Select(d => d.ToDocumentInfo()),
                metadataReferences: this.metadataReferences);

        public Project ToProject() =>
            this.TestSolution
                .ToSolution()
                .GetProject(this.Id);

        private void AddCommonReferences()
        {
            AddReferences(
                MetadataReference.CreateFromAssembly(typeof(object).GetTypeInfo().Assembly),
                MetadataReference.CreateFromAssembly(typeof(Enumerable).GetTypeInfo().Assembly));
        }

        private string GetNextDefaultDocumentName() =>
            $"{nameof(TestDocument)}{defaultDocumentNameOrdinal++}";

        public TestDocument AddDocument(string name = null, string text = null)
        {
            name = name ?? GetNextDefaultDocumentName();
            text = text ?? string.Empty;

            var result = new TestDocument(this, name, text);
            this.documents.Add(result);

            return result;
        }

        public void AddReference(MetadataReference reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            this.metadataReferences.Add(reference);
        }

        public void AddReferences(IEnumerable<MetadataReference> references)
        {
            if (references == null)
            {
                throw new ArgumentNullException(nameof(references));
            }

            this.metadataReferences.AddRange(references);
        }

        public void AddReferences(params MetadataReference[] references)
        {
            if (references == null)
            {
                throw new ArgumentNullException(nameof(references));
            }

            this.metadataReferences.AddRange(references);
        }
    }
}
