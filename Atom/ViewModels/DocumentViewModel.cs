using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        /// <summary>
        /// Индекс поля с описанием
        /// </summary>
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
                Validate();
                SelectTable();
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

        private IEnumerable<Table> _tables;
        private IEnumerable<KeyValuePair<int, string>> _headers;

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
            }

        }
        /// <summary>
        /// Выбранна таблица
        /// </summary>
        private void SelectTable()
        {
            if (!IsValid)
                return;
            Table table = _tables.ElementAt(CurrentTable);
            IEnumerable<TableRow> rows = table.Elements<TableRow>();
            Headers = rows.First().Elements<TableCell>().Select((i, c) => new KeyValuePair<int, string>(c, i.InnerText));
        }

        public IEnumerable<string> GetDescriptions()
        {
            Table table = _tables.ElementAt(CurrentTable);
            //Пропускаем первую строку
            return
                table.Elements<TableRow>().Skip(1).Select(i => i.Elements<TableCell>().ElementAt(DescriptionField.Key).InnerText);
        }
    }
}
