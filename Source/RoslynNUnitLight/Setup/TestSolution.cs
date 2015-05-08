using System.Collections.Generic;

namespace RoslynNUnitLight.Setup
{
    public class TestSolution
    {
        public string Name { get; }
        private readonly List<TestProject> projects;

        public TestSolution(string name)
        {
            this.Name = name;
            this.projects = new List<TestProject>();
        }

        public TestProject AddProject(string name, string languageName)
        {
            var result = new TestProject(name, languageName);
            this.projects.Add(result);
            return result;
        }
    }
}
