using System;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using RoslynNUnitLight.Setup;

namespace RoslynNUnitLight.Tests
{
    [TestFixture]
    public class SetupTests
    {
        [Test]
        public void SolutionCreatedWithNoNameHasDefaultName()
        {
            var solution = TestSolution.Create();
            solution.Name.Should().Be(nameof(TestSolution));
        }

        [Test]
        public void SolutionCreatedWithNameHasName()
        {
            var solution = TestSolution.Create("MySolution");
            solution.Name.Should().Be("MySolution");
        }

        [Test]
        public void ProjectAddedWithNoNameHasDefaultName()
        {
            var solution = TestSolution.Create();

            var project1 = solution.AddProject(LanguageNames.CSharp);
            project1.Name.Should().Be($"{nameof(TestProject)}1");

            var project2 = solution.AddProject(LanguageNames.VisualBasic);
            project2.Name.Should().Be($"{nameof(TestProject)}2");
        }

        [Test]
        public void ProjectAddedWithNameHasName()
        {
            var solution = TestSolution.Create();

            var project1 = solution.AddProject(LanguageNames.CSharp, "MyProject1");
            project1.Name.Should().Be("MyProject1");

            var project2 = solution.AddProject(LanguageNames.VisualBasic, "MyProject2");
            project2.Name.Should().Be("MyProject2");
        }

        [Test]
        public void ProjectAddedWithBadLanguageNameThrows()
        {
            var solution = TestSolution.Create();

            Action a = () => solution.AddProject("Swift");
            a.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void ProjectCreatedWithNoNameHasDefaultName()
        {
            var project = TestProject.Create(LanguageNames.CSharp);
            project.Name.Should().Be($"{nameof(TestProject)}1");
        }

        [Test]
        public void ProjectCreatedWithNameHasName()
        {
            var project = TestProject.Create(LanguageNames.CSharp, "MyProject");
            project.Name.Should().Be("MyProject");
        }

        [Test]
        public void ProjectCreatedWithBadLanguageNameThrows()
        {
            Action a = () => TestProject.Create("Swift");
            a.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void DocumentAddedWithNoNameHasDefaultName()
        {
            var project = TestProject.Create(LanguageNames.CSharp);

            var document = project.AddDocument();
            document.Name.Should().Be($"{nameof(TestDocument)}1");
        }

        [Test]
        public void DocumentAddedWithNameHasName()
        {
            var project = TestProject.Create(LanguageNames.CSharp);

            var document = project.AddDocument("MyDocument1");
            document.Name.Should().Be("MyDocument1");
        }

        [Test]
        public void DocumentCreatedWithNoNameHasDefaultName()
        {
            var document = TestDocument.Create(LanguageNames.CSharp);
            document.Name.Should().Be($"{nameof(TestDocument)}1");
        }

        [Test]
        public void DocumentCreatedWithNameHasName()
        {
            var document = TestDocument.Create(LanguageNames.CSharp, "MyDocument");
            document.Name.Should().Be("MyDocument");
        }

        [Test]
        public void DocumentCreatedWithBadLanguageNameThrows()
        {
            Action a = () => TestDocument.Create("Swift");
            a.ShouldThrow<ArgumentException>();
        }
    }
}
