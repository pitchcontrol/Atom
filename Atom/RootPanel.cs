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
        
        public override ObservableCollection<WebPageBaseViewModel> ParentCollection { get; }
        public override WebPageBaseViewModel Parent { get; set; }

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

        private string _tableName;
        private string _parentTableId;

        /// <summary>
        /// Основная таблица для страницы
        /// </summary>
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

        /// <summary>
        /// Ид таблица на которую идет ссылка например idul
        /// </summary>
        public string ParentTableId
        {
            get { return _parentTableId; }
            set
            {
                if (value == _parentTableId) return;
                _parentTableId = value;
                OnPropertyChanged();
            }
        }

        public override string Type { get { return "RootPanel"; } }

        public override string ToString()
        {
            return "RootPanel";
        }
    }
}
