using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Atom.Behavior;
using Atom.ViewModels;
using Newtonsoft.Json;

namespace Atom
{
    /// <summary>
    /// Базовая модель для страницы
    /// </summary>
    public abstract class WebPageBaseViewModel : ViewModelBase
    {
        [JsonIgnore]
        public ObservableCollection<WebPageBaseViewModel> ParentCollection { get; set; }

        public WebPageBaseViewModel(ObservableCollection<WebPageBaseViewModel> parentCollection)
        {
            ParentCollection = parentCollection;
            Children = new ObservableCollection<WebPageBaseViewModel>();
            IsEditable = true;
            //Лень заполнять
            _enDescription = "Some text";
        }

        /// <summary>
        /// Можно таскать
        /// </summary>
        [JsonIgnore]
        public abstract bool IsDragable { get; }
        [JsonIgnore]
        public abstract bool IsDropable { get; }
        /// <summary>
        /// Бросаем обьект
        /// </summary>
        public virtual void Drop(WebPageBaseViewModel data, int index = -1)
        {
            //Удаляем у родителя
            data.ParentCollection.Remove(data);
            Children.Add(data);
            data.ParentCollection = Children;
        }
        private string _controlIdView;
        private string _controlIdEdit;
        private string _fieldInDb;
        private string _ruDescription;
        private string _enDescription;
        private string _type;
        private bool _isEditable;

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
        /// <summary>
        /// Означает что данное свойство редактируемо
        /// </summary>
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                if (value.Equals(_isEditable)) return;
                _isEditable = value;
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
        /// <summary>
        /// Описание на русском
        /// </summary>
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
        /// <summary>
        /// Описание на английском
        /// </summary>
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
        public ObservableCollection<WebPageBaseViewModel> Children { get; private set; }
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
                case "panel":
                    view = "cp";
                    edit = "cp";
                    break;
            }
            ControlIdView = view + FieldInDb;
            ControlIdEdit = edit + FieldInDb;
        }
        /// <summary>
        /// Кратинка
        /// </summary>
        public virtual string Image
        {
            get { return @"/Images/change.gif"; }
        }
        public override string ToString()
        {
            return string.Format("{0}: {1}", Type, FieldInDb);
        }
    }
}