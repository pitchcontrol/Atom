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
        /// <summary>
        /// Создать и добавить поле
        /// </summary>
        /// <returns></returns>
        public ModalViewModel CreateField()
        {
            ModalViewModel obj = new ModalViewModel(Children);
            Children.Add(obj);
            return obj;
        }
        /// <summary>
        /// Создать и добавить грид
        /// </summary>
        /// <returns></returns>
        public GridViewModel CreateGrid()
        {
            GridViewModel obj = new GridViewModel(Children);
            Children.Add(obj);
            return obj;
        }
    }
}