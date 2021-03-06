using System.Collections.ObjectModel;

namespace Atom.ViewModels
{
    public class PanelViewModel : WebPageBaseViewModel
    {
        public override WebPageBaseViewModel Parent { get; set; }
        public PanelViewModel(WebPageBaseViewModel parent)
        {
            Parent = parent;
            //ParentCollection = parent?.Children;
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
        /// ������� � �������� ����
        /// </summary>
        /// <returns></returns>
        public ModalViewModel CreateField()
        {
            ModalViewModel obj = new ModalViewModel(this);
            Children.Add(obj);
            return obj;
        }
        /// <summary>
        /// ������� � �������� ����
        /// </summary>
        /// <returns></returns>
        public GridViewModel CreateGrid()
        {
            GridViewModel obj = new GridViewModel(this);
            Children.Add(obj);
            return obj;
        }
    }
}