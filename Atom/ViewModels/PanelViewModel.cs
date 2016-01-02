using System.Collections.ObjectModel;

namespace Atom.ViewModels
{
    public class PanelViewModel : WebPageBaseViewModel
    {

        public PanelViewModel(ObservableCollection<WebPageBaseViewModel> parentCollection) : base(parentCollection)
        {
            Validate();
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
            return string.Format("PanelViewModel: {0}", FieldInDb);
        }
    }
}