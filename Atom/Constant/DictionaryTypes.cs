using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Constant
{
    /// <summary>
    /// Типы словарей
    /// </summary>
    public static class DictionaryTypes
    {
        public static readonly string[] Types = { UlName, FlName, SimpleDictionary, DictionaryTable };
        /// <summary>
        /// Название ЮЛ
        /// </summary>
        public const string UlName = "ulname";
        /// <summary>
        /// ФИО
        /// </summary>
        public const string FlName = "flname";
        /// <summary>
        /// Словарь в виде простой таблицы
        /// </summary>
        public const string SimpleDictionary = "simpledictionary";
        /// <summary>
        /// Словарь с использованием таблицы dic_History
        /// </summary>
        public const string DictionaryTable = "dictionarytable";
    }
}
