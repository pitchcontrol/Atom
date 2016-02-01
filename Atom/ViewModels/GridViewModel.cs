using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.ViewModels
{
    public class GridViewModel: WebPageBaseViewModel
    {
        public GridViewModel(WebPageBaseViewModel parent)
        {
            Parent = parent;
            //ParentCollection = parent.Children;
            Validate();
        }
        public override string Type
        {
            get { return "grid"; }
        }
        public override string Image
        {
            get
            {
                return @"/Images/fullList.png";
            }
        }
        private string _tableName;
        [Required]
        public string TableName
        {
            get { return _tableName; }
            set
            {
                if (value == _tableName) return;
                _tableName = value;
                ValidateProperty(value);
                OnPropertyChanged();
            }
        }
        
        public override WebPageBaseViewModel Parent { get; set; }

        public override bool IsDragable { get { return true; } }
        public override bool IsDropable { get { return true; } }
        public override string ToString()
        {
            return string.Format("GridViewModel: {0}", FieldInDb);
        }
    }
}
