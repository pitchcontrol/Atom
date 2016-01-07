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

        public string GetRightType()
        {

            if (Type.Equals("число", StringComparison.InvariantCultureIgnoreCase))
            {
                return "int";
            }
            if (Type.Equals("дата", StringComparison.InvariantCultureIgnoreCase))
            {
                return "date";
            }
            if (Type.Equals("время", StringComparison.InvariantCultureIgnoreCase))
            {
                return "time";
            }
            if (Type.Equals("текст", StringComparison.InvariantCultureIgnoreCase))
            {
                return "varchar";
            }
            if (Type.Equals("справочник", StringComparison.InvariantCultureIgnoreCase))
            {
                return "dictionary";
            }
            if (Type.ToLower().Contains("ссылка"))
            {
                return "hyperlink";
            }
            return "";
        }
    }
}
