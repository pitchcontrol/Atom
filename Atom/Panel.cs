using System;
using System.Collections.ObjectModel;
using Atom.Behavior;

namespace Atom
{
    public class Panel : WebPageBaseViewModel, IDropable
    {
        public Panel(ObservableCollection<WebPageBaseViewModel> parentCollection) : base(parentCollection)
        {
        }

        public override string Type
        {
            get { return "panel"; }
        }

        public override bool IsDragable { get { return true; } }
        public override bool IsDropable { get { return true; } }
    }
}