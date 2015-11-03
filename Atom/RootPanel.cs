using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Behavior;

namespace Atom
{
    public class RootPanel: WebPageBaseViewModel
    {
        public override bool IsDragable { get { return false; } }
        public override bool IsDropable { get { return true; } }
        public string FieldInDb
        {
            get { return "Корень"; }
        }
        public override string Image
        {
            get { return @"/Images/Organization.png"; }
        }
        public RootPanel(ObservableCollection<WebPageBaseViewModel> parentCollection) : base(parentCollection)
        {
        }
    }
}
