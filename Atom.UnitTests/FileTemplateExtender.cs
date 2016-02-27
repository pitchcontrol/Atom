using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Atom.UnitTests
{
    public class FileTemplateExtender
    {
        public string GetTemplateText(string extension = ".txt", [CallerMemberName] string name = null)
        {

            string root = TestContext.CurrentContext.TestDirectory;
            string folder = this.GetType().Name;
            return File.ReadAllText($"{root}/../../{folder}/{name}{extension}");
        }
    }
}
