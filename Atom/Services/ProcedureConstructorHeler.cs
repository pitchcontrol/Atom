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
    public class ProcedureConstructorHeler
    {
        StringBuilder _stringBuilder = new StringBuilder();
        /// <summary>
        /// Поле которое является истинным ид таблицы
        /// </summary>
        public string IdFieldName { get; set; } = "idRecord";
        /// <summary>
        /// Таблица на которую идет зависимость например - idul
        /// </summary>
        public string ParentTableIdName { get; set; } = "idul";
        public void Construct(IEnumerable<WebPageBaseViewModel> collection, string folderName)
        {
            var plain = collection.Flatten(i => i.Children);

            var tables = plain.OfType<ModalViewModel>().Where(i => !(i.Parent is GridViewModel)).GroupBy(i => i.TableName);
            foreach (IGrouping<string, ModalViewModel> groupTable in tables)
            {
                AddRecordProcedure(groupTable, groupTable.Key, ParentTableIdName, folderName);
                DeleteRecordProcedure(groupTable, groupTable.Key, ParentTableIdName, folderName);
                ByIdRecordProcedure(groupTable, groupTable.Key, ParentTableIdName, folderName);
                ViewRecordsFunction(groupTable, groupTable.Key, ParentTableIdName, folderName);
                EditRecordsFunction(groupTable, groupTable.Key, ParentTableIdName, folderName);
                UpdateRecordProcedureIEnumerable(groupTable, groupTable.Key, ParentTableIdName, folderName);


            }
        }

        private string GetDBType(ModalViewModel model)
        {
            switch (model.Type)
            {
                case "int":
                case "file":
                case "dictionary":
                case "hyperlink":
                    return "int";
                case "decimal":
                    return "decimal";
                case "bit":
                    return "bit";
                case "varchar":
                    return "varchar(max)";
                case "datetime":
                    return "DATETIME";
                case "date":
                    return "DATE";
                case "time":
                    return "TIME";
            }

            return model.Type;
        }

        /// <summary>
        /// Добавление процедуры - удалить запись
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="table"></param>
        /// <param name="parenTableId"></param>
        /// <param name="folderName"></param>
        private void DeleteRecordProcedure(IEnumerable<ModalViewModel> fields, string table, string parenTableId, string folderName)
        {
            _stringBuilder.AppendFormat("--Удаление записи из таблицы {0}\n", table);
            _stringBuilder.AppendFormat("--Пареметры\n");
            _stringBuilder.AppendFormat("--@id - pkid записи\n");
            _stringBuilder.AppendFormat("--@idreq - ИД заявки\n");
            _stringBuilder.AppendFormat("--@dat - ТА\n");

            _stringBuilder.AppendLine($"CREATE PROCEDURE [dbo].[usp_{table}Delete]");
            _stringBuilder.AppendFormat("\t@id int,\n");
            _stringBuilder.AppendFormat("\t@idreq int,\n");
            _stringBuilder.AppendFormat("\t@dat datetime\n");
            _stringBuilder.AppendFormat("AS\n");
            _stringBuilder.AppendFormat("declare @idt int = (select {0} from {1} where pkid=@id and fl_del in (1, 2, 3) and idreq=@idreq)\n", IdFieldName, table);
            _stringBuilder.AppendFormat("delete from MainTable\n");
            _stringBuilder.AppendFormat("where pkid=@id and fl_del in (1, 2, 3) and idreq=@idreq;\n");
            _stringBuilder.AppendFormat("delete MainTable_id where pkid = @idt\n");
            string fieldsLine = string.Join(", ", fields.Select(i => i.FieldInDb));
            _stringBuilder.AppendFormat("insert into MainTable (dats, datf, fl_del, idreq, idcp, {0}, {1}, {2})\n", parenTableId, IdFieldName, fieldsLine);
            _stringBuilder.AppendFormat("select  cast('3000-01-01' as datetime),cast('3000-01-01' as datetime), 3, @idreq, pkid, {0}, {1}, {2}\n", parenTableId, IdFieldName, fieldsLine);
            _stringBuilder.AppendFormat("from MainTable where pkid = @id and @dat>=dats and @dat<datf and fl_del=0\n");
            _stringBuilder.AppendFormat("DECLARE @iddel int = SCOPE_IDENTITY();\n");
            _stringBuilder.AppendFormat("update {0}_id set fl_del = 3 where pkid = @iddel", table);

            File.WriteAllText($"{folderName}/{table}Delete.sql", _stringBuilder.ToString());
            _stringBuilder.Clear();
        }

        private void ByIdRecordProcedure(IEnumerable<ModalViewModel> fields, string table, string parenTableId,
            string folderName)
        {
            _stringBuilder.AppendLine($"--Чтение записи по pkid из таблицу {table}");
            _stringBuilder.AppendLine("--Пареметры");
            _stringBuilder.AppendLine("--@id - pkid");
            _stringBuilder.AppendLine($"CREATE PROCEDURE [dbo].[usp_{table}ById]");
            _stringBuilder.AppendLine("@id int");
            _stringBuilder.AppendLine("AS");
            _stringBuilder.Append("SELECT t.pkid id, t.fl_del");
            StringBuilder join = new StringBuilder();
            WriteFields(fields, join);
            _stringBuilder.AppendLine($" from {table} t");
            _stringBuilder.Append(join.ToString());
            _stringBuilder.Append($"where t.pkid = @id");
            File.WriteAllText($"{folderName}/{table}ById.sql", _stringBuilder.ToString());
            _stringBuilder.Clear();
        }

        private void WriteFields(IEnumerable<ModalViewModel> fields, StringBuilder join)
        {
            foreach (ModalViewModel field in fields)
            {
                if (field.Type == ControlTypes.File)
                {
                    _stringBuilder.AppendFormat(", df.prim, df.usr, df.format, df.typ, df.nam, df.datadd, df.path");
                    join.AppendLine($"left join vw_ut_files df on df.pkid = t.{field.FieldInDb}");
                    continue;
                }
                if (field.Type == ControlTypes.Dictionary)
                {
                    switch (field.DictionaryType)
                    {
                        case DictionaryTypes.FlName:
                            _stringBuilder.Append($", {field.FieldInDb}, fl.nam");
                            join.AppendLine($"left join fl_history fl on fl.pkid = t.{field.FieldInDb}");
                            break;
                        case DictionaryTypes.UlName:
                            _stringBuilder.Append($", {field.FieldInDb}, ul.nam");
                            join.AppendLine($"left join ul_history ul on fl.pkid = t.{field.FieldInDb}");
                            break;
                        case DictionaryTypes.SimpleDictionary:
                            _stringBuilder.Append($", t.{field.FieldInDb}, {field.DictionaryTableName}.name");
                            join.AppendLine($"left join {field.DictionaryTableName} on {field.DictionaryTableName}.pkid = t.{field.FieldInDb}");
                            break;
                        case DictionaryTypes.DictionaryTable:
                            _stringBuilder.Append($", t.{field.FieldInDb}, {field.DictionaryTableName}.nam");
                            join.AppendLine($"left join ufn_Universal_dic('{field.DictionaryTableName}',1) {field.DictionaryTableName} on {field.DictionaryTableName}.id = t.{field.FieldInDb}");
                            break;
                    }
                    continue;
                }
                _stringBuilder.AppendFormat($", t.{field.FieldInDb}");
            }
        }

        private void EditRecordsFunction(IEnumerable<ModalViewModel> fields, string table, string parenTableId,
            string folderName)
        {
            _stringBuilder.AppendFormat("--Чтение записей из таблицы {0}\n", table);
            _stringBuilder.AppendFormat("--Пареметры\n");
            _stringBuilder.AppendFormat("--@{0} - ИД родительской таблицы\n", ParentTableIdName);
            _stringBuilder.AppendLine($"CREATE FUNCTION [dbo].[ufn_{table}Edit]");
            _stringBuilder.AppendLine("(@dat datetime, @idul int, @idreq int)");
            _stringBuilder.AppendLine("RETURNS TABLE");
            _stringBuilder.AppendLine("AS");
            _stringBuilder.AppendLine("RETURN");
            _stringBuilder.AppendLine("(");
            _stringBuilder.Append("SELECT t.pkid id, t.fl_del");
            StringBuilder join = new StringBuilder();
            WriteFields(fields, join);
            _stringBuilder.AppendLine($" from {table} t");
            _stringBuilder.Append(join.ToString());
            _stringBuilder.AppendLine("where (t.idul = @idul and t.fl_del in (1,2,3) and t.idreq = @idreq) or");
            _stringBuilder.AppendLine("(t.idul = @idul and t.fl_del=0 and (@dat >= t.[dats] and @dat < t.[datf])");
            _stringBuilder.AppendLine("and t.pkid not in (select [idcp] from [MainTable] where [idreq] = @idreq and [idcp] is not null))");
            _stringBuilder.Append(")");


            File.WriteAllText($"{folderName}/{table}Edit.sql", FormattUtil.Format(_stringBuilder.ToString()));
            _stringBuilder.Clear();
        }

        private void ViewRecordsFunction(IEnumerable<ModalViewModel> fields, string table, string parenTableId,
            string folderName)
        {
            _stringBuilder.AppendFormat("--Чтение записей из таблицы {0}\n", table);
            _stringBuilder.AppendFormat("--Пареметры\n");
            _stringBuilder.AppendFormat("--@{0} - ИД родительской таблицы\n", ParentTableIdName);
            _stringBuilder.AppendLine($"CREATE FUNCTION [dbo].[ufn_{table}View]");
            _stringBuilder.AppendLine("(@dat datetime, @idul int)");
            _stringBuilder.AppendLine("RETURNS TABLE");
            _stringBuilder.AppendLine("AS");
            _stringBuilder.AppendLine("RETURN");
            _stringBuilder.AppendLine("(");
            _stringBuilder.Append("SELECT t.pkid id, t.fl_del");
            StringBuilder join = new StringBuilder();
            WriteFields(fields, join);
            _stringBuilder.AppendLine($" from {table} t");
            _stringBuilder.Append(join.ToString());
            _stringBuilder.AppendLine("where t.dats<=@dat and t.datf>@dat and  t.fl_del=0 and  t.idul = @idul and t.idreq is null");
            _stringBuilder.Append(")");
            File.WriteAllText($"{folderName}/{table}View.sql", _stringBuilder.ToString());
            _stringBuilder.Clear();
        }

        private void UpdateRecordProcedureIEnumerable(IEnumerable<ModalViewModel> fields, string table, string parenTableId, string folderName)
        {
            _stringBuilder.AppendFormat("--Обнавление записи в таблицу {0}\n", table);
            _stringBuilder.AppendFormat("--Пареметры\n");
            _stringBuilder.AppendFormat("--@id - pkid записи\n--@idreq - ИД заявки\n--@dat - ТА\n");
            string header = string.Join("", fields.Select(i => "--@" + i.FieldInDb + " - " + i.RuDescription + "\n"));
            _stringBuilder.AppendFormat(header);
            _stringBuilder.AppendLine($"CREATE PROCEDURE [dbo].[usp_{table}Update]");
            header = string.Join(",\n", fields.Select(i => "\t@" + i.FieldInDb + " " + GetDBType(i)));
            _stringBuilder.AppendLine($"@id int,\n@idreq int,\n@dat datetime,\n {header}");
            _stringBuilder.AppendLine("AS");
            _stringBuilder.AppendLine($"if(exists(select * from [{table}] where fl_del <>0 and idreq = @idreq and pkid = @id))");
            _stringBuilder.AppendLine("begin");
            _stringBuilder.AppendLine($"update {table}");
            _stringBuilder.AppendLine("SET");
            header = string.Join("", fields.Select(i => i.FieldInDb + " = @" + i.FieldInDb + ",\n"));
            _stringBuilder.Append(header);
            _stringBuilder.AppendLine("fl_del = (case when [fl_del] = 3 then 2 else [fl_del] end)");
            _stringBuilder.AppendLine("where pkid = @id");
            _stringBuilder.AppendLine("end");
            _stringBuilder.AppendLine("else");
            _stringBuilder.AppendLine("begin");
            _stringBuilder.AppendLine($"insert into [{table}]");
            header = string.Join(", ", fields.Select(i => "[" + i.FieldInDb + "]"));
            _stringBuilder.AppendLine($"([dats], [datf], [{IdFieldName}], [idreq], [idcp], [fl_del], [{parenTableId}], {header})");
            header = string.Join(", ", fields.Select(i => "@" + i.FieldInDb));
            _stringBuilder.AppendLine($"select cast('3000-01-01' as datetime), cast('3000-01-01' as datetime), {IdFieldName}, @idreq, pkid, 2, {parenTableId}, {header}");
            _stringBuilder.AppendLine($"from {table}");
            _stringBuilder.AppendLine($"where pkid = @id");
            _stringBuilder.AppendLine("end");
            File.WriteAllText($"{folderName}/{table}Update.sql", _stringBuilder.ToString());
            _stringBuilder.Clear();
        }
        /// <summary>
        /// Добавление процедуры - добавить запись
        /// </summary>
        private void AddRecordProcedure(IEnumerable<ModalViewModel> fields, string table, string parenTableId, string folderName)
        {
            _stringBuilder.AppendFormat("--Добавление записи в таблицу {0}\n", table);
            _stringBuilder.AppendFormat("--Пареметры\n");
            _stringBuilder.AppendFormat("--@idreq - ИД заявки\n");
            _stringBuilder.AppendFormat("--@{0} - ИД родительской таблицы\n", ParentTableIdName);
            _stringBuilder.AppendFormat("{0}\n", string.Join("\n", fields.Select(i => "--@" + i.FieldInDb + " - " + i.RuDescription)));

            _stringBuilder.AppendFormat("CREATE PROCEDURE [dbo].[usp_{0}Add]\n", table);
            string header = string.Join(",\n", fields.Select(i => "\t@" + i.FieldInDb + " " + GetDBType(i)));
            _stringBuilder.AppendFormat("\t@idreq int,\n\t@{0} int,\n{1}\nAS\n", parenTableId, header);
            _stringBuilder.AppendFormat("INSERT INTO {0}_id (fl_del) VALUES (1)\n", table);
            _stringBuilder.AppendFormat("DECLARE @id int = SCOPE_IDENTITY();\n");

            string fieldsLine = string.Join(", ", fields.Select(i => i.FieldInDb));
            string fieldsLineParam = string.Join(", ", fields.Select(i => "@" + i.FieldInDb));
            _stringBuilder.AppendFormat("INSERT INTO {3} ({0}, datf, dats, fl_del, {1}, idreq, {2})\n", parenTableId, IdFieldName, fieldsLine, table);
            _stringBuilder.AppendFormat("VALUES (@{0}, '30000101', '30000101', 1, @id, @idreq, {1})\n", parenTableId, fieldsLineParam);

            _stringBuilder.AppendFormat("DECLARE @idcp int=scope_identity();\n");
            _stringBuilder.AppendFormat("update {0} set idcp=@idcp where pkid=@idcp;\n", table);
            _stringBuilder.AppendFormat("select @idcp id");

            File.WriteAllText($"{folderName}/{table}Add.sql", _stringBuilder.ToString());
            _stringBuilder.Clear();
        }
    }
}
