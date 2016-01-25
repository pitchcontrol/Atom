using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Atom.ViewModels
{
    /// <summary>
    /// Модель для для создания контрола
    /// </summary>
    public class ModalViewModel : WebPageBaseViewModel
    {
        private string _tableName;

        public ModalViewModel(WebPageBaseViewModel parent)
        {
            Parent = parent;
            //Types = new[] { "int", "decimal", "bit", "varchar", "file", "dictionary", "hyperlink", "datetime", "date", "time" };
            Validate();
        }
        //[JsonIgnore]
        //public IEnumerable<string> Types { get; set; }


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

        public override WebPageBaseViewModel Parent { get; }
        public override bool IsDragable { get { return true; } }
        public override bool IsDropable { get { return false; } }
    }
}