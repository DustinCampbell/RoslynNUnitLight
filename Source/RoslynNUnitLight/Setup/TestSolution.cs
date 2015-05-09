using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace RoslynNUnitLight.Setup
{
    public class TestSolution
    {
        private int defaultProjectNameOrdinal = 1;
        private readonly List<TestProject> projects;

        public string Name { get; }

        private TestSolution(string name)
        {
            this.projects = new List<TestProject>();
            this.Name = name ?? nameof(TestSolution);
        }

        private string GetNextDefaultProjectName() =>
            $"{nameof(TestProject)}{defaultProjectNameOrdinal++}";

        public TestProject AddProject(string languageName, string name = null)
        {
            if (languageName == null)
            {
                throw new ArgumentNullException(nameof(languageName));
            }

            if (languageName != LanguageNames.CSharp &&
                languageName != LanguageNames.VisualBasic)
            {
                throw new ArgumentException(
                    $"Expected {LanguageNames.CSharp} or {LanguageNames.VisualBasic}, but was {languageName}",
                    nameof(languageName));
            }

            name = name ?? GetNextDefaultProjectName();

            var result = new TestProject(this, languageName, name);
            this.projects.Add(result);

            return result;
        }

        public static TestSolution Create(string name = null) =>
            new TestSolution(name);
    }
}
