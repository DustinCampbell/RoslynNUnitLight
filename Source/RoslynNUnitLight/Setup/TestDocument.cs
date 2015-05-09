using Microsoft.CodeAnalysis;

namespace RoslynNUnitLight.Setup
{
    public class TestDocument
    {
        public TestProject TestProject { get; }
        public DocumentId Id { get; }
        public string Name { get; }

        internal TestDocument(TestProject testProject, string name)
        {
            this.TestProject = testProject;
            this.Id = DocumentId.CreateNewId(testProject.Id);
            this.Name = name;
        }

        public static TestDocument Create(string languageName, string name = null)
        {
            var testProject = TestProject.Create(languageName);
            return testProject.AddDocument(name);
        }

        internal DocumentInfo ToDocumentInfo() =>
            DocumentInfo.Create(
                id: this.Id,
                name: this.Name);

        public Document ToDocument() =>
            this.TestProject
                .ToProject()
                .GetDocument(this.Id);
    }
}
