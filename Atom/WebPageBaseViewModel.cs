using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Atom.Behavior;

namespace Atom
{
    /// <summary>
    /// Базовая модель для страницы
    /// </summary>
    public abstract class WebPageBaseViewModel : ViewModelBase
    {
        public ObservableCollection<WebPageBaseViewModel> ParentCollection { get; set; }

        public WebPageBaseViewModel(ObservableCollection<WebPageBaseViewModel> parentCollection)
        {
            ParentCollection = parentCollection;
            Children = new ObservableCollection<WebPageBaseViewModel>();
        }
        /// <summary>
        /// Можно таскать
        /// </summary>
        public abstract bool IsDragable { get; }
        public abstract bool IsDropable { get; }
        public virtual void Remove(WebPageBaseViewModel i)
        {
            ParentCollection.Remove(i);
        }
        /// <summary>
        /// Бросаем обьект
        /// </summary>
        public virtual void Drop(WebPageBaseViewModel data, int index = -1)
        {
            Children.Add(data);
        }
        private string _controlIdView;
        private string _controlIdEdit;
        private string _fieldInDb;
        private string _ruDescription;
        private string _enDescription;
        private string _type;

        /// <summary>
        /// Тип поля
        /// </summary>
        [Required]
        public virtual string Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                ValidateProperty(value);
                OnPropertyChanged();
                SetID();
            }
        }
        /// <summary>
        /// Ид контрола на странице редактирования
        /// </summary>
        public string ControlIdEdit
        {
            get { return _controlIdEdit; }
            set
            {
                if (value == _controlIdEdit) return;
                _controlIdEdit = value;
                ValidateProperty(value);
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Ид контрола на странице просмотра
        /// </summary>
        public string ControlIdView
        {
            get { return _controlIdView; }
            set
            {
                if (value == _controlIdView) return;
                _controlIdView = value;
                ValidateProperty(value);
                OnPropertyChanged();
            }
        }

        [Required]
        public string FieldInDb
        {
            get { return _fieldInDb; }
            set
            {
                if (value == _fieldInDb) return;
                _fieldInDb = value;
                ValidateProperty(value);
                OnPropertyChanged();
                SetID();
            }
        }

        [Required]
        public string RuDescription
        {
            get { return _ruDescription; }
            set
            {
                if (value == _ruDescription) return;
                _ruDescription = value;
                ValidateProperty(value);
                OnPropertyChanged();
            }
        }

        public string EnDescription
        {
            get { return _enDescription; }
            set
            {
                if (value == _enDescription) return;
                _enDescription = value;
                ValidateProperty(value);
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Дочерние контролы
        /// </summary>
        public ObservableCollection<WebPageBaseViewModel> Children { get; }
        protected void SetID()
        {
            string edit = "";
            string view = "";
            switch (Type)
            {
                case "int":
                case "decimal":
                case "varchar":
                    edit = "vtxt";
                    view = "vlbl";
                    break;
                case "datetime":
                    edit = "vtcl";
                    view = "vlbl";
                    break;
                case "file":
                    edit = "vtcl";
                    view = "vcfu";
                    break;
                case "dictionary":
                    view = "vlbl";
                    edit = "vddl";
                    break;
            }
            ControlIdView = view + FieldInDb;
            ControlIdEdit = edit + FieldInDb;
        }
    }
}