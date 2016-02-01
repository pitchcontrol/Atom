using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Models;
using Atom.Validation;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Atom.ViewModels
{
    public class DocumentViewModel : ValidationBase
    {
        private KeyValuePair<int, string> _descriptionField;
        private int _tableCount;
        private int _currentTable;

        public DocumentViewModel()
        {
            _currentTable = -1;
            Validate();
        }
        /// <summary>
        /// Выбранное поле
        /// </summary>
        public KeyValuePair<int, string> SelectedField
        {
            get { return _selectedField; }
            set
            {
                if (value.Equals(_selectedField)) return;
                _selectedField = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Индекс поля с описанием
        /// </summary>
        [Required]
        public KeyValuePair<int, string> DescriptionField
        {
            get { return _descriptionField; }
            set
            {
                if (_descriptionField.Equals(value)) return;
                _descriptionField = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Индекс поля с групами полей
        /// </summary>
        [Required]
        public KeyValuePair<int, string> GroupName
        {
            get { return _groupName; }
            set
            {
                if (value.Equals(_groupName)) return;
                _groupName = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Колонка с типом
        /// </summary>
        public KeyValuePair<int, string> TypeField
        {
            get { return _typeField; }
            set
            {
                if (value.Equals(_typeField)) return;
                _typeField = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Количество таблиц в документе
        /// </summary>
        public int TableCount
        {
            get { return _tableCount; }
            set
            {
                if (value == _tableCount) return;
                _tableCount = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Фиктивная колекция для выбора
        /// </summary>
        public IEnumerable<KeyValuePair<int, string>> ComboboxTables
        {
            get { return _comboboxTables; }
            set
            {
                if (value.Equals(_comboboxTables)) return;
                _comboboxTables = value;
                OnPropertyChanged();
            }
        }

        [MaxValue("TableCount")]
        /// <summary>
        /// Выбранная таблица
        /// </summary>
        public int CurrentTable
        {
            get { return _currentTable; }
            set
            {
                if (value == _currentTable) return;
                _currentTable = value;
                OnPropertyChanged();
                ValidateProperty(value);
                SelectTable();
            }
        }
        /// <summary>
        /// Выбранна валидная таблица
        /// </summary>
        public bool TableSelcted
        {
            get { return _tableSelcted; }
            set
            {
                if (value == _tableSelcted) return;
                _tableSelcted = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Базовая таблица
        /// </summary>
        [Required]
        public string BaseTable
        {
            get { return _baseTable; }
            set
            {
                if (value == _baseTable) return;
                _baseTable = value;
                OnPropertyChanged();
                ValidateProperty(value);
            }
        }

        /// <summary>
        /// Заголовок таблице
        /// </summary>
        public IEnumerable<KeyValuePair<int, string>> Headers
        {
            get { return _headers; }
            set
            {
                if (value.Equals(_headers)) return;
                _headers = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurentPageIndex
        {
            get { return _curentPageIndex; }
            set
            {
                if (value == _curentPageIndex) return;
                _curentPageIndex = value;
                OnPropertyChanged();
                PageCahnge(value);
            }
        }
        /// <summary>
        /// Выбор гридов
        /// </summary>
        public List<GridConvertViewModel> Grids
        {
            get { return _grids; }
            set
            {
                if (Equals(value, _grids)) return;
                _grids = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<Table> _tables;
        private IEnumerable<KeyValuePair<int, string>> _headers;
        private KeyValuePair<int, string> _groupName;
        private KeyValuePair<int, string> _selectedField;
        private KeyValuePair<int, string> _typeField;
        private string _baseTable;
        private bool _tableSelcted;
        private IEnumerable<KeyValuePair<int, string>> _comboboxTables;
        private int _curentPageIndex;
        private List<GridConvertViewModel> _grids;


        public void SetDescription()
        {
            DescriptionField = new KeyValuePair<int, string>(SelectedField.Key, SelectedField.Value);
        }

        public void SetGroupName()
        {
            GroupName = new KeyValuePair<int, string>(SelectedField.Key, SelectedField.Value);
        }

        public void SetType()
        {
            TypeField = new KeyValuePair<int, string>(SelectedField.Key, SelectedField.Value);
        }
        /// <summary>
        /// Загрузить
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Open(path, false))
            {
                _tables = document.MainDocumentPart.Document.Body.Elements<Table>();
                TableCount = _tables.Count();
                ComboboxTables = _tables.Select((i, c) => new KeyValuePair<int, string>(c, "Table №" + c));
            }

        }
        /// <summary>
        /// Выбранна таблица
        /// </summary>
        private void SelectTable()
        {
            if (!IsPropertyValid(nameof(CurrentTable)))
            {
                TableSelcted = false;
                return;
            }
            TableSelcted = true;
            Table table = _tables.ElementAt(CurrentTable);
            IEnumerable<TableRow> rows = table.Elements<TableRow>();
            Headers = rows.First().Elements<TableCell>().Select((i, c) => new KeyValuePair<int, string>(c, i.InnerText));
            TrySelect();
        }
        /// <summary>
        /// Валидность выставления гридов
        /// </summary>
        public bool GridConvertValid
        {
            get { return Grids.All(i => i.IsValid); }
        }
        /// <summary>
        /// Смена страницы
        /// </summary>
        /// <param name="page"></param>
        private void PageCahnge(int page)
        {
            //Выбираем гриды
            if (page == 2)
            {
                Grids?.ForEach(i => i.ErrorsChanged -= I_ErrorsChanged);
                Grids = GetGroupNames().Select(i => new GridConvertViewModel(i)).ToList();
                Grids?.ForEach(i => i.ErrorsChanged += I_ErrorsChanged);
            }

        }

        private void I_ErrorsChanged(object sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(GridConvertValid));
        }
        /// <summary>
        /// Попробуем выставить поля
        /// </summary>
        private void TrySelect()
        {
            var tmp = Headers.FirstOrDefault(i => i.Value.Equals("Наименование поля", StringComparison.InvariantCultureIgnoreCase));
            if (!tmp.Equals(default(KeyValuePair<int, string>)))
                DescriptionField = tmp;
            tmp = Headers.FirstOrDefault(i => i.Value.Equals("Группа полей", StringComparison.InvariantCultureIgnoreCase));
            if (!tmp.Equals(default(KeyValuePair<int, string>)))
                GroupName = tmp;
            tmp = Headers.FirstOrDefault(i => i.Value.Equals("Тип данных", StringComparison.InvariantCultureIgnoreCase));
            if (!tmp.Equals(default(KeyValuePair<int, string>)))
                TypeField = tmp;
        }

        public IEnumerable<string> GetGroupNames()
        {
            Table table = _tables.ElementAt(CurrentTable);
            //Пропускаем первую строку
            return
                table.Elements<TableRow>()
                    .Skip(1)
                    .Select(i => i.Elements<TableCell>().ElementAt(GroupName.Key).InnerText)
                    .Where(i => !string.IsNullOrEmpty(i))
                    .Distinct();
        }
        public IEnumerable<string> GetDescriptions()
        {
            Table table = _tables.ElementAt(CurrentTable);
            //Пропускаем первую строку
            return
                table.Elements<TableRow>().Skip(1).Select(i => i.Elements<TableCell>().ElementAt(DescriptionField.Key).InnerText);
        }
        /// <summary>
        /// Добавить в дерево
        /// </summary>
        public void Build(ObservableCollection<WebPageBaseViewModel> properties)
        {
            var description = GetGroupNames();
            RootPanel root = (RootPanel)properties.First();
            //Устанавливаем базовою таблицу
            root.TableName = BaseTable;
            var grids = Grids.Where(i => i.IsGrid);
            description.ForEach((i, c) =>
            {
                var grid = grids.FirstOrDefault(j => j.Description == i);
                if (grid != null)
                {
                    GridViewModel model = new GridViewModel(root);
                    model.FieldInDb = "grid" + c;
                    model.RuDescription = i;
                    model.TableName = grid.TableName;
                    root.Children.Add(model);
                }
                else
                {
                    PanelViewModel model = new PanelViewModel(root) { FieldInDb = "panel" + c, RuDescription = i };
                    root.Children.Add(model);
                } 
            });
            Table table = _tables.ElementAt(CurrentTable);
            //Загрузка полей
            var fields =
                table.Elements<TableRow>()
                    .Skip(1)
                    .Select(i => new DocProperty(i.Elements<TableCell>().ElementAt(DescriptionField.Key).InnerText, i.Elements<TableCell>().ElementAt(TypeField.Key).InnerText, i.Elements<TableCell>().ElementAt(GroupName.Key).InnerText));
            int count = 0;
            foreach (DocProperty field in fields)
            {
                ModalViewModel model;
                if (string.IsNullOrEmpty(field.Parent))
                {
                    model = new ModalViewModel(root);
                    root.Children.Add(model);
                }
                else
                {
                    //Поиск панели
                    var tmp = root.Children.FirstOrDefault(i => i.RuDescription == field.Parent && i.Type == "panel" || i.Type =="grid");
                    if (tmp == null)
                    {
                        model = new ModalViewModel(root);
                        root.Children.Add(model);
                    }
                    else
                    {
                        model = new ModalViewModel(tmp);
                        tmp.Children.Add(model);
                        //model.TableName = (tmp as GridViewModel)?.TableName;
                    }
                }
                model.FieldInDb = "field" + count.ToString();
                model.RuDescription = field.Description;
                model.Type = field.GetRightType();
                //model.TableName = model.TableName??BaseTable;
                count++;
            }
        }
    }
}
