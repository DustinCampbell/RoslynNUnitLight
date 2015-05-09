namespace RoslynNUnitLight.Setup
{
    public class TestDocument
    {
        public string Name { get; }

        internal TestDocument(string name)
        {
            this.Name = name;
        }

        public static TestDocument Create(string languageName, string name = null)
        {
            var testProject = TestProject.Create(languageName);
            return testProject.AddDocument(name);
        }
    }
}
