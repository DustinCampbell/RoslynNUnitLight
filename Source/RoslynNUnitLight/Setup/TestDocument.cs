namespace RoslynNUnitLight.Setup
{
    public class TestDocument
    {
        public TestProject TestProject { get; }
        public string Name { get; }

        internal TestDocument(TestProject testProject, string name)
        {
            this.TestProject = testProject;
            this.Name = name;
        }

        public static TestDocument Create(string languageName, string name = null)
        {
            var testProject = TestProject.Create(languageName);
            return testProject.AddDocument(name);
        }
    }
}
