using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RoslynNUnitLight.Setup
{
    public class TestDocument
    {
        public TestProject TestProject { get; }
        public DocumentId Id { get; }
        public string Name { get; }
        public string Text { get; }

        internal TestDocument(TestProject testProject, string name, string text)
        {
            this.TestProject = testProject;
            this.Id = DocumentId.CreateNewId(testProject.Id);
            this.Name = name;
            this.Text = text;
        }

        public static TestDocument Create(string languageName, string name = null, string text = null)
        {
            var testProject = TestProject.Create(languageName);
            return testProject.AddDocument(name, text);
        }

        internal DocumentInfo ToDocumentInfo() =>
            DocumentInfo.Create(
                id: this.Id,
                name: this.Name,
                loader: TextLoader.From(SourceText.From(this.Text).Container, VersionStamp.Create()));

        public Document ToDocument() =>
            this.TestProject
                .ToProject()
                .GetDocument(this.Id);
    }
}
