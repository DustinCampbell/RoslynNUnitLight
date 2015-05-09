using System.Collections.Generic;

namespace RoslynNUnitLight.Setup
{
    public class TestProject
    {
        private int defaultDocumentNameOrdinal = 1;
        private readonly List<TestDocument> documents;

        public TestSolution TestSolution { get; }
        public string Name { get; }
        public string LanguageName { get; }

        internal TestProject(TestSolution testSolution, string languageName, string name)
        {
            this.documents = new List<TestDocument>();

            this.TestSolution = testSolution;
            this.Name = name;
            this.LanguageName = languageName;
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

        public static TestProject Create(string languageName, string name = null)
        {
            var testSolution = TestSolution.Create(name);
            return testSolution.AddProject(languageName, name);
        }
    }
}
