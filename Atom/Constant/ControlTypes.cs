using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Constant
{
    /// <summary>
    /// Константы для контролов
    /// </summary>
    public static class ControlTypes
    {
        public static readonly string[] Types = { Int,Decimal,Bit,Varchar,File,Dictionary,Hyperlink,Datetime,Date,Time };
        public const string Int = "int";
        public const string Decimal = "decimal";
        public const string Bit = "bit";
        public const string Varchar = "varchar";
        public const string File = "file";
        public const string Dictionary = "dictionary";
        public const string Hyperlink = "hyperlink";
        public const string Datetime = "datetime";
        public const string Date = "date";
        public const string Time = "time";
    }
}
