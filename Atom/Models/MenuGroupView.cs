using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Models
{
    public class MenuGroupView : ut_MenuGroupView
    {
        public IEnumerable<MenuGroupView> Childrens { get; set; }
        public string RuName { get; set; }
    }
}
