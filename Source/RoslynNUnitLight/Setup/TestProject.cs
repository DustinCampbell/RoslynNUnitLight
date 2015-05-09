using System.Collections.Generic;

namespace RoslynNUnitLight.Setup
{
    public class TestProject
    {
        private const string DefaultDocumentName = "TestDocument";
        private int defaultDocumentNameOrdinal = 1;

        private readonly TestSolution testSolution;
        private readonly List<TestDocument> documents;

        public string Name { get; }
        public string LanguageName { get; }

        internal TestProject(TestSolution testSolution, string languageName, string name)
        {
            this.testSolution = testSolution;
            this.documents = new List<TestDocument>();

            this.Name = name;
            this.LanguageName = languageName;
        }

        private string GetNextDefaultDocumentName() =>
            $"{DefaultDocumentName}{defaultDocumentNameOrdinal++}";

        public TestDocument AddDocument(string name)
        {
            name = name ?? GetNextDefaultDocumentName();

            var result = new TestDocument(name);
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
