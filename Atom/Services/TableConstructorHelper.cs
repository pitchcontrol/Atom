using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Constant;
using Atom.ViewModels;

namespace Atom.Services
{
    /// <summary>
    /// Создает таблицы
    /// </summary>
    public class TableConstructorHelper
    {
        StringBuilder _stringBuilder = new StringBuilder();
        List<string> _indexes = new List<string>(); 
        /// <summary>
        /// Таблица на которую идет зависимость например - idul
        /// </summary>
        public string ParentTableIdName { get; set; } = "idul";
        /// <summary>
        /// Построить таблицы
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="folderName">Папка для выгрузки</param>
        public void Construct(IEnumerable<WebPageBaseViewModel> collection, string folderName)
        {

            var plain = collection.Flatten(i => i.Children);

            var tables = plain.OfType<ModalViewModel>().Where(i => !(i.Parent is GridViewModel)).GroupBy(i => i.TableName);
            foreach (IGrouping<string, ModalViewModel> groupTable in tables)
            {
                WriteHeader(groupTable.Key,ParentTableIdName);
                groupTable.ForEach(i=> WriteField(i,groupTable.Key));
                WriteIndexes(groupTable.Key, ParentTableIdName);
                string tableName = $"{folderName}/{groupTable.Key}.sql";
                File.WriteAllText(tableName, _stringBuilder.ToString());
                _stringBuilder.Clear();
                _indexes.Clear();
                WriteIdentifier(groupTable.Key);
                tableName = $"{folderName}/{groupTable.Key}_id.sql";
                File.WriteAllText(tableName, _stringBuilder.ToString());
                _stringBuilder.Clear();
                _indexes.Clear();
            }
            var grids = plain.OfType<GridViewModel>();
            foreach (GridViewModel groupTable in grids)
            {
                WriteHeader(groupTable.TableName, "externalId");
                groupTable.Children.OfType<ModalViewModel>().ForEach(i => WriteField(i, groupTable.TableName));
                WriteIndexes(groupTable.TableName, "externalId");
                string tableName = $"{folderName}/{groupTable.TableName}.sql";
                File.WriteAllText(tableName, _stringBuilder.ToString());
                _stringBuilder.Clear();
                _indexes.Clear();
                WriteIdentifier(groupTable.TableName);
                tableName = $"{folderName}/{groupTable.TableName}_id.sql";
                File.WriteAllText(tableName, _stringBuilder.ToString());
                _stringBuilder.Clear();
                _indexes.Clear();
            }
        }

        private void WriteHeader(string tableName, string parentTableIdName)
        {
            _stringBuilder.AppendFormat("CREATE TABLE [dbo].[{0}] (\n", tableName);
            _stringBuilder.AppendFormat("[pkid] INT IDENTITY (1, 1) NOT NULL,\n");
            _stringBuilder.AppendFormat("[fl_del] INT CONSTRAINT [DF_{0}_del] DEFAULT ((0)) NULL,\n", tableName);
            _stringBuilder.AppendFormat("[idreq] INT NULL,\n[dats] datetime NOT NULL,\n[datf] datetime NOT NULL,\n[idcp] INT NULL,\n[fldchange] varchar(MAX) NULL,\n");
            _stringBuilder.AppendFormat("[idRecord] INT NOT NULL,\n");
            _stringBuilder.AppendFormat("[{0}] INT NOT NULL,\n", parentTableIdName);
        }

        private void WriteIndexes(string tableName, string parentTableIdName)
        {
            //Ид
            _stringBuilder.AppendFormat("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED([pkid] ASC),\n", tableName);
            //Ключевое поле
            _stringBuilder.AppendFormat(
                "CONSTRAINT [FK_{0}_id] FOREIGN KEY (idRecord) REFERENCES [{0}_id]([pkid]),\n", tableName);
            //Таблица на которую идет зависимость
            if (parentTableIdName == "idul")
            {
                _stringBuilder.AppendFormat(
                    "CONSTRAINT [FK_{0}_ul] FOREIGN KEY ({1}) REFERENCES [ul]([pkid]),\n", tableName, parentTableIdName);
            }
            _indexes.ForEach((it)=> _stringBuilder.AppendFormat(it));
            //Удаляем в конце ,\n
            _stringBuilder.Remove(_stringBuilder.Length - 2, 2);
            _stringBuilder.AppendFormat("\n)");
        }

        private string GetIndexForDictionary(ModalViewModel model, string tableName)
        {
            switch (model.DictionaryType)
            {
                case DictionaryTypes.FlName:
                    return $"CONSTRAINT [FK_{tableName}_{model.FieldInDb}_fl] FOREIGN KEY ({model.FieldInDb}) REFERENCES [fl]([pkid]),\n";
                case DictionaryTypes.UlName:
                    return $"CONSTRAINT [FK_{tableName}_{model.FieldInDb}_ul] FOREIGN KEY ({model.FieldInDb}) REFERENCES [ul]([pkid]),\n";
            }
            return "";
        }

        private void WriteField(ModalViewModel model, string tableName)
        {
            switch (model.Type)
            {
                case "int":
                    _stringBuilder.AppendFormat("[{0}] INT,\n", model.FieldInDb);
                    break;
                case "decimal":
                    _stringBuilder.AppendFormat("[{0}] decimal,\n", model.FieldInDb);
                    break;
                case "bit":
                    _stringBuilder.AppendFormat("[{0}] bit,\n", model.FieldInDb);
                    break;
                case "varchar":
                    _stringBuilder.AppendFormat("[{0}] varchar(max),\n", model.FieldInDb);
                    break;
                case "file":
                    //Констрейн
                    _stringBuilder.AppendFormat("[{0}] INT,\n", model.FieldInDb);
                    break;
                case "dictionary":
                    //Констрейн
                    _stringBuilder.AppendFormat("[{0}] INT,\n", model.FieldInDb);
                    _indexes.Add(GetIndexForDictionary(model, tableName));
                    break;
                //Констрейн
                case "hyperlink":
                    _stringBuilder.AppendFormat("[{0}] INT,\n", model.FieldInDb);
                    break;
                case "datetime":
                    _stringBuilder.AppendFormat("[{0}] DATETIME,\n", model.FieldInDb);
                    break;
                case "date":
                    _stringBuilder.AppendFormat("[{0}] DATE,\n", model.FieldInDb);
                    break;
                case "time":
                    _stringBuilder.AppendFormat("[{0}] TIME,\n", model.FieldInDb);
                    break;
            }
        }

        private void WriteIdentifier(string tableName)
        {
            _stringBuilder.AppendFormat("CREATE TABLE [dbo].[{0}_id] (\n", tableName);
            _stringBuilder.AppendFormat("[pkid] INT IDENTITY(1, 1) NOT NULL,\n");
            _stringBuilder.AppendFormat("[fl_del] INT CONSTRAINT[DF_{0}_id_del] DEFAULT((0)) NULL,\n", tableName);
            _stringBuilder.AppendFormat("CONSTRAINT[PK_{0}_id] PRIMARY KEY CLUSTERED([pkid] ASC)\n", tableName);
            _stringBuilder.AppendFormat(")\n");

        }
    }
}
