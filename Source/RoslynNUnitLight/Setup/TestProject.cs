using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynNUnitLight.Setup
{
    public class TestProject
    {
        public string Name { get; }
        public string LanguageName { get; }

        private readonly List<TestDocument> documents;

        public TestProject(string name, string languageName)
        {
            this.Name = name;
            this.LanguageName = languageName;

            this.documents = new List<TestDocument>();
        }

        public TestDocument AddDocument(string name)
        {
            var result = new TestDocument(name);
            this.documents.Add(result);
            return result;
        }
    }
}
