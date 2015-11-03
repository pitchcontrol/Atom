using System;
using System.Collections.ObjectModel;
using Atom.Behavior;
using Newtonsoft.Json;

namespace Atom
{
    public class Panel : WebPageBaseViewModel
    {
        public Panel(ObservableCollection<WebPageBaseViewModel> parentCollection) : base(parentCollection)
        {
        }
        
        public override string Type
        {
            get { return "panel"; }
        }
        public override string Image
        {
            get { return @"/Images/mainMenuFolderGray.gif"; }
        }
        public override bool IsDragable { get { return true; } }
        public override bool IsDropable { get { return true; } }
        public override string ToString()
        {
            return string.Format("Panel: {0}", FieldInDb);
        }
    }
}