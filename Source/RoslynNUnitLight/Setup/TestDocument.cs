using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynNUnitLight.Setup
{
    public class TestDocument
    {
        public string Name { get; }

        public TestDocument(string name)
        {
            this.Name = name;
        }
    }
}
