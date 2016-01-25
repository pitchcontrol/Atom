using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Behavior;

namespace Atom
{
    public class RootPanel : WebPageBaseViewModel
    {
        public override WebPageBaseViewModel Parent => null;
        public override bool IsDragable { get { return false; } }
        public override bool IsDropable { get { return true; } }
        public override string FieldInDb
        {
            get { return "Корень"; }
        }
        public override string Image
        {
            get { return @"/Images/Organization.png"; }
        }
        public RootPanel(ObservableCollection<WebPageBaseViewModel> parentCollection)
        {
            ParentCollection = parentCollection;
        }

        public override string Type { get { return "RootPanel"; } }

        public override string ToString()
        {
            return "RootPanel";
        }
    }
}
