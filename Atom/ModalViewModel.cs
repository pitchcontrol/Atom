using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Atom.Behavior;
using Newtonsoft.Json;

namespace Atom
{
    public class ModalViewModel : WebPageBaseViewModel
    {
        private string _tableName;

        public ModalViewModel(ObservableCollection<WebPageBaseViewModel> parent) : base(parent)
        {
            Types = new[] { "int", "decimal", "bit", "varchar", "file", "dictionary" };
            Validate();
        }
        [JsonIgnore]
        public IEnumerable<string> Types { get; set; }


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


        public override bool IsDragable { get { return true; } }
        public override bool IsDropable { get { return false; } }
    }
}