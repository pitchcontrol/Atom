using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Atom.ViewModels
{
    /// <summary>
    /// Модель для массового изменения контролов
    /// </summary>
    public class MassiveSetViewModel : ValidationBase
    {
        public MassiveSetViewModel(ObservableCollection<WebPageBaseViewModel> properties)
        {
            _properties =new ObservableCollection<ModalViewModel>(properties.Flatten(i => i.Children).OfType<ModalViewModel>());
            SelectedModels = new ObservableCollection<ModalViewModel>();
        }

        private ObservableCollection<ModalViewModel> _properties;
        private string _tableName;
        private ObservableCollection<ModalViewModel> _selectedModels;

        /// <summary>
        /// Контролы на странице
        /// </summary>
        public ObservableCollection<ModalViewModel> Properties
        {
            get { return _properties; }
            private set
            {
                if (value == _properties) return;
                _properties = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Выбранные контролы
        /// </summary>
        public ObservableCollection<ModalViewModel> SelectedModels
        {
            get { return _selectedModels; }
            set
            {
                if (Equals(value, _selectedModels)) return;
                _selectedModels = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Таблица в базе
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set
            {
                if (value == _tableName) return;
                _tableName = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Применить свойства всем
        /// </summary>
        public void Set()
        {
            //SelectedModels.ForEach(i=> { i.TableName = TableName; });
        }
    }
}