using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Atom.Constant;
using Atom.Validation;
using Newtonsoft.Json;

namespace Atom.ViewModels
{
    /// <summary>
    /// Модель для для создания контрола
    /// </summary>
    public class ModalViewModel : WebPageBaseViewModel
    {
        private string _tableName;
        private string _dictionaryType;
        private string _dictionaryTableName;

        public ModalViewModel(WebPageBaseViewModel parent)
        {
            Parent = parent;
            Validate();
        }

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
        /// Тип словаря(если тип dictionary)
        /// </summary>
        [TriggerRequired("Type", new[] { ControlTypes.Dictionary })]
        public string DictionaryType
        {
            get { return _dictionaryType; }
            set
            {
                if (value == _dictionaryType) return;
                _dictionaryType = value;
                OnPropertyChanged();
                Validate();
            }
        }

        /// <summary>
        /// Таблица словаря если тип словаря SimpleDictionary, DictionaryTable
        /// </summary>
        [TriggerRequired("DictionaryType", new[] { DictionaryTypes.DictionaryTable, DictionaryTypes.SimpleDictionary })]
        public string DictionaryTableName
        {
            get { return _dictionaryTableName; }
            set
            {
                if (value == _dictionaryTableName) return;
                _dictionaryTableName = value;
                OnPropertyChanged();
                ValidateProperty(value);
            }
        }

        public override WebPageBaseViewModel Parent { get; }
        public override bool IsDragable { get { return true; } }
        public override bool IsDropable { get { return false; } }
    }
}