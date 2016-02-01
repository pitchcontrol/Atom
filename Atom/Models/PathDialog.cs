using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Models
{
    public class PathDialog
    {
        public PathDialog()
        {
        }

        public PathDialog(string filter)
        {
            Filter = filter;
        }

        public string Value { get; set; }
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Фильтр для выбора
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// Имя в параметра в app.config
        /// </summary>
        public string DefaultPathName { get; set; }
        /// <summary>
        /// Кэшировать значение
        /// </summary>
        public bool Cache { get; set; }
        /// <summary>
        /// Папка
        /// </summary>
        public bool IsFolder { get; set; }
    }
}
