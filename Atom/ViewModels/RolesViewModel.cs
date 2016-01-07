using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Atom.Models;
using JetBrains.Annotations;

namespace Atom.ViewModels
{
    public class RolesViewModel:INotifyPropertyChanged
    {
        private IEnumerable<Role> _roles;
        /// <summary>
        /// Роли все пользователей
        /// </summary>
        public IEnumerable<Role> Roles
        {
            get { return _roles; }
            set
            {
                if (value == _roles) return;
                _roles = value;
                OnPropertyChanged();
            }
        }
        private IEnumerable<Role> _selectRoles;
        /// <summary>
        /// Выбранные роли для страницы
        /// </summary>
        public IEnumerable<Role> SelectRoles
        {
            get { return _roles; }
            set
            {
                if (value == _selectRoles) return;
                _selectRoles = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
