using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Atom.UnitTests
{
    public class Utils
    {
        public static string ReplaceSpaceFrom(string path)
        {
            string text = File.ReadAllText(path);
            return ReplaceSpaces(text);
        }
        public static string ReplaceSpaces(string text)
        {
            Regex regex = new Regex(@"(\s)+");
            return regex.Replace(text, " ");
        }
    }
}
