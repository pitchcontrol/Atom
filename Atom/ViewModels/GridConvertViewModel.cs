using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Validation;

namespace Atom.ViewModels
{
    public class GridConvertViewModel: ValidationBase
    {
        private bool _isGrid;
        private string _tableName;
        private string _description;

        public GridConvertViewModel(string description)
        {
            _description = description;
        }

        /// <summary>
        /// Преобразовать в грид
        /// </summary>
        public bool IsGrid
        {
            get { return _isGrid; }
            set
            {
                if (value == _isGrid) return;
                _isGrid = value;
                OnPropertyChanged();
                Validate();
            }
        }
        /// <summary>
        /// Название таблицы
        /// </summary>
        [IfRequired(nameof(IsGrid))]
        public string TableName
        {
            get { return _tableName; }
            set
            {
                if (value == _tableName) return;
                _tableName = value;
                OnPropertyChanged();
                ValidateProperty(value);
            }
        }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }
    }
}
