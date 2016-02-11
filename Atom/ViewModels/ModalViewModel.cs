using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Atom.Constant;
using Atom.Validation;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Atom.ViewModels
{
    /// <summary>
    /// Модель для для создания контрола
    /// </summary>
    public class ModalViewModel : WebPageBaseViewModel
    {
        private string _dictionaryType;
        private string _dictionaryTableName;
        private string _tableJoinAlias;

        public ModalViewModel(WebPageBaseViewModel parent)
        {
            Parent = parent;
            //ParentCollection = Parent?.Children;
            Validate();

        }

        public string TableName => GetTableName(Parent);

        /// <summary>
        /// Тип поля
        /// </summary>
        [Required]
        public override string Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                Validate();
                OnPropertyChanged();
                SetID();
                SetTableJoinAlias();
            }
        }

        /// <summary>
        /// Получить таблицу родителя
        /// </summary>
        private string GetTableName(WebPageBaseViewModel parent)
        {
            if (parent == null)
                return null;
            if (parent is RootPanel)
                return (parent as RootPanel).TableName;
            if (parent is GridViewModel)
                return (parent as GridViewModel).TableName;
            return GetTableName(parent.Parent);
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
                SetTableJoinAlias();
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
                SetTableJoinAlias();
            }
        }
        /// <summary>
        /// Алиас таблицы который будет использоватся при джойне
        /// </summary>
        [TriggerRequired("Type", new[] { ControlTypes.Dictionary, ControlTypes.File })]
        public string TableJoinAlias
        {
            get { return _tableJoinAlias; }
            set
            {
                if (value == _tableJoinAlias) return;
                _tableJoinAlias = value;
                OnPropertyChanged();
                ValidateProperty(value);
            }
        }

        private void SetTableJoinAlias()
        {
            if (string.IsNullOrEmpty(TableJoinAlias))
            {
                switch (Type)
                {
                    case ControlTypes.File:
                        TableJoinAlias = "df";
                        break;
                    case ControlTypes.Dictionary:
                        switch (DictionaryType)
                        {
                            case DictionaryTypes.UlName:
                                TableJoinAlias = "ul";
                                break;
                            case DictionaryTypes.FlName:
                                TableJoinAlias = "fl";
                                break;
                            case DictionaryTypes.DictionaryTable:
                            case DictionaryTypes.SimpleDictionary:
                                if (!string.IsNullOrEmpty(DictionaryTableName))
                                    TableJoinAlias = DictionaryTableName;
                                break;
                        }
                        break;
                }
            }
        }
        public override WebPageBaseViewModel Parent { get; set; }

        public override bool IsDragable { get { return true; } }
        public override bool IsDropable { get { return false; } }
    }
}