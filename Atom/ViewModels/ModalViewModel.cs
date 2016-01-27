using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Atom.Constant;
using Atom.Validation;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Atom.ViewModels
{
    /// <summary>
    /// ������ ��� ��� �������� ��������
    /// </summary>
    public class ModalViewModel : WebPageBaseViewModel
    {
        private string _dictionaryType;
        private string _dictionaryTableName;

        public ModalViewModel([NotNull] WebPageBaseViewModel parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            Parent = parent;
            ParentCollection = Parent.Children;
            Validate();
        }

        public string TableName => GetTableName(Parent);

        /// <summary>
        /// �������� ������� ��������
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
        /// ��� �������(���� ��� dictionary)
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
        /// ������� ������� ���� ��� ������� SimpleDictionary, DictionaryTable
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