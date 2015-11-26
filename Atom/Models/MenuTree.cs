using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Models
{
    public class MenuTree
    {
        private string _name;

        public MenuTree()
        {
            Children = new List<MenuTree>();
        }

        public int Id { get; set; }
        public int ParentGroupId { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (!string.IsNullOrEmpty(_name))
                {
                    NameRu = _name.Split(';')[0].Split(':')[1] + " (" + Id + ")";
                }
            }
        }

        public string Image
        {
            get { return IsGroup ? @"/Images/mainMenuFolderGray.gif" : @"/Images/change.gif"; }
        }
        public string NameRu { get; set; }
        public string Url { get; set; }
        public bool IsGroup { get; set; }
        public int PageId { get; set; }
        public List<MenuTree> Children { get; set; }
    }
}
