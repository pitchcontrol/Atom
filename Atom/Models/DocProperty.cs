using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Models
{
    public class DocProperty
    {
        public DocProperty(string description, string type, string parent)
        {
            Description = description;
            Type = type;
            Parent = parent;
        }

        public string Description { get; set; }
        public string Type { get; set; }
        public string Parent { get; set; }
    }
}
