using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Interfaces
{
    public interface IDiskPath
    {
        /// <summary>
        /// Получить путь по имени диалога
        /// </summary>
        /// <param name="name">Имя диалога</param>
        /// <returns>Путь на диске</returns>
        bool GetPath(string name);

        string Path { get; }
    }
}
