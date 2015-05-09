using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis;

namespace RoslynNUnitLight.Setup
{
    public class TestSolution
    {
        private int defaultProjectNameOrdinal = 1;

        private readonly List<TestProject> projects;
        private readonly ReadOnlyCollection<TestProject> readOnlyProjects;

        public string Name { get; }

        public ReadOnlyCollection<TestProject> Projects => this.readOnlyProjects;

        private TestSolution(string name)
        {
            this.Name = name ?? nameof(TestSolution);

            this.projects = new List<TestProject>();
            this.readOnlyProjects = new ReadOnlyCollection<TestProject>(this.projects);
        }

        private string GetNextDefaultProjectName() =>
            $"{nameof(TestProject)}{defaultProjectNameOrdinal++}";

        public TestProject AddProject(string languageName, string name = null, bool includeCommonReferences = true)
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

            var result = new TestProject(this, languageName, name, includeCommonReferences);
            this.projects.Add(result);

            return result;
        }

        public static TestSolution Create(string name = null) =>
            new TestSolution(name);
    }
}
