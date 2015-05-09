using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace RoslynNUnitLight.Setup
{
    public class TestSolution
    {
        private int defaultProjectNameOrdinal = 1;

        private readonly List<TestProject> projects;
        private readonly ReadOnlyCollection<TestProject> readOnlyProjects;

        public SolutionId Id { get; }

        public ReadOnlyCollection<TestProject> Projects => this.readOnlyProjects;

        private TestSolution()
        {
            this.projects = new List<TestProject>();
            this.readOnlyProjects = new ReadOnlyCollection<TestProject>(this.projects);

            this.Id = SolutionId.CreateNewId();
        }

        public static TestSolution Create() => new TestSolution();

        internal SolutionInfo ToSolutionInfo() =>
            SolutionInfo.Create(
                id: this.Id,
                version: VersionStamp.Create(),
                projects: this.projects.Select(p => p.ToProjectInfo()));

        public Solution ToSolution() =>
            new AdhocWorkspace()
                .AddSolution(this.ToSolutionInfo());

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
    }
}
